# Znanje za Carpo bot: šta je novo na grani Devex_26

**Grana:** `$/Carpo/Devex_26` (DevExpress **26.1.3**), od branch-a **#5081** (4.8.2026) do Wave 12 + sitni follow-up.  
**Publika:** XML autor / Carpo bot na pitanje **„šta je novo”**.  
**Tehnički vodič (isti sadržaj, više detalja):** [Carpo_DX_XmlAutor_Vodic.md](Carpo_DX_XmlAutor_Vodic.md)  
**Wave changelog:** [Carpo_DX_Changelog.md](Carpo_DX_Changelog.md)

Ovaj dokument **nije** deploy uputa i **nije** popis svih check-inova. Check-inovi su izvor; isporuka je grupirana po tome što autor smije staviti u XML.

---

## 1. Što reći na „šta je novo”

Na Devex_26 Carpo je ostao **isti XML jezik**. Stara granula **bez novih atributa** ponaša se kao na Devex 23.

Novo je **opt-in**: atribut (ili novi element) moraš **eksplicitno** napisati. Ako ga nema — stari default.

Tri stvari koje autor treba znati odmah:

1. **Grid / tree** — Find traka, async populate, Filter Editor, EditForm (F2), Column Chooser, prazan-grid poruka, lazy tree.
2. **Editori** — LookUp search mode (`autosearch` / `autocomplete` / `autosuggest`), watermark (`nulltext`), novi element `<tokenedit>`.
3. **Akcije** — `#grid.selectedids` umjesto N× `foreachrow`+`datarun`; `demo="true"` za Outlook/ZKI/JIR u TestShellu.

**VirtualServerMode** (`virtualservermode`) postoji kao **demo**, nije default i **nije** za svakodnevne granule (vidi §7).

Demo granule: `Demo\DxAudit26\` u TestShellu (XML = putanja, Form = ime iz §8).

---

## 2. Brzi katalog

| Atribut / element | Kontrola | Šta je novo | Demo |
|-------------------|----------|-------------|------|
| `findpanel` | grid, tree | Find traka: `true`/`always` (pinned), `allow` (Ctrl+F) | UxPolish 2/2a/2b; Demo |
| `async` | grid, tree | Populate u backgroundu, forma ne mrzne | Demo gumb 1 / 1b |
| `asynccaption` | grid, tree | Tekst na splash uz async | UxPolish gumb 1 |
| `emptymessage` | grid | Poruka kad je 0 redova | UxPolish gumb 3 |
| `filtereditor` | grid | DX Filter Editor u filter UI | Demo master |
| `editform` | grid | F2 otvara EditForm; double-click ostaje tvoj event | Demo gumb 3 |
| `columnchooser` | grid | Sakrij/prikaži kolone | Demo gumb 6 |
| `filtermode` | tree | npr. `smart` uz Find | Demo `trFilter` |
| `lazy` + `$TreeParentId` | tree | Djeca na expand, ne cijelo stablo | Demo gumb 1c |
| `nulltext` | comboedit, tokenedit | Sivi watermark praznog polja | `frmStavka` |
| `immediatepopup` | comboedit | `false` = popup ne skače odmah | `frmStavka` |
| `dxsearchmode` | comboedit | `autosearch` / `autocomplete` / `autosuggest` | `frmStavka` |
| `$DxLookupSearch` | data uz autosuggest | Tekst koji korisnik tipka ide u SQL | `dtPartnerSuggest` |
| `<tokenedit>` | **novi element** | Tagovi; EditValue = CSV | `frmStavka` |
| `#grid.selectedids` | akcija / data | CSV označenih ID-eva, **jedan** SQL | Demo gumb 7 |
| `demo="true"` | outlook / ZKI / JIR | Dry-run bez Outlooka/cert/SOAP | Demo 8 / 8b |
| `virtualservermode` | grid | Paging SP, 50k+ — **parkirano** | BigGrid_Vsm |

---

## 3. Grid

### 3.1 Find Panel — `findpanel`

**Šta korisnik vidi:** traka za pretragu preko kolona. Kad je Find uključen, Carpo pali i highlight pogodaka i Clear.  
**Default:** isključeno.

| Vrijednost | Ponašanje |
|------------|-----------|
| `true` ili `always` | Traka **pinned** (uvijek vidljiva) |
| `allow` | Find dozvoljen, **nije** pinned — tipično **Ctrl+F** |
| `false` | Isključeno |

**Kada da:** manager liste, tražilica po nazivu/šifri.  
**Kada ne:** gusta forma — stavi `allow`, ne pinned.

```xml
<grid name="grdMaster" findpanel="true" key="id" dataname="~dtMaster" ... />
<grid name="grdDetail" findpanel="allow" key="id" dataname="~dtDetail" ... />
```

Runtime (gumb „Prikaži Find”):

```xml
<action type="control attribute" value="#grdMaster">
	<parameter name="findpanel" value="true"/>
	<!-- true | always | allow | false -->
</action>
```

Isto radi na `<tree>`. Ako autor kaže „nema trake” uz `allow` — to je namjerno, Ctrl+F.

### 3.2 Async populate — `async` + `asynccaption`

**Šta korisnik vidi:** forma ne mrzne dok SQL radi; loading panel, ili splash s tvojim tekstom ako ima `asynccaption`.  
**Default:** sync (kao prije).

**Kada da:** SELECT / refresh koji traje osjetno (> ~300 ms).  
**Kada ne:** brzi lookup gridovi.

```xml
<grid name="grdMaster"
	async="true"
	asynccaption="Učitavam master…"
	dataname="~dtMaster" ...>
	<data type="text" command="SELECT ..."/>
</grid>
```

`asynccaption` bez `async="true"` nema efekta. Populate ostaje `<action type="control populate" value="#grdMaster"/>` — atribut na kontroli bira sync vs async.

### 3.3 Prazan grid — `emptymessage`

**Šta korisnik vidi:** siva poruka na sredini kad je 0 redova (filter, prazan rezultat, „isprazni”).  
**Default:** prazan grid bez objašnjenja.

```xml
<grid name="grdMaster" emptymessage="Nema stavki za odabrani filter." ... />
```

### 3.4 Filter Editor — `filtereditor`

**Šta korisnik vidi:** napredni DX Filter Editor, ne samo auto-filter red.  
**Default:** ne forsiramo.

```xml
<grid name="grdMaster" filtereditor="true" showautofilterrow="true" ... />
```

### 3.5 EditForm — `editform`

**Šta korisnik vidi:** **F2** na redu otvara DX EditForm.  
**Važno za bota:** **double-click nije preuzet** — i dalje ide autorov `<event type="doubleclick">`.

**Default:** off. Polja za edit: `editable="true"` na `<field>`.

```xml
<grid name="grdMaster" editform="true" ...>
	<fields>
		<field name="naziv" caption="Naziv" editable="true"/>
		<field name="iznos" caption="Iznos" editable="true"/>
		<field name="status" caption="Status"/>
	</fields>
</grid>
```

| `editform` | Mode |
|------------|------|
| `true` / `editform` | Dialog EditForm |
| `editforminplace` | Inplace EditForm |
| `inplace` | Cell inplace |
| `false` | Off |

Ako autor kaže „EditForm ne radi, double-click otvara formu” — reci: EditForm je F2.

### 3.6 Column Chooser — `columnchooser`

**Šta korisnik vidi:** prozor sakrij/prikaži kolone (s pretragom).  
**Default:** off. Layout i dalje preko `savelayout` / postojećeg mehanizma.

```xml
<grid name="grdMaster" columnchooser="true" savelayout="true" ... />
```

Runtime (odmah otvori):

```xml
<action type="control attribute" value="#grdMaster">
	<parameter name="columnchooser" value="true"/>
</action>
```

---

## 4. Tree

### 4.1 Find / async / filtermode

Isti ugovor kao na gridu.

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

| `filtermode` | Sažeto |
|--------------|--------|
| `smart` | Pametniji filter (često uz Find) |
| `default` | DX Default |
| `extended` | Extended (Carpo fallback ako nije smart/default) |

### 4.2 Lazy expand — `lazy` + `$TreeParentId`

**Šta korisnik vidi:** na loadu samo rootovi; **+** pokreće SQL za djecu.  
**Zašto:** velika stabla — ne vući sve odjednom.

Carpo na expand postavlja FormsVar **`$TreeParentId`**. SQL mora slušati `@ParentId`:

- prvi load: `@ParentId` NULL → rootovi (`parentid IS NULL`);
- expand: `@ParentId` = key roditelja → djeca.

Kolona **`haschildren`** (0/1) — expand glyph *prije* loada djece. Bez nje glyph može izostati.

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
| `async="true"` | Cijeli populate u backgroundu (sve što SQL vrati) |
| `lazy="true"` | Djeca na expand |

Malo filter-stablo: `async` + `expandallonload`. Organizacija / artikli: `lazy`.

---

## 5. Editori

### 5.1 LookUp / `<comboedit>`

`nulltext` = sivi watermark kad nema vrijednosti.

| `dxsearchmode` | Ponašanje | Data |
|----------------|-----------|------|
| `autosearch` | Filter tipkanjem po **već napunjenom** listu | `autopopulate="true"` |
| `autocomplete` | DX AutoComplete; `immediatepopup="false"` = popup ne skače odmah | lokalni set |
| `autosuggest` | Svaki unos → **re-query** SQL; Carpo seta `$DxLookupSearch` | SQL s `@q=$DxLookupSearch`, često `autopopulate="false"` |

Aliasi koje runtime prihvaća: `autofilter`, `autocomplete`, `autosuggest`, `none` (uz `autosearch`).

```xml
<comboedit name="cmbPartner"
	valuefield="id" displayfield="naziv" dataname="~dtPartner"
	dxsearchmode="autosearch" filtermode="contains"
	autopopulate="true"
	nulltext="Tipkaj npr. om / al / beta..."/>

<comboedit name="cmbPartner2"
	valuefield="id" displayfield="naziv" dataname="~dtPartner"
	dxsearchmode="autocomplete" immediatepopup="false"
	filtermode="contains" autopopulate="true"
	nulltext="Autocomplete (popup nije odmah)"/>

<comboedit name="cmbPartner3"
	valuefield="id" displayfield="naziv" dataname="~dtPartnerSuggest"
	dxsearchmode="autosuggest" filtermode="contains"
	autopopulate="false"
	nulltext="AutoSuggest: tipkaj om / ju..."/>

<data name="dtPartnerSuggest" type="text"
	command="SELECT id, naziv FROM dbo.Partneri
	         WHERE (@q IS NULL OR @q = N'' OR Naziv LIKE N'%' + @q + N'%')">
	<parameter name="@q" value="$DxLookupSearch"/>
</data>
```

AutoSuggest bez `$DxLookupSearch` u SQL-u **ne filtrira na serveru**.

### 5.2 TokenEdit — `<tokenedit>` (novi element)

Nije zamjena za `combomultiedit`. Korisnik bira / tipka **tagove**.

| Atribut | Uloga |
|---------|--------|
| `separator` | default `,` — spoji tokene u string |
| `allowcustom` | `true` = smije tipkati nove |
| `valuefield` / `displayfield` | kao combo |
| `nulltext` | watermark |

**Value:** `EditValue` = CSV, npr. `hitno,partner,demo` → `{#tokTagovi}` i SP parametri.

```xml
<tokenedit name="tokTagovi"
	valuefield="id" displayfield="naziv" dataname="~dtTagovi"
	separator="," allowcustom="true" autopopulate="true"
	nulltext="Odaberi / tipkaj tag..."/>

<action type="message" value="Tagovi: {#tokTagovi}"/>
```

Još **nije**: TokenEdit kao editor u grid ćeliji.

---

## 6. Akcije / patterni

### 6.1 Batch ID — `#grid.selectedids`

**Problem:** `foreachrow` + `datarun` po redu = N SQL.  
**Novo preporučeno:** checkbox multi-select → CSV ID-eva u **jednom** pozivu.

```xml
<grid name="grdMaster" key="id" showcheckboxes="true" ... />

<action type="datarun reader" value="~dtBatchIds" if="#grdMaster.selectedids!=null"/>
<action type="message" value="IDs={#grdMaster.selectedids}" if="#grdMaster.selectedids!=null"/>

<data name="dtBatchIds" type="text"
	command="SELECT COUNT(*) AS cnt FROM STRING_SPLIT(@Ids, ',')">
	<parameter name="@Ids" value="#grdMaster.selectedids"/>
</data>
```

`foreachrow` **nije uklonjen**. Uz `datarun` Carpo može upisati **warn** u log (anti N+1). Semantika je ista kao na 23.

Na VSM gridu `selectedids` = samo **označeni / već učitani** redovi, ne „svih 50k”.

### 6.2 Outlook / ZKI / JIR — `demo="true"`

Za TestShell / smoke **bez** pravog Outlooka, certifikata, SOAP-a. Kod je izvučen iz `Events` u `FiscalOutlookActions`; XML semantika produkcije **nije** mijenjana.

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

**Produkcija:** nemoj stavljati `demo`.

---

## 7. VirtualServerMode — parkirano (B6)

Postoji opt-in `virtualservermode="true"` + paging SP (`@Skip` / `@Take` / `@Sort` / `@Filter` / `@TotalCount OUT`). Demo: `DxAudit26_BigGrid_Vsm` (~50k, brži start od full DataTable).

**Bot treba reći:** ovo **nije** default i **nije** za običnu granulu. Treba paging ugovor, buy-in odjela, i prihvaćanje ograničenja (nema `foreachrow` preko cijelog seta; print/export „sve” = hydrate). Detalji: [Carpo_B6_VirtualServerMode_Spike.md](Carpo_B6_VirtualServerMode_Spike.md).

Ne predlagati `virtualservermode` dok autor nema 50k+ i SP s tim parametrima.

---

## 8. Demo mapa (TestShell)

| Feature | Forma | Kako |
|---------|-------|------|
| Find always / allow / off | `DxAudit26_UxPolish` | Gumbi **2 / 2a / 2b** |
| emptymessage | `DxAudit26_UxPolish` | **3** Isprazni / **3b** Napuni |
| asynccaption | `DxAudit26_UxPolish` | Gumb **1** |
| Find + async + FilterEditor + EditForm + ColumnChooser | `DxAudit26_Demo` | Gumbi **1–6**, F2 = EditForm |
| Lazy tree | `DxAudit26_Demo` | Gumb **1c** → expand Root |
| LookUp + TokenEdit | `DxAudit26_Demo` | Gumb **4** / subforma stavke |
| selectedids vs foreachrow | `DxAudit26_Demo` | Gumbi **7** / **7b** (checkbox) |
| Outlook / ZKI+JIR dry-run | `DxAudit26_Demo` | Gumbi **8** / **8b** |
| VSM 50k (samo ako pitaju B6) | `DxAudit26_BigGrid_Vsm` | SQL seed jednom: `Demo\DxAudit26\Sql\B6_VirtualServerMode_Test.sql` |

XML putanje: `Demo\DxAudit26\DxAudit26_Demo.xml`, `DxAudit26_UxPolish.xml`, `DxAudit26_BigGrid_Vsm.xml`.

---

## 9. Šta nije novo / ne dirati

- **Stara XML semantika** (foreachrow, double-click, print, prava, savelayout) — nije lomljena.
- **REPX / print predlošci** — nisu Carpo DX wave; zaseban track.
- **JupiterLogs** (`GranuleName` u loggeru) — runtime, nije XML atribut za autore.
- **HintPath / Fakes / build** — ops, nije za „šta je novo” u granulama.
- **B13 DX AI** — parked.

Audit A* (SQL dispose, password u logu, FormatRules.Clear, bez GC.Collect na close, ARITHABORT) — stabilnost, bez novog XML-a. Autor ih ne uključuje atributom.

---

## 10. Česte greške (za bota)

| Autor kaže / radi | Odgovor |
|-------------------|---------|
| `findpanel="allow"` pa „nema trake” | Namjerno — Ctrl+F |
| `asynccaption` bez `async` | Caption radi samo uz async |
| EditForm „ne radi”, double-click otvara formu | EditForm je **F2** |
| AutoSuggest ne filtrira | SQL mora imati `@q=$DxLookupSearch` |
| Lazy tree bez plusića | Dodaj kolonu `haschildren` |
| `virtualservermode` + `SELECT *` cijela tablica | Data mora biti paging **SP** |
| `foreachrow` na „svih 50k” u VSM | Ne radi; selectedids / server batch |
| TokenEdit u grid ćeliji | Još nije podržano |
| `demo="true"` na produkcijskoj fiskalnoj akciji | Nemoj — samo TestShell/smoke |

---

## 11. Dodatak: indeks changeseta (istraživanje)

Grana: **#5081** branch od `$/Carpo/Devex_23_4.8_framework`.

| Changeset | Datum | Za XML autora |
|-----------|-------|----------------|
| **5081** | 04.08.2026 | Branch |
| **5090** | 04.08.2026 | DX 26.1 runtime (nema novog XML-a) |
| **5109** | 07.08.2026 | Waves 1–7: Find, async, LookUp search, FilterEditor, EditForm, ColumnChooser, TokenEdit, selectedids |
| **5110** | 10.08.2026 | `demo="true"` na outlook / ZKI / JIR |
| **5111** | 10.08.2026 | HintPath — skip |
| **5112** | 10.08.2026 | Tree `lazy="true"` + `$TreeParentId` |
| **5113–5125** | 10.08.2026 | B6 VSM demo — parkirano |
| **5126** | 10.08.2026 | `findpanel` allow/always, `emptymessage`, `asynccaption` |
| **5127** | 10.08.2026 | Demo `DxAudit26_UxPolish` |
| **5128–5131** | 10.08.2026 | Docs (XmlAutor vodič, wave CLOSED) |
| **5135, 5145** | 13–17.08.2026 | JupiterLogs GranuleName — nije XML |

Povezani fajlovi u runtimeu (ako bot treba „gdje je kod”): `Carpo.UI` kontrole (grid/tree/combo/tokenedit), `Events` / `FiscalOutlookActions`, demo XML u `Demo\DxAudit26\`.
