# Klijentski XML

Ove datoteke ne zamjenjuju `definitions/netgen`. Postojeći XML-ovi nisu dirani. Izvor svake kopije je u `IZVOR.md` (md5 i mape s klijenata).

Referentna verzija za granulu koja je čekala datoteku je najrasprostranjenija, a ako je samo jedna, ta.

| Granula | Referentna | Zašto | Ostale |
|---|---|---|---|
| Poruke | `Poruke.v1.xml` | jedina verzija, 6 kopija | nema |
| RealizacijaFakture | `RealizacijaFakture.v1.xml` | jedina verzija | nema |
| RealizacijaStavkeNG | `RealizacijaStavkeNG.v4.xml` | 6 kopija (dva klijentska stabla) | v1, v2, v3 po jedna |
| TecRazOTV | `TecRazOTV.v2.xml` | 4 kopije | v1 i v3 po jedna |

Ulaz svih referentnih datoteka je forma (`:Parametri` ili `:FILTERFORM`), osim `Poruke` koja nema `ONLOAD` i drži stablo `spTreePorukeNG:Poruke`. `scripts/spin/Otvori.ps1` ih sve otvori i zapiše u `artifacts/spin/otvaranje.md`. Testovi koji i dalje traže datoteku u `definitions/netgen` ostaju otvoreni: putanja testa nije prepravljena da prođe.
