# -*- coding: utf-8 -*-
"""Status portano smije stajati samo uz UI test koji otvara imenovanu granulu.

Katalog je tests/ui/katalog.txt:
    UI<TAB>granula<TAB>test
TestShell ili scripts/ui moraju spomenuti i test i granulu.
XML granule mora postojati pod definitions/.
"""
from __future__ import print_function

import glob
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
KATALOG = os.path.join(ROOT, "tests", "ui", "katalog.txt")


def katalog():
    redovi = {}
    if not os.path.isfile(KATALOG):
        return redovi
    for linija in open(KATALOG, encoding="utf-8"):
        if not linija.strip() or linija.strip().startswith("#"):
            continue
        dijelovi = re.split(r"\t+", linija.strip())
        if len(dijelovi) < 3:
            print("LOSE " + linija.strip())
            continue
        kat, granula, test = dijelovi[0], dijelovi[1], dijelovi[2]
        redovi.setdefault(test, []).append((kat, granula))
    return redovi


def xml_postoji(granula):
    korijen = os.path.join(ROOT, "definitions")
    if not os.path.isdir(korijen):
        return False
    cilj = granula.lower()
    for dirpath, dirs, files in os.walk(korijen):
        for f in files:
            if not f.lower().endswith(".xml"):
                continue
            ime = os.path.splitext(f)[0].lower()
            if ime == cilj or cilj in ime:
                return True
    return False


def spomenuto(test, granula):
    izvori = []
    for put in (
        os.path.join(ROOT, "Devex_26", "Carpo.TestShell"),
        os.path.join(ROOT, "scripts", "ui"),
    ):
        if not os.path.isdir(put):
            continue
        for dirpath, dirs, files in os.walk(put):
            for f in files:
                if f.endswith(".cs") or f.endswith(".ps1"):
                    izvori.append(os.path.join(dirpath, f))
    tekst = []
    for p in izvori:
        try:
            tekst.append(open(p, encoding="utf-8", errors="replace").read())
        except OSError:
            pass
    sve = "\n".join(tekst)
    return test in sve and granula in sve


def redovi_portano():
    izlaz = []
    for put in glob.glob(os.path.join(ROOT, "docs", "pregled-funkcija*.md")):
        linije = open(put, encoding="utf-8").read().splitlines()
        for n, linija in enumerate(linije, 1):
            if not linija.startswith("|"):
                continue
            celije = [c.strip() for c in linija.strip().strip("|").split("|")]
            if len(celije) < 2:
                continue
            status = ""
            test = ""
            for c in celije:
                if c in ("portano", "nedostajalo-portano") or c.startswith("portano ") or c.startswith("nedostajalo-portano"):
                    status = c
                if c.startswith("`") and status:
                    test = c
            # Zadnja celija je test kad tablica ima stupac Test.
            if celije[-1] and celije[-1] not in ("Test", "Status", "Carpo", "NetGen"):
                if "portano" in status:
                    test = celije[-1]
            if "portano" not in status:
                continue
            if status in ("Status",):
                continue
            izlaz.append((put, n, status, test, linija))
    return izlaz


def main():
    kat = katalog()
    greske = []
    for test, stavke in kat.items():
        for kategorija, granula in stavke:
            if kategorija != "UI":
                greske.append("katalog %s nije UI" % test)
            if not granula or granula == "*":
                greske.append("katalog %s nema granulu" % test)
            elif not xml_postoji(granula):
                greske.append("katalog %s granula %s nema XML" % (test, granula))
            if not spomenuto(test, granula):
                greske.append("katalog %s / %s nije u TestShell ni u scripts/ui" % (test, granula))
    for put, n, status, test, _linija in redovi_portano():
        rel = os.path.relpath(put, ROOT)
        imena = re.findall(r"[A-Za-z0-9_.-]+", test.replace("`", ""))
        if not imena or imena == ["|"]:
            greske.append("%s:%d status %s nema test" % (rel, n, status))
            continue
        pogodjen = False
        for ime in imena:
            if ime in kat:
                pogodjen = True
                for kategorija, granula in kat[ime]:
                    if kategorija != "UI" or not granula:
                        greske.append("%s:%d test %s nije UI granula" % (rel, n, ime))
        if not pogodjen:
            greske.append("%s:%d status %s test nije UI katalog (%s)" % (rel, n, status, test))
    print("Redaka portano: %d" % len(redovi_portano()))
    print("Katalog UI: %d" % len(kat))
    for g in greske:
        print("PORTANO " + g)
    if greske:
        print("Status portano trazi tests/ui/katalog.txt: UI, ime granule, test koji TestShell otvara.")
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
