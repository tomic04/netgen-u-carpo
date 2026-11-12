# NetGen Bot

Folder se kopira korisniku cijeli. Korisnik u Cursoru otvori **ovaj** folder (File → Open Folder), ne roditeljski CarpoBot.

Unutra je sve što agent treba da napiše ili doradi NetGen XML:

| Folder | Šta je |
|---|---|
| `docs/` | Pravila, komande, kontrole, tok granule, cheatsheet |
| `.cursor/rules/netgen-bot.mdc` | Pravilo koje Cursor učita čim se projekat otvori |
| `AGENTS.md` | Ista uputa, kratko |
| `netgen/` | Izvorni kod NetGenDLL, commit `b0d5d41` (9. 9. 2026.) |
| `granule/Biovega`, `granule/Agronom`, `granule/Nevkos` | Postojeće XML granule, kao primjeri |
| `granule/Nove` | Ovdje agent snima nove granule |

## Kako se radi

1. Otvori ovaj folder u Cursoru.
2. U chatu reci šta granula treba da radi, ili priloži postojeći XML iz `granule/`.
3. Agent čita `docs/`, a VB u `netgen/` otvara samo ako pojma nema u dokumentima.
4. Novi fajl je `granule/Nove/ImeGranule.xml`.
5. Taj XML se stavlja u Jupiter `SharedDLLs\xml\` pod istim imenom.

## Šta nije u paketu

- Git historija klona i folder `DevExpress131` (oko 80 MB binarnih paketa). Za pisanje XML-a ne trebaju.
- Privatni certifikat `kkk.pfx`.
- Prazan folder `biovega dodatne granule` iz izvora nije kopiran.

Ako ikad treba složiti DLL, `DevExpress131` se uzima iz punog klona NetGen2026, ne iz ovog paketa.
