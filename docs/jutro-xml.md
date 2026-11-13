# Četiri XML-a za ujutro

Ove datoteke nisu u repou. Stavke koje ih traže ostaju otvorene dok ne stignu neizmijenjene, onakve kakve su na klijentu. Ne treba ssMeni ni izmjena postojećih XML-ova.

| Datoteka | Zašto | Što donijeti |
|---|---|---|
| `TecRazOTV.xml` | smoke je `nema-xml` | cijeli XML iz mape koju `ssMeni.Parametar` navodi |
| `Poruke.xml` | smoke je `nema-xml` | isto, cijeli XML |
| `MER\RealizacijaFakture.xml` | izbornik traži MER primjerak, ne Agronom | XML iz foldera `MER` |
| `MER\RealizacijaStavkeNG.xml` | isto; Agronom primjerak u repou nije taj izbornički | XML iz foldera `MER` |

Ne dirati: KAR-12, INV-13, DOB-08, MPB-07, RIJ-30, RIJ-32.

Statusi `Unesi` (INSERTFORM) drži UI nit u `Dijalog.Prikazi` prije `ShowDialog`, pa klik na Odustani ne stigne. Smoke tu granulu i dalje samo otvara. Klik koji ostaje: OznakaKL `Izmjena` pa Odustani, i RealizacijaStavkeNG Odustani.
