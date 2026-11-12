# Carpo XML SUPER-CHEATSHEET (rev. 2026-09)
# Brza referenca. Ako konfliktira s FINAL-v2 — FINAL-v2 pobjeđuje.
# Zadnja usklađenost: allowcopycell, form structure, string literali, lookup+tree, fixpdf.

## 0) Opšte
- Root: `<root version="1.2"> … </root>`
- `form/@type ∈ { custom, manager, master }` — glavna forma s managerlayout: `type="master"`
- **Sve `<form>` su direktna djeca `<root>`.** Glavna (`type="master"`) je **zadnja**. Nikad `<form>` unutar `<form>`.
- Unutar `<form>` redoslijed: `event` → `layout` **ili** `managerlayout` → `panelbuttons` → `datalist` (zadnje)
- Sve kontrole unutar `<layout>` — nikad van. `panelbuttons` na **nivou forme**, ne u group/item.
- XML: `&` u `if` → `&amp;&amp;`. Jedan `type` po elementu (nikad `type="assign"` + `type="datetime"`).
- **Ne uvoditi** atribute/čvorove van XSD-a i FINAL-v2.

## 1) Redoslijed atributa (preporučen)
`name, type, dataname, keyfield, caption, visible, readonly, required, format, width, height, bestfitonrefresh, multiselect, showautofilterrow, showgrouppanel, savelayout, allowcopycell, wordwrap, key, value, if, timeout`

## 2) Layout
- `<layout>` ima jedan `<group rows=".." cols="..">` root.
- Djeca `<group>`: `columnsdefinition`, `rowsdefinition`, `group`, `item`. (`<tabs>` van layout-a **nije** podržano.)
- Label | kontrola: col 1 = samo caption, col 2 = kontrola.
- `<item>` mora imati `row`, `col` i zatvarajući `</item>`.
- NetGen `TYPE="number"` → `<textedit format="n2"/>` (**nema** `numberedit`).
- NetGen `multidrop` → `<combomultiedit>` (**nema** `multidrop`).

### Primjer minimalni grid (custom forma):
```xml
<layout>
  <group rows="1" cols="1">
    <item row="1" col="1">
      <grid name="grdMain" dataname="~dtMain" keyfield="Id"
            showautofilterrow="true" showgrouppanel="false" savelayout="true">
        <fields inherited="true">
          <field name="Id" visible="false"/>
          <field name="Name"/>
        </fields>
      </grid>
    </item>
  </group>
</layout>
```

## 3) Managerlayout
- Dozvoljena djeca: `lookup`, `tree`, `grid`, `tabs`. **Nema** `splitter` / `group` / `item`.
- Grid (`type="master"` / `type="detail"`) mora biti **direktno dijete** managerlayout.
- Evente kačiti **na dijete**, ne na `managerlayout`.
- Tok: `lookup → tree → grid master → grid detail`.
- **`<lookup>` SAMO ako postoji `<tree>` + `dtTree`.** Bez tree → frmFilter, ne lookup (NullReferenceException).
- Filter iznad tree: `Tekuća godina` / `Ostale godine` / `Operateri` — ne „Filter Od-Do“.

## 4) Validni događaji i akcije

### Dozvoljeni `event/@type` (najčešći):
`init, load, shown, close, click, doubleclick, focusedrowchanged, selectionchanged, valuechanged, selectedtabchanged, checkedchanged, cellvaluechanged`

### Dozvoljene akcije (najčešće):
`control populate|populate keepfocus|value|focus|printcontrol|export|refreshvalue|enable|disable|visible|caption|attribute|datasourceempty`,
`form open|close|caption`,
`assign`, `message`, `validate`, `confirm`,
`datarun reader|nonquery|scalar|text|integrated`,
`variable value`, `method run`, `menu enable|disable`,
`ZKI`, `JIR` (fiskalizacija — FINAL-v2 §8.3 / Novosti 10.0.8.37),
`text export` (parametri `filePath`, `fileName`, `extension`, `textContent`; opc. `showOpenFileDialog`, `encoding`).

### `control attribute` (primjeri):
- `dbimg`, `dbpdf`, `format`
- `fixpdf` (DPI, npr. `300`) na **`#imePdfKontrole`** — dugme „Popravi prikaz“, nikad u `init`/`shown`. `#mojPdfViewer` je placeholder.

### `text export` — encoding datoteke:
- Default **UTF-8 bez BOM-a**. Za ANSI latinicu (platni nalozi RS/HR): `<parameter name="encoding" value="windows-1250"/>`.
- Prihvaća i `ansi`, `utf-8`, `utf-8-bom`, `ascii`, broj code page-a ili IANA ime. Nepoznato → greška, datoteka se ne kreira. FINAL-v2 §8.3.

### Ugniježđene akcije:
- Containeri: `confirm`, `transaction`. Ostale akcije (npr. nakon `datarun nonquery`) = **sibling**, ne child.

## 5) Tabs (lazy populate)

```xml
<tabs name="tabs">
  <event type="selectedtabchanged">
    <action if="#tabs.selectedtab==tabItems" type="control populate" value="#grdItems"/>
  </event>
</tabs>
```

## 6) Datalist / Data pozivi

### Tipovi:
- SP s rezultatom: bez `type` ili implicit reader
- `type="text"` — **obavezno** za `command="SELECT …"` (inače traži SP)
- `type="nonquery"` — INSERT/UPDATE/DELETE (samo jedan `type`)
- `scalar`

### Parametri:
- String **literal** u SP: `value="'tree'"` (apostrofi). `value="tree"` = referenca na kontrolu → NULL.
- Broj: `value="1"` bez apostrofa. Kontrole/varijable: `#cmb`, `@@operateriid`, `Global.$X` — bez apostrofa.
- `command="SELECT … WHERE x=@p"` + `<parameter>` — **ne** `#kontrola` unutar `command` stringa.
- Više `<data>` s istim `name` + `if` / `elseif` / `else` — engine bira prvu istinitu.

```xml
<datalist>
  <data name="dtMain" command="dbo.sp_List"/>
  <data name="dtDelete" command="dbo.sp_Del" type="nonquery">
    <parameter name="@Id" value="#grdMain.@Id"/>
  </data>
</datalist>
```

## 7) Menu

```xml
<menu>
  <menuitem name="mnuInfo" caption="Info">
    <event type="click">
      <action type="form open" value="frmInfo">
        <parameter name="Id" value="#grdMain.@Id"/>
      </action>
    </event>
  </menuitem>
</menu>
```

## 8) Patterni
- CRUD: grid → subforma (inline u istom `<root>`) → confirm → `control populate` parent grida. Iz dijaloga: `parent.#grdMaster`.
- Manager: lookup + tree + grid; dtTree/dtMaster granati po `#filter`.
- frmFilter: `type="dialog"` + `width`/`height`/`fixedsize`; Global vars; Prihvati → `parent.#grdMaster`.
- Tabs lazy load: populate samo na `selectedtabchanged`.
- Grid ćelija `Ctrl+C`: `allowcopycell="true"` samo na read-only pregledu — **ne** s editable kolonama ni s `showcheckboxes`. Zadano `Ctrl+C` = dijalog izvoza. FINAL-v2 §5.3.2.
- PDF pokidan prikaz: dugme + `control attribute` / `fixpdf` na stvarnom `#name` viewera.

## 9) TODO politika
- `<!-- TODO: confirm DB structure -->` kada je tabela/SP/kolona nepoznata.
- Ne izmišljati atribute ili elemente koji nisu u XSD-u ili FINAL-v2.
