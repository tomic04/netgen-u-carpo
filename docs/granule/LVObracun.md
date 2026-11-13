# LVObracun — stablo, detail po `@@lvglaid`, poziv forme s parametrima

XML: `definitions/netgen/BIOVEGA/LVObracun.xml`. U `ssMeni`: `asm:NetGenDll.Loader:LVObracun.xml`.

## Podloga

`ONLOAD="spLVGla_Query @action='master', @treenode='0'"`. Nije `:`, pa je SQL mastera (`Form1.vb` 3879–3882).

`DETAILGRID` veže `@lvglaid=@@lvglaid`. `@@id` se ne koristi. Vrijednost dolazi iz stupca `lvglaid` na masteru (`PokupiParametreSaGrida`, ime stupca, ne natpis).

`TREE="Osvježi drvo:REFRESHTREEFORM#spLVGla_Tree @action='vrpodjela':STATUS#spLVGla_Tree @action='abc':STATUS"`. Prvi segment je statički čvor. Druga dva sadrže `tree` lijevo od `:`, pa `DazovaIzStore` (`4347–4351`).

`REFRESHTREEFORM` nema `INPUT`, `FILLEXPLORER="onlytree"`. Prazna forma se ne prikaže (`Form2.vb` 4430). `onlytree` samo ponovo puni stablo (`Form1.vb` 5475–5476), master SQL se ne šalje.

`STATUS` je prazna forma, `FILLEXPLORER="TRUE"`, `ONSUBMIT` postaje master s `@@node` (`5445–5452`).

`SETDEFAULTS="spLVGla_DefaultsNG @action=0"` nema `#`, jedan `ExecSql` (`3862–3866`). `PERMISIO="TRUE"`.

## Gumbi

`BUTTONS` ima devet stavki, sve oblika `ImeForme(@param=@@x)`. `NovaGenerickaForma2` odreže zagradu (`Form2.vb` 4407–4418): ime forme je do `(`, parametri se lijepe na `strParametriSvi`. `-freezid` u tom nizu bi zabranio promjenu id-a (`4413–4415`); ovdje ga nema.

`MASTERDBLCLICK=":UPDATELVGLA(@lvglaid=@@lvglaid)"`. Vodeća `:` i dalje otvara formu (`Form1.vb` 4862–4867), zagrada se skine isto.

`DETAILDBLCLICK` je `:UPDATELVDET(...)`.

Na dijalogu `FOREACHBLUEROW(grdet)` vrti plave retke grida (`Form2.vb` 7043–7045). Na podlozi ista riječ ne vrti petlju.

`datobracuna` i `datisplate` imaju `SHOWCHECKBOX="true"`: kvačica krene ugašena, parametar je `null` dok je se ne upali (`2275–2278`, `3560–3567`).

## Rupe

- `onlytree` ne osvježava master.
- `@@lvglaid` nije `@@id`. Ako master nema taj stupac, detail dobije prazno.
- Parametri u zagradi nisu dio imena forme.
