# Lokalne provjere (Linux, bez Windowsa i DevExpressa)

U cloud sesiji nema Windowsa ni DevExpressa, pa se rješenje ne može izgraditi. Pravi build i testovi idu u CI na sql-ai. Lokalno se mogu napraviti tri brze provjere prije pusha.

## 1. Sintaksa C# i dvostruki članovi

```
pip install --break-system-packages tree-sitter tree-sitter-c-sharp
python3 tools/provjera/cs_sintaksa.py Devex_26/Carpo.NetGen/Ui/Podloga.cs Devex_26/Carpo.NetGen/Grid/NgVeza.cs
```

Ispiše `problems 0` ili mjesta s greškom sintakse i dvostruke metode ili polja u istoj klasi.

Skripta ne vidi partial klase u drugim datotekama (`Podloga*.cs`), pa za njih provjeri ručno. `OnFormClosed` i `Dispose(bool)` su već u `PodlogaRafal.cs`. Skripta ne vidi ni tipove ni sukob imena lokalnih varijabli u ugniježđenim blokovima (CS0136). To vidi tek CI.

## 2. Ne-UI klase pod Mono

```
apt-get install -y mono-mcs mono-runtime   # ako nema mcs
mcs -target:library -out:/tmp/ng.dll -r:System.Data.dll -r:System.Xml.dll \
    -r:System.Windows.Forms.dll -r:System.Drawing.dll $(cat tools/provjera/mono-datoteke.txt)
```

`mono-datoteke.txt` su klase iz `Carpo.NetGen` koje se prevode bez DevExpressa (SQL tekst, lanac, model, XML). `stubs/Log.cs` zamjenjuje `NgDnevnik`, a `tools/diff/stubs/PrintBridgeStub.cs` zamjenjuje `PrintBridge`. Nad dobivenim `/tmp/ng.dll` napiši mali `.cs` program i pokreni ga s `mono` (dll mora biti u istoj mapi kao exe).

## 3. Diferencijalni testovi prema VB-u

`tools/diff/sql_diff.py` i `tools/diff/lanac_diff.py` uspoređuju doslovni Python model VB koda (`vb_oracle.py`, `lanac_vb.py`) s C# kodom (`CsVoz.cs`, `LanacVoz.cs` pod Mono) na svim SQL tekstovima i lancima iz korpusa. Upute su na vrhu svake skripte.

## Mrtve metode i QA kuka

```
python3 scripts/ci/mrtve-metode.py
```

Isto pokreće CI. Ruši se ako je metoda mrtva ili ako produkcijski kod spominje `TestHooks`/`GranulaSesija`/`GranulaIzvrsenje` (PRD-03).
