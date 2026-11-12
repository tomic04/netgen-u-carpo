# KntBlaZak — statički čvor, tri storea, detail

XML: `definitions/netgen/BIOVEGA/KntBlaZak.xml`.

## Stablo

`TREE` ima četiri segmenta odvojena s `#` (`Form1.vb` 4344):

1. `Blagajne:` — lijevo od `:` nema podstring `tree`, pa je statički čvor (`4352-4355`). Natpis `Blagajne`, tag `:` (prazan desni dio). Klik: `@@node` prazan, ime forme prazno, `NovaGenerickaForma` izađe (`4546`, `7185-7187`).
2. `spBlagajneZakTreeNG @action=2,@operateriid=@@operateriid:MJESEC_FORM0`
3. isti store `@action=0` → `MJESEC_FORM`
4. isti store `@action=1` → `MJESEC1_FORM`

Segmenti 2–4 imaju `tree` u imenu store **lijevo** od `:`, pa svaki zove `DazovaIzStore` i doda **svoj** korijen (`4350`, `4480`). Nisu djeca čvora Blagajne.

`@@operateriid` se zamijeni prije `Fill` (`4442`). Adapter nije `ExecSql`.

Pravila stupaca i preskok id `0` ista su kao u `Statusi.md` (`4450`, `4498`).

Splitter 290 (`4326`). Nema `COMBO`.

## Master i detail

Početni master: `ONLOAD="spBLZakljucakQueryNG @action=0"` (`3882`).

Klik na čvor storea otvori `MJESEC_FORM0` / `MJESEC_FORM` / `MJESEC1_FORM`. Nema `INPUT`, `FILLEXPLORER=TRUE`, `ONSUBMIT` nosi `@@node` u navodnicima (`5452`). To postane novi master SQL.

Detail: `spBLPrometGlaQueryNG @Action=10,@Id=@@blagajneid,@BLZakljucciId=@@id`.

`@@id` zamijeni `SrediSQL` iz `GetID` (`2482`, `2294`). `@@blagajneid` nije fiksni token. Ostane za `SrediSQLPosljeNextForme`, koji ga traži među parametrima retka. `PokupiParametreSaGrida` lijepi `@` + `DataMember` (`2564-2572`, `2301-2304`). Ime stupca mora biti `blagajneid`, ne natpis.

Poziv detaila: jednom nakon mastera (`2109`) i na svaku promjenu selekcije (`7413-7416`), osim dok `PunimMasterGrid` (`2270`).

`CLEARCOLUMS=TRUE` na `MASTERCOLUMNS` i `DETAILCOLUMNS`: sve kolone se sakriju, pa se navedene vrate (`7459-7462`). `HEADERALIGNMENT=2` (`7530`). `AGREGATEFUNCTION="0"` (jedno G) nije prazan, pa se dodjela agregata izvrši (`7506`). Širina `VrstaBlagajne` je `2500/14` = 179 (VB `/`, ne cjelobrojno dijeljenje, `7522`).

`CHECKBOXVISIBLE=FALSE`. `TOTALROWVISIBLE` **nema**: grana `FALSE` ne ulazi, total ostaje uključen (`2049-2053`). `PERMISIO=TRUE` zove select nad `ssPermission` (`3414`, `2706`).

`BUTTONS` prepiše gumbe nakon `CMD` (`3770-3776`): Ispis zaključka, Ispis dokumenta, Poništi zaključak. `CMD6CAPTION=""` ne doda gumb.

## Rupe (NetGen, datoteka:linija)

- `Blagajne:` je susjed, ne roditelj storeova (`4347-4356`). Klik na njega ne radi ništa (`4546`).
- Tri storea su tri korijena (`4480`).
- Detail ovisi o imenu stupca `blagajneid` na masteru (`2564`, `2304`). Natpis kolone se ne gleda.
- `CLEARCOLUMS` na detailu vrijedi, ali se kolone slože samo dok je `RootTable` prazan (`2334`). Drugi red mastera ne obnovi popis kolona.
- `AGREGATEFUNCTION=0` se ipak dodjeljuje (`7506`).
