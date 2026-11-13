# -*- coding: utf-8 -*-
"""Faza B3: koje XML atribute, TYPE vrijednosti i naredbe NetGen cita, a koje Carpo.NetGen.

Ulaz: tools/out/netgen-xml-katalog.json (tools/netgen_xml_katalog.py) i izvor
Devex_26/Carpo.NetGen (bez testova). Izlaz: docs/pregled/B3-xml-matrica.tsv
i sazetak na stdout.

Carpo "cita" atribut ako se ime pojavi kao string literal u kodu (bez obzira na
velika i mala slova), ili ako ga pokriva poznati dinamicki obrazac (CMD1CAPTION
se slaze iz "CMD" + broj + "CAPTION"). To je gornja granica: literal ne dokazuje
da je ponasanje isto, samo da kod ime uopce poznaje. Atribut bez literala je
kandidat za "nedostaje" i ide u rucni pregled.
"""
from __future__ import print_function

import io
import json
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
KATALOG = os.path.join(ROOT, "tools", "out", "netgen-xml-katalog.json")
CARPO = os.path.join(ROOT, "Devex_26", "Carpo.NetGen")
NETGEN = os.path.join(ROOT, "netgen")
IZLAZ = os.path.join(ROOT, "docs", "pregled", "B3-xml-matrica.tsv")

LITERAL = re.compile(r'@"((?:""|[^"])*)"|"((?:\\.|[^"\\\n])*)"')

# Imena koja kod slaze od dijelova. Kljuc je regex nad imenom atributa,
# vrijednost su literali koji moraju postojati u kodu.
DINAMICKI = [
    (re.compile(r"^CMD(\d+)CAPTION$"), ["CMD", "CAPTION"]),
    (re.compile(r"^CMD(\d+)$"), ["CMD"]),
    (re.compile(r"^BUTTON(\d+)$"), ["BUTTON"]),
    (re.compile(r"^(DET)(.+)$"), None),  # DETxxx se cesto slaze kao "DET" + ime mastera
]


def literali(korijen, iskljuci_testove=True):
    skup = set()
    for dp, dirs, files in os.walk(korijen):
        dirs[:] = [d for d in dirs if d not in ("bin", "obj")]
        if iskljuci_testove and ("GridTest" in dp or os.sep + "tests" in dp):
            continue
        for f in files:
            if not f.endswith(".cs"):
                continue
            t = io.open(os.path.join(dp, f), encoding="utf-8", errors="replace").read()
            for m in LITERAL.finditer(t):
                s = m.group(1) if m.group(1) is not None else m.group(2)
                if s:
                    skup.add(s.upper())
    return skup


def vb_literali(korijen):
    skup = set()
    for f in os.listdir(korijen):
        if not f.lower().endswith(".vb") or ".designer." in f.lower():
            continue
        raw = open(os.path.join(korijen, f), "rb").read()
        try:
            t = raw.decode("utf-8-sig")
        except UnicodeDecodeError:
            t = raw.decode("cp1250", errors="replace")
        for m in re.finditer(r'"((?:""|[^"\n])*)"', t):
            if m.group(1):
                skup.add(m.group(1).upper())
    return skup


def carpo_cita(ime, lit, tekst_svi):
    u = ime.upper()
    if u in lit:
        return "da"
    for rx, dijelovi in DINAMICKI:
        if rx.match(u):
            if dijelovi is None:
                ostatak = rx.match(u).group(2)
                if "DET" in lit and ostatak in lit:
                    return "dinamicki"
                continue
            if all(d in lit for d in dijelovi):
                return "dinamicki"
    # Usporedba unutar duljeg literala (npr. "TYPE=...") je slaba, ali se biljezi.
    if len(u) >= 6 and u in tekst_svi:
        return "u-tekstu"
    return "ne"


def main():
    k = json.load(io.open(KATALOG, encoding="utf-8"))
    lit = literali(CARPO)
    tekst_svi = "\n".join(sorted(lit))
    vb = vb_literali(NETGEN)

    redovi = []
    for a in k["attributes"]:
        ime = a["name"]
        nc = bool(a.get("in_code"))
        cc = carpo_cita(ime, lit, tekst_svi)
        if nc and cc == "ne":
            st = "NEDOSTAJE?"
        elif nc:
            st = "oba"
        elif cc != "ne":
            st = "samo-carpo"
        else:
            st = "nitko"
        redovi.append(("atribut", ime, a.get("files", 0), a.get("count", 0), "da" if nc else "ne", cc, st))

    for t in k["types"]:
        ime = t["type"]
        u = ime.upper()
        nc = "da" if (u in vb or any(u in x for x in vb if len(x) < 40)) else "ne"
        cc = carpo_cita(ime, lit, tekst_svi)
        st = "NEDOSTAJE?" if nc == "da" and cc == "ne" else ("oba" if nc == "da" else ("samo-carpo" if cc != "ne" else "nitko"))
        redovi.append(("type", ime, t.get("files", 0), t.get("count", 0), nc, cc, st))

    for c in k["commands"]:
        ime = c["name"]
        if ime.startswith("<"):
            continue
        u = ime.upper()
        nc = "da" if any(u in x for x in vb if len(x) < 60) else "ne"
        cc = carpo_cita(ime, lit, tekst_svi)
        st = "NEDOSTAJE?" if nc == "da" and cc == "ne" else ("oba" if nc == "da" else ("samo-carpo" if cc != "ne" else "nitko"))
        redovi.append(("naredba", ime, c.get("files", ""), c.get("count", 0), nc, cc, st))

    os.makedirs(os.path.dirname(IZLAZ), exist_ok=True)
    with io.open(IZLAZ, "w", encoding="utf-8", newline="\n") as f:
        f.write("vrsta\time\tgranula\tpojava\tnetgen_cita\tcarpo_cita\tstatus\n")
        for r in sorted(redovi, key=lambda r: (r[0], r[6] != "NEDOSTAJE?", -int(r[2] or 0), r[1])):
            f.write("\t".join(str(x) for x in r) + "\n")

    for vrsta in ("atribut", "type", "naredba"):
        dio = [r for r in redovi if r[0] == vrsta]
        po = {}
        for r in dio:
            po[r[6]] = po.get(r[6], 0) + 1
        print("%-8s ukupno=%d %s" % (vrsta, len(dio), " ".join("%s=%d" % kv for kv in sorted(po.items()))))
    print("Izlaz: " + os.path.relpath(IZLAZ, ROOT))


if __name__ == "__main__":
    main()
