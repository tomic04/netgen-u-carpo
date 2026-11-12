# Carpo XML Rules (FINAL v2)

Ovaj dokument definira konsolidirani skup pravila za Carpo XML forme: strukturu, imenovanje, layout, data pozive, evente, poslovne pattern-e i integracije.  
Služi kao jedinstveni referentni dokument za tim i AI generaciju.

---

## 0. Filozofija i kompatibilnost

- **Cilj**: jasno pravilo za novi razvoj + razumijevanje postojećeg legacy koda.
- **Novi kod**:
  - Pridržavati se ovih pravila.
  - Pri izboru između više viđenih patterna, koristiti *označeni preporučeni*.
- **Postojeći kod**:
  - Sadrži razne legacy varijante (`oninit`, inline `<data>` unutar `grid`, globalne varijable bez `$` itd.).
  - **Ne radi masovne refaktore** bez potrebe i bez testova.
  - Ako trebaš proširiti postojeću formu, drži se ovog dokumenta koliko je moguće, ali nemoj lomiti stabilnu poslovnu logiku.

Oznake u tekstu:

- **[NOVI STANDARD]** – aktivno preferirani pristup za nove forme.
- **[LEGACY – DOZVOLJENO ZA ODRŽAVANJE]** – koristi se u starom kodu, ne uvoditi u novi.
- **[MIX – TOLERIRA SE]** – oba patterna postoje; jedan je preporučen.

---

## 1. Root dokumenta i forme

### 1.1 Root

- Root element je uvijek:
  ```xml
  <root>
    ...
  </root>
  ```
- **[NOVI STANDARD]**: koristiti verziju:
  ```xml
  <root version="1.2">
    ...
  </root>
  ```
- U kodu postoje varijante (`version="1.0.0.2"`, `1.0.0.1`, `1.2.12.`, `1.4`) – tolerirati, ali nove dokumente otvarati s `version="1.2"`.
- Root može sadržavati:
  - više `<form>` elemenata (glavna forma + subforme, dialog forme),
  - (opcionalno) globalni `<datalist>` za globalne datasete (npr. `dtPart*`, lookup-i),
  - (opcionalno) `<parts>` definicije (`part` layout za re‑use),
  - (opcionalno) atribute poput `loglevel`.

### 1.2 Tipovi formi

- **[NOVI STANDARD]** – dozvoljeni tipovi:
  - `form/@type ∈ { custom, manager, master }`
- Semantika:
  - `custom` – klasična forma s `<layout>`.
  - `manager` – forma s `managerlayout` (lookup/tree/grid/tabs).
  - `master` – glavne/veće forme (često `managerlayout`, ponekad više layout panela).
- **[LEGACY – DOZVOLJENO]**:
  - `type` često izostavljen – tretira se kao `custom`.
  - `type="detail"` stari naziv za subforme; ne koristiti u novom kodu.

Preporuka: za bilo koju glavnu formu s `managerlayout` koristi `type="master"`.

### 1.3 Struktura unutar `<form>`

**[NOVI STANDARD]** – kanonski redoslijed:

```xml
<form name="Example" type="master" caption="Primjer">
  <method .../>              <!-- opcionalno -->

  <event type="init">...</event>   <!-- 0..N -->
  <event type="load">...</event>
  <event type="shown">...</event>
  <event type="close">...</event>

  <layout>...</layout>       <!-- ili -->
  <managerlayout>...</managerlayout>

  <panelbuttons>...</panelbuttons>   <!-- opcionalno -->
  <actionpanel>...</actionpanel>     <!-- alternativno -->

  <datalist>...</datalist>   <!-- uvijek zadnji -->
</form>
```

- U nekim projektima postoje 2+ `layout` panela u formi (`template="splitter"`, `layout name="panel1"`, `panel2`) – to je dopušteno.
- **Uvijek** držati `<datalist>` na kraju forme.

**[NOVI STANDARD – KRITIČNO]**: Forma može imati samo **JEDAN** `<layout>` element **bez `name` atributa**.

**Izuzetak – Info template**  
Ako je `form/@template="info"`, tada je dozvoljeno imati **više** `<layout>` elemenata bez `name` atributa, jer **svaki `<layout>` predstavlja jedan tab** u Info formi. Ovo je specifično ponašanje InfoPunktovi template‑a i dio je standarda za taj template.

❌ **POGREŠNO** – više običnih layout elemenata (bez `name` atributa):
```xml
<form name="Example">
  <layout>
    <group cols="2" rows="3">
      <item row="1" col="1">
        <textedit name="txtNaziv"/>
      </item>
    </group>
  </layout>
  <layout>  <!-- DRUGI LAYOUT BEZ NAME - NE RADI! -->
    <group cols="1" rows="1">
      <item row="1" col="1">
        <grid name="grdPregled"/>
      </item>
    </group>
  </layout>
</form>
```

✅ **ISPRAVNO** – sve kontrole u jednom layoutu:
```xml
<form name="Example">
  <layout>
    <group cols="2" rows="4">
      <item row="1" col="1">
        <textedit name="txtNaziv"/>
      </item>
      <item row="4" col="1" colspan="2">
        <grid name="grdPregled"/>
      </item>
    </group>
  </layout>
</form>
```

**[LEGACY – DOZVOLJENO ZA ODRŽAVANJE]**: Forme sa `template="splitter"` mogu imati više **imenovanih** layout elemenata:
```xml
<form name="Splitter" template="splitter">
  <layout name="panel1" type="collapsable">
    <group cols="2" rows="3">
      <!-- kontrole u lijevom panelu -->
    </group>
  </layout>
  <layout name="panel2">
    <group cols="1" rows="1">
      <!-- kontrole u desnom panelu -->
    </group>
  </layout>
</form>
```

**Napomena**: Ako forma ima više običnih layout elemenata (bez `name` atributa), engine će koristiti samo posljednji ili ignorirati prethodne, što rezultira time da kontrole iz prvog layouta nisu vidljive. Sve kontrole (uključujući gridove) moraju biti u jednom `<layout>` elementu, osim ako se koristi splitter template sa imenovanim panelima.

### 1.4 Više formi i `path`

- U jednom rootu možeš imati:
  - jednu ili više glavnih formi,
  - subforme deklarirane s `path`:
    ```xml
    <form name="frmTarifa" path="Tarifa.frmTarifa.xml"/>
    ```
- **Imenovanje subform XML‑ova**:
  - `[GlavniNaziv].[NazivForme].xml`, npr.:
    - `KnjigaUra.frmKnjigaUraInsert.xml`
    - `Tarifa.frmTarifa.xml`
- U `path` koristi točan naziv datoteke.

### 1.5 Info template (InfoPunktovi)

**[NOVI STANDARD – OBAVEZNO KADA KORISTIŠ `template="info"`]**  
Info template je poseban UI (InfoPunktovi). Aktivira se kad `form/@template` sadrži `"info"`.

Ključna pravila:

- `form/@template="info"` + `form/@type="master"` je očekivani pattern.
- `infocontrols` je **obavezan** i mora sadržati:
  - `<tabs>`, `<search>`, `<navigation>`, `<info>`, `<newbutton>`
  - `entity="..."` (InfoHistory koristi ovaj naziv)
  - `searchtype="popup|default"`
- Svaki `<layout>` u formi postaje **tab** (caption = naslov taba, redoslijed layouta = indeks taba).
- `layout/@function` se koristi za permissions (enable/visible) na tabovima.
- `search` dataset mora vraćati **Id u prvoj koloni** i display tekst u drugoj.
- `navigation` dataset mora imati kolonu `Id` (po imenu, koristi se `Select("Id=...")`).
- `infocontrols` se čita bez null‑checkova → ne izostavljati nijedan obavezni element.

Detaljnija pravila i primjer skeleton‑a su u: `docs/Carpo-Info-Template-Rules.md`.

### 1.6 Naslov forme – font i boja (`captionfontsizedelta`, `captionfontbold`, `captionforecolor`)

**[NOVI STANDARD – OPT‑IN]** – dostupno u Devex_23 i Devex_26.

Naslov forme (`form/@caption`, prikazan u naslovnoj traci prozora) po defaultu dolazi iz skina i sistemskih postavki. Tri opcionalna atributa na `<form>` omogućuju da se za pojedinu formu naslov poveća, podeblja ili oboji:

| Atribut | Tip | Značenje |
|---|---|---|
| `captionfontsizedelta` | int | Relativna promjena veličine fonta naslova (npr. `"3"` = 3pt veće, `"-1"` = 1pt manje). Relativno na skin, radi s DPI skaliranjem. |
| `captionfontbold` | bool | `"true"` = podebljan naslov. |
| `captionforecolor` | string | Boja naslova: `"#RRGGBB"` ili ime .NET boje (`"DarkRed"`). Primjenjuje se i kad forma nije aktivna. |

```xml
<form name="MPPregDok" type="master" caption="Pregled dokumenata"
      captionfontsizedelta="3" captionfontbold="true" captionforecolor="#B00020">
```

Pravila:

- Bez ovih atributa **ništa se ne mijenja** – postojeće granule izgledaju identično kao prije.
- Radi na svim tipovima formi (master/manager, dialog, `template="splitter"`, info, sidebar) jer se primjenjuje u baznoj klasi forme.
- Utječe **samo na naslovnu traku**; font kontrola unutar forme se i dalje regulira po kontroli (`fontsizedelta`/`fontbold` na `<item>`, `FontSizeDelta`/`FontBold` na editorima i gridu).
- Nevaljana boja ne ruši formu – engine zapiše `Warn` u log i zadrži default boju; `captionfontsizedelta` koji nije cijeli broj baca grešku pri učitavanju XML‑a (kao i ostali int atributi na `<form>`).
- Ne koristiti za „naslove“ unutar forme – za to služi `<item caption="..." fontsizedelta="4" fontbold="true"/>` u layoutu.

---

## 2. Imenovanje formi, kontrola i dataset-a

### 2.1 Forme

- Svaka forma: jedinstveni `name` unutar XML-a.
- **Glavna forma (ovaj repo)**: **bez** `frm` prefiksa u `name`.
- **Subforme**: koristiti `frm` prefiks (npr. `frmTarifa`).
- Primjeri (glavne forme):
  - `Master`, `Glavna`, `EvidencijaNaplate`
  - `PonudaNova`, `PonudaEdit`
  - `KnjigaUraInsert`, `KnjigaUraRucno`
  - `HRKarticaRadnika`, `GrupaPodataka`, `Jezici`

### 2.2 Prefiksi za kontrole

**[NOVI STANDARD]**

| Tip               | Prefix        | Primjeri                             |
|-------------------|--------------|--------------------------------------|
| Form (subforma)   | `frm`        | `frmPartneri`                        |
| TextEdit          | `txt`        | `txtNaziv`, `txtBroj`                |
| MemoEdit          | `mem`        | `memNapomena`                        |
| HyperlinkEdit     | `hyp`        | `hypWeb`                             |
| ComboEdit         | `cmb`        | `cmbStatus`                          |
| ComboMultiEdit    | `cmb`        | `cmbVrstaDok`                        |
| ComboTreeEdit     | `cmb`        | `cmbOrgShema`                        |
| SearchEdit        | `src`        | `srcPartner`, `srcRoba`             |
| BrowseEdit        | `brw`        | `brwPutanja`                         |
| DateEdit          | `dat`        | `datDatum`, `datDatumOd`             |
| ButtonTextEdit    | `cmd`        | `cmdPretraga`                        |
| Button            | `btn`        | `btnSpremi`, `btnZatvori`           |
| LabelEdit         | `lbl`        | `lblStatus`, `lblKnjigaOpis`        |
| CheckEdit         | `chk`        | `chkAktivan`, `chkDatumi`           |
| RadioEdit         | `rdo`        | `rdoOpcija1`, `rdoR1`                |
| DaysEdit          | `day`        | `dayRok`                             |
| TimeEdit          | `tme`        | `tmeVrijeme`                         |
| PictureEdit       | `pic`        | `picLogo`, `picPrivitak`            |
| PdfViewer         | `pic`        | `picPrivitak`                        |
| Grid              | `grd`        | `grdGlava`, `grdDetalj`             |
| Tree              | `tree`       | `tree`, `treeKlase`                 |
| Separator         | `sep`        | `sepLinija`                          |
| CustomControl     | `ucc` / `cus`| `uccCalc`, `cusCalc`                |
| Chart             | `chr`        | `chrProdaja`                         |
| Container         | `ctr`        | `ctrButtons`                         |

**[LEGACY – DOZVOLJENO]**

- `cmd*` za gumbe, `date*` za datume, `cb*` za combo, `tr*` / `tree*` za tree, `mem*` bez prefiksa `txt`.
- U novom kodu preferiraj tablicu iznad; u održavanju ne diraj stabilne nazive.

**Dodatna pravila:**

- Imena kontrola:
  - moraju biti jedinstvena unutar forme (ne imati više `btnUnos` u istoj formi),
  - ne smiju sadržavati razmake ni dijakritike.

### 2.2.1 DaysEdit (`daysedit`) – bazni datum

**[NOVI STANDARD]**: `daysedit` podržava atribut `basedate` koji definiše bazni datum od kojeg se računa broj dana.

- Ako je `basedate` postavljen → razlika se računa **od tog datuma** do kalendarskog datuma; pri otvaranju kalendara (pop-up) prikazuje se **današnji datum** kako bi korisnik lako odabrao „danas”.
- Ako `basedate` nije postavljen → ponašanje ostaje kao do sada (računa od `DateTime.Now`, kalendar se otvara na bazni datum + broj dana iz polja) radi backward compatibility.

**Primjeri:**

```xml
<daysedit name="dayRok" dataname="dtGlava" field="RokDana" basedate="#datDatumFakture"/>
<daysedit name="dayRok2" dataname="dtGlava" field="RokDana2" basedate="~dtGlava.@DatumFakture"/>
```

Napomena: `basedate` može referencirati kontrolu (`#datDatumFakture`) ili dataset field (`~dtGlava.@DatumFakture`).

### 2.3 Dataset imena i `dataname`

- **Imena dataset-a**:
  - `dtX` + opis:
    - `dtGlava`, `dtDetalj`
    - `dtMaster`, `dtDetail`
    - `dtGet`, `dtInsert`, `dtUpdate`, `dtDelete`
    - `dtGridRoba`, `dtTree`, `dtKnjigePopuni`
- U `<datalist>`:
  ```xml
  <data name="dtGlava" command="spX_Query" .../>
  ```
- U kontrolama:
  ```xml
  <grid name="grdGlava" dataname="dtGlava"/>          <!-- lokalni -->
  <grid name="grdGlava" dataname="global.dtGlava"/>   <!-- globalni -->
  ```

**[MIX – TOLERIRA SE]**:

- U starom kodu:
  - `dataname="~dtGlava"` za lokalno,
  - referenca u akcijama `value="~dtGlava"`.
- **Preporuka**: u novom kodu birati jedan stil po projektu; ovaj dokument koristi `dataname="dtX"` i `value="~dtX"` u `datarun`.

### 2.4 Globalni dataseti

- Deklarirani na *root* razini:
  ```xml
  <datalist>
    <data name="dtPartFilter" command="spPartQuery".../>
    ...
  </datalist>
  ```
- U kontrolama/akcijama:
  - `dataname="global.dtPartFilter"` ili `value="global.~dtPartFilter"`.
- **[NOVI STANDARD]**:
  - Ako u `dataname` postoji `global.` prefiks → dataset je definiran u root `<datalist>` (ne u formi).

---

## 3. Layout (custom forme)

### 3.1 Osnovna struktura

```xml
<layout>
  <group rows="..." cols="..." groupbordervisible="false">
    ...
  </group>
</layout>
```

- Jedan root `<group>` po `layout`.
- Obavezni atributi: `rows`, `cols`.
- Česti dodatni:
  - `groupbordervisible="false|true"`
  - `caption` (za sekcije)
  - `spacing`, `padding`
  - `alignmode="parent|local|auto"`
  - `backgroundcolor="#RRGGBB"`

### 3.2 Djeca `group`

Dozvoljeno:

- `columnsdefinition`:
  ```xml
  <columnsdefinition>
    <column position="1" size="80%"/>
    <column position="2" size="20%"/>
  </columnsdefinition>
  ```
- `rowsdefinition`
- `group` (ugniježđene sekcije)
- `tabs` (direktno dijete `group`; NE stavljati u `<item>`)
- `item`

### 3.3 `<item>` pravila

- Obvezni atributi:
  - `row`, `col` (1-based)
- Dodatno:
  - `caption`, `rowspan`, `colspan`, `height`, `width`, `padding`, `name`, `highlight`
- Djeca:
  - jedna primarna kontrola (`textedit`, `grid`, `tree`, `button`, `searchedit`, `tabs`, `part`, …),
  - opcionalno: `event` (vrlo rijetko u novom kodu), `menu`, `styles`.
- **Validacija layouta (logička)**:
  - `row`/`col` moraju biti unutar definiranog `rows`/`cols` na parent `group`.
  - Izbjegavaj dvije kontrole u istoj ćeliji bez `rowspan/colspan`.

**[NOVI STANDARD – KRITIČNO]**: Gridovi i sve kontrole u `<layout>` MORAJU biti unutar `<item>` elementa!
**Izuzetak**: `<tabs>` je layout‑container i ide direktno u `group` (ne u `<item>`), a kontrole unutar tabova i dalje moraju biti u `<item>` elementima.

❌ **POGREŠNO**:
```xml
<layout>
  <group cols="1" rows="2">
    <grid name="grd1" row="1" col="1">  <!-- NE RADI! -->
      <fields>...</fields>
    </grid>
  </group>
</layout>
```

✅ **ISPRAVNO**:
```xml
<layout>
  <group cols="1" rows="2">
    <item row="1" col="1">
      <grid name="grd1">  <!-- Grid mora biti dijete <item> elementa -->
        <fields>...</fields>
      </grid>
    </item>
  </group>
</layout>
```

**Napomena**: Grid unutar `<item>` elementa ne treba `row` i `col` atribute – pozicioniranje se definiše na `<item>` elementu.

**Greška – XmlException (nedostaje `</item>`):** Pri layoutu s `group rows="N" cols="2"` svaki red mora imati **zatvorene** `<item>` elemente. Za label (col 1): `<item row="X" col="1" caption="Label:"></item>`. Za kontrolu (col 2): `<item row="X" col="2"><dateedit .../></item>`. Ako nedostaje `</item>` za bilo koji item (npr. Skladište), XML parser baca `XmlException` i granula se ne učitava. **Provjera:** svaki `<item>` mora imati odgovarajući `</item>` prije sljedećeg elementa ili kraja grupe. **Uzor greške:** MPPregDokMPVPCarpo frmFilter – nedostajalo `</item>` za Skladište.

### 3.4 `columnsdefinition` i `rowsdefinition`

- Koristi se za kontrolu širina/visina; `size`:
  - broj → px (`size="250"`)
  - postotak (`"50%"`)
- Ne koristi `*` ili `1*` notacije.

### 3.5 Patterni layouta

**Grid + gumbi sa strane:**

- Root `group cols="2" rows="N"`.
- `columnsdefinition`: npr. 80% za grid, 120 px za gumb kolonu.
- Grid item s `rowspan="N"`, gumbi u desnoj koloni, jedan po redu.

**Dijalog filter + grid (preklapanje kontrola):**

Kada dijalog ima filter kontrole (combo, textedit, checkedit) i grid – ako su u dvije odvojene `<group>` strukture, može doći do preklapanja (grid „Drag a column header“ prelazi preko checkboxova). **Rješenje**: spojiti sve u jednu `<group rows="N" cols="2">` – filter redovi 1 do N-1, grid u posljednjem redu s `colspan="2"`. U `rowsdefinition` zadati eksplicitne visine za filter redove (npr. `position="1" size="30"`, `position="2" size="30"`, …) i za grid red `position="N" size="100%"`. Uzor: MPEtiketeTerminalCarpo frmDonosIzRobe (DONOSIZROBE).

**Tabs u layoutu:**

```xml
<tabs>
  <group caption="Osnovno" cols="2" rows="5">
    <item row="1" col="1">...</item>
  </group>
  <group caption="Dokumenti" cols="1" rows="3">
    <item row="1" col="1">...</item>
  </group>
</tabs>
```

- Svaki `<group>` predstavlja tab.

---

## 4. Managerlayout (master forme)

### 4.1 Osnovna struktura

```xml
<form name="Master" type="master">
  <managerlayout>
    <lookup name="filter">...</lookup>
    <tree name="tree" ...>...</tree>
    <grid name="grdGlava" ...>...</grid>
    <tabs name="detTabs">
      <tab name="tabDetail1">...</tab>
      <tab name="tabDetail2">...</tab>
    </tabs>
  </managerlayout>
  <panelbuttons>...</panelbuttons>
  <datalist>...</datalist>
</form>
```

- `managerlayout` djeca:
  - `lookup`, `tree`, `grid`, `tabs`.

### 4.2 Tipični pattern: lookup → tree → grid

```xml
<lookup name="filter">
  <comboitem value="Vremenska podjela"/>
  <comboitem value="Status"/>
  <event type="valuechanged">
    <action type="variable value" value="global.$filter">
      <parameter value="#filter"/>
    </action>
  </event>
</lookup>

<tree name="tree" dataname="dtTree" key="Id" parentfield="ParentId">
  <fields><field name="Name" caption="Razdoblje"/></fields>
  <event type="focusedrowchanged">
    <action type="control populate" value="#grdGlava"/>
  </event>
</tree>

<grid name="grdGlava" type="master" dataname="dtGlava" keyfield="Id">
  <fields inherited="true"/>
  <event type="focusedrowchanged">
    <action type="control populate" value="#grdDetail"/>
  </event>
  <event type="doubleclick">
    <action type="form open" value="frmEdit">
      <parameter name="Id" value="#grdGlava.@Id" type="int"/>
    </action>
  </event>
</grid>
```

### 4.3 Tabs u managerlayoutu

```xml
<tabs name="detTabs">
  <event type="selectedtabchanged">
    <action if="#detTabs.selectedtab==tabItems" type="control populate" value="#grdItems"/>
  </event>
  <tab name="tabItems" caption="Stavke">
    <grid name="grdItems" dataname="dtItems">
      <fields inherited="true"/>
    </grid>
  </tab>
  <tab name="tabDocs" caption="Dokumenti">
    <grid name="grdDocs" dataname="dtDocs">
      <fields inherited="true"/>
    </grid>
  </tab>
</tabs>
```

---

## 5. Grid i Tree

### 5.1 Grid – opća pravila

Svaki `grid`:

- mora imati `name` (prefiks `grd`),
- mora imati `dataname`,
- mora imati `<fields>`.

**[NOVI STANDARD – KRITIČNO]**: Grid pozicioniranje zavisi od konteksta:

- ✅ **Unutar `<layout>`**: grid MORA biti dijete `<item>` elementa:
  ```xml
  <layout>
    <group cols="1" rows="1">
      <item row="1" col="1">
        <grid name="grd1">  <!-- Grid bez row/col atributa -->
          <fields>...</fields>
        </grid>
      </item>
    </group>
  </layout>
  ```

- ✅ **Unutar `<managerlayout>`**: grid ide direktno, BEZ `<item>` omotača:
  ```xml
  <managerlayout>
    <grid name="grdGlava" dataname="dtGlava">  <!-- Direktno dijete -->
      <fields>...</fields>
    </grid>
  </managerlayout>
  ```

**Ključ:**

- **[NOVI STANDARD]**: ne dodavati `key`/`keyfield` bez potrebe.
  - Koristi ih kad:
    - treba fokus po ID-u (`control focus`),
    - treba `selectedids`/`selectedids.table` pouzdano.

**Primjer:**

```xml
<grid name="grdGlava" dataname="dtGlava" keyfield="FaktureId" savelayout="true">
  <fields inherited="true">
    <field name="BrojDok" caption="Broj"/>
    <field name="Datum" caption="Datum" format="dd.MM.yyyy"/>
  </fields>
</grid>
```

### 5.2 Fields

- **[NOVI STANDARD]**:
  ```xml
  <fields inherited="true"/>
  ```
  ili
  ```xml
  <fields inherited="true">
    <field name="Id" visible="false"/>
    <field name="Iznos" format="n2"/>
  </fields>
  ```
- Ručno nabrajanje *svih* polja je legacy; koristi ga samo gdje treba potpuno customizirati grid.

**[NOVI STANDARD] Fiksna boja kolone (`fontcolor`, `backcolor` na `<field>`):**

- Na pojedinačnom `<field>` možeš postaviti fiksnu boju **teksta** i/ili **pozadine** za cijelu kolonu:
  - `fontcolor` – imenovane boje (`red`, `blue`, `green`, `gray`, `orange`, `black`) ili hex (`#ff0000`, `#BF5F04`).
  - `backcolor` – isti format.
- Oba atributa su opcionalna; ako ih nema, ponašanje je kao dosad (backward compatible).
- Možeš kombinirati: npr. `fontcolor="red" backcolor="lightgreen"` na istom `<field>`.
- Ako je na gridu definiran i uslovni stil po redu (`<styles>` s `fontcolor`/`backgroundcolor` i `if`), **uslovni stil ima prednost** za taj red.

**Primjer:**

```xml
<field name="Iznos" caption="Iznos" format="n2" fontcolor="#BF5F04" backcolor="lightyellow"/>
<field name="Status" caption="Status" fontcolor="green"/>
```

### 5.3 Grid UX & performanse

Preporuke:

- `async="true"` za veće gride.
- `showautofilterrow="true"` za pregledne liste.
- `showgrouppanel="false"` default.
- `savelayout="true"` omogućiti korisnički layout.
- `bestfitonrefresh="true"` koristiti selektivno (skuplje na jako širokim gridovima).
- `multiselect="true"` + `showcheckboxes="true"` kad koristiš `selectedids`/`selectedids.table` za batch operacije.
- `allowcopycell="true"` samo kad korisnik treba kopirati **sadržaj jedne ćelije** (`Ctrl+C`) — vidi §5.3.2. **Ne** stavljati default na svaki grid.

### 5.3.1 DevExpress GridView – korisničko conditional formatting i CarpoVars

Runtime grid koristi **DevExpress** `GridView`. Korisnik može **desnim klikom na zaglavlje kolone** otvoriti kontekst meni; ako je u engine-u uključeno **`ShowConditionalFormattingItem`**, dostupan je podmeni **Conditional Formatting** (npr. Highlight Cell Rules, Unique/Duplicate, **Manage Rules…**).

**Da li se ta pravila pamte posle zatvaranja aplikacije?**

- Na gridu mora biti **`savelayout="true"`** – isti mehanizam kao za čuvanje širina kolona, redosleda, filtera u **CarpoVars**.
- DevExpress **`SaveLayoutToStream`** / **`SaveLayoutToXml`** po podrazumevanim opcijama **ne moraju** uključiti kolekciju **`FormatRules`**. U odgovarajućoj verziji Carpa postavlja se **`GridView.OptionsLayout.StoreFormatRules = true`** da bi se korisnička pravila bojanja snimila zajedno s layoutom (nema nove SQL šeme – isti zapis layouta, proširen sadržajem).
- **Razlika:** uslovno stilizovanje zadano u **Carpo XML-u** (`<styles>`, `GridFormatRuleList` u definiciji grida) puni engine pri učitavanju – to nije isto što i pravila koje korisnik dodaje kroz DevExpress meni; vidi §5.5.

**Dalje čitanje (repozitorij):** `docs/Novosti/DevExpress-WinForms-Grid-KontekstMenija-ConditionalFormatting.md` (pregled DevExpress menija i tipova pravila), `docs/Handoff-DevExpress-FormatRules-Layout-Persistence.md` (šta menjati u kodu za zvanični branch).

### 5.3.2 Kopiranje jedne ćelije (`allowcopycell`)

**[NOVI STANDARD]**: zadani grid **ne** kopira jednu ćeliju. `Ctrl+C` otvara dijalog **Izvoz podataka** (standardni izvoz / Excel) i u clipboard ide red ili izvoz, često s naslovima kolona.

Kad korisnik treba kopirati **sadržaj jedne ćelije** (tekst, broj, datum, string — tip je nebitan):

```xml
<grid name="grdMaster" dataname="~dtMaster" id="id" savelayout="true" allowcopycell="true">
```

Što engine radi kad je `allowcopycell="true"` (`ComponentGrid`):

- uključuje odabir **po ćelijama** (`MultiSelect` + `CellSelect`);
- **isključuje uređivanje** cijelog grida (`Editable = false`);
- kopira **bez** naslova kolone (`CopyColumnHeaders = false`).

**Kad staviti:** pregledni / read-only grid gdje je traženo „kopiraj ovu ćeliju“.

**Kad NE staviti:**

- grid s **editabilnim kolonama** (`<field editable="true">`) — atribut gazi edit;
- grid s **`multiselect` + `showcheckboxes`** / `selectedids` / `foreachrow selected` — mijenja način selekcije i kvari batch UX;
- default na svakoj novoj granuli — samo na zahtjev.

**UX napomena:** handler za `Ctrl+C` i dalje može prvo otvoriti dijalog izvoza ako postoji selekcija. Ako korisnik otkaže dijalog, DevExpress može odraditi kopiranje ćelije. Ako i tada ne kopira, to je runtime ograničenje, ne greška XML-a.

### 5.4 Tree

- Atributi:
  - `name="tree..."`,
  - `dataname="dtTree"` ili integrirani iz `data` unutar `tree` (legacy stil),
  - `key` (primarni ključ),
  - `parentfield` (hijerarhija),
  - `expandallonload`, `expandedlevel`,
  - `allowselfdrag="true"` za drag&drop.

**Primjer:**

```xml
<tree name="tree" dataname="dtTree" key="Id" parentfield="ParentId" expandallonload="true">
  <fields>
    <field name="Name" caption="Stablo"/>
  </fields>
  <event type="focusedrowchanged">
    <action type="control populate" value="#grdGlava"/>
  </event>
</tree>
```

**Drag & drop pattern:**

- `tree allowselfdrag="true"`.
- `panelbuttons/buttons type="tree">
   <button type="selfdrag"/>
   <button type="selfdragcancel"/>
  </panelbuttons>`
- `dtOrgAzur` SP s parametrima `@TableVar="#tree.datasource"` za spremanje novog rasporeda.

### 5.5 Styles / conditional formatting (bojanje grida)

Element `<styles>` omogućava uslovno stilizovanje redova u gridu.

#### 5.5.1 Šta MOŽE

**Uslovno bojanje teksta u redu:**

```xml
<styles>
    <style name="fontcolor" value="red" if="#grdMaster.@Status==0"/>
    <style name="fontcolor" value="green" if="#grdMaster.@Status==1"/>
    <style name="fontcolor" value="gray" if="#grdMaster.@Status==2"/>
</styles>
```

**Podržane boje:**
- Imenovane: `red`, `blue`, `green`, `gray`, `orange`, `black`
- Hex format: `#ff0000`, `#00ff00`, `#0000ff`, `#BF5F04`, `#256200`, `#F130F7`

**Podebljani tekst (bold):**

```xml
<styles>
    <style name="bold" value="true" if="#grdMaster.@Aktivna==DA"/>
</styles>
```

**Kurziv tekst (italic):**

```xml
<styles>
    <style name="italic" value="true" if="#grdMaster.@Komentar==Ručni unos"/>
</styles>
```

**Kombinovanje više stilova:**

```xml
<styles>
    <style name="fontcolor" value="blue" if="#grdMaster.@Tip==Premium"/>
    <style name="bold" value="true" if="#grdMaster.@Tip==Premium"/>
    <style name="italic" value="true" if="#grdMaster.@Napomena!=null"/>
</styles>
```

**Uslovni lanci (if / elseif / else):**

```xml
<styles>
    <style name="fontcolor" value="green" if="#grdMaster.@Status==Aktivan"/>
    <style name="fontcolor" value="orange" elseif="#grdMaster.@Status==Na čekanju"/>
    <style name="fontcolor" value="red" elseif="#grdMaster.@Status==Neaktivan"/>
    <style name="fontcolor" value="gray" else=""/>
</styles>
```

**Složeni uslovi (AND / OR):**

```xml
<styles>
    <!-- AND uslov -->
    <style name="fontcolor" value="red" if="#grdMaster.@Status==1&amp;&amp;#grdMaster.@Iznos&gt;0"/>
    
    <!-- OR uslov -->
    <style name="bold" value="true" if="#grdMaster.@Prioritet==Visok||#grdMaster.@Prioritet==Kritičan"/>
</styles>
```

**Napomena:** U XML-u koristi `&amp;&amp;` za `&&` i `&gt;` za `>`.

**Provera null vrednosti:**

```xml
<styles>
    <style name="fontcolor" value="red" if="#grdMaster.@PartnerId==null"/>
    <style name="fontcolor" value="green" if="#grdMaster.@PartnerId!=null"/>
</styles>
```

**Podebljana polja u gridu (fiksno):**

```xml
<field name="Sifra" caption="Šifra" fontbold="True"/>
```

#### 5.5.2 Šta NE MOŽE

- **Bojanje pojedinačnih ćelija** – stil se primenjuje na **ceo red**, ne na jednu ćeliju
- **Boja pozadine reda** – ne postoji `backcolor` ili `backgroundcolor` za redove grida
- **Uslovno bojanje kolona** – ne može se primeniti uslov na nivou kolone
- **Različite boje za različite kolone u istom redu** – ako je red crven, sve kolone su crvene
- **Dinamička promena veličine fonta po redu** – `fontsize` se ne može menjati uslovno

#### 5.5.3 Kompletan primer

```xml
<styles>
    <style name="fontcolor" value="gray" if="#grdNarudzbe.@Status==Otkazana"/>
    <style name="fontcolor" value="orange" elseif="#grdNarudzbe.@Status==Na čekanju"/>
    <style name="fontcolor" value="green" elseif="#grdNarudzbe.@Status==Isporučena"/>
    <style name="fontcolor" value="blue" elseif="#grdNarudzbe.@Status==U pripremi"/>
    <style name="bold" value="true" if="#grdNarudzbe.@Prioritet==Hitno"/>
    <style name="italic" value="true" if="#grdNarudzbe.@Napomena!=null"/>
</styles>
```

#### 5.5.4 Preporučene boje po značenju

| Značenje | Boja | Hex |
|----------|------|-----|
| Uspešno / Aktivno | `green` | `#256200` |
| Greška / Neaktivno | `red` | `#ff0000` |
| Upozorenje | `orange` | `#BF5F04` |
| Informacija | `blue` | `#1e02f7` |
| Arhivirano | `gray` | - |

#### 5.5.5 Napomene

- `if` izraz koristi grid i polje (`#grd.@Kolona`).
- Ako dataset vraća npr. `rowforecolorid`, engine može koristiti to direktno bez `styles`.
- Fiksno bojanje po koloni (`fontcolor`/`backcolor` na `<field>`) može se kombinirati s `<styles>`: za red koji zadovolji uslov, stil po redu nadjačava fiksnu boju kolone za taj red.
- **Referenca:** `#imeGrida.@imePolja`

---

## 6. Datalist, `data` i parametri

### 6.1 Lokacija i scope

- **Lokalni dataseti**:
  - `<datalist>` unutar forme.
  - `data/@name="dtX"`, `dataname="dtX"` na kontrolama.
- **Globalni dataseti**:
  - `<datalist>` na root razini.
  - Kontrole/akcije koriste `dataname="global.dtX"` ili `value="global.~dtX"`.

### 6.2 Tipovi `data/@type`

**[NOVI STANDARD]**

- `reader` – SP ili SQL koji vraća tablicu.
- `nonquery` – DML (INSERT/UPDATE/DELETE) bez rezultata.
- `scalar` – vraća jednu vrijednost.
- `text` – literal SQL (SELECT …).

Primjeri (kada je `type` potreban):

```xml
<data name="dtPrazno" command="SELECT TOP 0 1 AS Id" type="text"/>
<data name="dtBrojac" command="spBrojac" type="scalar"/>
<data name="dtDelete" command="spDelete" type="nonquery"/>
```

**[MIX – TOLERIRA SE]**:

- U starim XML-ovima `type` se često izostavlja → default je `reader`.
- U ovom repou **ne navoditi `type`** osim ako korisnik eksplicitno trazi ili
  postoji dokumentiran problem gdje engine ne pogadja tip.
- **Izuzetak**: `partsql` dataseti u root `<datalist>` moraju imati
  `type="reader"`.

### 6.3 Parametri (`<parameter>`)

- `parameter/@name` = stvarni naziv parametra (`@Action`, `@Id`, ...).
- `parameter/@value` može biti:
  - kontrola:
    - `#txtNaziv`, `#cmbStatus`, `#srcPartner`
    - `#grdMaster.@Id` (vrijednost iz grida)
    - `#grd.selectedids` ili `#grd.selectedids.table`
  - varijabla:
    - `$Id`, `global.$Filter`
    - `@@operateriid`, `@@server`, `@@database`, `@@uid`, `@@pwd`
  - literal:
    - `"unos"`, `"izmjena"`, `0`, `1`, `'Fakture'`.

**Type na parametru:**

- **[NOVI STANDARD]**: izbjegavati `parameter/@type` osim kad engine ne može pogoditi (npr. `bool`, specifični decimalni slučajevi).
- Legacy kod često koristi `type="int"`, `type="decimal"`, `type="date"` – tolerirati.

### 6.4 Grananje `<data>` elemenata

Dozvoljeno:

```xml
<data name="dtGrid" command="spX" type="reader" if="$Mode=='A'">
  <parameter name="@Action" value="1"/>
</data>
<data name="dtGrid" command="spX" type="reader" elseif="$Mode=='B'">
  <parameter name="@Action" value="2"/>
</data>
<data name="dtGrid" command="spX" type="reader" else="">
  <parameter name="@Action" value="3"/>
</data>
```

- Engine koristi prvi `<data>` čiji je `if` (ili `elseif`) istinit; `else` pokriva preostale.

### 6.5 `autopopulate="true"` ograničenje

- Ako grid ima `autopopulate="true"`, **njegov** dataset (`<data name="...">`) **ne smije** imati `if` uvjet.
- Uvjeti idu u parametre ili u drugi dataset.

### 6.6 Insert koji vraća ID

**Preporučeni pattern:**

```xml
<action type="datarun scalar" value="~dtInsert">
  <parameter name="$Id"/>
</action>
```

- SP vraća ID (output ili SELECT) koji engine sprema u `$Id`.

**[LEGACY – DOZVOLJENO]**:

- Output parametar u `<data>`:
  ```xml
  <data name="dtInsert" command="spX" type="reader">
    <parameter name="@Id.output.int" variablename="Id"/>
  </data>
  ```
- Ili `datarun scalar` + naknadno čitanje `~dtInsert.@Id`.

---

## 7. Varijable i parametri formi

### 7.1 Lokalna vs globalna varijabla

- **Lokalna**:
  - sintaksa: `$Naziv`
  - scope: jedna forma
  - postavljanje:
    ```xml
    <action type="variable value" value="$FilterMode">
      <parameter value="1"/>
    </action>
    ```
- **Globalna**:
  - sintaksa: `global.$Naziv`
  - scope: cijeli dokument (i subforme)
  - postavljanje:
    ```xml
    <action type="variable value" value="global.$TarifaId">
      <parameter value="null"/>
    </action>
    ```

**[LEGACY – DOZVOLJENO]**:

- Stariji stil:
  ```xml
  <action type="variable value">
    <parameter name="FilterMode" value="1"/>
  </action>
  ```
- Novi kod preferira stil s `value="$VarName"`.

### 7.2 Parametri `form open`

- Parametri u `form open` postaju lokalne varijable u subformi:

```xml
<action type="form open" value="frmEdit">
  <parameter name="Id" value="#grdMaster.@Id"/>
</action>
```

- U subformi koristiš `$Id` direktno.

### 7.3 Globalni „signalni” flagovi

Čest pattern:

- `global.$RefreshajMaster` – subforma ga postavi na `1` nakon spremanja; parent forma na `shown`/`close` provjerava i radi `populate`.
- Ostali:
  - `global.$Filter`, `global.$DokId`, `global.$DatumOd`, `global.$DatumDo`.
  - U HR modulima: `global.$HRKadroviId`, `global.$BlokadaOd`, `global.$BlokadaDo`.

---

## 8. Eventi i akcije

### 8.1 Dozvoljeni `event/@type`

**[NOVI STANDARD]**:

- Form-level:
  - `init`, `load`, `shown`, `close`, `keydown`
- Kontrole:
  - `click`, `doubleclick`,
  - `focusedrowchanged`, `selectionchanged`,
  - `valuechanged`, `checkedchanged`, `selectedtabchanged`,
  - `cellvaluechanged`, `showingeditor`,
  - `leave`, `dragoverself`, `dragover`.

**[LEGACY – DOZVOLJENO]**:

- `oninit`, `onload`, `onshown`, `onclose`.
- U novim formama ne koristiti, ali tolerirati u postojećima.

### 8.2 Uloge `init`, `load`, `shown`, `close`

- `init`:
  - prvi `datarun` za lookup datasete,
  - inicijalne varijable (lokalne/globalne),
  - otvaranje param formi (`frmParametri`).
- `load`:
  - `control populate` za gridove,
  - enable/disable osnovnih gumbi ovisno o `rowcount` ili statusu,
  - postavljanje default vrijednosti (godina, datumi).
- `shown`:
  - fine UI prilagodbe (caption, fokus, vidljivost),
  - dodatna punjenja (npr. slike, PDF),
  - reakt na varijable (`$Tip`, `$Status`).
- `close`:
  - osvježi parent gride (`control populate parent.#grdX`, `control focus`),
  - cleanup (brisanje temp tablica, reset globalnih varijabli).

**[NOVI STANDARD – KRITIČNO]**: 

- **NE raditi `control populate` u `init` eventu na kontrole koje već imaju `dataname` atribut** (npr. `comboedit`, `searchedit`).
  - Engine automatski popunjava te kontrole kada se dataset popuni.
  - Ručno `control populate` na kontrolama s `dataname` može uzrokovati greške pri pokretanju forme.
  
❌ **POGREŠNO**:
```xml
<comboedit name="cmbPrinter" dataname="dtPrinteri" .../>
<event type="init">
  <action type="control populate" value="#cmbPrinter"/>  <!-- NE RADI! -->
</event>
```

✅ **ISPRAVNO**:
```xml
<comboedit name="cmbPrinter" dataname="dtPrinteri" .../>
<!-- Engine automatski popunjava kontrolu kada se dtPrinteri dataset popuni -->
```

### 8.3 Dozvoljene akcije

Najčešće:

- **Kontrole**:
  - `control populate` / `control populate keepfocus`
  - `control datasourceempty`
  - `control value`
  - `control focus`
  - `control enable` / `control disable`
  - `control visible`
  - `control caption`
  - `control attribute` (npr. `dbimg`, `dbpdf`, `format`; od Carpo 10.0.8.39 i `fixpdf` — vidi dolje)
  - `control refreshvalue` (često `"##"` – osvježi sve kontrole)
  - `control printcontrol` / `control export`
- **Forme**:
  - `form open`
  - `form close`
  - `form caption`
- **Data/varijable**:
  - `datarun reader`
  - `datarun nonquery`
  - `datarun scalar`
  - `datarun text`
  - `datarun integrated` (za integrated podatke, npr. partsql tree)
  - `variable value`
- **Ostalo**:
  - `validate`
  - `message`
  - `confirm`
  - `menu enable` / `menu disable`
  - `method run`
  - `transaction` (ugrađeni container u nekim modulima)
  - `wait` (progress prikaz, koristi se rijetko)
- **Fiskalizacija** (od Carpo 10.0.8.35, srpanj 2026):
  - `ZKI` — izračun zaštitnog koda; parametar `loadZKI` = ime data elementa s točno definiranim kolonama (string vrijednosti: `datumivrijeme`, `OIB`, `brojcanaOznakaRacuna`, `oznakaPoslovnogProstora`, `oznakaNaplatnogUredaja`, `ukupniIznosRacuna`)
  - `JIR` — potpis i slanje računa na CIS; parametri `loadJIR` (XML zahtjev s placeholderima `UUUUUUUUUU`/`ZZZZZZZZZZ`) i `saveZKIJIRUUD` (nonquery s parametrima `variablename="JIR"/"ZKI"/"UUID"`)
  - Detalji i primjeri: `docs/Novosti/2026-08-Carpo-novosti-10.0.8.37.md`
- **PDF „Popravi prikaz"** (od Carpo 10.0.8.39 / 10.0.10.39, rujan 2026):
  - `control attribute` s parametrom **`fixpdf`** (vrijednost = DPI, preporučeno `300`) na `pdfviewer` ili `documentviewer` kontroli — rasterizira trenutno učitani PDF (Pdfium) i ponovno ga sastavi (PdfSharp). Rješava pokidan prikaz macOS/Type0 PDF-ova koje automatski `PdfMacRepair` ne pokriva.
  - Rezultat je slika (nema selekcije teksta) → **samo na dugme, nikad automatski** u `init`/`shown`.
  - `fixpdf` je **parametar akcije**, ne atribut kontrole (`<pdfviewer fixpdf="…">` nije valjano).

  ```xml
  <button name="btnPopravi" caption="Popravi prikaz">
    <event type="click">
      <action type="control attribute" value="#pdfDokument">
        <parameter name="fixpdf" value="300"/>
      </action>
    </event>
  </button>
  ```

  - Detalji i isporuka (`PdfiumViewer.dll`, `PdfSharp.dll`): `docs/Novosti/2026-09-Carpo-novosti-10.0.8.39-fixpdf.md`
- **`text export`** — zapis teksta u datoteku (npr. platni nalozi, razmjenske datoteke). Obavezni parametri: `filePath`, `fileName`, `extension`, `textContent` (svi prolaze Parser: `#kontrola`, `~dt.@kolona`, `'literal'`). Opcionalni: `showOpenFileDialog` (`true` → nakon spremanja pita „Želite li otvoriti?“) i **`encoding`** (rujan 2026, Devex_23 + Devex_26).
  - **Bez `encoding`** datoteka je **UTF-8 bez BOM-a** (nepromijenjeno dosadašnje ponašanje).
  - `encoding` vrijednosti: `windows-1250` / `cp1250` / `1250` (ANSI latinica HR/RS/SI — Č, Ć, Š, Ž, Đ), `ansi` (sistemska ANSI kodna stranica računala — ovisi o Windows postavci, radije eksplicitno `windows-1250`), `utf-8`, `utf-8-bom`, `ascii`, `unicode`, ili bilo koji .NET/IANA naziv odn. broj code page-a (`iso-8859-2`, `windows-1251`…).
  - Znak koji ne postoji u ciljnoj kodnoj stranici (npr. ćirilica u 1250) postaje `?`.
  - Nepoznat naziv → poruka korisniku, datoteka se ne kreira.
  - Vrijednost može biti literal (bez ili s apostrofima) ili referenca (`#cmbEncoding`, `Global.$Encoding`).

  ```xml
  <action type="text export">
    <parameter name="filePath" value="#txtPutanja"/>
    <parameter name="fileName" value="'nalozi'"/>
    <parameter name="extension" value="'txt'"/>
    <parameter name="textContent" value="~dtNalozi.@sadrzaj"/>
    <parameter name="showOpenFileDialog" value="true"/>
    <parameter name="encoding" value="windows-1250"/>
  </action>
  ```

### 8.4 Ugniježđene akcije

**[NOVI STANDARD]**:

- Ugniježđivanje koristi se primarno unutar:
  - `action type="confirm"`
  - `action type="transaction"`
- Primjer:

```xml
<action type="confirm" value="Želite li obrisati?">
  <action type="datarun nonquery" value="~dtDelete"/>
  <action type="control populate" value="#grdMain"/>
</action>
```

**[LEGACY – DOZVOLJENO]**:

- U starijem kodu postoje ugniježđeni `action` unutar `datarun` ili `action if="..."` bez `type`. Ne koristiti u novom kodu.

### 8.5 `if` sintaksa

**[NOVI STANDARD]**:

- Bez razmaka:
  - `if="$Id==null"`
  - `if="$Status==1"`
  - `if="#grd.rowcount>0"`
  - `if="#grd.@Field=='X'&&$Flag==1"`

Podržane kombinacije:

- `&&`, `||` (logički operatori),
- zagrade su dopuštene, ali rijetko korištene (`if="($A==1||$B==2)&&$C==3"`).

`elseif` i `else`:

```xml
<action type="control enable" value="#btnSpremi" if="$Id!=null"/>
<action type="control disable" value="#btnSpremi" else=""/>
```

### 8.6 Više kontrola u jednoj akciji

```xml
<action type="control enable" value="#btnUnesi,#btnIzmijeni,#btnObrisi" if="#grd.rowcount>0"/>
<action type="control disable" value="#btnUnesi,#btnIzmijeni,#btnObrisi" if="#grd.rowcount==0"/>
```

---

## 9. Data-binding kontrola

### 9.1 `datavalue` / `datatext` (preporuka)

- **[NOVI STANDARD]**:

```xml
<textedit  name="txtNaziv" datavalue="~dtGet.@Naziv"/>
<checkedit name="chkAktivan" datavalue="~dtGet.@Aktivan"/>
<dateedit  name="datDatum" datavalue="~dtGet.@Datum" format="dd.MM.yyyy"/>

<searchedit name="srcPartner"
            dataname="dtPartneri"
            fieldvalue="PartneriId"
            fieldtext="Naziv"
            datavalue="~dtGet.@PartneriId"
            datatext="~dtGet.@PartnerNaziv"/>
```

- Ne raditi ručno `control value` u `init` ako postoji dataset + `datavalue`.

**[NOVI STANDARD – KRITIČNO]**:

- **NE koristiti `datavalue` ako dataset možda ne postoji ili nije popunjen pri pokretanju forme**.
  - Ako dataset nije popunjen u `init`/`load`, `datavalue` može uzrokovati greške pri pokretanju forme.
  - Umjesto toga, koristi `control value` u `load` eventu nakon što se dataset popuni, ili jednostavno ostavi kontrolu bez `datavalue` ako vrijednost nije kritična.

❌ **POGREŠNO** (ako dataset nije popunjen pri pokretanju):
```xml
<textedit name="txtBrojKopija" datavalue="~dtPrintOptions.@BrojKopija"/>
<datalist>
  <data name="dtPrintOptions" command="SELECT 1 AS BrojKopija" type="text"/>
</datalist>
<!-- Problem: ako dataset nije popunjen u init, forma se možda neće pokrenuti -->
```

✅ **ISPRAVNO** (opcija 1 - bez datavalue):
```xml
<textedit name="txtBrojKopija" format="n0"/>
<!-- Kontrola je prazna, korisnik može unijeti vrijednost -->
```

✅ **ISPRAVNO** (opcija 2 - s control value u load):
```xml
<textedit name="txtBrojKopija" format="n0"/>
<event type="load">
  <action type="datarun reader" value="~dtPrintOptions"/>
  <action type="control value" value="#txtBrojKopija">
    <parameter value="~dtPrintOptions.@BrojKopija"/>
  </action>
</event>
```

### 9.2 `control value` – kad je opravdano

Koristi se kada:

- treba kombinirati više izvora (npr. spajanje KUP i URA podataka),
- treba puni `text` i `value` za combo/searchedit u jednom koraku.

Primjer:

```xml
<action type="control value" value="#cmbOrgShema">
  <parameter value="~dtGet.@OrgShemaId"/>
</action>
<action type="control value" value="#srcPartner">
  <parameter value="~dtGet.@PartneriId"/>
  <parameter text="~dtGet.@PartnerNaziv"/>
</action>
```

### 9.3 Datumske kontrole

- Uvijek koristi `dateedit` (`dat*` prefiks) za datume.
- Format:
  - `format="dd.MM.yyyy"` ili lokalni standard.
- Regex validacije (`formattype="regex"`) koristi samo gdje je nužno (JMBG, OIB, staž).

---

## 10. Panelbuttons, actionpanel i gumbi

### 10.1 Panelbuttons

**[NOVI STANDARD – KRITIČNO]**:

- `<panelbuttons>` **MORA** imati `<buttons>` kao direktno dijete.
- Pojedinačni `<button>` elementi idu **unutar** `<buttons>`, **NE direktno** u `<panelbuttons>`.
- Bez `<buttons>` wrappera, buttoni se neće prikazati u formi.

Primjer:

```xml
<panelbuttons>
  <buttons>
    <button name="btnUnesi"   caption="UNESI"/>
    <button name="btnIzmijeni" caption="IZMIJENI"/>
    <button name="btnObrisi"  caption="OBRIŠI"/>
    <button name="btnZatvori" caption="ZATVORI" dock="right">
      <event type="click">
        <action type="form close"/>
      </event>
    </button>
  </buttons>
</panelbuttons>
```

**Pogrešno (buttoni se neće prikazati):**
```xml
<panelbuttons>
  <button name="btnUnesi" caption="UNESI"/>
  <button name="btnIzmijeni" caption="IZMIJENI"/>
</panelbuttons>
```

**Ispravno:**
```xml
<panelbuttons>
  <buttons>
    <button name="btnUnesi" caption="UNESI"/>
    <button name="btnIzmijeni" caption="IZMIJENI"/>
  </buttons>
</panelbuttons>
```

- Gumbi se obično implementiraju kroz `event type="click"` na gumbu.

### 10.2 Zatvaranje forme (bez `type="cancel"`)

**[NOVI STANDARD – OVAJ REPO]**:

```xml
<button name="btnZatvori" caption="ZATVORI">
  <event type="click">
    <action type="form close"/>
  </event>
</button>
```

- **Ne koristiti `type="cancel"`** u novom kodu ovog repo-a.

**[LEGACY – DOZVOLJENO]**:

- `type="cancel"` u starim formama; ne uvoditi u nove.

### 10.3 Actionpanel

- Alternativa `panelbuttons`, često na vrhu (toolbar):
  ```xml
  <actionpanel>
    <items>
      <button name="btnProslijedi" caption="ODOBRAVANJE"/>
      <button name="btnOdustani" caption="ZATVORI">
        <event type="click">
          <action type="form close"/>
        </event>
      </button>
    </items>
  </actionpanel>
  ```

---

## 11. Menu i kontekstne akcije

### 11.1 Menu na grid/tree

```xml
<grid name="grdMain" dataname="dtMain">
  <fields inherited="true"/>
  <menu showalways="false">
    <menuitem name="mnuInfo" caption="INFO">
      <event type="click">
        <action type="form open" value="frmInfo">
          <parameter name="Id" value="#grdMain.@Id"/>
        </action>
      </event>
    </menuitem>
    <menuitem name="mnuObrisi" caption="Obriši">
      <event type="click">
        <action type="confirm" value="Želite li obrisati?">
          <action type="datarun nonquery" value="~dtDelete"/>
          <action type="control populate" value="#grdMain"/>
        </action>
      </event>
    </menuitem>
  </menu>
</grid>
```

- `menu enable/disable` koristi se za dinamično uključivanje ovisno o statusu redova.

---

## 12. Tabs – layout vs managerlayout

### 12.1 Tabs u layoutu (custom)

- Implementirani preko `<tabs>` + `<group caption="...">` unutar `layout`.
- `<tabs>` mora biti direktno dijete root `group` u layoutu (bez `<item>` wrappera),
  u suprotnom kontrole nisu registrirane i forma ostaje prazna.

**Napomena**: Ako kontrole nisu ispravno registrirane u layoutu, `load` eventi koji pozivaju `control populate` će logirati grešku **"Kontrola '#...' nije pronađena"** i UI će ostati prazan.

### 12.2 Tabs u managerlayoutu

- Implementirani preko `<tabs>` + `<tab>` elemenata.

---

## 13. Part i PartSQL

### 13.1 `part` – statički layout (XML)

- Definicija u posebnom `parts` XML-u:

```xml
<parts>
  <part name="prtRobaUpsert">
    <item row="1" col="1" caption="Naziv">
      <textedit name="txtNaziv" datavalue="~dtGet.@Naziv"/>
    </item>
  </part>
</parts>
```

- U glavnoj formi:

```xml
<item row="1" col="1">
  <part name="prtRobaUpsert" path="prtRobaUpsert.xml"/>
</item>
```

### 13.2 `partsql` – layout iz baze

**Pattern:**

- Globalni `<datalist>`:

```xml
<datalist>
  <data command="spPartQuery" name="dtPartFilter" type="reader">
    <parameter name="@Action" value="filter"/>
    <parameter name="@Datoteka" value="HRManagerCP"/>
  </data>
</datalist>
```

- U formi:

```xml
<lookup name="filter">
  <partsql name="filter" dataname="~dtPartFilter"/>
</lookup>
```

- Koristi se i za button-barove (`partUraKupSpremiProknjizi`) i validacije (`partSpremiValidation`).

---

## 14. DLL integracije (`method run`) i custom kontrole

### 14.1 `method` definicija

```xml
<method name="fnDimenzije"
        dll="Dimenzije"
        class="DimenzijeNet.Dimenzije"
        method="run"/>
```

### 14.2 `method run`

```xml
<action type="method run" value="fnDimenzije">
  <parameter name="server"  value="@@server"/>
  <parameter name="database" value="@@database"/>
  <parameter name="OperateriId" value="@@operateriid"/>
  <parameter name="tip" value="FakDetaljId"/>
  <parameter name="id"  value="#grdRoba.@FakDetaljId"/>
</action>
```

Preporuka:

- Poslovna logika u DLL-u; forma šalje samo tehničke parametre.

**[NOVO – POVRATNA VRIJEDNOST IZ DLL-a]**

- Engine (JCustomMethod) sprema return vrijednost u FormsVar samo ako .NET metoda ima **barem jedan parametar**; bez parametara return se ne mapira.
- Vrijednost se sprema pod imenom metode iz XML-a (`<method name="...">`), pa se u akcijama koristi `$<methodName>`.
- Siguran pattern:
  ```xml
  <method name="fnDllResult" dll="X" class="X.EntryPoint" method="Run"/>
  ...
  <action type="method run" value="fnDllResult">
    <parameter name="input" value="'Poziv iz Carpa'"/> <!-- dummy param -->
  </action>
  <action type="message" value="$fnDllResult"/>
  ```
- .NET potpis npr. `public string Run(string input = null)`; vraćeni string će biti dostupan kao `$fnDllResult`.

### 14.3 CustomControl

```xml
<item row="1" col="1">
  <customcontrol name="uccCalc"
                 dll="Carpo.CustomControls.dll"
                 class="CalcControl">
    <parameter name="Cijena" value="#txtCijena"/>
    <parameter name="PorezPosto" value="#txtPorez"/>
  </customcontrol>
</item>
```

---

## 15. Poslovni patterni (sažetak)

Ovdje su sažeti glavni poslovni patterni koji se kroz module ponavljaju.

### 15.1 CRUD: master grid + subforma

- Glavna forma:
  - `grdMaster` s `dataname="dtMaster"`.
  - Panelbuttons:
    - `UNESI` – `form open frmEdit` s `Id=null`.
    - `IZMIJENI` – `form open frmEdit` s `Id=#grdMaster.@Id`.
    - `OBRIŠI` – `confirm` → `~dtDelete` → `populate #grdMaster`.
- Subforma:
  - `init`:
    - ako `$Id!=null` → `~dtGet` → `datavalue` puni kontrole.
  - `btnSpremi`:
    - `validate`.
    - Insert:
      ```xml
      <action type="datarun scalar" value="~dtInsert">
        <parameter name="$Id"/>
      </action>
      ```
    - Update:
      ```xml
      <action type="datarun nonquery" value="~dtUpdate"/>
      ```
    - Nakon toga:
      ```xml
      <action type="control populate" value="parent.#grdMaster"/>
      <action type="form close"/>
      ```

### 15.2 Master-detail unutar jedne forme

- `managerlayout`:
  - `tree` → `grdMaster` → `grdDetail`.
  - `focusedrowchanged` na `tree` puni `grdMaster`; `focusedrowchanged` na `grdMaster` puni `grdDetail`.

### 15.3 Blokada po datumu

- Dataset `dtBlokada` vraća `Od` i `Do`.
- Na `btnSpremi`:
  ```xml
  <action type="datarun reader" value="~dtBlokada"/>
  <action type="variable value" value="global.$BlokadaOd">
    <parameter value="~dtBlokada.@Od"/>
  </action>
  <action type="variable value" value="global.$BlokadaDo">
    <parameter value="~dtBlokada.@Do"/>
  </action>
  <action type="message" if="#datDatumDok<global.$BlokadaOd||#datDatumDok>global.$BlokadaDo"
          value="'Datum dokumenta je izvan dopuštenog razdoblja knjiženja!'"/>
  ```
- Ako je blokirano, prekini daljnje `datarun`.

---

## 16. UX, validacija i formati

### 16.1 Validacija

- Svaki gumb „SPREMI“/„UNESI“/„IZMIJENI“ mora imati `validate` kao prvu akciju.
- Obavezna polja:
  - `required="true"` i/ili `allownull="false"` na kontrolama.

### 16.2 Guard poruke i `stop` atribut

**NIKADA ne koristiti `stop="true"` na `<action type="message">`.**

Engine semantika `stop="true"` je: **ako `if` uvjet vrati FALSE, prekini cijeli lanac akcija**. To je suprotno od očekivanog ponašanja (da se lanac prekine nakon prikaza poruke). Umjesto `stop`, oslanjati se na `if` uvjete na sljedećim akcijama.

**`action type="message"` – value MORA biti u navodnicima.** Engine poziva `Parser.GetLogicPartValue(action.Value)`; bez navodnika (npr. `value="Odaberite red."`) parser tretira tekst kao logički izraz, vraća null i poruka se ne prikaže (log: "Neuspješno parsiranje vrijednosti ili je vrijednost null"). Ispravno: `value="'Odaberite red u listi.'"` (jednostruki navodnici unutar value atributa).

Ispravan obrazac za gumb koji zahtijeva odabrani red (single-select, focused row):

```xml
<button name="btnIzmjeni" caption="Izmjeni">
  <event type="click">
    <action type="message" value="'Odaberite red u listi.'" if="#grdMaster.rowcount==0"/>
    <action type="form open" value="frmEdit" if="#grdMaster.rowcount&gt;0">
      <parameter name="id" value="#grdMaster.@Id"/>
    </action>
    <action type="control populate" value="#grdMaster" waittype="progress" if="#grdMaster.rowcount&gt;0"/>
  </event>
</button>
```

**Multiselect grid – dugmad koja zahtijevaju selekciju: disable umjesto message**

Kada dugme koristi `control foreachrow selected` ili `#grdX.selectedids` (TotalGenMultiSelectTemp pattern), **preporučeno** je dugme onemogućiti kad nema selekcije – umjesto poruke nakon klika. Grid mora imati `multiselect="true"` i `showcheckboxes="true"`. Na grid dodati `selectionchanged` i `focusedrowchanged` s `control enable`/`control disable`; u `shown` i nakon svakog `control populate` ažurirati stanje dugmadi. Vidi rule **konvertirane-granule** za kompletan pattern.

Za brisanje s potvrdom koristiti `type="confirm"` (NE `type="message" confirm="true"`):

```xml
<button name="btnObrisi" caption="Obriši">
  <event type="click">
    <action type="message" value="'Odaberite red u listi.'" if="#grdMaster.rowcount==0"/>
    <action type="confirm" value="Želite li obrisati odabrani zapis?" if="#grdMaster.rowcount&gt;0"/>
    <action type="datarun nonquery" value="~dtDelete" if="#grdMaster.rowcount&gt;0"/>
    <action type="control populate" value="#grdMaster" waittype="progress" if="#grdMaster.rowcount&gt;0"/>
  </event>
</button>
```

`CheckConfirm` po defaultu prekida lanac ako korisnik klikne "Ne" (bez potrebe za dodatnim atributima).

### 16.3 Decimalni separator i formati

- Koristiti zarez u literalima:
  ```xml
  <parameter value="0,00"/>
  ```
- Formati:
  - `format="n0"`, `format="n2"`, `n3`, `n5`, `n8` ovisno o preciznosti.

### 16.4 Performance napomene

- Veliki gridovi + async:
  - `async="true"`,
  - paginacija i filteri u SP-u.
- Ne raditi dupli `control populate` istog grida u `load` i `shown`.

### 16.5 ONLOADED i ONLOAD2 mapiranje

- NetGen `ONLOADED` (izvršava se nakon renderiranja forme) → Carpo `event type="shown"`.
- NetGen `ONLOAD2` (dodatna inicijalizacija nakon ONLOAD) → Carpo `event type="shown"`.
- Ako forma ima i ONLOADED i ONLOAD2, oba idu u `event type="shown"` (redoslijed akcija po potrebi).
- `event type="init"` koristi se za ONLOAD (prije renderiranja).

### 16.6 EDITCOLUMNS i ONCELLEDITED u gridu

- NetGen `EDITCOLUMNS="Kolona1,Kolona2"` → Carpo: dodati `<field name="Kolona1" editable="true"/>` u `<fields>` sekciju grida.
- NetGen `ONCELLEDITED="spXxx ..."` → Carpo: `<event type="cellvaluechanged">` na gridu s odgovarajućim akcijama.
- Engine podržava `editable` atribut na `JField` (`Carpo.Structure/Models/Core/Fields.cs`).

### 16.7 FOREACHROW sa inline SQL

- NetGen `FOREACHROW(gridname)#SQL1#SQL2` → Carpo:

```xml
<action type="control foreachrow" value="#grdIme">
  <action type="datarun nonquery" value="~dtPerRow"/>
</action>
```

- Za inline SQL koristiti `data type="text"` s parametrima koji referenciraju grid kolone (`#grdIme.@kolona`).
- SQL koji se odnosi na zaglavlje (ne na svaki red) staviti kao zasebnu akciju IZVAN `control foreachrow` bloka.
- `#OK` na kraju NetGen poziva → `<action type="form close" value="true"/>` nakon svih akcija.

### 16.8 COMMANDTIMEOUT

- NetGen `COMMANDTIMEOUT="N"` → Carpo `commandtimeout="N"` na `<form>` i/ili `<data>` elementima.
- Preporuča se staviti na oba mjesta: na formu (globalno) i na data element koji poziva sporu SP (precizno).

### 16.9 Globalne temp tablice (##)

- SQL globalne temp tablice (`##TablicaIme`) perzistiraju dok postoji konekcija.
- Carpo koristi istu SQL konekciju za sve forme → `##` tablice funkcioniraju isto kao u NetGen-u.
- Ako forma koristi `##` tablicu koja se puni u drugoj formi, osigurati da se punjenje izvrši PRIJE otvaranja zavisne forme (npr. u `event type="shown"`).

---

## 17. Kompatibilnost i migracija (sažetak)

- Novi razvoj:
  - Koristiti ovaj dokument kao izvor pravila.
  - Uvijek:
    - `root version="1.2"`,
    - `form/@type`,
    - `fields inherited="true"`,
    - `datavalue` za binding,
    - `type` na `<data>` samo kad je potrebno (ili eksplicitno trazeno).
- Postojeći kod:
  - Poštovati postojeće obrasce ako su usko vezani uz poslovni zahtjev ili engine specifičnosti.
  - Ne uvoditi nove legacy pattern-e (`oninit`, inline `<data>` u gridovima, `button type="cancel"`).
  - Kod većih refaktora (promjena SP-ova, strukture dataseta) – raditi postupno i testirano.

---

## 18. Sažetak ključnih „DA / NE“

**DA (za novi kod):**

- `root version="1.2"`.
- `form name="..." type="custom|master|manager"` za glavnu formu (bez `frm` prefiksa).
- Jedan root `<group>` po `layout`.
- `fields inherited="true"` + ručni override gdje treba.
- `dataname="dtX"` / `global.dtX` + `<datalist>` na odgovarajućoj razini.
- `datarun scalar` za insert koji vraća ID.
- `confirm` za destruktivne radnje.
- `button` za zatvaranje forme s `action type="form close"`.
- `datavalue`/`datatext` za binding, `control value` za specijalne slučajeve.
- `event type="init/load/shown/close"` (bez `on` prefiksa).
- `if` uvjeti bez razmaka (`$Id==null`, ne `$Id == null`).

**NE (u novom kodu):**

- `oninit`/`onload`/`onshown`/`onclose`.
- Inline `<data>` unutar `grid`/`tree` (bez `<datalist>`).
- `type` na `<parameter>` bez stvarne potrebe.
- `type="cancel"` u novom kodu.
- Nasumično korištenje `Global.` vs `global.`; standardizirati na `global.$Var` i `global.dtX`.
- Više kontrola s istim `name` unutar jedne forme.

---

## 19. Projektna pravila (Filip/Skorup) – obavezno u ovom repou

Ova pravila su nastala iz prakse nakon koristenja CarpoBota. Ne uvode novu
strukturu i **ne smiju biti u konfliktu** s `carpo.xsd`. Ako je u konfliktu sa
ranijim smjernicama iz ovog dokumenta, **za ovaj repo prednost imaju pravila iz
ove sekcije**.

### 19.1 Stored procedure pravila

- **Ne izmisljati nazive stored procedura.**
- **Ne pretpostavljati postojanje** procedura ili tablica bez provjere.
- Prije koristenja `command` atributa u `<data>` obavezno provjeriti da SP
  postoji u `db/StoredProcedures/`.
- Ako procedura ne postoji:
  - **Ne izmisljati.**
  - Dodati TODO komentar u XML i obavijestiti korisnika.
  - Kreirati novu SP ili SQL **samo ako korisnik eksplicitno trazi**.

Primjer:
```xml
<!-- TODO: Kreirati stored procedure spNazivProcedure -->
<data command="spNazivProcedure" name="dtNaziv">
  ...
</data>
```

- **Ne mijenjati stored procedure** u `db/StoredProcedures/` (nema izmjena
  kolona, parametara ni logike).
- Ako procedura vraca drugacije kolone po akcijama, rjesenje je u XML-u
  (`key`, `valuefield`, prilagodba bindinga), ne u SQL-u.

### 19.2 `type` u `<data>` i `<parameter>`

- **Ne dodavati `type`** u `<data>` elementima u `datalist` osim ako korisnik
  eksplicitno trazi ili postoji dokumentiran problem.
- **Ne dodavati `type`** u `<parameter>` elementima osim ako korisnik to
  eksplicitno trazi.
- **Izuzetak**: `partsql` dataseti (vidi 19.9) zahtijevaju `type="reader"`.

### 19.3 Tree i lookup pravila

- U `<tree>` kontroli koristiti `key`, ne `keyfield`.

Primjer:
```xml
<tree name="tree" dataname="dtTree" key="sortkey" parentfield="parentid">
  ...
</tree>
```

- **Lookup u managerlayout zahtijeva tree:** Carpo Manager.cs kada vidi `<lookup>` u managerlayout, očekuje da postoji tree kontrola (`trMain`). U `bbiLookUpFilter_EditValueChanged` poziva se `trMain.SetSearchField(searchField)` – ako nema tree, `trMain` je null i dolazi do **NullReferenceException** pri otvaranju granule. **Rješenje:** Ne koristiti lookup u managerlayout ako granula nema tree. Umjesto lookup-a (Danas/Jučer/Parametri) koristiti frmFilter dijalog za parametre; default (npr. Danas) postaviti u init varijabli i dtMaster s `if="global.$FilterFromForm==0"` koristiti node/datum. **Uzor greške:** MPPregDokMPVPCarpo – lookup bez tree; ispravljeno uklanjanjem lookupa i korištenjem frmFilter.

- **Ne dodavati `control populate`** u `valuechanged` eventu lookupa ako:
  - tree ima `dataname`,
  - dataset koristi `if/elseif` koji ovisi o varijabli postavljenoj u
    `valuechanged`.
- Engine tada automatski osvjezava tree. `control populate` se dodaje samo
  kada kontrola nema `dataname` ili je eksplicitno trazeno.

### 19.4 Button pravila (prefix, caption, zatvaranje, dock)

- U ovom projektu koristi se prefix **`btn`** za `<button>` kontrole (ne `cmd`).
- Caption treba pratiti postojece stilove (npr. svi uppercase).
- Gumb za zatvaranje forme treba koristiti `action type="form close"` i **ne
  koristiti `type="cancel"`**.
- `dock="left"` se **ne navodi** (default). `dock="right"` i `dock="fill"`
  se navode eksplicitno.

Primjer:
```xml
<button name="btnZatvori" caption="ZATVORI" dock="right">
  <event type="click">
    <action type="form close"/>
  </event>
</button>
```

### 19.5 Boldiranje

- Pojedinacna kontrola u layoutu: `fontbold="true"` na kontroli.
- Pojedinacna kolona u gridu: `fontbold="true"` na `<field>`.
- Uvjetno boldiranje: `styles` s `style name="fontbold" value="true" if="..."`.

### 19.6 Enable/disable i vidljivost

- `control enable`, `control disable`, `control visible` stavljati u
  `event type="shown"` ili `event type="load"`.
- U `init` koristiti `datarun` i `variable value`.

### 19.7 Osvjezavanje parent gridova

- Nakon spremanja u subformi **uvijek** osvjeziti parent gridove i fokusirati
  odgovarajuci red:
```xml
<action type="control populate" value="parent.#grdMaster">
  <action type="control focus" value="parent.#grdMaster">
    <parameter value="$Id"/>
  </action>
</action>
<action type="control populate" value="parent.#grdDetail"/>
```

- Nakon brisanja koristi se `control populate` na gridu (ne `datarun reader`).
- Ako je brisanje u subformi, osvjeziti i parent master grid.

### 19.8 Forma za unos/izmjenu – zatvaranje samo pri izmjeni

- Kod izmjene (kada `$Id` postoji) **zatvoriti formu**.
- Kod unosa (kada `$Id` ne postoji) **ne zatvarati formu**.
- Implementirati kroz dva `if` bloka u `event type="click"` na `SPREMI`.

### 19.9 Imenovanje XML datoteka i formi

- **Glavni XML fajl**: bez `frm` prefiksa u nazivu fajla.
- **`name` atribut forme**: isti kao naziv fajla bez ekstenzije (bez `frm`
  prefiksa).

Primjer:
- Fajl: `KonfigSrvRN.xml`
- Forma: `<form name="KonfigSrvRN" ...>`

**Subforme kao posebni XML fajlovi:**
- Fizicki fajl: `[NazivGlavnogXMLa].[NazivForme].xml`
- U glavnom XML-u `path` koristi samo naziv forme (bez prefiksa glavnog XML-a).

Primjer:
```xml
<!-- Glavni XML: Tarifa.xml -->
<form name="frmTarifa" path="frmTarifa.xml"/>
<!-- Stvarni fajl subforme: Tarifa.frmTarifa.xml -->
```

**Part-ovi kao posebni XML fajlovi:**
- Fajl part-a je isti kao u `path` (bez prefiksa glavnog XML-a).

### 19.10 PartSQL (layout iz baze)

- Dataset za partsql mora biti u **root** `<datalist>`.
- `type="reader"` je obavezan jer procedura vraca XML.
- Procedura mora vratiti:
```sql
SELECT '<parts>
  <part name="prtTestCursor">
    <layout>...</layout>
    <panelbuttons>...</panelbuttons>
  </part>
</parts>'
```

Upotreba u formi:
```xml
<form name="frmTestPARTSQL" caption="Test PART SQL">
  <partsql name="prtTestCursor" dataname="global.~dtPartTestPARTSQL"></partsql>
</form>
```

Napomene:
- `partsql name` mora odgovarati `part name` iz procedure.
- `dataname` koristi `global.` prefiks.
- `init` event nije potreban; globalni dataset se ucitava automatski.

### 19.11 Lokacija novih granula

- **Nove Carpo granule** se po defaultu spremaju u `GeneriraneGranule/CarpoGranule/`.
- **Konvertirane NetGen granule** se spremaju u `GeneriraneGranule/KonvertiraneGranule/{ImeGranule}/` (jedan subfolder po granuli).

---

## 20. Info template (InfoPunktovi) – detaljna pravila

Ovaj dio konsolidira sva pravila za `template="info"` forme (InfoPunktovi).

### 20.1 Aktivacija

Forma koristi **Info** UI samo ako `form/@template` sadrži `info` (case-insensitive).  
U suprotnom se koristi standardna forma.

### 20.2 Obavezna struktura

Info template očekuje `form` sa:

- `template="info"`
- `type="master"` (init forma se bira po `type="master"` ako postoji)
- `infocontrols` sekcijom
- **jednim ili više** `<layout>` elemenata (svaki layout postaje tab)

Minimalni skeleton:

```xml
<form name="...Info" type="master" template="info" ...>
  <event type="init">...</event>
  <event type="load">...</event>
  <event type="shown">...</event>

  <infocontrols entity="..." searchtype="popup|default">
    <tabs name="tabs">...</tabs>
    <search name="srcInfo" ...>...</search>
    <navigation name="navInfo">
      <data .../>
    </navigation>
    <info name="info"/>
    <newbutton name="btnNoviUnos" caption="...">...</newbutton>
  </infocontrols>

  <layout caption="Tab 1" function="tabOpcenito">...</layout>
  <layout caption="Tab 2" function="tabX">...</layout>

  <datalist>...</datalist>
</form>
```

### 20.3 `infocontrols` – obavezni elementi i atributi

`infocontrols` je core definicija. U runtime-u se čita bez null-checkova, zato mora postojati sa svim elementima.

Obavezno:
- `entity="..."` – koristi se za InfoHistory (zadnje pregledani zapisi).
- `searchtype="popup|default"` – određuje primarni search.
- `<tabs>`, `<search>`, `<navigation>`, `<info>`, `<newbutton>` – svi elementi moraju biti prisutni.

### 20.4 `tabs` + `layout` – formiranje tabova

**Sadržaj kartica (tab stranica)** ne definira se listom **`<tab>`** unutar **`infocontrols`**. Umjesto toga:

- svaki **`<layout>`** izravno pod **`<form>`** (iza `infocontrols`) postaje **jedna stranica** u tab kontroli
- caption tab-a = **`layout/@caption`**
- redoslijed layouta = redoslijed tabova (0-based indeks)
- **`layout/@function`** se koristi za permissions (enable/visible tab)

Praktično: ako želiš 5 tabova → definiraš 5 **`layout`** blokova.

**Usklađeno s runtime-om (referenca):**

- Element **`infocontrols/<tabs>`** nije „prazan XML radi sheme“: u **`Carpo.UI.Forms.Info`** definicija se dodjeljuje **`tabs.JTabsInit`** i odnosi se na **isti** `JXtraTabControl` u koji **`JupiterEditor.SetTab`** (poziv iz **`SetLayout`**) dodaje **`XtraTabPage`** za svaki layout. Stranice se **ne** grade iz **`JTabs.TabList`** / **`<tab>`** pod **`infocontrols/tabs`** u Info predlošku (ta struktura je tipična za druge obrasce, npr. Manager s grid tabovima).
- Na **`<tabs>`** mogu stajati atributi **`tabslocation`**, **`tabsorientation`** te **`<event>`** s tipovima koji počinju na **`tabchanging`** / **`tabchanged`** (obrađuju se kroz tab kontrolu).
- Do prvog **`shown`** forme tab kontrola ima **`RunActions = false`**; u **`Info_Shown`** postavlja se **`true`** — do tada se događaji promjene taba ne izvršavaju.
- Forma izlaže **`$SelectedTabIndex`** (inicijalno 0; ažurira se pri promjeni taba).

Za korisnički orijentirani opis vidi i **`docs/Novosti/Carpo-Info-predlozak-korisnicki-vodic.md`** (odjeljak o tabovima).

### 20.5 `search` – Info pretraga (AutoSuggest)

`<search>` koristi `JAutoSuggest` i mora imati:

- `name="srcInfo"` (ili drugi name, ali u praksi sve reference idu na `#srcInfo`)
- `dataname="dtX"` ili `<data>` unutar `<search>`
- `fieldvalue` i `fieldtext`

**Data contract (obavezno):**
- Kolona 0 = `Id` (int)
- Kolona 1 = display (tekst)

### 20.6 `navigation` – Info navigator

`<navigation>` je `JDataNavigator` i mora imati dataset koji vraća **Id + display**.

**Data contract (obavezno):**
- Kolona `Id` mora postojati po imenu (koristi se `Select("Id=...")`)
- Druga kolona = display (tekst)

### 20.7 `info` – informativni blok

`<info>` se u kodu koristi minimalno (log opis punjenja), ali se preporučuje da uvijek postoji:

```xml
<info name="info"/>
```

### 20.8 `newbutton` – gumb za novi unos

`<newbutton>` se mapira na standardni “Novi unos” gumb u Info UI.  
Ako nije definiran, gubi se standardni gumb.

### 20.9 Edge-case ponašanja (što puca)

- Ako fali `<infocontrols>` → runtime greška pri inicijalizaciji.
- Ako fali `<search>` ili `<navigation>` → nema fallbacka.
- Ako fali `entity` → InfoHistory logika neće raditi.

### 20.10 Minimalni primjer

```xml
<root version="1.2">
  <form name="PartnerInfoMini" type="master" template="info" caption="Partner info">
    <infocontrols entity="partneri" searchtype="popup">
      <tabs name="tabs"/>
      <search name="srcInfo" dataname="dtSearch" fieldvalue="Id" fieldtext="Naziv"/>
      <navigation name="navInfo">
        <data name="dtNav" command="spCarpoInfo_Query">
          <parameter name="@action" value="partneriNav"/>
        </data>
      </navigation>
      <info name="info"/>
      <newbutton name="btnNew" caption="Novi partner"/>
    </infocontrols>

    <layout caption="Općenito" function="tabOpcenito">
      <group cols="1" rows="1">
        <item row="1" col="1" caption="Naziv">
          <textedit name="txtNaziv"/>
        </item>
      </group>
    </layout>
  </form>
</root>
```

---

## 21. DaysEdit (`daysedit`) – bazni datum

### 21.1 Ponašanje

- **Bez `basedate`** → računa od današnjeg datuma (`DateTime.Now`), kao do sada; pri otvaranju kalendara prikazuje se bazni datum + broj dana iz polja.
- **Sa `basedate`** → računa od baznog datuma (npr. datum fakture); pri otvaranju kalendara (pop-up) prikazuje se **današnji datum** kako bi korisnik lako odabrao „danas”.

Ovo je backward‑compatible i ne lomi postojeće XML‑ove.

### 21.2 Atribut `basedate`

`basedate` može biti:

- referenca na kontrolu: `#datDatumFakture`
- referenca na dataset field: `~dtGlava.@DatumFakture`

Primjeri:

```xml
<daysedit name="dayRok" dataname="dtGlava" field="RokDana" basedate="#datDatumFakture"/>
<daysedit name="dayRok2" dataname="dtGlava" field="RokDana2" basedate="~dtGlava.@DatumFakture"/>
```

### 21.3 Razlog

Tipičan poslovni slučaj: rok plaćanja treba računati od datuma fakture, a ne od “danas”.

---


## 22. SETDEFAULTS -- inline SQL pri pokretanju granule

### 22.1 Ponasanje

NetGen `PROJECT SETDEFAULTS="SQL..."` izvrsava inline SQL pri pokretanju granule (prije otvaranja bilo koje forme).
Koristi se za cleanup -- npr. brisanje temp podataka za operatera.

### 22.2 Carpo mapiranje

U master formi, `event type="init"`:

```xml
<event type="init">
  <action type="datarun nonquery" value="~dtSetDefaults"/>
</event>
```

Data element u `datalist`:

```xml
<data type="text" name="dtSetDefaults" command="delete from TempTabela where operateriid = @operateriid">
  <parameter name="@operateriid" value="@@operateriid"/>
</data>
```

Napomena: NetGen SETDEFAULTS koristi `(select OperateriId from ssOperateriSession where HostName=HOST_NAME() and systemuser=SUSER_NAME())` za identifikaciju operatera. U Carpu koristiti `@@operateriid` koji je dostupan kao ugradjena varijabla.

---

## 23. NEXTFORM -- lancano otvaranje formi

### 23.1 Ponasanje

NetGen `NEXTFORM="ImeSljedeceFormeNaziv"` na formi znaci: nakon zatvaranja ove forme (sa OK), otvori sljedecu formu.
Cest pattern: kreiranje narudzbenice, zatim prikaz kreiranog dokumenta.

### 23.2 Carpo mapiranje

U Carpo nema NEXTFORM koncepta. Mapirati na sekvencijalne akcije u button click eventu:

```xml
<action type="datarun reader" value="~dtKreiraj"/>
<action type="form open" value="frmSljedeca">
  <parameter name="id" value="~dtKreiraj.@noviId"/>
</action>
<action type="form close"/>
```

Kljucno: ako SP vraca ID novog zapisa koji se prosljedjuje sljedecoj formi, koristiti `datarun reader` (ne nonquery) za dohvat ID-a.

---

## 24. Conditional SP calls -- vise SP-ova po istoj logici

### 24.1 Ponasanje

Kada NetGen forma poziva razlicite stored procedure ovisno o odabiru korisnika (npr. vrsta izvjestaja), koristi `if @@Var=1 begin exec sp1... end if @@Var=2 begin exec sp2... end`.

### 24.2 Carpo mapiranje

Koristiti `data type="text"` s conditional EXEC u inline SQL-u:

```xml
<data type="text" name="dtConditional" commandtimeout="600" command="
if @VrstaId = 1 exec spVarijanta1 @action=1, @Param1=@Val1, @Param2=@Val2
if @VrstaId = 2 exec spVarijanta2 @action=1, @Param1=@Val1, @Param2=@Val2
if @VrstaId = 3 exec spVarijanta3 @action=1, @Param1=@Val1, @Param2=@Val2">
  <parameter name="@VrstaId" value="#cmbVrsta"/>
  <parameter name="@Val1" value="#kontrola1"/>
  <parameter name="@Val2" value="#kontrola2"/>
</data>
```

Pozivati sa `datarun nonquery` za DML operacije ili `control populate` za grid podatke (ako grid koristi isti data element).

### 24.3 Napomene

- Svaki `if` blok mora pozivati SP sa istom strukturom parametara.
- Parametri koji su specificni za jednu varijantu (npr. `@IndIsporuceno` samo u jednom SP-u) ne trebaju biti u inline SQL-u ako SP ima default vrijednost.
- `commandtimeout` staviti na data element jer conditional SP-ovi mogu biti dugotrajni.

---

## 25. Filter forme sa Global varijablama -- persistence pattern

### 25.1 Ponasanje

Kada filter forma treba pamtiti parametre izmedju otvaranja (NetGen ONLOAD vraca prethodne `@@` vrijednosti).

### 25.2 Carpo mapiranje

**Master init -- deklaracija:**

```xml
<event type="init">
  <action type="variable value">
    <parameter name="Global.$VrstaId" value="null"/>
    <parameter name="Global.$DatumOd" value="null"/>
    <parameter name="Global.$Dimenzija" value="2"/>
  </action>
</event>
```

**Filter shown -- restore:**

```xml
<event type="shown">
  <action type="control populate" value="#cmbVrsta,#cmbDimenzija"/>
  <action type="datarun reader" value="~dtRestoreFilter"/>
  <action type="assign" target="#cmbVrsta" value="Global.$VrstaId"/>
  <action type="assign" target="#datDatumOd" value="~dtRestoreFilter.@DatumOd"/>
</event>
```

`dtRestoreFilter` je inline SQL koji vraca Global vars sa coalesce za defaulte:

```xml
<data type="text" name="dtRestoreFilter" command="select @DatumOd as DatumOd, coalesce(@Dimenzija, 2) as Dimenzija">
  <parameter name="@DatumOd" value="Global.$DatumOd"/>
  <parameter name="@Dimenzija" value="Global.$Dimenzija"/>
</data>
```

**Filter Prihvati -- save:**

```xml
<action type="variable value" value="Global.$VrstaId">
  <parameter value="#cmbVrsta"/>
</action>
<action type="variable value" value="Global.$DatumOd">
  <parameter value="#datDatumOd"/>
</action>
```

**Grid data element -- koristi Global vars:**

```xml
<data type="text" name="dtMasterGrid" command="exec spXxx @action=12, @DatumOd=@p_DatumOd">
  <parameter name="@p_DatumOd" value="Global.$DatumOd"/>
</data>
```

Za searchedit restore koristiti `control value` sa oba parametra (value + text):

```xml
<action type="control value" value="#srcPartner" if="Global.$PartnerId!=null">
  <parameter value="~dtRestore.@PartnerId"/>
  <parameter text="~dtRestore.@PartnerNaziv"/>
</action>
```

---


### 40. Pravilo `@p_` prefiksa za `data type="text"` sa `exec` pozivima

Kod `data type="text"`, Carpo vrsi **tekstualnu zamjenu** svih `@paramName` tokena u `command` atributu sa stvarnim vrijednostima iz `<parameter>` elemenata. Problem nastaje kada se u `exec spXxx @SPParam=@CarpoParam` koristi **isti naziv** za SP parametar i Carpo placeholder -- oba se zamijene i dobije se npr. `7=7` umjesto `@operateriid=7`.

**Pravilo:** Uvijek koristiti `@p_` prefiks za Carpo placeholder parametre u `command` tekstu koji sadrzi `exec` pozive stored procedura:

```xml
<!-- KRIVO: @operateriid=@operateriid -- obje zamjene daju 7=7 -->
<data type="text" name="dtX" command="exec spXxx @action=1, @operateriid=@operateriid">
  <parameter name="@operateriid" value="@@operateriid"/>
</data>

<!-- ISPRAVNO: @operateriid=@p_operateriid -- samo @p_operateriid se zamijeni -->
<data type="text" name="dtX" command="exec spXxx @action=1, @operateriid=@p_operateriid">
  <parameter name="@p_operateriid" value="@@operateriid"/>
</data>
```

Ovo pravilo **ne utjece** na:
- `data type="text"` sa SELECT upitima (npr. `select @DatumOd as DatumOd`) -- tu nema kolizije jer se SQL alias pise bez `@`.
- `data` bez `type="text"` (standardni SP pozivi) -- Carpo koristi `SqlParameter` objekte, ne tekstualnu zamjenu.

---

*Ovaj dokument „Carpo XML Rules (FINAL v2)” je konsolidirani standard. Sve nove forme i izmjene trebaju se ravnati prema njemu, uz poštivanje kompatibilnosti s postojećim XML-ovima.*