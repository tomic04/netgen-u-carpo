# PrijemniList — combo puni stablo, detail ispod mastera

XML: `definitions/netgen/Agronom granule/PrijemniList.xml`. U `ssMeni` je `asm:NetGenDLL.loader:PrijemniList.xml` (dokument 06). Atributa `TREE` nema. Lijevo stablo dolazi iz `COMBO`.

## Combo umjesto TREE

`COMBO="exec spNg_combo @action = 'Drvo_NabPri'"`.

`ExecSql` napuni combo. Stupac 0 je `ValueMember`, stupac 1 `DisplayMember` (`Form1.vb` 3340-3344). Vrijednost mora biti tekst istog oblika kao `TREE` (segmenti `#`, unutar segmenta `lijevo:forma`).

`ComboBox1_SelectedValueChanged` (`7730-7733`) upiše `SelectedValue` u `strTreeNodes` i zove `NapuniDrvoVrijeme`.

Prvi prolaz, dok je `TREE` atribut prazan, a combo još nije prepisao `strTreeNodes` (`4310-4320`):

- sakrivanje stabla traži i prazan combo; combo nije prazan, pa se stablo ne gasi
- procedura ipak izađe prije `SplitterDistance`, prije `ButtonZaDrvo.Enabled = True` i prije crtanja čvorova

Tek drugi poziv, s vrijednošću iz comboa, uđe u granu nepraznog stabla (`4322-4330`). `SPLITTERDISTANCE="180"` tada postavi lijevi splitter (`4323-4324`).

## Master i detail

Nema `ONLOAD` ni `STARTUPSTORA`. `PokaziUpit("")` izađe (`1830`). Master ostane prazan dok combo/stablo ili filter ne dadu SQL.

`DETAILGRID` je batch:

`declare @robaid int exec spNabPriDetKKvalQuery_NG @action = 2, @NabPriGlaId = @@id, @robaid = @robaid`

Ne počinje s `:`, pa nije poruka `sorkac` (`2105-2109`). Ide u `PokaziUpitDetalj`. `@@id` je `GetID` (`2294`). `@robaid` u batchu je SQL varijabla, ostaje `int` koji declare stvori; NetGen je ne puni iz grida.

Detail se zove nakon mastera (`2109`) i na `SelectionChanged` ako ima redova (`7413-7416`). Dok se master puni, `PunimMasterGrid` detail preskoči (`2270`).

`DETCHECKBOXVISIBLE=FALSE` (`2402`). `CHECKBOXVISIBLE=FALSE` (`2043`). `GROUPBYBOXVISIBLE=TRUE` (`3350`). `COMMANDTIMEOUT=300`.

`DETAILCOLUMNS`: `Kolicina` širina `1000/14` = 71, format `#,####0.0000` (`7522-7524`). `AGGREGATEFUNCTION` je prazan dvo-G, agregat se ne veže (`7496`). `CLEARCOLUMS=false`.

## Gumbi i dvostruki klik

`MASTERDBLCLICK=":IzmijeniPL#REFRESHMASTER"` (`6935`, `4560`).

- `:IzmijeniPL` otvori formu `IzmijeniPL` (`Replace` dvotočke u `NovaGenerickaForma2`)
- `REFRESHMASTER` **ne** sadrži `REFRESHMASTERGRID` (`4570`). Traži se forma tog imena. Nije sinonim za osvježenje grida.

Isto vrijedi za `CMD2CAPTION` (`&Izmijeni:IzmijeniPL#REFRESHMASTER`). `&` je mnemonik WinFormsa.

`CMD1` je `&Unesi:UnesiPL`.

## Rupe (NetGen, datoteka:linija)

- Nema `TREE`. Stablo postoji tek kad combo prepiše `strTreeNodes` (`7730-7733`). Prvi `NapuniDrvoVrijeme` izađe bez čvorova i bez enable gumba (`4310-4320`).
- Kostur koji vidi samo atribut `TREE` ostavi ovu granulu bez stabla. Combo je drugi ulaz u isti `NapuniDrvoVrijeme`.
- `REFRESHMASTER` ne osvježava master (`4570`). Riječ koja to radi je `REFRESHMASTERGRID`.
- Detail `@robaid` je lokalna SQL varijabla u `declare`, nije `@@robaid` (`DETAILGRID` na projektu).
- `SPLITTERDISTANCE` se primijeni samo u grani nepraznog `strTreeNodes` (`4323`), ne čim atribut postoji.
