# BisnodeExport — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/BisnodeExport.xml`. Rang 113, 1 baza. Natpis je `Bisnode export`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":PARAM_FORM"`. Ulaz je dijalog, master čeka njegov `FILLEXPLORER` (`3879`, `5445–5452`, R18).

| # | Natpis | Naredba |
|---|---|---|
| 1 | FORMA | `PARAM_FORM` |
| 2 | EXPORT | `Export` |
| 3 | PUTANJA | `PATH` |

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## PARAM_FORM

`FILLEXPLORER="true"`. `ONSUBMIT="spBisnodeExportQuery @action=@@ComboIzv, @Razdoblje='@@Razdoblje', @export=0--"`. `ONLOAD` stavi jučerašnji kraj prošlog mjeseca u `Razdoblje` i `1` u combo. `REFRESHAFTERCHILD="TRUE"` (R82).

`buttonOK` ima `ONCLICK="#OK"`. Prazan segment prije `#` se preskoči (`6821`), zatim `OK` pokupi parametre, zatvori dijalog i master zamijeni submitom (`6888–6890`, `5445–5452`). `buttonCLOSE` je `CLOSE`: dijalog se zatvori bez submita (`7033–7034`).

`@@` u submitu se zamijeni. Procedura počinje s `sp`, pa se parametri kontrola još jednom lijepe (R14).

## Kasniji dijalozi

`Export` gumb `Prihvati` zove `CREATETEXTFILEUTF8` (`7342`). To piše datoteku, ne šalje `ONSUBMIT`.

`PATH` gumb `Prihvati` je `update Pubvars set VarSet = '@@putanja' where varime = 'BisnodeExport'#OK`. `update` ide u `ExecSql` prije OK-a (`7509–7510`). Na Jupiteru se taj `update` ne šalje.

## Praznine

- `@export=0` je upit za grid. Gumb `EXPORT` otvara drugi dijalog i tek on piše txt.
