# Carpo × DevExpress Inventory — Devex_26

**Status:** Done (Iter 2)  
**DX version in UI csproj:** DevExpress **26.1.3**  
**Focus surface:** Manager (tree → master grid → detail) + all `Component*` wrappers

---

## DX assemblies referenced (Carpo.UI)

| Package / assembly | Role |
|--------------------|------|
| XtraGrid | Grids / views |
| XtraTreeList | Tree + TreeListLookUp |
| XtraEditors | Editors, LookUp, ButtonEdit, … |
| XtraLayout | LayoutControl |
| XtraBars | Ribbon / bars (via RibbonBase) |
| XtraCharts | Charts |
| XtraPdfViewer | PDF |
| XtraRichEdit / Spreadsheet / Printing / Office | Document viewer stack |
| Sparkline / Utils / Data / Drawing / Pdf.Core | Support |

Upgrade 23→26 is **version**, not automatic feature adoption.

---

## Component* → DX control

| Component | DX / host type | XML surface (typical) |
|-----------|----------------|------------------------|
| ComponentGrid | `GridControl` + `GridView` (`JGridControl`) | `<grid>` |
| ComponentTree | `TreeList` (`JTreeListControl`) | `<tree>` |
| ComponentLookUpEdit | `GridLookUpEdit` | `<lookup>` |
| ComponentTreeLookUpEdit | `TreeListLookUpEdit` | `<treelookup>` |
| ComponentAutoSuggest | `LookUpEdit` | autosuggest |
| ComponentTabs | `XtraTabControl` | tabs |
| ComponentTextEdit / DateEdit / TimeEdit / CheckEdit | matching XtraEditors | editors |
| ComponentCheckedComboBoxEdit | `CheckedComboBoxEdit` | multi combo |
| ComponentButton / ButtonText / BrowseEdit / Searh | Button / DropDown / ButtonEdit | buttons/search |
| ComponentChart | `ChartControl` | chart |
| ComponentPdfViewer | `PdfViewer` | pdf |
| ComponentDocumentViewer | Panel + Pdf/RichEdit/Spreadsheet/Picture | document |
| ComponentPictureEdit / HyperLinkEdit / DaysEdit | matching editors | misc |
| ComponentLabel / SimpleLabel | LabelControl / SimpleLabelItem | labels |

---

## Feature usage (Manager hot path)

### Already used

- BestFit (grid + tree)
- FormatRules (grid; StoreFormatRules in layout)
- Clipboard options + custom Excel copy
- ExportTo Xlsx/Xls/Pdf/Html/Rtf/Csv (grid + Manager bars)
- Begin/EndUpdate (partial — often commented on populate)
- OptionsView / OptionsBehavior (filter row, group, footer, HTML headers, …)
- FixedStyle columns
- AllowSort / column panel
- SaveLayout / RestoreLayout (grid + Manager filter-tree layouts)
- Auto-filter row

### Partially used

- FilterEditor — **disabled** on Tree; unused on Grid API surface
- Async populate — XML `async` exists; real `StartNew` path commented
- FormatRules — added repeatedly without Clear

### Never touched (relevant to gap)

- FindPanel / OptionsFind
- VirtualServerMode / async server mode APIs
- EditForm
- RowDetail / master-detail row templates
- SearchMode / PopupFilterMode (LookUp/TreeLookUp)
- HTML cell templates (beyond header HTML)
- Freeze panes beyond FixedStyle already used

---

## Manager stack (hot path)

1. Filter **TreeList** (layout persisted)
2. Master **GridControl** (populate, focus, selection → detail)
3. Detail grid(s) + layout controls + editors / lookups
4. Ribbon / bars for export, print, actions (RibbonBase)

---

## Already used / Partial / Never — summary matrix

| API surface | Status |
|-------------|--------|
| Grid BestFit / Export / Clipboard / Layout | Already |
| Grid FormatRules | Partial (accumulate bug) |
| Grid Find Panel | Never |
| Grid VirtualServerMode | Never |
| Grid EditForm / RowDetail | Never |
| Tree BestFit / AutoFilter / HTML headers | Already |
| Tree Find / async load | Never / Partial |
| LookUp SearchMode / async server | Never / Partial (Info form has QuerySuggestions pattern) |
| LayoutControl DX serialization | Partial (Carpo VarsLayout) |
| Chart / Pdf / Document | Already (secondary) |
