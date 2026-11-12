#!/usr/bin/env python3
"""Broji elemente, atribute, TYPE vrijednosti, komande i @@ tokene u NetGen XML korpusu.

Koristi tolerantni skener (regex), jer velik dio granula nije well-formed XML
(windows-1250, neescapani & i <, Callisto komentari). Usporedno pokusava i
strogi xml.etree parse i javlja koliko fajlova padne.

Izvor atributa koje interpreter cita: GetNamedItem("...") i .Attributes("...")
u netgen/*.vb. Rezultat ide u tools/out/netgen-xml-katalog.json.
"""

from __future__ import annotations

import hashlib
import json
import re
import sys
import xml.etree.ElementTree as ET
from collections import Counter, defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
NETGEN_SRC = ROOT / "netgen"
OUT_DIR = ROOT / "tools" / "out"

CORPORA = {
    "definitions_netgen": ROOT / "definitions" / "netgen",
    "netgenbot_granule": ROOT / "NetGenBot" / "granule",
    "prepisane_netgen": ROOT / "definitions" / "Prepisane_granule" / "Netgen",
}

# Strukturni elementi koji nisu "forma". Sve ostalo direktno pod PROJECT je ime forme.
STRUCTURAL = {
    "PROJECT",
    "NAME",
    "CAPTION",
    "KOMENTAR",
    "HISTORY",
    "INPUT",
    "COLUMN",
    "MASTERCOLUMNS",
    "DETAILCOLUMNS",
    "DETAILCOLUMNS2",
    "DETAILCOLUMNS3",
    "DETAILCOLUMNS4",
    "ROW",
    "ROWS",
    "FILTER",
    "MENUITEM",
    "MENU",
    "INCLUDE",
}

COMMANDS = [
    "REFRESHMASTERGRID",
    "REFRESHCONTROL",
    "UNCHECKMASTERGRID",
    "PRINTREPXMASTER",
    "PRINTREPXGRID",
    "PRINTMASTER",
    "PRINTGRID",
    "PRINTDET",
    "PREPAREMULTISELECTEDPRINT",
    "FOREACHSELECTED",
    "FOREACHBLUEROW",
    "FOREACHROW",
    "MASTERMOVENEXT",
    "MASTERMOVEPREVIOUS",
    "REFRESHMOVETOID",
    "FINDNODE",
    "OPENFORM",
    "OKSTAY",
    "CLOSEPARENT",
    "CANCEL",
    "CLOSE",
    "OK",
    "HIDECOLUMNS",
    "SHOWCOLUMNS",
    "ISEMPTYORZEROCANCELBOX",
    "ISEMPTYORZEROALERT",
    "ISEMPTY",
    "DISABLEALLCONTROLS",
    "ENABLEALLCONTROLS",
    "SHOWFOLDERBROWSERDIALOG",
    "SHOWOPENFILEDIALOG",
    "COLORPICKER",
    "STARTSEARCH",
    "ASSEMBLYLOAD",
    "BLAGAJNAENABLE",
    "FISKALIZACIJAN",
    "FISKALIZACIJAFAKTURE",
    "FISKALIZACIJABSRACUNI",
    "FISKALIZACIJAPD",
    "FISKALIZACIJAPONUDE",
    "FISKALIZACIJA",
    "POSLOVNIPROSTOR",
    "POSALJIZAKLJUCKENASERVER",
    "POSALJIPORUKENASERVER",
    "POSALJIRACUNENASERVER",
    "CREATETEXTFILEUTF8",
    "CREATETEXTFILE1",
    "CREATETEXTFILE2",
    "CREATETEXTFILE3",
    "PROCESSSTART",
    "PROGRESSON",
    "PROGRESSOFF",
    "TIMERON",
    "TIMEROFF",
    "RECALCGRIDROW",
    "RECALCGRID",
    "TOGGLEGRIDGROUP",
    "PIVOTGRID",
    "CHARTGRID",
    "GRID_COLUMN_VALUE",
    "CLICK",
    "BEEP",
    "MSGBOX",
    "EVAL",
    "DOCK1",
    "DOCK2",
    "DOCK3",
    "DOCK4",
    "DOCK5",
    "CREATEOBJECT",
    "SLOPP",
    "MULTISELECT",
    "MULTIBLUE",
]

ATTR_RE = re.compile(
    r"""([A-Za-z_][\w:.-]*)\s*=\s*(?:"([^"]*)"|'([^']*)')""",
    re.DOTALL,
)
TAG_RE = re.compile(r"<\s*(/?)\s*([A-Za-z_][\w:.-]*)\b([^<>]*?)\s*(/?)>", re.DOTALL)
COMMENT_RE = re.compile(r"<!--.*?-->", re.DOTALL)
AT_TOKEN_RE = re.compile(r"@@[A-Za-z_][A-Za-z0-9_]*")
CODE_ATTR_RE = re.compile(
    r"""(?:GetNamedItem|\.Attributes)\(\s*"([A-Za-z0-9_]+)"\s*\)""",
    re.IGNORECASE,
)

ACTION_ATTRS = {
    "ONCLICK",
    "ONDBLCLICK",
    "ONLOAD",
    "ONLOAD2",
    "ONSUBMIT",
    "ONCLOSED",
    "ONCLOSING",
    "ONACTIVATED",
    "ONLOADED",
    "ONLOSTFOCUS",
    "ONCHANGE",
    "ONGOTFOCUS",
    "ONENTERKEY",
    "ONCELLEDITED",
    "BEFORECELLEDITED",
    "ONCELLEDITEDDET",
    "BEFORECELLEDITEDDET",
    "BUTTONS",
    "POPUP",
    "POPUPMASTER",
    "POPUPDETAIL",
    "MASTERDBLCLICK",
    "DETAILDBLCLICK",
    "DBLCLICKFORM",
    "NEXTFORM",
    "AFTERREFRESH",
    "NFCSQL",
    "ONVALUECHANGED",
}


def read_text(path: Path) -> tuple[str, str]:
    raw = path.read_bytes()
    if raw.startswith(b"\xef\xbb\xbf"):
        return raw.decode("utf-8-sig", errors="replace"), "utf-8-sig"
    # Callisto granule su gotovo uvijek windows-1250. Ako header kaze drugo,
    # ipak prvo pokusaj cp1250 jer utf-8 decode na cp1250 bajtovima ne puca
    # (latin slova su validni utf-8 nastavci? ne — š je 0x9A, nije validan utf-8).
    try:
        text = raw.decode("utf-8")
        if "\ufffd" not in text:
            return text, "utf-8"
    except UnicodeDecodeError:
        pass
    return raw.decode("cp1250", errors="replace"), "cp1250"


def scan_file(text: str) -> dict:
    cleaned = COMMENT_RE.sub(" ", text)
    elements = Counter()
    attrs_by_el = defaultdict(Counter)
    attr_global = Counter()
    type_values = Counter()
    project_attrs = Counter()
    form_names = Counter()
    commands = Counter()
    at_tokens = Counter()
    encodings_note = []

    stack = []
    for m in TAG_RE.finditer(cleaned):
        closing, name, raw_attrs, self_close = m.group(1), m.group(2), m.group(3), m.group(4)
        uname = name.upper()
        if closing:
            if stack and stack[-1] == uname:
                stack.pop()
            elif uname in stack:
                while stack and stack[-1] != uname:
                    stack.pop()
                if stack:
                    stack.pop()
            continue

        parent = stack[-1] if stack else ""
        elements[uname] += 1
        pairs = []
        for am in ATTR_RE.finditer(raw_attrs):
            aname = am.group(1).upper()
            aval = am.group(2) if am.group(2) is not None else am.group(3)
            aval = aval or ""
            pairs.append((aname, aval))
            attrs_by_el[uname][aname] += 1
            attr_global[aname] += 1
            if aname in ACTION_ATTRS or aname.startswith("CMD") and aname.endswith("CAPTION"):
                harvest_commands(aval, commands)
            for tok in AT_TOKEN_RE.findall(aval):
                at_tokens[tok.lower()] += 1

        if uname == "INPUT":
            tval = next((v for k, v in pairs if k == "TYPE"), "")
            type_values[tval.strip().lower() or "(prazno)"] += 1
        if uname == "PROJECT":
            for k, _v in pairs:
                project_attrs[k] += 1
        if parent == "PROJECT" and uname not in STRUCTURAL:
            form_names[uname] += 1

        if not self_close:
            stack.append(uname)

    return {
        "elements": elements,
        "attrs_by_el": attrs_by_el,
        "attr_global": attr_global,
        "type_values": type_values,
        "project_attrs": project_attrs,
        "form_names": form_names,
        "commands": commands,
        "at_tokens": at_tokens,
        "notes": encodings_note,
    }


def _classify_chunk(chunk: str, sink: Counter) -> None:
    c = chunk.strip()
    if not c:
        return
    # Gumb je "Natpis:Akcija". Ako lijevo nije SQL ni komanda, akcija je desno.
    if ":" in c:
        left, right = c.split(":", 1)
        left_up = left.strip().upper()
        left_is_command = any(left_up.startswith(cmd) for cmd in COMMANDS)
        left_is_sql = left_up.startswith(("SELECT", "EXEC", "SP", "IF ", "IF(", "@", "=="))
        if left.strip() and not left_is_command and not left_is_sql:
            _classify_chunk(right, sink)
            return
    probe = c.lstrip(":").strip()
    up = probe.upper()
    if up.startswith("REPX/") or up.startswith("REPX:"):
        sink["REPX"] += 1
        return
    hit = None
    for cmd in COMMANDS:
        if up.startswith(cmd):
            hit = cmd
            break
    if hit:
        sink[hit] += 1
    elif up.startswith("SELECT") or up.startswith("EXEC") or up.startswith("SP") or up.startswith("IF ") or up.startswith("IF("):
        sink["<SQL>"] += 1
    elif up.startswith("=="):
        sink["==SQL"] += 1
    elif re.match(r"^[A-Z_][A-Z0-9_]*", up) and not up.startswith("@"):
        token = re.match(r"^([A-Z_][A-Z0-9_]*)", up).group(1)
        if token in {"NULL", "BEGIN", "END", "DECLARE", "SET"}:
            sink["<SQL>"] += 1
        else:
            sink["<FORMA:" + token + ">"] += 1


def harvest_commands(value: str, sink: Counter) -> None:
    # NetGen lanci: # je separator, ~ je alias za # unutar BUTTONS.
    chunks = re.split(r"[#~]", value)
    for chunk in chunks:
        _classify_chunk(chunk, sink)


def strict_parse_ok(text: str) -> bool:
    try:
        ET.fromstring(text)
        return True
    except ET.ParseError:
        return False


def code_attributes() -> Counter:
    found = Counter()
    files = {}
    for vb in NETGEN_SRC.glob("*.vb"):
        text, _enc = read_text(vb)
        for m in CODE_ATTR_RE.finditer(text):
            name = m.group(1).upper()
            found[name] += 1
            files.setdefault(name, set()).add(vb.name)
    return found, {k: sorted(v) for k, v in files.items()}


def merge_counter(dst: Counter, src: Counter) -> None:
    dst.update(src)


def top(counter: Counter, n: int | None = None):
    items = counter.most_common(n)
    return [{"name": k, "count": v} for k, v in items]


def main() -> int:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    per_corpus = {}
    grand_elements = Counter()
    grand_attrs = Counter()
    grand_attrs_by_el = defaultdict(Counter)
    grand_types = Counter()
    grand_project = Counter()
    grand_forms = Counter()
    grand_commands = Counter()
    grand_at = Counter()
    files_per_el = defaultdict(set)
    files_per_attr = defaultdict(set)
    files_per_type = defaultdict(set)
    file_rows = []
    parse_fail = []
    encoding_counts = Counter()
    seen_hash = {}
    duplicate_of = []

    for label, folder in CORPORA.items():
        if not folder.exists():
            per_corpus[label] = {"missing": True, "path": str(folder)}
            continue
        paths = sorted(p for p in folder.rglob("*") if p.suffix.lower() in {".xml", ".frm"} and p.is_file())
        cel = Counter()
        cat = Counter()
        cty = Counter()
        enc = Counter()
        n_fail = 0
        for path in paths:
            raw = path.read_bytes()
            digest = hashlib.sha256(raw).hexdigest()
            rel = str(path.relative_to(ROOT))
            text, enc_name = read_text(path)
            enc[enc_name] += 1
            ok = strict_parse_ok(text)
            if not ok:
                n_fail += 1
            scanned = scan_file(text)
            merge_counter(cel, scanned["elements"])
            merge_counter(cat, scanned["attr_global"])
            merge_counter(cty, scanned["type_values"])
            # Grand total je unija po sadrzaju. definitions/netgen i
            # NetGenBot/granule su isti skup bajtova u razlicitim mapama.
            if digest in seen_hash:
                duplicate_of.append({"path": rel, "same_as": seen_hash[digest]})
                file_rows.append(
                    {
                        "corpus": label,
                        "path": rel,
                        "encoding": enc_name,
                        "strict_xml": ok,
                        "bytes": path.stat().st_size,
                        "elements": sum(scanned["elements"].values()),
                        "project": scanned["elements"].get("PROJECT", 0),
                        "input": scanned["elements"].get("INPUT", 0),
                        "duplicate_of": seen_hash[digest],
                    }
                )
                continue
            seen_hash[digest] = rel
            encoding_counts[enc_name] += 1
            if not ok:
                parse_fail.append(rel)
            merge_counter(grand_elements, scanned["elements"])
            merge_counter(grand_attrs, scanned["attr_global"])
            merge_counter(grand_types, scanned["type_values"])
            merge_counter(grand_project, scanned["project_attrs"])
            merge_counter(grand_forms, scanned["form_names"])
            merge_counter(grand_commands, scanned["commands"])
            merge_counter(grand_at, scanned["at_tokens"])
            for el, attrs in scanned["attrs_by_el"].items():
                grand_attrs_by_el[el].update(attrs)
            for el in scanned["elements"]:
                files_per_el[el].add(rel)
            for at in scanned["attr_global"]:
                files_per_attr[at].add(rel)
            for tv in scanned["type_values"]:
                files_per_type[tv].add(rel)
            file_rows.append(
                {
                    "corpus": label,
                    "path": rel,
                    "encoding": enc_name,
                    "strict_xml": ok,
                    "bytes": path.stat().st_size,
                    "elements": sum(scanned["elements"].values()),
                    "project": scanned["elements"].get("PROJECT", 0),
                    "input": scanned["elements"].get("INPUT", 0),
                }
            )
        per_corpus[label] = {
            "path": str(folder.relative_to(ROOT)),
            "files": len(paths),
            "strict_parse_fail": n_fail,
            "encoding": dict(enc),
            "top_elements": top(cel, 40),
            "top_types": top(cty, 40),
            "top_attrs": top(cat, 40),
        }

    code_attr_counts, code_attr_files = code_attributes()
    code_names = set(code_attr_counts)
    corpus_names = set(grand_attrs)
    only_code = sorted(code_names - corpus_names)
    only_corpus = sorted(corpus_names - code_names)

    rare_attrs = []
    for name, cnt in grand_attrs.most_common():
        nf = len(files_per_attr[name])
        if nf <= 3 or cnt <= 5:
            rare_attrs.append({"name": name, "count": cnt, "files": nf})
    rare_attrs.sort(key=lambda r: (r["files"], r["count"], r["name"]))

    rare_types = []
    for name, cnt in grand_types.most_common():
        nf = len(files_per_type[name])
        if nf <= 3:
            rare_types.append({"type": name, "count": cnt, "files": nf})

    rare_elements = []
    for name, cnt in grand_elements.most_common():
        nf = len(files_per_el[name])
        if nf <= 3:
            rare_elements.append({"element": name, "count": cnt, "files": nf})

    # Atributi po kljucnim elementima — puni popis, sortiran po broju.
    key_elements = [
        "PROJECT",
        "INPUT",
        "COLUMN",
        "MASTERCOLUMNS",
        "DETAILCOLUMNS",
    ]
    attrs_detail = {}
    for el in sorted(grand_attrs_by_el):
        if el in key_elements or grand_elements[el] >= 20:
            attrs_detail[el] = [
                {
                    "name": n,
                    "count": c,
                    "in_code": n in code_names,
                }
                for n, c in grand_attrs_by_el[el].most_common()
            ]

    report = {
        "corpora": per_corpus,
        "totals": {
            "files_on_disk": len(file_rows),
            "unique_files": len(seen_hash),
            "duplicate_files": len(duplicate_of),
            "strict_parse_fail_unique": len(parse_fail),
            "encoding_unique": dict(encoding_counts),
            "distinct_elements": len(grand_elements),
            "distinct_attributes": len(grand_attrs),
            "distinct_types": len(grand_types),
            "distinct_form_names": len(grand_forms),
            "note": (
                "Svi brojevi ispod 'elements'/'attributes'/'types'/'commands'/'at_tokens' "
                "su nad jedinstvenim sadrzajem (SHA-256). definitions/netgen i "
                "NetGenBot/granule su isti skup; drugi se ne broji ponovo. "
                "per-corpus brojevi u 'corpora' su sirovi, s duplikatima."
            ),
        },
        "duplicate_sample": duplicate_of[:15],
        "elements": [
            {"name": n, "count": c, "files": len(files_per_el[n])}
            for n, c in grand_elements.most_common()
        ],
        "attributes": [
            {"name": n, "count": c, "files": len(files_per_attr[n]), "in_code": n in code_names}
            for n, c in grand_attrs.most_common()
        ],
        "types": [
            {"type": n, "count": c, "files": len(files_per_type[n])}
            for n, c in grand_types.most_common()
        ],
        "project_attributes": [
            {"name": n, "files_with_attr_on_project": c, "in_code": n in code_names}
            for n, c in grand_project.most_common()
        ],
        "form_names_top": top(grand_forms, 60),
        "commands": top(grand_commands),
        "at_tokens": top(grand_at, 80),
        "attrs_by_element": attrs_detail,
        "rare_attributes": rare_attrs,
        "rare_types": rare_types,
        "rare_elements": rare_elements,
        "code_attributes_not_in_corpus": [
            {"name": n, "code_hits": code_attr_counts[n], "files": code_attr_files.get(n, [])}
            for n in only_code
        ],
        "corpus_attributes_not_in_code_lookup": [
            {"name": n, "count": grand_attrs[n], "files": len(files_per_attr[n])}
            for n in sorted(only_corpus, key=lambda x: -grand_attrs[x])
        ],
        "strict_parse_fail_sample": parse_fail[:40],
        "strict_parse_fail_count": len(parse_fail),
    }

    out = OUT_DIR / "netgen-xml-katalog.json"
    out.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")

    summary = OUT_DIR / "netgen-xml-katalog-sazetak.txt"
    lines = []
    t = report["totals"]
    lines.append(
        f"unique_files={t['unique_files']} on_disk={t['files_on_disk']} "
        f"duplicates={t['duplicate_files']} strict_fail_unique={t['strict_parse_fail_unique']} "
        f"elements={t['distinct_elements']} attrs={t['distinct_attributes']} "
        f"types={t['distinct_types']} forms={t['distinct_form_names']}"
    )
    lines.append("ENCODING_UNIQUE " + json.dumps(t["encoding_unique"]))
    lines.append("--- CORPORA ---")
    for k, v in per_corpus.items():
        lines.append(f"{k}: files={v.get('files')} fail={v.get('strict_parse_fail')} enc={v.get('encoding')}")
    lines.append("--- ELEMENTS ---")
    for row in report["elements"][:50]:
        lines.append(f"{row['count']:7} files={row['files']:4} {row['name']}")
    lines.append("--- PROJECT ATTRS ---")
    for row in report["project_attributes"][:80]:
        lines.append(
            f"{row['files_with_attr_on_project']:5} code={row['in_code']} {row['name']}"
        )
    lines.append("--- INPUT ATTRS ---")
    for row in report["attrs_by_element"].get("INPUT", [])[:80]:
        lines.append(f"{row['count']:7} code={row['in_code']} {row['name']}")
    lines.append("--- COLUMN ATTRS ---")
    for row in report["attrs_by_element"].get("COLUMN", []):
        lines.append(f"{row['count']:7} code={row['in_code']} {row['name']}")
    lines.append("--- TYPES ---")
    for row in report["types"]:
        lines.append(f"{row['count']:7} files={row['files']:4} {row['type']}")
    lines.append("--- COMMANDS (non-form) ---")
    for row in report["commands"]:
        if row["name"].startswith("<FORMA"):
            continue
        lines.append(f"{row['count']:7} {row['name']}")
    lines.append("--- @@ top ---")
    for row in report["at_tokens"][:40]:
        lines.append(f"{row['count']:7} {row['name']}")
    lines.append("--- CODE ATTRS NOT IN CORPUS ---")
    for row in report["code_attributes_not_in_corpus"]:
        lines.append(f"{row['name']} hits={row['code_hits']} {','.join(row['files'][:4])}")
    lines.append("--- CORPUS ATTRS NOT LOOKED UP (top 40) ---")
    for row in report["corpus_attributes_not_in_code_lookup"][:40]:
        lines.append(f"{row['count']:7} files={row['files']:4} {row['name']}")
    lines.append("--- RARE TYPES ---")
    for row in rare_types:
        lines.append(f"{row['type']} count={row['count']} files={row['files']}")
    summary.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print("\n".join(lines[:80]))
    print(f"\nWrote {out}")
    print(f"Wrote {summary}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
