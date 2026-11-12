# B9 — TokenEdit review

**Datum:** 2026-08-07  
**Verdict:** **GO** — uzak opt-in XML element (wave 6)

## Fit

| Pitanje | Odgovor |
|---------|---------|
| DX tip postoji (26.1)? | Da — `DevExpress.XtraEditors.TokenEdit` |
| Carpo rupa? | Da — najbliže je `combomultiedit` (checkbox list), ne token UX |
| Breaking? | Ne — novi element; postojeće granule netaknute |
| Value ugovor | `EditValue` = string sa separatorom (default `,`) — kompatibilno s `#ctrl` / SP param |

## Scope wave 6

- XML `<tokenedit>` u layout itemu  
- `dataname` + `valuefield`/`displayfield` **ili** statički `<comboitem>`  
- `separator` (opt), `allowcustom` (opt), `nulltext`  
- Demo na `frmStavka`

## Van scope (kasnije)

- Grid repository TokenEdit  
- Panelbuttons host  
- Binding na TVP/tablepar automatski  
- Full designer toolbox polish  

## Parking

**B6 VirtualServerMode** ostaje poseban projekt (ne u ovom waveu).
