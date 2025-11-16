using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TenderiApp.Models;

namespace TenderiApp.Views
{
    /// <summary>
    /// UI layer – interakcija s korisnikom (Console I/O).
    /// Isključeno iz code coverage-a jer je logika u servisima (Service Layer).
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MenuView
    {
        private readonly ConsoleView consoleView;

        public MenuView()
        {
            consoleView = new ConsoleView();
        }

        public int PrikaziGlavniMeni()
        {
            consoleView.PrikaziNaslov("APLIKACIJA ZA TENDERE - GLAVNI MENI");

            Console.WriteLine("1. Upravljanje tenderima");
            Console.WriteLine("2. Upravljanje ponudama");
            Console.WriteLine("3. Evaluacija i izvještaji");
            Console.WriteLine("4. Testni podaci");
            Console.WriteLine("5. Čuvanje i učitavanje tendera");// za potrebe TASK 4


            Console.WriteLine("0. Izlaz");
            Console.WriteLine();

            return consoleView.UcitajBroj("Izaberite opciju");
        }

        public int PrikaziMeniTendera()
        {
            consoleView.PrikaziNaslov("UPRAVLJANJE TENDERIMA");

            Console.WriteLine("1. Kreiraj novi tender");
            Console.WriteLine("2. Prikaži sve tendere");
            Console.WriteLine("3. Prikaži aktivne tendere");
            Console.WriteLine("4. Prikaži detalje tendera");
            Console.WriteLine("5. Zatvori tender");
            Console.WriteLine("0. Nazad");
            Console.WriteLine();

            return consoleView.UcitajBroj("Izaberite opciju");
        }

        public int PrikaziMeniPonuda()
        {
            consoleView.PrikaziNaslov("UPRAVLJANJE PONUDAMA");

            Console.WriteLine("1. Prijavi novu ponudu");
            Console.WriteLine("2. Prikaži ponude za tender");
            Console.WriteLine("3. Filtriraj ponude");
            Console.WriteLine("0. Nazad");
            Console.WriteLine();

            return consoleView.UcitajBroj("Izaberite opciju");
        }

        public int PrikaziMeniEvaluacije()
        {
            consoleView.PrikaziNaslov("EVALUACIJA I IZVJEŠTAJI");

            Console.WriteLine("1. Evaluiraj tender");
            Console.WriteLine("2. Odredi pobjednika");
            Console.WriteLine("3. Prikaži izvještaj");
            Console.WriteLine("4. Prikaži detaljan izvještaj");
            Console.WriteLine("5. Prikaži sažetak ponuda");
            Console.WriteLine("0. Nazad");
            Console.WriteLine();

            return consoleView.UcitajBroj("Izaberite opciju");
        }

        public (string naziv, string opis, DateTime rok, KriterijiTendera kriteriji) UcitajPodatkeTendera()
        {
            consoleView.PrikaziNaslov("KREIRANJE NOVOG TENDERA");

            string naziv = consoleView.UcitajTekst("Naziv tendera");
            string opis = consoleView.UcitajTekst("Opis tendera");

            DateTime rok = DateTime.Now.AddDays(30);
            int danaDoRoka = consoleView.UcitajBroj("Rok za prijavu (broj dana od danas)");
            rok = DateTime.Now.AddDays(danaDoRoka);

            Console.WriteLine();
            consoleView.PrikaziPoruku("Unesite kriterije tendera:", TipPoruke.Info);
            consoleView.PrikaziLiniju();

            double minCijena = consoleView.UcitajDecimalniBroj("Minimalna cijena (KM)");
            double maxCijena = consoleView.UcitajDecimalniBroj("Maksimalna cijena (KM)");
            int maxRok = consoleView.UcitajBroj("Maksimalan rok isporuke (dani)");
            int minGarancija = consoleView.UcitajBroj("Minimalna garancija (mjeseci)");
            int minKvalitet = consoleView.UcitajBroj("Minimalan kvalitet (0-100)");

            Console.WriteLine();
            consoleView.PrikaziPoruku("Unesite pondere kriterija (ukupno mora biti 1.0):", TipPoruke.Info);
            consoleView.PrikaziLiniju();

            double ponderCijene = consoleView.UcitajDecimalniBroj("Ponder cijene (0.0-1.0)");
            double ponderRoka = consoleView.UcitajDecimalniBroj("Ponder roka (0.0-1.0)");
            double ponderGarancije = consoleView.UcitajDecimalniBroj("Ponder garancije (0.0-1.0)");
            double ponderKvaliteta = consoleView.UcitajDecimalniBroj("Ponder kvaliteta (0.0-1.0)");

            PonderiKriterija ponderi = new PonderiKriterija(ponderCijene, ponderRoka, ponderGarancije, ponderKvaliteta);

            KriterijiTendera kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = minCijena,
                MaksimalnaCijena = maxCijena,
                MaksimalanRok = maxRok,
                MinimalnaGarancija = minGarancija,
                MinimalanKvalitet = minKvalitet
            };

            return (naziv, opis, rok, kriteriji);
        }

        public (int tenderId, Ponudjac ponudjac, double cijena, int rok, int garancija, int kvalitet) UcitajPodatkePonude()
        {
            consoleView.PrikaziNaslov("PRIJAVA NOVE PONUDE");

            int tenderId = consoleView.UcitajBroj("ID tendera");

            Console.WriteLine();
            consoleView.PrikaziPoruku("Podaci o ponuđaču:", TipPoruke.Info);
            consoleView.PrikaziLiniju();

            string naziv = consoleView.UcitajTekst("Naziv ponuđača");
            string email = consoleView.UcitajTekst("Email");
            string telefon = consoleView.UcitajTekst("Telefon");

            Ponudjac ponudjac = new Ponudjac(naziv, email, telefon);

            Console.WriteLine();
            consoleView.PrikaziPoruku("Detalji ponude:", TipPoruke.Info);
            consoleView.PrikaziLiniju();

            double cijena = consoleView.UcitajDecimalniBroj("Cijena (KM)");
            int rok = consoleView.UcitajBroj("Rok isporuke (dani)");
            int garancija = consoleView.UcitajBroj("Garancija (mjeseci)");
            int kvalitet = consoleView.UcitajBroj("Kvalitet (0-100)");

            return (tenderId, ponudjac, cijena, rok, garancija, kvalitet);
        }

        public (int? tenderId, double? minCijena, double? maxCijena, int? maxRok, int? minKvalitet) UcitajKriterijeFiltriranja()
        {
            consoleView.PrikaziNaslov("FILTRIRANJE PONUDA");

            consoleView.PrikaziPoruku("Ostavite prazno za preskakanje kriterija", TipPoruke.Info);
            Console.WriteLine();

            int? tenderId = null;
            string unosTenderId = consoleView.UcitajTekst("ID tendera (opciono)");
            if (!string.IsNullOrWhiteSpace(unosTenderId) && int.TryParse(unosTenderId, out int tid))
            {
                tenderId = tid;
            }

            double? minCijena = null;
            string unosMinCijena = consoleView.UcitajTekst("Minimalna cijena (opciono)");
            if (!string.IsNullOrWhiteSpace(unosMinCijena) && double.TryParse(unosMinCijena.Replace(',', '.'), out double minC))
            {
                minCijena = minC;
            }

            double? maxCijena = null;
            string unosMaxCijena = consoleView.UcitajTekst("Maksimalna cijena (opciono)");
            if (!string.IsNullOrWhiteSpace(unosMaxCijena) && double.TryParse(unosMaxCijena.Replace(',', '.'), out double maxC))
            {
                maxCijena = maxC;
            }

            int? maxRok = null;
            string unosMaxRok = consoleView.UcitajTekst("Maksimalan rok (opciono)");
            if (!string.IsNullOrWhiteSpace(unosMaxRok) && int.TryParse(unosMaxRok, out int maxR))
            {
                maxRok = maxR;
            }

            int? minKvalitet = null;
            string unosMinKvalitet = consoleView.UcitajTekst("Minimalan kvalitet (opciono)");
            if (!string.IsNullOrWhiteSpace(unosMinKvalitet) && int.TryParse(unosMinKvalitet, out int minK))
            {
                minKvalitet = minK;
            }

            return (tenderId, minCijena, maxCijena, maxRok, minKvalitet);
        }

        public void PrikaziGresku(string poruka)
        {
            consoleView.PrikaziPoruku(poruka, TipPoruke.Greska);
            consoleView.CekajEnter();
        }

        public void PrikaziUspjeh(string poruka)
        {
            consoleView.PrikaziPoruku(poruka, TipPoruke.Uspjeh);
            consoleView.CekajEnter();
        }

        public int PrikaziMeniSacuvajTender() // za potrebe TASK 4
        {
            consoleView.PrikaziNaslov("ČUVANJE I UČITAVANJE TENDERA");

            Console.WriteLine("1. Sačuvaj tendere u JSON fajl");
            Console.WriteLine("2. Učitaj tendere iz JSON fajla");
            Console.WriteLine("0. Nazad");
            Console.WriteLine();

            return consoleView.UcitajBroj("Izaberite opciju");
        }




    }
}