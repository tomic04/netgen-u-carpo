# -*- coding: utf-8 -*-
"""Diferencijalni test stvarnog puta detail grida: VB Form1.vb 2304 naspram C# DetailGrid.Slozi.

Parametri su u obliku PokupiParametreSaGrida: @Ime='vrijednost' , (s razmakom).
ExecSql zamijeni ~~ zarezom (Class1.vb 85), pa se to radi i na VB strani.
"""
from __future__ import print_function
import base64, collections, io, os, subprocess, sys
sys.path.insert(0, os.path.dirname(__file__))
import vb_oracle as vb
import sql_diff as sd

def main():
    exe = sys.argv[1] if len(sys.argv) > 1 else "/tmp/harn/CsVoz.exe"
    svi = sd.sqlovi()
    k = vb.Kontekst()
    slucajevi = []
    for sql, izvor in svi.items():
        for ime, prm, cb, dat in sd.scenariji(sql):
            if ime in ("grid", "prefiks"):
                slucajevi.append((sql, izvor, ime, prm))
    ocek = []
    for sql, izvor, ime, prm in slucajevi:
        try:
            ocek.append(vb.detail(sql, prm, [], k).replace("~~", ","))
        except vb.Nedefinirano:
            ocek.append(None)
    ulaz = "\n".join("%d\t%s\t%s\t\t0" % (i, sd.b64(s), sd.b64(p)) for i, (s, _, _, p) in enumerate(slucajevi)) + "\n"
    env = dict(os.environ, MONO_PATH=os.path.dirname(exe))
    out = subprocess.run(["mono", exe, "detail"], input=ulaz, capture_output=True, text=True, env=env, check=True).stdout
    cs = {}
    for red in out.splitlines():
        i, v = red.split("\t", 1)
        cs[int(i)] = v if v.startswith("!") else base64.b64decode(v).decode("utf-8")
    raz = collections.defaultdict(list)
    for i, (sql, izvor, ime, prm) in enumerate(slucajevi):
        if ocek[i] is not None and cs.get(i) != ocek[i]:
            raz[ime].append((izvor, sql, prm, ocek[i], cs.get(i)))
    put = os.path.join(sd.ROOT, "docs", "pregled", "D2-detail-razlike.tsv")
    with io.open(put, "w", encoding="utf-8", newline="\n") as f:
        f.write("scenarij\tgranula\tatribut\tsql\tparametri\tnetgen\tcarpo\n")
        for ime, l in raz.items():
            for izvor, sql, prm, o, c in l:
                f.write("\t".join(x.replace("\t", " ").replace("\n", "\\n").replace("\r", "") for x in (ime, izvor[0], izvor[1], sql, prm, o, c or "")) + "\n")
    for ime in ("grid", "prefiks"):
        n = sum(1 for s in slucajevi if s[2] == ime)
        print("detail %-8s slucajeva=%d razlika=%d" % (ime, n, len(raz.get(ime, []))))

if __name__ == "__main__":
    main()
