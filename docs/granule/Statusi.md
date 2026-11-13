# Statusi — lijevo stablo i master-detail

XML: `definitions/netgen/Agronom granule/Statusi.xml`. Tipičan raspored: SQL stablo lijevo, master iz `ONLOAD`, detail iz `DETAILGRID`, klik na čvor zamijeni master.

## Stablo

`TREE="spTreeVrstaDok:POPUNI_FORM"`.

`NapuniDrvoVrijeme` (`Form1.vb` 4296) dijeli na `#`. Lijevo od prve `:` je `spTreeVrstaDok`. To sadrži podstring `tree` (`4350`), pa ide `DazovaIzStore`, ne statički čvor. Desno je ime forme `POPUNI_FORM`.

Store se ne šalje kroz `ExecSql`. Ide `SrediSQL` + `SrediSQLPosljeNextForme`, zatim `SqlDataAdapter.Fill` (`4442-4447`). Preskok ako `@@` preživi (`Class1.vb` 113) ovdje **ne vrijedi**.

Četiri stupca (`4450`, `4461-4478`):

| Stupac | Uloga |
|---|---|
| 0 | id, roditelj relacije `TreeParentChild` |
| 1 | parentid, dijete relacije |
| 2 | natpis čvora |
| 3 | tag |

Korijen je **samo prvi red** (`4455`). Djeca idu relacijom. Red čije je `id` tekst `"0"` se kao dijete preskače (`4498`). Ako stupac 3 već ima `:`, tag je cijeli stupac. Inače je `vrijednost:POPUNI_FORM` (`4474-4477`).

Nema `SPLITTERDISTANCE`: splitter je 290, stablo vidljivo, gumb drveta uključen (`4322-4330`). Nema `COMBO`.

`NetGenDBFilter` doda još čvorova ako tablica ima red za ime fajla (`4359-4369`). Greška te tablice se guta (`On Error Resume Next`).

## Klik i master

`TreeView2_AfterSelect` (`7182-7188`): `@@node` je tag do prve `:`, ime forme je ostatak, pa `NovaGenerickaForma`, pa fokus natrag na stablo.

`POPUNI_FORM` nema `INPUT`. `BrojKontrola` ostaje 0, dijalog se ne otvara, `OKClick` postane true (`5369-5370`). `FILLEXPLORER=TRUE` i `ONSUBMIT="spStatusiQuery @action=0, @node=@@node"` zamijene master (`5445-5452`). `@@node` je ono što je AfterSelect spremio.

Prije ijednog klika master je `ONLOAD="spStatusiQuery @action=0, @node=0"`. To je literalna nula, ne `@@node` (`3882`).

## Detail

`DETAILGRID="spStatusiQuery @action=1, @ID=@@ID"`.

`PokaziUpit` na kraju, kad `PunimMasterGrid` padne na false, zove `PokaziUpitDetalj` (`2099-2109`). Usred punjenja mastera detail izađe odmah (`2270`). `GridEX1_SelectionChanged` zove detail opet ako master ima redova (`7413-7416`). `@@ID` je `GetID` (`2294-2295`).

`DETCHECKBOXVISIBLE=FALSE` sakrije stupac 1 detaila (`2402-2406`). `DETTOTALROWVISIBLE=FALSE` gasi total detaila (`2408-2410`).

`CreateGridColumns` na detailu samo ako je `RootTable` još prazan (`2334-2336`). Drugi upit zadrži stare kolone i samo veže podatke. `CustomFormatKolona` ide svaki put (`2347`).

Dvostruki klik mastera: `UPDATEFORM` (`6935`). Dvostruki klik detaila: `OPFORM` (`7201-7202`).

Naslov je `Statusi  v[1.1]` (`3524-3525`, dva razmaka).

## Rupe (NetGen, datoteka:linija)

- Stablo ne ide kroz `ExecSql`. `@@` koji preživi zamjenu se ipak pošalje (`Form1.vb` 4446-4447 naspram `Class1.vb` 113-118).
- U stablo uđe samo podstablo prvog reda. Ostali korijeni iz store ne (`4455`, `4492`).
- Dijete s id `0` se ne crta (`4498`).
- Prazna forma s `FILLEXPLORER=TRUE` ne pokaže prozor, ali prepiše master (`5369`, `5452`).
- Detail kolone se slože samo prvi put (`2334`).
