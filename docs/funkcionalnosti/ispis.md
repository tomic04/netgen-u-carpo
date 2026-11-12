# Ispis

`PRINTMASTER`, `PRINTREPXMASTER`, `PRINTGRID` i `PRINTREPXGRID` zovu isti `IspisGRIDREPX` (`Form1.vb` 4945–4955, `Form2.vb` 6950–6991). `PAGEORIENTATION` se na tom putu ne čita (`6039` je Janus gumb koji se ne zove).

`:repx/ime` na dijalogu postane `repx:/ime` (`Form2.vb` 4297). Na podlozi postane `repx:ime` (`Form1.vb` 5238).

NetGen prvo učita `Spin.Printing.v23`, pa `v9.1` (`Class1.vb` 297–304). Ako to padne, `Form6` zove `rptDevRepx` (`134–161`). Port zove v26 i isti fallback. v23 i v9.1 se ne učitavaju. Pisač se iz testa ne zove.

`:vrstadokid-ime` nije forma. Predložak je iz `vrstadok.template` (`Class1.vb` 443–486).

Ime forme koje počinje s `MULTISELECT` ili `MULTIBLUE` briše `TotalGenMultiSelectTemp` prije ispisa i prije Carpo forme (`Form1.vb` 4932–4936). Test te naredbe ne šalje.
