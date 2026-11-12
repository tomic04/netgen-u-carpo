# Carpo Devex_26 — vodič za XML autore

**Za:** odjele / kolege koji pišu granule (XML).  
**Grana:** `$/Carpo/Devex_26` (DX 26.1), od branch-a (#5081) do Wave 12 (#5127).  
**Živi changelog (waveovi):** [`Carpo_DX_Changelog.md`](Carpo_DX_Changelog.md)  
**B6 detalji / buy-in:** [`Carpo_B6_VirtualServerMode_Spike.md`](Carpo_B6_VirtualServerMode_Spike.md)

---

## 1. Uvod

Sve navedeno je **opt-in**. Ako atributa nema, granula se ponaša kao prije — default DataTable / sync path **nije** lomljen.

| Pravilo | Značenje |
|---------|----------|
| Opt-in | Moraš eksplicitno staviti atribut (`findpanel="true"`, …) |
| Bez atributa | Staro ponašanje |
| Demo ≠ produkcija | Demo granule su za učenje; na pravoj granulі uključi samo što treba |
| B6 posebno | `virtualservermode` zahtijeva paging SP + buy-in odjela (vidi §7) |

### Demo u TestShellu (`Demo\DxAudit26\`)

| XML | Forma | Šta pokazuje |
|-----|-------|--------------|
| `DxAudit26_Demo.xml` | `DxAudit26_Demo` | Find, async, EditForm, LookUp, TokenEdit, lazy tree, selectedids, fiscal dry-run |
| `DxAudit26_UxPolish.xml` | `DxAudit26_UxPolish` | Find allow/always, emptymessage, asynccaption |
| `DxAudit26_BigGrid_Vsm.xml` | `DxAudit26_BigGrid_Vsm` | VirtualServerMode 50k (paging SP) |
| `DxAudit26_BigGrid_Full.xml` | `DxAudit26_BigGrid_Full` | Stari full DataTable (usporedba brzine) |

**Kako pokrenuti:** TestShell → XML = putanja do filea → Form = ime forme iz tablice → OK. Za VSM jednom pokreni `Sql\B6_VirtualServerMode_Test.sql` na bazi.

---

## 2. Brzi katalog

| Atribut / element | Kontrola | Kratko | Demo |
|-------------------|----------|--------|------|
| `findpanel` | grid, tree | Find traka (`true`/`always`/`allow`) | UxPolish, Demo |
| `async` | grid, tree | Populate u backgroundu | Demo, UxPolish |
| `asynccaption` | grid, tree | Tekst na splash uz async | UxPolish |
| `emptymessage` | grid | Poruka kad 0 redova | UxPolish |
| `filtereditor` | grid | DX Filter Editor | Demo |
| `editform` | grid | F2 EditForm | Demo |
| `columnchooser` | grid | Sakrij/prikaži kolone | Demo gumb 6 |
| `virtualservermode` | grid | Paging SP, veliki setovi | BigGrid_Vsm |
| `filtermode` | tree | DX filter način | Demo `trFilter` |
| `lazy` + `$TreeParentId` | tree | Children na expand | Demo 1c |
| `nulltext` | comboedit, tokenedit | Watermark praznog polja | `frmStavka` |
| `immediatepopup` | comboedit | Popup odmah / ne | `frmStavka` |
| `dxsearchmode` | comboedit | autosearch / autocomplete / autosuggest | `frmStavka` |
| `<tokenedit>` | novi element | Tagovi, CSV value | `frmStavka` |
| `#grid.selectedids` | akcija / data | Batch ID (1 SQL) | Demo gumb 7 |
| `demo="true"` | outlook / ZKI / JIR | Dry-run bez cert/Outlook | Demo 8 / 8b |

---

## 3. Grid

### 3.1 Find Panel — `findpanel`

**Šta korisnik vidi:** traka za pretragu preko kolona. Kad je Find uključen, Carpo pali i **highlight** pogodaka i **Clear** dugme.  
**Default:** isključeno (gužva na formi se ne mijenja).

| Vrijednost | Ponašanje |
|------------|-----------|
| `true` ili `always` | Traka **pinned** (uvijek vidljiva) |
| `allow` | Dozvoljen Find, ali **nije** pinned — tipično **Ctrl+F** |
| `false` | Isključeno |

**Kada:** manager liste, tražilica po nazivu/šifri.  
**Kada ne:** vrlo gusta forma gdje pinned traka smeta → koristi `allow`.

```xml
<!-- Pinned -->
<grid name="grdMaster" findpanel="true" key="id" dataname="~dtMaster" ... />

<!-- Ctrl+F, manje mjesta na UI -->
<grid name="grdDetail" findpanel="allow" key="id" dataname="~dtDetail" ... />
```

Runtime (npr. gumb “Prikaži Find”):

```xml
<action type="control attribute" value="#grdMaster">
	<parameter name="findpanel" value="true"/>
	<!-- value: true | always | allow | false -->
</action>
```

Napomena: isto radi na `<tree>`.

### 3.2 Async populate — `async` + `asynccaption`

**Šta korisnik vidi:** forma ne “mrzne” dok SQL radi; loading panel na gridu, ili **splash** s tvojim tekstom ako je `asynccaption` postavljen.  
**Default:** sync populate (kao prije).

**Kada:** sporiji SELECT / WAITFOR-osjećaj, refresh koji traje > ~300 ms.  
**Kada ne:** vrlo brzi lookup gridovi — sync je jednostavniji.

```xml
<grid name="grdMaster"
	async="true"
	asynccaption="Učitavam master…"
	dataname="~dtMaster"
	...>
	<data type="text" command="SELECT ..."/>
</grid>
```

| Atribut | Obavezno uz async? | Efekt |
|---------|-------------------|--------|
| `async="true"` | — | Background SQL + loading |
| `asynccaption="..."` | Ne | Umjesto generičkog panela → splash s opisom |

Populate i dalje: `<action type="control populate" value="#grdMaster"/>` (async atribut na kontroli odlučuje način).

### 3.3 Prazan grid — `emptymessage`

**Šta korisnik vidi:** siva poruka na sredini panela kad je **0 redova** (filter, prazan rezultat, “isprazni”).  
**Default:** prazan grid bez objašnjenja.

```xml
<grid name="grdMaster"
	emptymessage="Nema stavki za odabrani filter."
	...>
```

Korisno uz tree-filter master/detail: korisnik odmah vidi zašto je lista prazna.

### 3.4 Filter Editor — `filtereditor`

**Šta korisnik vidi:** napredni DX Filter Editor (iz filter UI / menija), ne samo auto-filter red.  
**Default:** ne forsiramo (DX default ostaje).

```xml
<grid name="grdMaster" filtereditor="true" showautofilterrow="true" ... />
```

Često ide uz `showautofilterrow="true"` (već postojeći atribut).

### 3.5 EditForm — `editform`

**Šta korisnik vidi:** **F2** na redu otvara DX EditForm (popup / inplace po modu).  
**Važno:** **double-click** i dalje pokreće tvoj Carpo `<event type="doubleclick">` — namjerno nije preuzet.

**Default:** off. Za polja koja se edituju stavi `editable="true"` na `<field>`.

```xml
<grid name="grdMaster" editform="true" ...>
	<fields>
		<field name="naziv" caption="Naziv" editable="true"/>
		<field name="iznos" caption="Iznos" editable="true"/>
		<field name="status" caption="Status"/> <!-- read-only u EditForm ako nema editable -->
	</fields>
</grid>
```

| Vrijednost `editform` | Mode |
|----------------------|------|
| `true` / `editform` | Klasični EditForm dialog |
| `editforminplace` | Inplace EditForm |
| `inplace` | Cell inplace (DX) |
| `false` | Isključeno |

### 3.6 Column Chooser — `columnchooser`

**Šta korisnik vidi:** prozor za prikaz/sakrivanje kolona (DX, s pretragom).  
**Default:** off. Layout save i dalje ovisi o `savelayout` / postojećem layout mehanizmu.

```xml
<grid name="grdMaster" columnchooser="true" savelayout="true" ... />
```

Runtime (odmah otvori chooser):

```xml
<action type="control attribute" value="#grdMaster">
	<parameter name="columnchooser" value="true"/>
</action>
```

### 3.7 VirtualServerMode — `virtualservermode` (B6)

**Šta korisnik vidi:** grid se brzo otvori i na **desetinama tisuća** redova — vuče **stranice** (~50) preko SP-a dok skrolaš.  
**Default:** off. **Ne uključuj na produkciji** dok odjel ne napravi SP i ne prihvati ograničenja (§7).

**Ne smije:** `type="text"` s `SELECT * FROM ogromna_tablica` kao data uz VSM.  
**Mora:** `command="sp_ImePagingProcedure"` (stored procedure).

```xml
<grid name="grdBig"
	type="master"
	key="Id"
	dataname="~dtBig"
	virtualservermode="true"
	findpanel="true"
	showautofilterrow="true"
	...>
	<fields>
		<field name="Id" caption="ID"/>
		<field name="Naziv" caption="Naziv"/>
		<!-- imena/tipovi = SP resultset -->
	</fields>
</grid>

<data name="dtBig" command="sp_DxAudit26_BigGrid_Page"/>
```

**SP ugovor (obavezno):**

| Parametar | Smjer | Uloga |
|-----------|-------|--------|
| `@Skip` | in | OFFSET |
| `@Take` | in | page size |
| `@Sort` | in | npr. `Id ASC` |
| `@Filter` | in | Find / jednostavan filter (literal) |
| `@TotalCount` | **OUT** | ukupan broj redova |

**Kratki SP + kako DX kešira (Configuration / MoreRows / šta je u RAM-u):**  
→ [`Carpo_B6_VirtualServerMode_Spike.md`](Carpo_B6_VirtualServerMode_Spike.md) — sekcija **„Kratko: SP + kako DX kešira“**.

Puni SQL primjer: `Demo\DxAudit26\Sql\B6_VirtualServerMode_Test.sql`.

**Radi:** scroll, sort (whitelist u SP), Find, Refresh.  
**Ne radi pouzdano:** `foreachrow` preko *cijelog* seta.  
**Print/export svega:** on-demand — korisnik potvrdi → privremeno učitavanje (hydrate) → print → dispose (vidi spike).

Usporedba: VSM vs FULL granule — lokalno ~**2× brži** start forme na 50k.

---

## 4. Tree

### 4.1 Find / async / filtermode

Isti Find i async ugovor kao na gridu.

```xml
<tree name="trFilter"
	key="id"
	parentfield="parentid"
	dataname="~dtTree"
	async="true"
	asynccaption="Učitavam filter tree…"
	findpanel="true"
	filtermode="smart"
	showcolumns="true"
	expandallonload="true">
	<fields>
		<field name="caption" caption="Filter"/>
	</fields>
	<event type="focusedrowchanged">
		<action type="control populate" value="#grdMaster"/>
	</event>
</tree>
```

| `filtermode` | DX značenje (sažeto) |
|--------------|----------------------|
| `smart` | Pametniji filter (često uz Find) |
| `default` | DX Default |
| `extended` | Extended (default u Carpo ako nije smart/default) |

### 4.2 Lazy expand — `lazy` + `$TreeParentId`

**Šta korisnik vidi:** na loadu samo rootovi; klik na **+** pokreće novi SQL za djecu.  
**Zašto:** velika stabla — ne vuči cijelo stablo odjednom.

Carpo na expand postavlja FormsVar **`$TreeParentId`**. SQL mora reagirati na `@ParentId`:

- prvi load: `@ParentId` NULL → samo rootovi (`parentid IS NULL`);
- expand: `@ParentId` = key roditelja → djeca.

Kolona **`haschildren`** (0/1) — pokazuje expand glyph *prije* nego što su djeca učitana.

```xml
<tree name="trLazy"
	key="id"
	parentfield="parentid"
	dataname="~dtTreeLazy"
	lazy="true"
	expandallonload="false"
	showcolumns="true">
	<fields>
		<field name="caption" caption="Lazy grana"/>
	</fields>
</tree>

<data name="dtTreeLazy" type="text"
	command="SELECT id, parentid, caption, haschildren
	         FROM ... 
	         WHERE (@ParentId IS NULL AND parentid IS NULL)
	            OR (parentid = @ParentId)">
	<parameter name="@ParentId" value="$TreeParentId" type="int"/>
</data>
```

| Atribut | Uloga |
|---------|--------|
| `async="true"` | Cijeli populate u backgroundu (svi redovi koje SQL vrati) |
| `lazy="true"` | Djeca na expand (paging po čvoru) |

Za filter-tree s malo čvorova obično dovoljan `async` + `expandallonload`. Za organizacijska / artikl stabla → `lazy`.

---

## 5. Editori

### 5.1 LookUp / `<comboedit>` — search + nulltext

`nulltext` = sivi watermark kad nema vrijednosti (NullValuePrompt).

| `dxsearchmode` | Ponašanje | Data |
|----------------|-----------|------|
| `autosearch` | Filter tipkanjem po **već napunjenom** listu | `autopopulate="true"`, lokalni set |
| `autocomplete` | DX AutoComplete; uz `immediatepopup="false"` popup ne skače odmah | lokalni set |
| `autosuggest` | Svaki unos → **re-query** SQL; Carpo seta `$DxLookupSearch` | SQL s `@q=$DxLookupSearch`, često `autopopulate="false"` |

```xml
<!-- Lokalni AutoSearch -->
<comboedit name="cmbPartner"
	valuefield="id"
	displayfield="naziv"
	dataname="~dtPartner"
	dxsearchmode="autosearch"
	filtermode="contains"
	autopopulate="true"
	nulltext="Tipkaj npr. om / al…"/>

<!-- AutoComplete, mirniji popup -->
<comboedit name="cmbPartner2"
	valuefield="id"
	displayfield="naziv"
	dataname="~dtPartner"
	dxsearchmode="autocomplete"
	immediatepopup="false"
	filtermode="contains"
	autopopulate="true"
	nulltext="Autocomplete…"/>

<!-- Server AutoSuggest -->
<comboedit name="cmbPartner3"
	valuefield="id"
	displayfield="naziv"
	dataname="~dtPartnerSuggest"
	dxsearchmode="autosuggest"
	filtermode="contains"
	autopopulate="false"
	nulltext="AutoSuggest…"/>

<data name="dtPartnerSuggest" type="text"
	command="SELECT id, naziv FROM dbo.Partneri
	         WHERE (@q IS NULL OR @q = N'' OR Naziv LIKE N'%' + @q + N'%')">
	<parameter name="@q" value="$DxLookupSearch"/>
</data>
```

`immediatepopup`: historijski Carpo default je popup odmah; `false` = tipkaj pa popup (manje agresivno).

### 5.2 TokenEdit — `<tokenedit>`

Novi XML element (nije zamjena za `combomultiedit`). Korisnik bira / tipka **tagove**.

| Atribut | Default / uloga |
|---------|-----------------|
| `separator` | `,` — spoji tokenе u string |
| `allowcustom` | `true` = smije tipkati nove vrijednosti |
| `valuefield` / `displayfield` | kao combo |
| `nulltext` | watermark |

**Value ugovor:** `EditValue` = CSV, npr. `hitno,partner,demo` — ide u `{#tokTagovi}` i SP parametre.

```xml
<tokenedit name="tokTagovi"
	valuefield="id"
	displayfield="naziv"
	dataname="~dtTagovi"
	separator=","
	allowcustom="true"
	autopopulate="true"
	nulltext="Odaberi / tipkaj tag..."/>
```

```xml
<action type="message" value="Tagovi: {#tokTagovi}"/>
```

Još nije: TokenEdit kao repository editor u grid ćeliji (vidi B9 review ako zatreba).

---

## 6. Akcije / patterni

### 6.1 Batch ID — `#grid.selectedids` (preferirano)

**Problem:** `foreachrow` + `datarun` po redu = N SQL (sporo, opterećuje SQL).  
**Rješenje:** checkbox multi-select → CSV ID-eva u **jednom** pozivu.

```xml
<grid name="grdMaster" key="id" showcheckboxes="true" ... />

<!-- Gumb -->
<action type="datarun reader" value="~dtBatchIds" if="#grdMaster.selectedids!=null"/>
<action type="message" value="IDs={#grdMaster.selectedids}" if="#grdMaster.selectedids!=null"/>

<data name="dtBatchIds" type="text"
	command="SELECT COUNT(*) AS cnt FROM STRING_SPLIT(@Ids, ',')">
	<parameter name="@Ids" value="#grdMaster.selectedids"/>
</data>
```

Na VSM gridu `selectedids` pokriva samo **označene / učitane** redove — ne “svih 50k u memoriji”.

`foreachrow` i dalje postoji; uz `datarun` Carpo može upisati **warn** u log (anti N+1). Semantika nije uklonjena.

### 6.2 Outlook / ZKI / JIR — `demo="true"` (dry-run)

Za TestShell / smoke **bez** pravog Outlooka, certifikata, SOAP-a:

```xml
<action type="outlook">
	<parameter name="demo" value="true"/>
	<parameter name="procedure" value="unused-in-demo"/>
</action>

<action type="ZKI">
	<parameter name="demo" value="true"/>
	<parameter name="loadZKI" value="~dtZkiDemo"/>
</action>

<action type="JIR">
	<parameter name="demo" value="true"/>
	<parameter name="loadJIR" value="~dtJirDemo"/>
	<parameter name="saveZKIJIRUUD" value="~dtJirDemo"/>
</action>
```

**Produkcija:** nemoj stavljati `demo` — ostali parametri ostaju kako su i prije (extract u kodu nije mijenjao XML semantiku).

---

## 7. B6 checklist za odjel (produkcija)

Prije `virtualservermode="true"` na pravoj granulі:

1. Napravi paging SP (`@Skip` / `@Take` / `@Sort` / `@Filter` / `@TotalCount OUT`).
2. XML: `virtualservermode="true"` + `<data command="sp_..."/>` — **ne** full `type="text"` SELECT.
3. Resultset = grid kolone (stabilna shema).
4. Prihvati: nema `foreachrow` preko **cijelog** seta.
5. `#grid.selectedids` = samo označeno / učitano.
6. Print/export “sve” = confirm + hydrate (spike memorije na print), ne “sve u RAM na open”.
7. UI sum/agregati nad *cijelim* setom mogu lagati cache — po potrebi server aggregate.
8. Smoke: open, scroll, sort, Find, Refresh, print.
9. Detalji i matrica ograničenja: [`Carpo_B6_VirtualServerMode_Spike.md`](Carpo_B6_VirtualServerMode_Spike.md).

**Buy-in** = odjel kaže “hoćemo na ovoj granulі”, SP+XML spremni, lista iz spikea prihvaćena.

---

## 8. Gdje testirati

| Feature | Forma | Kako |
|---------|-------|------|
| Find always / allow / off | `DxAudit26_UxPolish` | Gumbi **2 / 2a / 2b** |
| emptymessage | `DxAudit26_UxPolish` | Gumb **3** Isprazni / **3b** Napuni |
| asynccaption | `DxAudit26_UxPolish` | Gumb **1** Refresh |
| Find + async + FilterEditor + EditForm + ColumnChooser | `DxAudit26_Demo` | Gumbi **1–6**, F2 za EditForm |
| Lazy tree | `DxAudit26_Demo` | Gumb **1c** → expand Root |
| LookUp modes + TokenEdit | `DxAudit26_Demo` | Otvori stavku / subformu |
| selectedids vs foreachrow | `DxAudit26_Demo` | Gumbi **7** / **7b** (checkbox) |
| Outlook / ZKI+JIR dry-run | `DxAudit26_Demo` | Gumbi **8** / **8b** |
| VirtualServerMode 50k | `DxAudit26_BigGrid_Vsm` | Scroll / sort / Find; SQL seed jednom |
| Full 50k usporedba | `DxAudit26_BigGrid_Full` | Mjeri vrijeme otvaranja vs VSM |

---

## 9. Česte greške

| Greška | Ispravak |
|--------|----------|
| `virtualservermode` + `type="text"` full SELECT | Data mora biti paging **SP** |
| Očekuješ `foreachrow` na svih 50k u VSM | Koristi selectedids / server batch / prihvati ograničenje |
| AutoSuggest bez `$DxLookupSearch` u SQL | Dodaj `@q=$DxLookupSearch` (ili ekvivalent) |
| EditForm “ne radi” a double-click otvara formu | EditForm je **F2**; double-click je tvoj event |
| Lazy tree bez `haschildren` | Expand glyph može izostati dok se ne učita |
| `findpanel="allow"` pa “nema trake” | Namjerno — **Ctrl+F** |
| `asynccaption` bez `async="true"` | Caption se koristi samo uz async path |
