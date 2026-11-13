# Combo na dijalogu — deset slučajeva

Port danas nacrta `LookUpEdit` i ne šalje `SQL` (`Dijalog.cs` 174–175). NetGen puni combo čim se kontrola složi (`Form2.vb` 2396–2427). Stupac 0 je vrijednost, stupac 1 je prikaz (`2430–2431`). Imena stupaca se ne gledaju.

`ALLOWEMPTY` je točno `true` nakon `Trim` i `ToLower` (`2402–2403`). Inače je prazan red isključen. Ako je `true` i upit vrati tablicu, na početak ide red `null` / prazan tekst (`2405–2411`).

Ako ništa nije odabrano i prazan red nije uključen, pokušaj `SelectedValue = 1` (`3455–3464`). Pogodi red čija je vrijednost 1. Ako ga nema, parametar je `null`.

`ONCLICK="select 'refresh' as ime"` ponovo puni taj combo istim SQL-om (`5158–5176`).

Multidrop nije combo. Parametar je `'id1~id2'` bez razmaka (`3441–3446`, R28).

| # | Granula i kontrola | Ulaz | Očekivano |
|---|---|---|---|
| 1 | `MPRealizNG` `FORM1.trgovinaid` | `ALLOWEMPTY="true"`, SQL `select t.trgovinaid, … from trgovina … where p.operaterid=@@operateriid` | `true` ubaci prazan red. `@@operateriid` se zamijeni prije slanja (`Class1.vb` 755). Prikaz je stupac 1. Odabir praznog reda šalje `@trgovinaid=null` (`3456–3457`). |
| 2 | `MPInventura` `sortpoid` | `ALLOWEMPTY="FALSE"`, SQL `select 1 … union all select 2 … 3 … 4` | Prazan red se ne ubaci. Ako ništa nije odabrano, `SelectedValue=1` pogodi prvi red, klasifikacija i naziv. |
| 3 | `MPInventura` `trgovinaid` | `ALLOWEMPTY="FALSE"`, `SQL="spMPDajTrgovina @operateriid=@@operateriid"`, `ONCLICK="select 'refresh' as skladisteid, 'refresh' as osobeid"` | Nema praznog reda. Klik ponovo puni `skladisteid` i `osobeid`, ne samu trgovinu (`5158`). |
| 4 | `MPInventura` `osobeid` | `ALLOWEMPTY="FALSE"`, `SQL="spMPDajOsobe @trgovinaid = @@trgovinaid"` | Pri otvaranju `@@trgovinaid` je vrijednost te kontrole, još prazna pa upit ide s praznim brojem ili se preskoči ako `@@` ostane (`Class1.vb` 113–118). Nakon refresha iz slučaja 3 SQL se pošalje s odabranom trgovinom. |
| 5 | `MPKntNacinPlacAzur` `svi` | `ALLOWEMPTY="FOLSE"`, SQL `select 2 as svi, 'SVI' … union all select 1, 'Samo za MP ' order by 1` | `FOLSE` nije `true` (`2403`, R31). Prazan red se ne ubaci. `order by 1` stavi vrijednost 1 prvu. `SelectedValue=1` pogodi `Samo za MP`, ne `SVI`. |
| 6 | `MPKaseOperNG` `kasa` | nema `ALLOWEMPTY`, `SQL="select Kasa from MPkase where TrgovinaId=@@id"` | Nedostajući atribut ostavi `AllowEmpty=False` (`2402`, `On Error Resume Next`). Prazan red se ne ubaci. `@@id` je id mastera. |
| 7 | `KntOPZ-STAT-1` `nizKonto` | `TYPE="multidrop"`, `SQL="spKntOPZSTAT @ACTION=4"`, nema `ALLOWEMPTY` | Nije combo. Nema `SelectedValue=1` ni praznog reda comba. Označeni id-evi idu kao `'id1~id2'` (`3446`). |
| 8 | `DobUgovori` `DobUgovoriVrsteId` | `ALLOWEMPTY="TRUE"`, `TRANSLATE="true"`, `SQL="select DobUgovoriVrsteId, naziv from DobUgovoriVrste"` | Prazan red na početku. `TRANSLATE` prevodi samo stupac 1 (`2420–2424`). Stupac 0 ostaje id. |
| 9 | `MPNivelacija` `FILTERFORM.xtrgovinaid` | `ALLOWEMPTY="TRUE"`, `SQL="spMPDajTrgovinaNG @operateriid = @@operateriid, @vrstatrg='M'"` | Prazan red. `@@operateriid` se zamijeni. `TRUE` jest `true`. |
| 10 | `MER_ZahtjevZaNabavkuNG` `namjena` | nema `ALLOWEMPTY`, SQL `select 0 as namjena, 'Roba' … union all select 1, 'Osnovno sredstvo'` | Prazan red se ne ubaci. Prvi red ima vrijednost 0. `SelectedValue=1` pogodi drugi red, osnovno sredstvo, ne robu. |

Port koji samo nacrta prazan `LookUpEdit` pada na svih deset. Multidrop (7) mora ostati posebna kontrola.
