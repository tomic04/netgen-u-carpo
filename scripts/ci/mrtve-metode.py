# -*- coding: utf-8 -*-
"""Javne i interne metode Carpo.NetGen koje zovu samo testovi ruse build.

Ulaz iz produkcije je svaki .cs izvan test projekata (GridTest, Granule, tests).
TestShell je host, nije test. Iznimke su u scripts/ci/mrtve-iznimke.txt.
"""
from __future__ import print_function

import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
NETGEN = os.path.join(ROOT, "Devex_26", "Carpo.NetGen")
IZNIMKE = os.path.join(ROOT, "scripts", "ci", "mrtve-iznimke.txt")

TEST_DIOVI = (
    "Carpo.NetGen.GridTest",
    "Carpo.NetGen.Granule",
    os.path.join("tests", ""),
    os.path.join("tests", "/"),
)


def je_test(put):
    p = put.replace("/", os.sep)
    if "Carpo.NetGen.GridTest" in p or "Carpo.NetGen.Granule" in p:
        return True
    if os.sep + "tests" + os.sep in p:
        return True
    return False


def datoteke_cs(korijen):
    izlaz = []
    for dirpath, dirs, files in os.walk(korijen):
        dirs[:] = [d for d in dirs if d not in ("bin", "obj", ".git")]
        for f in files:
            if f.endswith(".cs") and not f.endswith(".Designer.cs"):
                izlaz.append(os.path.join(dirpath, f))
    return izlaz


TOKENI = re.compile(
    r'@"(?:""|[^"])*"'          # verbatim string
    r'|"(?:\\.|[^"\\\n])*"'     # obican string
    r"|'(?:\\.|[^'\\\n])+'"     # char
    r"|//[^\n]*"                # komentar do kraja reda
    r"|/\*.*?\*/",             # blok komentar
    re.S,
)


def ocisti(tekst):
    """Stringovi i komentari u jednom prolazu, redom kako se pojave.

    Prije su se prvo brisali komentari pa stringovi. Tada "https://x" u stringu
    izgleda kao komentar, navodnik ostane bez para i sljedeci string proguta
    stotine redova koda (Podloga.cs: 1747 redova postane 1057).
    Broj redova ostaje isti da linije u ispisu MRTVO budu tocne.
    """
    def zamjena(m):
        t = m.group(0)
        n = t.count("\n")
        if t.startswith("//") or t.startswith("/*"):
            return " " + "\n" * n
        if t.startswith("'"):
            return "''"
        return '""' + "\n" * n

    return TOKENI.sub(zamjena, tekst)


METODA = re.compile(
    r"\b(public|internal)\s+"
    r"(?:(?:static|virtual|override|sealed|async|new|unsafe|partial|extern|readonly|volatile|abstract)\s+)*"
    r"([A-Za-z0-9_<>,\[\]\.?]+(?:\s+[A-Za-z0-9_<>,\[\]\.?]+)*)\s+"
    r"([A-Za-z_][A-Za-z0-9_]*)\s*\(",
    re.M,
)


def klase_u(tekst):
    """Redoslijed deklaracija klasa: (indeks, ime)."""
    nadj = []
    for m in re.finditer(r"\b(?:class|struct|interface)\s+([A-Za-z_][A-Za-z0-9_]*)", tekst):
        nadj.append((m.start(), m.group(1)))
    return nadj


def klasa_za(klase, indeks):
    ime = ""
    for i, k in klase:
        if i <= indeks:
            ime = k
        else:
            break
    return ime


def metode(put, tekst):
    klase = klase_u(tekst)
    izlaz = []
    for m in METODA.finditer(tekst):
        ime = m.group(3)
        if ime in ("if", "for", "while", "switch", "catch", "lock", "using", "foreach", "return", "new"):
            return_skip = True
        else:
            return_skip = False
        if return_skip:
            continue
        prethodno = tekst[max(0, m.start() - 40):m.start()]
        if re.search(r"\b(class|struct|interface|enum|namespace)\s*$", prethodno):
            continue
        tip = m.group(2).strip()
        if tip in ("class", "struct", "interface", "enum", "namespace", "event", "delegate"):
            continue
        klasa = klasa_za(klase, m.start())
        if not klasa or ime == klasa:
            continue
        if ime.startswith("get_") or ime.startswith("set_"):
            continue
        linija = tekst[:m.start()].count("\n") + 1
        izlaz.append((klasa, ime, linija, put))
    return izlaz


def ucitaj_iznimke():
    dozvoli = set()
    if not os.path.isfile(IZNIMKE):
        return dozvoli
    for linija in open(IZNIMKE, encoding="utf-8"):
        s = linija.split("#", 1)[0].strip()
        if not s:
            continue
        dijelovi = s.split()
        if len(dijelovi) < 1:
            continue
        kljuc = dijelovi[0]
        if "." not in kljuc:
            continue
        razlog = linija.split("#", 1)[1].strip() if "#" in linija else ""
        if len(razlog) < 20:
            print("IZNIMKA bez razloga: " + kljuc)
            continue
        dozvoli.add(kljuc)
    return dozvoli


def main():
    if not os.path.isdir(NETGEN):
        print("Nema " + NETGEN)
        return 2
    sve = []
    for dirpath, dirs, files in os.walk(os.path.join(ROOT, "Devex_26")):
        dirs[:] = [d for d in dirs if d not in ("bin", "obj", ".git")]
        for f in files:
            if f.endswith(".cs"):
                sve.append(os.path.join(dirpath, f))
    for dirpath, dirs, files in os.walk(os.path.join(ROOT, "tests")):
        dirs[:] = [d for d in dirs if d not in ("bin", "obj")]
        for f in files:
            if f.endswith(".cs"):
                sve.append(os.path.join(dirpath, f))

    cisto = {}
    for p in sve:
        try:
            t = open(p, encoding="utf-8", errors="replace").read()
        except OSError:
            continue
        cisto[p] = ocisti(t)

    deklaracije = []
    for p in datoteke_cs(NETGEN):
        if p not in cisto:
            continue
        deklaracije.extend(metode(p, cisto[p]))

    # Jedinstveni par klasa.metoda. Isti par u više datoteka ostaje jedan.
    jedinstvene = {}
    for klasa, ime, linija, put in deklaracije:
        kljuc = klasa + "." + ime
        jedinstvene.setdefault(kljuc, (linija, put))

    iznimke = ucitaj_iznimke()
    mrtve = []
    for kljuc, (linija, put) in sorted(jedinstvene.items()):
        if kljuc in iznimke:
            continue
        klasa, ime = kljuc.split(".", 1)
        poziv = re.compile(r"\b" + re.escape(ime) + r"\s*\(")
        produkcija = False
        test = False
        for p, tekst in cisto.items():
            if p == put and tekst.count("\n") and False:
                pass
            # Deklaracija se ne broji: makni potpis, ostavi tijelo i ostale pozive.
            if not poziv.search(tekst):
                continue
            if p == put:
                # U istoj datoteci traži poziv koji nije sam potpis.
                tijelo = poziv.sub(" ", tekst, count=1)
                if not poziv.search(tijelo):
                    continue
            if je_test(p):
                test = True
            else:
                produkcija = True
                break
        if produkcija:
            continue
        if test or not produkcija:
            rel = os.path.relpath(put, ROOT)
            mrtve.append((kljuc, rel, linija, "test" if test else "niko"))

    print("Javne i interne metode: %d" % len(jedinstvene))
    print("Mrtve (samo test ili nitko ne zove): %d" % len(mrtve))
    for kljuc, rel, linija, vrsta in mrtve:
        print("MRTVO %s %s:%d %s" % (kljuc, rel, linija, vrsta))
    kuka = qa_kuka_u_produkciji()
    for rel, linija in kuka:
        print("QA-KUKA %s:%d produkcijski kod spominje TestHooks" % (rel, linija))
    if mrtve:
        print("Build staje. Ulaz iz Dijalog/Podloga/TestShell, ili iznimka s razlogom u scripts/ci/mrtve-iznimke.txt.")
        return 1
    if kuka:
        print("Build staje. Loader, frm i Podloga ne smiju dosegnuti GranulaSesija/GranulaIzvrsenje (PRD-03).")
        return 1
    return 0


QA_KUKA = re.compile(r"\b(?:TestHooks|GranulaSesija|GranulaIzvrsenje)\b")


def qa_kuka_u_produkciji():
    """PRD-03: QA kuka je samo za testove. Komentari se ne broje."""
    nadjeno = []
    mapa_kuke = os.path.join(NETGEN, "TestHooks") + os.sep
    for put in datoteke_cs(NETGEN):
        if put.startswith(mapa_kuke) or je_test(put):
            continue
        with open(put, encoding="utf-8-sig", errors="replace") as f:
            for i, red in enumerate(f, 1):
                kod = red.split("//", 1)[0]
                if QA_KUKA.search(kod):
                    nadjeno.append((os.path.relpath(put, ROOT), i))
    return nadjeno


if __name__ == "__main__":
    sys.exit(main())
