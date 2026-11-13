# JOPPD — stablo, detail, popup s tildom

XML: `definitions/netgen/BIOVEGA/JOPPD.xml`. U `ssMeni` dva ista retka `asm:NetGenDll.Loader:JOPPD.xml`.

Naslov je `JOPPD  v[1.16]` (`Form1.vb` 3524–3525, `VERSION="1.16"`).

`TREE` je `Osvježi drvo:REFRESHTREEFORM` plus `spKOD_JOPPDTree @action='vremenska_podjela':STATUS`. Store grana jer lijevo sadrži `tree` (`Form1.vb` 4350–4351). `REFRESHTREEFORM` je `FILLEXPLORER="onlytree"` i nema kontrola: samo novo stablo (`5475–5476`).

`STATUS` `FILLEXPLORER="TRUE"`, `ONSUBMIT` puni master s `@treenode='@@node'`.

`DETAILGRID="spKOD_JOPPDQuery @action='detalji', @kodjoppdaid = @@id"`. `@@id` je `GetID` (stupac `id`, inače ćelija 2). Popup koristi `@@kodjoppdaid`. To su dva različita tokena.

`TOTALROWVISIBLE="false"` gasi sumu mastera. `DETTOTALROWVISIBLE="TRUE"` ostavlja sumu detaila (`Form1.vb` 2049 i detail grana). `COMMANDTIMEOUT="600"`.

`BUTTONS` su samo `Unos A` i `Postavke`. Ostalo je `POPUPMASTER`. Tilda u `Unesi zapis…~REFRESHMASTERGRID` postane `#` (`4558`). `REFRESHMASTERGRID` ponovi zadnji master SQL (`4570`). `POPUPDETAIL` nema tildu.

`PERMISIO="TRUE"`.

## Rupe

- Dva retka izbornika otvaraju isti XML. Nisu dvije granule.
- `@@id` u detailu i `@@kodjoppdaid` u popupu nisu zamjena.
- `onlytree` ne dira master. Suma mastera je ugašena, suma detaila upaljena.
