# -*- coding: utf-8 -*-
"""Doslovni model VB funkcija SQL sloja NetGena, za diferencijalni test.

Prenosi ponasanje, ne namjeru: VB String.Replace mijenja sve pojave i razlikuje
velika i mala slova, Substring baca iznimku kad je string prekratak, a
On Error Resume Next nastavlja s naredbom iza one koja je pala, i kad je to
naredba unutar If bloka cija je glava pala.

Izvori: netgen/Form2.vb 2876-2920 (SrediSQL2), netgen/Class1.vb 740-853
(SrediSQLPosljeNextForme) i 856-977 (NadjiVrijednostuPrevParametrima).
"""


class Nedefinirano(Exception):
    """VB bi ovdje usao u beskonacnu petlju ili ponasanje ovisi o stanju koje model nema."""


class Kontekst(object):
    def __init__(self, **kw):
        self.master_id = kw.get("master_id", "12")
        self.operater = kw.get("operater", "7")
        self.det_id = kw.get("det_id", "3")
        self.node = kw.get("node", "N1")
        self.temp = kw.get("temp", "TMP")
        self.edited = kw.get("edited", "Kol")  # None = TrenutnaKolona Is Nothing
        self.last = kw.get("last", "LV")
        self.dbl = kw.get("dbl", "Dbl")  # None = DblClickedColumn Is Nothing
        self.version = kw.get("version", "NetGen v10.1.17")
        self.opisni = kw.get("opisni", "OPIS")
        self.focused = kw.get("focused", "ctl")  # None = AktivnaKontola Is Nothing
        self.sel_file = kw.get("sel_file", "F.txt")
        self.sel_path = kw.get("sel_path", "C:\\p")
        self.xml_path = kw.get("xml_path", "C:\\x\\SharedDLLs\\xml\\Test.xml")
        self.jsp = kw.get("jsp", "\\\\srv\\j")
        self.color = kw.get("color", "255")


def _r(s, a, b):
    return s.replace(a, "" if b is None else b)


def sredi_sql2(sql, k):
    # Naredba 1: sve vrijednosti su stringovi, Nothing u Replace brise token.
    for a, b in (("@@ID", k.master_id), ("@@id", k.master_id), ("@@operateriid", k.operater),
                 ("@@OPERATERIID", k.operater), ("@@DETID", k.det_id), ("@@detid", k.det_id),
                 ("@@DetId", k.det_id), ("@@node", k.node), ("@@NODE", k.node),
                 ("@@tempstring", k.temp), ("@@TEMPSTRING", k.temp)):
        sql = _r(sql, a, b)
    # Naredba 2: TrenutnaKolona.Caption; Nothing baci iznimku i cijela naredba se preskoci.
    if k.edited is not None:
        sql = _r(_r(sql, "@@editedcolumn", k.edited), "@@EDITEDCOLUMN", k.edited)
    sql = _r(_r(sql, "@@lastvalue", k.last), "@@LASTVALUE", k.last)
    if k.dbl is not None:
        sql = _r(_r(sql, "@@dblclickedcolumn", k.dbl), "@@DBLCLICKEDCOLUMN", k.dbl)
    sql = _r(sql, "@@VERSION", k.version)
    sql = _r(sql, "@@OPISNIPARAMETRI", k.opisni)
    if k.focused is not None:
        sql = _r(sql, "@@FOCUSEDCONTROL", k.focused)
    sql = _r(sql, "@@selectedfile", k.sel_file)
    sql = _r(sql, "@@selectedpath", k.sel_path)
    if "@@xmlname" in sql.lower():
        ime = k.xml_path[k.xml_path.rfind("\\") + 1:]
        sql = _r(_r(sql, "@@xmlname", ime), "@@XMLNAME", ime)
    if "@@jupitersrvpath" in sql.lower():
        sql = _r(sql, "@@jupitersrvpath", k.jsp)
    sql = _r(_r(sql, "@@selectedcolor", k.color), "@@SELECTEDCOLOR", k.color)
    return sql


def _vb_trim(s):
    return s.strip(" \t\r\n\x0b\x0c\u00a0")


def _sub(s, start, length):
    """VB/.NET String.Substring: baca kad izlazi iz granica."""
    if start < 0 or length < 0 or start + length > len(s):
        raise IndexError("Substring")
    return s[start:start + length]


def nadji(param_ul, svi, cb, datum_za_ispis=False):
    """NadjiVrijednostuPrevParametrima. cb je lista (kljuc, vrijednost) kao CB(j,0), CB(j,1)."""
    rez = ""
    param = _vb_trim(_vb_trim(param_ul).replace("@@", "@"))
    ar2 = _vb_trim(svi).split(",")
    for a in ar2:
        # On Error Resume Next: pad u glavi If-a nastavlja u tijelu, a ono provjerava Err.
        try:
            glava = _sub(_vb_trim(a), 0, len(param)).upper() == param.upper()
        except IndexError:
            continue  # tijelo pocinje s If Err.Number = 0, pa se preskace
        if not glava:
            continue
        eq = a.find("=")
        rez = a[eq + 1:] if eq >= 0 else a
        if "ajdijeva" in param.lower():
            rez = rez.replace("~", ",")
        try:
            dat = _sub(param, 0, 4).upper().startswith("@DAT")
        except IndexError:
            dat = True  # glava pala: Resume Next ulazi u tijelo
        if dat and "." in rez and not datum_za_ispis:
            try:
                rez = _sub(rez, 7, 4) + _sub(rez, 4, 2) + _sub(rez, 1, 2)
            except IndexError:
                pass  # naredba pala, rez ostaje
        break

    try:
        je_datum = _sub(param, 0, 4).upper() == "@DAT" or ("GRID" in param.upper() and "DAT" in param.upper())
    except IndexError:
        je_datum = True  # glava pala, Resume Next ulazi u Then granu
    if je_datum:
        if datum_za_ispis:
            try:
                if "." not in rez:
                    rez = _sub(rez, 7, 2) + "." + _sub(rez, 5, 2) + "." + _sub(rez, 1, 4)
                else:
                    rez = rez.replace(" 0:00:00", "")
            except IndexError:
                pass
            rez = rez.replace("'", "")
        else:
            rez = rez.replace("'", "")
            if _vb_trim(rez) != "":
                if rez != "null":
                    rez = "'" + rez + "'"
    else:
        rez = rez.replace("'", "")

    if _vb_trim(rez) == "":
        rez = "null"

    idi = True
    if _vb_trim(param).lower() == "@napomena" and _vb_trim(rez).lower() == "null":
        rez = ""
        idi = False

    if idi and (rez == "null" or rez == ""):
        for kljuc, vr in cb:
            lijevo = "@" + kljuc.upper()
            desno = ("@" if kljuc.upper().startswith("@") else "") + param.upper() + "="
            if lijevo != desno:
                continue
            try:
                dat2 = param.strip().upper().startswith("@DAT") or ("GRID" in param.upper() and "DAT" in param.upper())
                if dat2:
                    if "." in vr:
                        if datum_za_ispis:
                            rez = vr.replace(" 0:00:00", "")
                        else:
                            rez = "'" + _sub(vr, 6, 4) + _sub(vr, 3, 2) + _sub(vr, 0, 2) + "'"
                    else:
                        if datum_za_ispis:
                            rez = _sub(vr, 6, 2) + "." + _sub(vr, 4, 2) + "." + _sub(vr, 0, 4)
                        else:
                            rez = vr if vr == "null" else "'" + vr + "'"
                else:
                    rez = vr.replace(",", ".") if _je_numeric(vr) else vr
            except IndexError:
                pass
    return rez


def _je_numeric(s):
    t = _vb_trim(s).replace(",", ".")
    if t == "":
        return False
    try:
        float(t)
        return True
    except ValueError:
        return False


GRANICE = "'%#,)*/\\+-.(="


def posle_next_forme(sql, svi, cb, k, datum_za_ispis=False):
    """SrediSQLPosljeNextForme bez prevParametri: svi je strSviParametriuModulu."""
    sql = _vb_trim(sredi_sql2(sql, k))
    if _vb_trim(svi) == "" and "@@" not in sql:
        return sql
    krug = 0
    while "@@" in sql:
        krug += 1
        if krug > 200:
            raise Nedefinirano("VB petlja ne zavrsava")
        i = sql.find("@@")
        sql2 = sql[i:] + " "
        kraj = sql2.find(" ")
        for ch in GRANICE:
            p = sql2.find(ch)
            if -1 < p < kraj:
                kraj = p
        token = sql2[:kraj]
        vr = nadji(token, svi, cb, datum_za_ispis)
        if token in vr:
            raise Nedefinirano("vrijednost sadrzi token, VB petlja ne zavrsava")
        sql = sql.replace(token, vr)
        sql = sql.replace("\u00b8\u00b8", "''")
    return sql


def sredi_sql(sql, k, tablica="t1", trenutna_kolona_dx=""):
    """Form1.SrediSQL (2470-2503). ID je GetID mastera, ovdje k.master_id."""
    for a, b in (("@@Id", k.master_id), ("@@ID", k.master_id), ("@@id", k.master_id),
                 ("@@operateriid", k.operater), ("@@OPERATERIID", k.operater), ("@@Operateriid", k.operater),
                 ("@@DETID", k.det_id), ("@@detid", k.det_id), ("@@DetId", k.det_id),
                 ("@@node", k.node), ("@@NODE", k.node), ("@@table", tablica), ("@@TABLE", tablica)):
        sql = _r(sql, a, b)
    if "@@xmlname" in sql.lower():
        ime = k.xml_path[k.xml_path.rfind("\\") + 1:]
        sql = _r(_r(sql, "@@xmlname", ime), "@@XMLNAME", ime)
    if "@@jupitersrvpath" in sql.lower():
        sql = _r(sql, "@@jupitersrvpath", k.jsp)
    kol = trenutna_kolona_dx if trenutna_kolona_dx else k.edited
    if kol is not None:
        sql = _r(_r(sql, "@@editedcolumn", kol), "@@EDITEDCOLUMN", kol)
    sql = _r(_r(sql, "@@lastvalue", k.last), "@@LASTVALUE", k.last)
    sql = _r(_r(sql, "@@selectedcolor", k.color), "@@SELECTEDCOLOR", k.color)
    return sql


def detail(sql, parametri_sa_grida, cb, k):
    """Form1.PokaziUpitDetalj 2304: SrediSQLPosljeNextForme(SrediSQL(sql).Trim, ParametriSaGrida)."""
    return posle_next_forme(_vb_trim(sredi_sql(sql, k)), parametri_sa_grida, cb, k, False)
