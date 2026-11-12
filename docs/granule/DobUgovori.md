# DobUgovori — SHOWCHECKBOX, TRANSLATE, search `sp`, radio refresh, group

XML: `definitions/netgen/Agronom granule/DobUgovori.xml`. Izrez za atribute kojih Knt/Inventura/FakPar nemaju.

## Datum s kvačicom

`INSERT_FORM.datugovorvrijedido` i `DatumMogucegOtkaza` imaju `SHOWCHECKBOX="TRUE"`, nemaju `CHECKED` i nemaju `DEFAULT`.

Redoslijed (`2275`–`2295`): kvačica krene kao true, `ShowCheckBox=true` je ugasi (`2277`–`2278`), zatim `CHECKED` ako postoji. Ovdje ne postoji, OERN (`706`) ostavi ugašeno. Ugašen datum šalje `null` (`3560`–`3567`). Upaljen, bez `CUSTOMFORMAT`, šalje `'yyyyMMdd'` (`3530`–`3532`).

Nema `firstday*` ni `today` na ovim kontrolama.

## Combo i tražilica

`DobUgovoriVrsteId` `ALLOWEMPTY="TRUE"` i `TRANSLATE="true"`. Prazan red (`2403`–`2411`), zatim stupac 1 ide kroz `Translate` (`2420`–`2424`).

`UNESI_STAVKU_FORM.robaid` SQL počinje s `spTraziRoba`. `Form3.vb` 99–100 ostavi taj tekst. `@@searchboxrobaid` je upisani tekst, ne id. Id ide u `Tag` (`3626`–`3628`).

`klmasterrobaid` na istoj formi `ALLOWEMPTY="TRUE"`. `rad2` `ONCLICK` završava s `select 'refresh' as klmasterrobaid` (`268`). To ponovo puni combo (`5158`), uključujući null red (`5165`).

## Radio, check, group, status

`rad1` / `rad2` su `radio` (`1509`), parametar `1` ili `0` (`3432`). `CopyStavke` je `check` (`1460`), `CheckState` (`3424`).

`GroupNG0` je `group` → `GroupBox` (`1354`), isto kao `frame` na `fr1`.

`UNESI_STAVKU_FORM` `s1` je `statusstrip` s `TEXT1="@@version"` i bez `TEXT2`/`TEXT3` (`257`). Tag je `@@VERSION` (`1258`). `TEXT2` i `TEXT3` fale: OERN.

## Rupe

- `SHOWCHECKBOX` bez `CHECKED` šalje `null`, ne današnji datum (`2277`, `3566`).
- `TRANSLATE` dira stupac prikaza, ne vrijednost (`2420`–`2424`).
- Statusstrip bez `TEXT2`/`TEXT3` ne smije pasti (`1258`–`1260`).
