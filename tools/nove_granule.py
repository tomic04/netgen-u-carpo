# -*- coding: utf-8 -*-
"""Brzi pregled novih NetGen granula prije nego udu u korpus (definitions/netgen/<server>/).

Upotreba:
  python3 tools/nove_granule.py <mapa s novim XML-ovima> [<mapa> ...]

Ispise:
  - XML-ove koji se ne mogu procitati (kodiranje, neispravan XML) i one koji nisu NetGen (<PROJECT>)
  - TYPE vrijednosti kojih nema u postojecem korpusu (definitions/), s granom koju bi NetGen uzeo
  - atribute (po elementu: PROJECT, forma, INPUT, COLUMN) koje Carpo.NetGen ne poznaje kao string
    literal, a NetGen VB ih cita (GetNamedItem), te one koje ne cita ni NetGen
  - naredbe lanca (XXX( u ON*/CMD*/POPUP) kojih nema u Carpo.NetGen

Literal u kodu ne dokazuje isto ponasanje (vidi tools/pregled_b3_xml.py); popis je za rucni pregled.
"""
from __future__ import print_function

import collections
import io
import os
import re
import sys
import xml.etree.ElementTree as ET

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
CARPO = os.path.join(ROOT, "Devex_26", "Carpo.NetGen")
NETGEN = os.path.join(ROOT, "netgen")
KORPUS = os.path.join(ROOT, "definitions")
LITERAL = re.compile(r'"((?:\\.|[^"\\\n])*)"')
NAREDBA = re.compile(r"\b([A-Z][A-Z0-9_]{2,})\(", re.I)
# Imena koja Carpo slaze iz dijelova ("CMD" + n + "CAPTION"), pa nisu literal.
DINAMICKI = re.compile(r"^CMD\d+CAPTION$")
NE_FORME = {"NAME", "CAPTION", "KOMENTAR", "HISTORY", "MASTERCOLUMNS", "DETAILCOLUMNS", "DETAILCOLUMNS2",
            "DETAILCOLUMNS3", "DETAILCOLUMNS4"}
# Tim redom NetGen bira kontrolu (InStr nad TYPE), vidi Xml/NgGrana.cs.
GRANE = ["grid", "text", "number", "cur", "combo", "dtpicker", "date", "trazilica", "search", "check", "radio",
         "label", "frame", "group", "panel", "button", "help", "tree", "multidrop", "statusstrip", "picture",
         "webbrowser", "split"]


def literali_carpa():
    s = set()
    for dp, _, fn in os.walk(CARPO):
        if os.sep + "obj" + os.sep in dp or os.sep + "bin" + os.sep in dp:
            continue
        for f in fn:
            if f.endswith(".cs"):
                txt = io.open(os.path.join(dp, f), encoding="utf-8", errors="replace").read()
                for m in LITERAL.finditer(txt):
                    s.add(m.group(1).upper())
    return s


def atributi_netgena():
    s = set()
    for f in os.listdir(NETGEN):
        if f.lower().endswith(".vb"):
            txt = io.open(os.path.join(NETGEN, f), encoding="utf-8", errors="replace").read()
            for m in re.finditer(r'GetNamedItem\("([A-Za-z0-9_]+)"\)', txt):
                s.add(m.group(1).upper())
    return s


def ucitaj(put):
    try:
        return ET.parse(put).getroot(), None
    except Exception as ex:  # noqa: BLE001 - samo izvjestaj
        return None, str(ex)


def postojeci_tipovi():
    t = set()
    for dp, _, fn in os.walk(KORPUS):
        for f in fn:
            if f.lower().endswith(".xml"):
                r, _ = ucitaj(os.path.join(dp, f))
                if r is not None and r.tag.upper() == "PROJECT":
                    for i in r.iter("INPUT"):
                        t.add(i.attrib.get("TYPE", "").strip().lower())
    return t


def grana(tip):
    for g in GRANE:
        if g in tip:
            return g
    return "(nista: NetGen je preskoci)"


def main(mape):
    carpo = literali_carpa()
    netgen = atributi_netgena()
    poznati_tipovi = postojeci_tipovi()
    pad, nije, ukupno = [], [], 0
    novi_tipovi = collections.Counter()
    atr = collections.defaultdict(collections.Counter)
    naredbe = collections.Counter()
    for mapa in mape:
        for dp, _, fn in os.walk(mapa):
            for f in fn:
                if not f.lower().endswith(".xml"):
                    continue
                put = os.path.join(dp, f)
                ukupno += 1
                r, greska = ucitaj(put)
                if r is None:
                    pad.append((put, greska))
                    continue
                if r.tag.upper() != "PROJECT":
                    nije.append(put)
                    continue
                for el in r.iter():
                    if el is r:
                        kontekst = "PROJECT"
                    elif el.tag == "INPUT":
                        kontekst = "INPUT"
                    elif el.tag == "COLUMN":
                        kontekst = "COLUMN"
                    elif el in list(r) and el.tag.upper() not in NE_FORME:
                        kontekst = "forma"
                    else:
                        kontekst = el.tag
                    for k, v in el.attrib.items():
                        if v.strip() and k.upper() not in carpo and not DINAMICKI.match(k.upper()):
                            atr[(kontekst, k.upper())][f] += 1
                        if k.upper().startswith(("ON", "CMD", "POPUP", "BUTTONS", "AFTER", "BEFORE")):
                            for m in NAREDBA.finditer(v):
                                n = m.group(1).upper() + "("
                                if n.upper() not in carpo and m.group(1).upper() not in carpo:
                                    naredbe[n] += 1
                    if el.tag == "INPUT":
                        tip = el.attrib.get("TYPE", "").strip().lower()
                        if tip not in poznati_tipovi:
                            novi_tipovi[tip] += 1
    print("XML-ova: %d, ne cita se: %d, nije NetGen: %d" % (ukupno, len(pad), len(nije)))
    for put, g in pad[:30]:
        print("  PAD   %s  %s" % (put, g))
    print("\nNovi TYPE (nema ih u definitions/):")
    for t, n in novi_tipovi.most_common():
        print("  %-28s %4d  grana: %s" % (t or "(prazno)", n, grana(t)))
    print("\nAtributi koje Carpo.NetGen ne poznaje (NetGen cita = VB ih cita GetNamedItem):")
    for (kont, ime), gr in sorted(atr.items(), key=lambda x: -sum(x[1].values())):
        print("  %-8s %-28s %5d puta, %3d granula, NetGen cita: %s" % (
            kont, ime, sum(gr.values()), len(gr), "da" if ime in netgen else "ne"))
    print("\nNaredbe lanca bez literala u Carpo.NetGen:")
    for n, c in naredbe.most_common():
        print("  %-30s %4d" % (n, c))


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(__doc__)
        sys.exit(2)
    main(sys.argv[1:])
