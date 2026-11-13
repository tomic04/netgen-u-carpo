# -*- coding: utf-8 -*-
"""Usporedba prepoznavanja segmenata lanca dijaloga: Form2.Rafalno (lanac_vb) naspram DijalogTok.Koraci.

Segmenti su iz dogadjaja kontrola u formama (ON* atributi elementa INPUT), podijeljeni na #.
Izlaz: docs/pregled/C2-lanac-razlike.md.
"""
from __future__ import print_function
import base64, collections, hashlib, io, os, re, subprocess, sys
sys.path.insert(0, os.path.dirname(__file__))
import lanac_vb
import sql_diff as sd

ATRIBUTI = ("ONCLICK", "ONLOSTFOCUS", "ONCHANGE", "ONVALUECHANGED", "ONENTERKEY", "ONDBLCLICK",
            "ONGOTFOCUS", "ONLEAVE", "ONCELLEDITED", "AFTERREFRESH", "ONROWCHANGED", "ONCHECKED")
INPUT = re.compile(r"<INPUT\b([^>]*)>", re.I | re.S)

# C# vrste koje odgovaraju VB oznakama (vise C# naziva za istu VB granu)
EKVIV = {
    "forma-otvori": {"forma"}, "isempty": {"naredba"}, "isemptyorzero": {"naredba"},
    "slopp": {"naredba"}, "prazno": {"prazno"},
    # VB ":repx/..." ide u NovaGenerickaForma2, koja REPX/ salje na ispis (Form2.vb 4276): isti ishod.
    "forma": {"forma", "predlozak"},
}


def segmenti():
    vidjeno = set(); izlaz = collections.OrderedDict()
    for dp, ds, fs in os.walk(sd.KORPUS):
        for f in sorted(fs):
            if not f.lower().endswith(".xml"):
                continue
            put = os.path.join(dp, f)
            h = hashlib.sha256(open(put, "rb").read()).hexdigest()
            if h in vidjeno:
                continue
            vidjeno.add(h)
            t = sd.citaj(put)
            for m in INPUT.finditer(t):
                for a in sd.ATR.finditer(m.group(1)):
                    if a.group(1).upper() not in ATRIBUTI:
                        continue
                    v = sd.html(a.group(2))
                    for seg in v.split("#"):
                        s = seg.strip()
                        if s and s not in izlaz:
                            izlaz[s] = (f, a.group(1).upper())
    return izlaz


def main():
    exe = sys.argv[1] if len(sys.argv) > 1 else "/tmp/harn/LanacVoz.exe"
    seg = segmenti()
    lista = list(seg.keys())
    ulaz = "\n".join(base64.b64encode(s.encode("utf-8")).decode() for s in lista) + "\n"
    env = dict(os.environ, MONO_PATH=os.path.dirname(exe))
    out = subprocess.run(["mono", exe], input=ulaz, capture_output=True, text=True, env=env, check=True).stdout.splitlines()
    raz = collections.defaultdict(list)
    tok = collections.Counter()
    for s, cs in zip(lista, out):
        vbv = lanac_vb.prepoznaj(s)
        ok = cs == vbv or cs in EKVIV.get(vbv, set())
        if not ok:
            raz[(vbv, cs)].append(s)
        if "@@" in s and s.strip().startswith("@@"):
            tok[s.split()[0][:30]] += 1
    print("segmenata:", len(lista), " razlicito prepoznatih:", sum(len(v) for v in raz.values()))
    for (vbv, cs), l in sorted(raz.items(), key=lambda kv: -len(kv[1])):
        print("  VB=%-14s C#=%-12s %4d  npr: %s" % (vbv, cs, len(l), l[0][:90].replace("\n", " ")))
    print("segmenti koji pocinju tokenom @@ (rjecnik se mijenja tek nakon zamjene):", sum(tok.values()))
    for k, n in tok.most_common(8):
        print("   ", n, k)
    return raz

if __name__ == "__main__":
    main()
