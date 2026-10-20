# TenderApp

Konzolna aplikacija u C# za upravljanje tenderima i evaluaciju ponuda, s naglaskom na **automatizovano testiranje poslovnih pravila pomoću xUnit-a i Moq-a**.

## Funkcionalnosti

- Kreiranje tendera s rokom prijave, kriterijima i ponderima.
- Prijavljivanje ponuda i validacija cijene, roka isporuke, garancije i kvaliteta.
- Bodovanje i rangiranje ponuda te izbor pobjednika.
- Izvještaji i sažeci ponuda.
- Čuvanje i učitavanje tendera u JSON formatu.

## Pokretanje

Potreban je **.NET 8 SDK**. Iz korijena repozitorija pokreni:

```sh
dotnet run --project PROJEKAT/TenderAPP/TenderAPP/TenderAPP.csproj
```

Za pregled aplikacije s primjerima izaberi **4. Testni podaci** u glavnom meniju.
Podaci se čuvaju u memoriji, a upis i učitavanje JSON fajla pokreću se kroz meni.

## Testovi

Testovi koriste **xUnit** i **Moq**, uz CSV/JSON testne podatke. Provjeravaju
validaciju, bodovanje, rangiranje, izvještaje i čuvanje podataka.

```sh
dotnet test PROJEKAT/TenderAPP/TenderAPP.sln
```

Za izvještaj o pokrivenosti koda:

```sh
dotnet test PROJEKAT/TenderAPP/TenderAPP.sln --collect:"XPlat Code Coverage"
```

Konzolni UI i kontroleri isključeni su iz mjerenja pokrivenosti koda.
