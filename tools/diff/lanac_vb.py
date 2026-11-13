# -*- coding: utf-8 -*-
"""Doslovni model prepoznavanja segmenta lanca u Form2.Rafalno (Form2.vb 6821-7509).

Vraca oznaku grane koju bi VB izvrsio za segment (nakon zamjene @@).
Redoslijed je redoslijed ElseIf grana; Contains je na Trim.ToUpper.
"""

GRANE = [
    # (uvjet, oznaka)
    ("eq:", "prazno"),
    ("eq:NULL", "prazno"),
    ("in:BEEP()", "beep"),
    ("in:DISABLEALLCONTROLS()", "svekontrole"),
    ("in:ENABLEALLCONTROLS()", "svekontrole"),
    ("in:OPENFORM(", "forma-otvori"),
    ("in:PROCESSSTART(", "proces"),
    ("in:SHOWFOLDERBROWSERDIALOG()", "mapa"),
    ("in:SHOWOPENFILEDIALOG()", "datotekadlg"),
    ("in:TIMERON(", "timer"),
    ("in:TIMEROFF()", "timeroff"),
    ("in:PROGRESSON()", "progres"),
    ("in:PROGRESSOFF()", "progres"),
    ("in:BLAGAJNAENABLE(", "blagajna"),
    ("in:STARTSEARCH(", "trazi"),
    ("pre::", "forma"),
    ("eq:OKSTAY", "okstay"),
    ("eq:OK", "ok"),
    ("in:POSALJIRACUNENASERVER(", "posalji"),
    ("in:POSALJIZAKLJUCKENASERVER(", "posalji"),
    ("in:POSALJIPORUKENASERVER(", "posalji"),
    ("in:REFRESHCONTROL(", "refresh"),
    ("in:DOCK5(", "dock"), ("in:DOCK3(", "dock"), ("in:DOCK2(", "dock"), ("in:DOCK1(", "dock"), ("in:DOCK4(", "dock"),
    ("in:ISEMPTY(", "isempty"),
    ("in:PRINTGRID(", "ispis"),
    ("in:RECALCGRID(", "recalc"),
    ("in:RECALCGRIDROW(", "recalcred"),
    ("in:TOGGLEGRIDGROUP(", "toggle"),
    ("in:PRINTREPXGRID(", "ispisext"),
    ("in:PIVOTGRID(", "pivot"),
    ("in:CHARTGRID(", "chart"),
    ("in:GRID_COLUMN_VALUE(", "stupac"),
    ("eq:CANCEL", "odustani"),
    ("eq:CLOSE", "odustani"),
    ("eq:CLOSEPARENT", "roditelj"),
    ("in:PREPAREMULTISELECTEDPRINT(", "priprema"),
    ("in:FOREACHSELECTED(", "foreach"),
    ("in:FOREACHBLUEROW(", "foreach"),
    ("in:FOREACHROW(", "foreach"),
    ("in:MASTERMOVENEXT", "sljedeci"),
    ("in:MASTERMOVEPREVIOUS", "prethodni"),
    ("in:ISEMPTYORZEROALERT(", "isemptyorzero"),
    ("in:EVAL(", "eval"),
    ("in:ISEMPTYORZEROCANCELBOX(", "isemptyorzero"),
    ("in:CLICK(", "klik"),
    ("eq:COLORPICKER()", "boja"),
    ("in:MSGBOX(", "poruka"),
    ("in:CREATETEXTFILE1(", "datoteka"),
    ("in:CREATETEXTFILE2(", "datoteka"),
    ("in:CREATETEXTFILE3(", "datoteka"),
    ("in:CREATETEXTFILEUTF8(", "datoteka"),
    ("in:.VBS", "pokretanje"),
    ("in:.BAT", "pokretanje"),
    ("in:CREATEOBJECT(", "objekt"),
    ("in:ASSEMBLYLOAD(", "assembly"),
    ("in:FISKALIZACIJA(", "fiskal"),
    ("in:FISKALIZACIJAN(", "fiskal"),
    ("in:FISKALIZACIJAFAKTURE(", "fiskal"),
    ("in:FISKALIZACIJABSRACUNI(", "fiskal"),
    ("in:FISKALIZACIJAPD(", "fiskal"),
    ("in:FISKALIZACIJAPONUDE(", "fiskal"),
    ("in:POSLOVNIPROSTOR(", "prostor"),
    ("in:SLOPP(", "slopp"),
]


def prepoznaj(segment):
    t = segment.strip()
    u = t.upper()
    for uvjet, oznaka in GRANE:
        vrsta, tekst = uvjet.split(":", 1)
        if vrsta == "eq" and u == tekst:
            return oznaka
        if vrsta == "in" and tekst in u:
            return oznaka
        if vrsta == "pre" and t[:1] == tekst:
            return oznaka
    return "sql"  # Else: RefreshControls(Ar5(i))
