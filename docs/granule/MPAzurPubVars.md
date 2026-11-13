# MPAzurPubVars — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPAzurPubVars.xml`. Sedam baza, Biovega. Natpis je `Ažuriranje parametara MP`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spMPAzurPubvars @action = 2"`. To je upit, master se puni odmah (`3879–3882`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Obnovi | `OBNOVI` |
| 4 | Izmjeni | `UPDATEFORM` |

`CHECKBOXVISIBLE="FALSE"`.

## Dijalozi

`OBNOVI` ima `FILLEXPLORER="TRUE"` i `ONSUBMIT="spMPAzurPubvars @action = 1"`. Gumb Obnovi šalje isti `@action = 1`, pa `#Ok`. Prvo ide naredba gumba (`7509`), zatim submit forme i master se zamijeni (`5445–5452`). Procedura se time pošalje dvaput.

`UPDATEFORM` gumb Spremi šalje `@action=3` i `#Ok`. To piše `pubvars`. Na Jupiteru se `@action = 1` i `@action=3` ne šalju. `@action = 2` na ulazu je prikaz.
