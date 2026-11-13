# -*- coding: utf-8 -*-
"""Diferencijalni test SQL sloja: doslovni VB model (vb_oracle) naspram C# (CsVoz.exe pod mono).

Ulaz su svi SQL tekstovi s @@ iz definitions/netgen (jedinstveni sadrzaji), u
nekoliko scenarija parametara. Izlaz: docs/pregled/D2-sql-razlike.md i .tsv.

Pokretanje (Linux, mono):
  mcs -target:library -out:/tmp/harn/sql.dll -r:System.Data.dll <SqlTekst, DetailGrid, ...>
  mcs -out:/tmp/harn/CsVoz.exe -r:/tmp/harn/sql.dll tools/diff/CsVoz.cs
  python3 tools/diff/sql_diff.py /tmp/harn/CsVoz.exe
"""
from __future__ import print_function

import base64
import collections
import hashlib
import io
import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.dirname(__file__))
import vb_oracle as vb  # noqa: E402

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
KORPUS = os.path.join(ROOT, "definitions", "netgen")
IZLAZ_MD = os.path.join(ROOT, "docs", "pregled", "D2-sql-razlike.md")
IZLAZ_TSV = os.path.join(ROOT, "docs", "pregled", "D2-sql-razlike.tsv")

ATR = re.compile(r'([A-Za-z_][A-Za-z0-9_]*)\s*=\s*"([^"]*)"', re.S)
TOKEN = re.compile(r"@@([A-Za-z_][A-Za-z0-9_]*)")
POZNATI = {"id", "operateriid", "detid", "node", "tempstring", "editedcolumn", "lastvalue",
           "dblclickedcolumn", "version", "opisniparametri", "focusedcontrol", "selectedfile",
           "selectedpath", "xmlname", "jupitersrvpath", "selectedcolor"}


def citaj(put):
    b = open(put, "rb").read()
    for enc in ("utf-8-sig", "utf-16", "cp1250"):
        try:
            if enc == "utf-16" and not b[:2] in (b"\xff\xfe", b"\xfe\xff"):
                continue
            return b.decode(enc)
        except UnicodeDecodeError:
            continue
    return b.decode("cp1250", "replace")


def html(s):
    return (s.replace("&quot;", '"').replace("&apos;", "'").replace("&lt;", "<")
            .replace("&gt;", ">").replace("&#xD;", "\r").replace("&#xA;", "\n").replace("&amp;", "&"))


def sqlovi():
    vidjeno = set()
    izlaz = collections.OrderedDict()
    for dp, ds, fs in os.walk(KORPUS):
        for f in sorted(fs):
            if not f.lower().endswith(".xml"):
                continue
            put = os.path.join(dp, f)
            h = hashlib.sha256(open(put, "rb").read()).hexdigest()
            if h in vidjeno:
                continue
            vidjeno.add(h)
            t = citaj(put)
            for m in ATR.finditer(t):
                v = html(m.group(2))
                if "@@" not in v:
                    continue
                dijelovi = [v] + ([x for x in v.split("#") if "@@" in x] if "#" in v else [])
                for d in dijelovi:
                    d = d.strip()
                    if d and d not in izlaz:
                        izlaz[d] = (f, m.group(1).upper())
    return izlaz


def imena(sql):
    return [m.group(1) for m in TOKEN.finditer(sql) if m.group(1).lower() not in POZNATI]


def vrijednost(ime, i):
    u = ime.upper()
    if u.startswith("DAT") or ("GRID" in u and "DAT" in u):
        return "datum"
    return "broj" if i % 2 == 0 else "tekst"


def scenariji(sql):
    im = []
    for n in imena(sql):
        # CB u NetGenu nema dva kljuca koji se razlikuju samo po velicini slova (Form1.vb 3990).
        if n.lower() not in [x.lower() for x in im]:
            im.append(n)
    sc = []
    # S1 dijalog: @Ime=vrijednost, (PokupiParametre: tekst u navodnicima, datum yyyyMMdd)
    s = ""
    for i, n in enumerate(im):
        v = vrijednost(n, i)
        s += "@%s=%s," % (n, {"broj": str(10 + i), "tekst": "'t%d'" % i, "datum": "'20261113'"}[v])
    sc.append(("dijalog", s, [], False))
    # S2 grid: @Ime='vrijednost' , (PokupiParametreSaGrida, hr-HR datum)
    s = ""
    for i, n in enumerate(im):
        v = vrijednost(n, i)
        s += "@%s='%s' , " % (n, {"broj": str(10 + i), "tekst": "t%d" % i, "datum": "13.11.2026. 0:00:00"}[v])
    sc.append(("grid", s, [], False))
    sc.append(("grid-ispis", s, [], True))
    # S3 samo medjuspremnik (CB kljuc je IME=)
    cb = []
    for i, n in enumerate(im):
        v = vrijednost(n, i)
        cb.append((n + "=", {"broj": "1,5", "tekst": "cb%d" % i, "datum": "13.11.2026"}[v]))
    sc.append(("clipboard", "", cb, False))
    sc.append(("clipboard-ispis", "", cb, True))
    # S4 nista: sve je null
    sc.append(("prazno", "", [], False))
    # S5 prefiks: dulje ime prvo (Substring(0, len) uzme prvi koji pocinje tim imenom)
    s = ""
    for i, n in enumerate(im):
        s += "@%sX='krivo%d' , @%s='%d' , " % (n, i, n, 20 + i)
    sc.append(("prefiks", s, [], False))
    return sc


def b64(s):
    return base64.b64encode(s.encode("utf-8")).decode("ascii")


def main():
    exe = sys.argv[1] if len(sys.argv) > 1 else "/tmp/harn/CsVoz.exe"
    svi = sqlovi()
    slucajevi = []
    for sql, izvor in svi.items():
        for ime, prm, cb, dat in scenariji(sql):
            slucajevi.append((sql, izvor, ime, prm, cb, dat))
    k = vb.Kontekst()
    ocek = []
    for sql, izvor, ime, prm, cb, dat in slucajevi:
        try:
            ocek.append(vb.posle_next_forme(sql, prm, cb, k, dat))
        except vb.Nedefinirano as e:
            ocek.append(None)
    ulaz = []
    for i, (sql, izvor, ime, prm, cb, dat) in enumerate(slucajevi):
        cbt = "\x1e".join(a + "\x1f" + b for a, b in cb)
        ulaz.append("%d\t%s\t%s\t%s\t%s" % (i, b64(sql), b64(prm), b64(cbt), "1" if dat else "0"))
    env = dict(os.environ, MONO_PATH=os.path.dirname(exe))
    p = subprocess.run(["mono", exe, "zamijeni"], input="\n".join(ulaz) + "\n", capture_output=True,
                       text=True, env=env, check=True)
    cs = {}
    for red in p.stdout.splitlines():
        i, v = red.split("\t", 1)
        cs[int(i)] = v if v.startswith("!") else base64.b64decode(v).decode("utf-8")

    razlike = collections.defaultdict(list)
    ukupno = collections.Counter()
    nedef = 0
    for i, (sql, izvor, ime, prm, cb, dat) in enumerate(slucajevi):
        ukupno[ime] += 1
        if ocek[i] is None:
            nedef += 1
            continue
        if cs.get(i) != ocek[i]:
            razlike[ime].append((izvor, sql, prm, cb, ocek[i], cs.get(i)))

    os.makedirs(os.path.dirname(IZLAZ_TSV), exist_ok=True)
    with io.open(IZLAZ_TSV, "w", encoding="utf-8", newline="\n") as f:
        f.write("scenarij\tgranula\tatribut\tsql\tparametri\tnetgen\tcarpo\n")
        for ime, lst in razlike.items():
            for izvor, sql, prm, cb, o, c in lst:
                f.write("\t".join(x.replace("\t", " ").replace("\n", "\\n").replace("\r", "")
                                  for x in (ime, izvor[0], izvor[1], sql, prm or str(cb), o, c or "")) + "\n")
    print("SQL tekstova s @@: %d, slucajeva: %d, VB nedefinirano: %d" % (len(svi), len(slucajevi), nedef))
    for ime in ukupno:
        print("  %-16s slucajeva=%-5d razlika=%d" % (ime, ukupno[ime], len(razlike.get(ime, []))))
    return razlike, ukupno, len(svi), nedef


if __name__ == "__main__":
    main()
