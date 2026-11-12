# Stablo (TREE)

Lijevo stablo podloge je `TreeView2`. Punjenje je `NapuniDrvoVrijeme` (`Form1.vb` 4296). `TreeView1` je izbornik XML-ova dok je ulaz folder; kad je ulaz `.xml` (dll), `NapuniDrvo` ga sakrije (4127–4134). Ovaj element je `TreeView2`.

Korpus: 633 XML-a, atribut `TREE` na `<PROJECT>` u **243** datoteke. Segmenti odvojeni s `#`: **453** stora (lijevo od `:` sadrži `tree`), **383** statična natpisa, **52** prazna. `yearmonth` u korpusu nema nijedan, a kod ga i dalje ima (2005–2009). `TREETRANSLATE="TRUE"` u 4 datoteke. `SPLITTERDISTANCE` npr. `200` (`prnPredajaM.xml`) i `180`. `TREEDRAGDROP` u NetGen korpusu nema; kod na `DragDrop` i dalje šalje taj SQL (`8175`).

## Atributi

| Atribut | Kad | Ponašanje |
|---|---|---|
| `TREE` prazan i `COMBO` prazan | `4310–4316` | `TreeView2` skriven, splitter **0**, gumb drveta ugašen |
| `TREE` prazan, `COMBO` nije | isti `If`, ne ulazi u sakrivanje | stablo ostaje, splitter ostaje dizajnerskih **243** |
| `TREE` nije prazan | `4323–4330` | vidljivo, gumb uključen, splitter **290** ili `SPLITTERDISTANCE` |
| `yearmonth:FORMA` | `4348`, `4527–4540` | godine 2005–2009, u svakoj 12 mjeseci. Tag godine `gggg0101#gggg1231:FORMA`. Tag mjeseca `gggg` + broj mjeseca bez nule + `01#` + isto + `30:FORMA` (siječanj 2005 je `2005101#2005130:FORMA`) |
| lijevo sadrži `tree` | `4350`, `4435` | SQL lijevo od `:`, ime forme desno. Korijen je prvi red. Veza je stupac 0 ← stupac 1. Natpis je stupac 2. Tag je stupac 3, a ako u njemu nema `:`, doda se `:imeForme`. Dijete čiji je id `0` se preskače |
| ostalo `NATPS:NAREDBA` | `4353–4355` | natpis je lijevo, tag je `:` + desno. `@@node` je prazan string, naredba je desno |
| segment bez `:` | `On Error Resume Next` | segment se preskoči |
| `TREETRANSLATE=true` | `4461–4464`, `4501–4504` | samo natpis iz store. Statični natpis ide kroz `Translate` uvijek. Rječnik prijevoda još nije spojen, tekst ostaje izvorni |
| `TREEDRAGDROP` | `8175` | SQL nakon `SrediSQL`. Upis se ne šalje |
| `NetGenDBFilter` | `4359–4368` | `select * from NetGenDBFilter where granula='ime.xml'`. Red dobije natpis `naslov` i tag `:==` + `sql`. Greška ne ruši stablo |

## Odabir čvora

`TreeView2_AfterSelect` (`7182–7188`): `Node` je tag do prvog `:`, naredba je ostatak. Prije odabira `Node` je `"null"` (`Form1.vb` 39). Zatim `NovaGenerickaForma(naredba)`. Dijalog koji ta naredba otvara nije ovaj element. `Odaberi` postavlja `Node` i pamti naredbu. Ako master SQL sadrži `@@node`, prozor ga ponovo pošalje s novim čvorom.

## Defaulti TreeView2

Dizajner `Form1.designer.vb` 572–581, plus kod kad se drvo puni (4336–4339).

| Svojstvo | Svježi TreeView | NetGen | Što Carpo.NetGen drži |
|---|---|---|---|
| `ShowRootLines` | True | kod ponovo True | True |
| `ShowPlusMinus` | True | kod ponovo True | True |
| `AllowDrop` | False | dizajner True | True |
| `BorderStyle` | Fixed3D | None | None |
| `BackColor` | Window | ControlLightLight | ControlLightLight |
| `HideSelection` | True | dizajner ne dira | True |
| `ImageList` | nema | `ImageList2` | otvoreno, ikone stabla nisu spojene |
| `Sorted` | False | ne dira | False |

## Log

`NgDnevnik`: „Idem puniti drvo“, „Punjenje drveta sa storom“, SQL stora, `TREE PROBLEM`, `AfterSelect`. Isti Carpo logger, Ctrl+F11.

## Otvoreno

- `Translate` / `TREETRANSLATE` nema rječnik. Natpis ostaje iz baze ili iz XML-a.
- `COMBO` koji mijenja SQL drveta (`ComboBox1_SelectedValueChanged`) nije ovaj prolaz. Pravilo vidljivosti kad je `TREE` prazan a `COMBO` nije jest urađeno.
- `ImageList2` i povlačenje čvora (tijelo `DragDrop` je komentar; živi je samo SQL `TREEDRAGDROP`).
- Dijalog koji `AfterSelect` otvara preko `NovaGenerickaForma` dolazi s gumbima, ne sa stablom.
