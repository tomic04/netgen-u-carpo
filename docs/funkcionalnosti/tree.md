# Stablo (TREE)

Lijevo stablo podloge je `TreeView2`. Punjenje je `NapuniDrvoVrijeme` (`Form1.vb` 4296). `TreeView1` je izbornik XML-ova dok je ulaz folder; kad je ulaz `.xml` (dll), `NapuniDrvo` ga sakrije (4127–4134). Ovaj element je `TreeView2`.

Korpus: 633 XML-a, atribut `TREE` na `<PROJECT>` u **243** datoteke. Segmenti odvojeni s `#`: **453** stora (lijevo od `:` sadrži `tree`), **383** statična natpisa, **52** prazna. `yearmonth` u korpusu nema nijedan, a kod ga i dalje ima (2005–2009). `TREETRANSLATE="TRUE"` u 4 datoteke. `SPLITTERDISTANCE` npr. `200` (`prnPredajaM.xml`) i `180`. `TREEDRAGDROP` u NetGen korpusu nema; kod na `DragDrop` i dalje šalje taj SQL (`8175`).

## Punjenje iz store (nije Carpo drvo)

`DazovaIzStore` (`Form1.vb` 4446–4498) puni `SqlDataAdapter.Fill`. Ne ide kroz `ExecSql`, pa preskok kad `@@` preživi (`Class1.vb` 113–118) ovdje ne vrijedi. `@@operateriid` se i dalje zamijeni u `SrediSQL` prije `Fill`.

Stupci se čitaju **samo po poziciji**, nikad po imenu:

| Pozicija | Uloga |
|---|---|
| 0 | id čvora. Mora biti jedinstven, inače `Relations.Add` padne i stablo ostane prazno (`TREE PROBLEM`) |
| 1 | id roditelja |
| 2 | natpis |
| 3 | tag; ako nema `:`, doda se `:imeForme` |

U drvo uđe samo podstablo **prvog reda**. Dijete čiji je id tekst `"0"` se ne crta, ni njegovi potomci. Redoslijed djece je redoslijed redaka u tablici.

Carpo `JTree` (`ComponentTree.SetTree`, `key` / `parentfield` / `valuefield`) traži stupce **po imenu**. Ista procedura, na primjer `spBlagajneZakTreeNG` ili `spTreeVrstaDok`, zato ne napuni Carpo drvo: imena polja nisu ugovor. U Carpo.NetGen je Carpo drvo samo crtež već složenih čvorova.

Integracijski test na `spin` (`CARPO_SPIN=1`) izvrši `spTreeVrstaDok`, `spBlagajneZakTreeNG` i `spNg_combo @action='Drvo_NabPri'`, pa čvor po čvor (id, roditelj, natpis, tag, redoslijed) usporedi motor s ovim algoritmom. Ispiše i imena stupaca, da se vidi da nisu Carpo `key`/`parentfield`.

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

`NgDnevnik`: „Idem puniti drvo“, „Punjenje drveta sa storom“, SQL stora, `TREE PROBLEM`, `AfterSelect`. Isti Carpo logger. Prozor `Podloga` na Ctrl+F11 otvara `Carpo.Core.Log.Log`.

## Otvoreno

- `Translate` / `TREETRANSLATE`: hrvatski i prazan jezik ostaju izvorni tekst (`Module1.vb` 266). Drugi jezik treba rječnik. `JupiterPrevoditelj` nije spojen.
- `COMBO` koji mijenja SQL drveta (`ComboBox1_SelectedValueChanged`) nije ovaj prolaz. Pravilo vidljivosti kad je `TREE` prazan a `COMBO` nije jest urađeno.
- Izvorni `OrdersDetails16.ico` nije u repou. Prozor i dalje veže `ImageList` i indeks 0.
- Dijalog s čvora pokazuje formu iz taga (naslov, polja, `ONLOAD` select). Cijeli `Form2` (`ONSUBMIT`, lijepljenje, `OKStay`) je kasniji element.
