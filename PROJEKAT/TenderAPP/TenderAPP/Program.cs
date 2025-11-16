using System;
using System.Collections.Generic;
using TenderiApp.Models;
using TenderiApp.Services;
using TenderiApp.Controllers;
using TenderiApp.Views;
using TenderiApp.Exceptions;
using TenderiApp.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private static ITenderService tenderService = new TenderService();
        private static IPonudaService ponudaService = new PonudaService();
        private static IEvaluacijaService evaluacijaService = new EvaluacijaService();
        private static IIzvjestajService izvjestajService = new IzvjestajService();

        private static TenderController tenderController = new TenderController(tenderService, evaluacijaService);
        private static PonudaController ponudaController = new PonudaController(ponudaService, tenderService);
        private static IzvjestajController izvjestajController = new IzvjestajController(izvjestajService, tenderService);

        private static MenuView menuView = new MenuView();
        private static ConsoleView consoleView = new ConsoleView();

        static void Main(string[] args)
        {
            bool nastaviti = true;

            while (nastaviti)
            {
                try
                {
                    int izbor = menuView.PrikaziGlavniMeni();

                    switch (izbor)
                    {
                        case 1:
                            MeniTendera();
                            break;
                        case 2:
                            MeniPonuda();
                            break;
                        case 3:
                            MeniEvaluacije();
                            break;
                        case 4:
                            KreirajTestnePodatke();
                            break;
                        case 5: // Nova opcija za potrebe TASK 4
                            MeniSacuvajTender();
                            break;
                        case 0:
                            nastaviti = false;
                            consoleView.PrikaziPoruku("Hvala što ste koristili aplikaciju!", TipPoruke.Info);
                            break;
                        default:
                            menuView.PrikaziGresku("Nepoznata opcija!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    menuView.PrikaziGresku($"Neočekivana greška: {ex.Message}");
                }
            }
        }

        static void MeniTendera()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    int izbor = menuView.PrikaziMeniTendera();

                    switch (izbor)
                    {
                        case 1:
                            KreirajTender();
                            break;
                        case 2:
                            tenderController.PrikaziSveTendere();
                            consoleView.CekajEnter();
                            break;
                        case 3:
                            tenderController.PrikaziAktivneTendere();
                            consoleView.CekajEnter();
                            break;
                        case 4:
                            PrikaziDetalje();
                            break;
                        case 5:
                            ZatvoriTender();
                            break;
                        case 0:
                            nazad = true;
                            break;
                        default:
                            menuView.PrikaziGresku("Nepoznata opcija!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    menuView.PrikaziGresku($"Greška: {ex.Message}");
                }
            }
        }

        static void MeniPonuda()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    int izbor = menuView.PrikaziMeniPonuda();

                    switch (izbor)
                    {
                        case 1:
                            PrijaviPonudu();
                            break;
                        case 2:
                            PrikaziPonudeZaTender();
                            break;
                        case 3:
                            FiltrirajPonude();
                            break;
                        case 0:
                            nazad = true;
                            break;
                        default:
                            menuView.PrikaziGresku("Nepoznata opcija!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    menuView.PrikaziGresku($"Greška: {ex.Message}");
                }
            }
        }

        static void MeniEvaluacije()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    int izbor = menuView.PrikaziMeniEvaluacije();

                    switch (izbor)
                    {
                        case 1:
                            EvaluirajTender();
                            break;
                        case 2:
                            OdrediPobjednika();
                            break;
                        case 3:
                            PrikaziIzvjestaj();
                            break;
                        case 4:
                            PrikaziDetaljanIzvjestaj();
                            break;
                        case 5:
                            PrikaziSazetak();
                            break;
                        case 0:
                            nazad = true;
                            break;
                        default:
                            menuView.PrikaziGresku("Nepoznata opcija!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    menuView.PrikaziGresku($"Greška: {ex.Message}");
                }
            }
        }

        static void KreirajTender()
        {
            try
            {
                var podaci = menuView.UcitajPodatkeTendera();
                tenderController.KreirajTender(podaci.naziv, podaci.opis, podaci.rok, podaci.kriteriji);
                consoleView.CekajEnter();
            }
            catch (ValidationException ex)
            {
                menuView.PrikaziGresku($"Greška validacije: {ex.Message}");
            }
            catch (Exception ex)
            {
                menuView.PrikaziGresku($"Greška: {ex.Message}");
            }
        }

        static void PrikaziDetalje()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            tenderController.PrikaziTender(tenderId);
            consoleView.CekajEnter();
        }

        static void ZatvoriTender()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera koji želite zatvoriti");
            tenderController.ZatvoriTender(tenderId);
            consoleView.CekajEnter();
        }

        static void PrijaviPonudu()
        {
            try
            {
                var podaci = menuView.UcitajPodatkePonude();
                ponudaController.PrijaviPonudu(podaci.tenderId, podaci.ponudjac, podaci.cijena,
                                               podaci.rok, podaci.garancija, podaci.kvalitet);
                consoleView.CekajEnter();
            }
            catch (ValidationException ex)
            {
                menuView.PrikaziGresku($"Greška validacije: {ex.Message}");
            }
            catch (Exception ex)
            {
                menuView.PrikaziGresku($"Greška: {ex.Message}");
            }
        }

        static void PrikaziPonudeZaTender()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            ponudaController.PrikaziPonudeZaTender(tenderId);
            consoleView.CekajEnter();
        }

        static void FiltrirajPonude()
        {
            var kriteriji = menuView.UcitajKriterijeFiltriranja();
            ponudaController.FiltrirajPonude(kriteriji.tenderId, kriteriji.minCijena,
                                             kriteriji.maxCijena, kriteriji.maxRok, kriteriji.minKvalitet);
            consoleView.CekajEnter();
        }

        static void EvaluirajTender()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera za evaluaciju");
            tenderController.EvaluirajTender(tenderId);
            consoleView.CekajEnter();
        }

        static void OdrediPobjednika()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            tenderController.OdrediPobjednika(tenderId);
            consoleView.CekajEnter();
        }

        static void PrikaziIzvjestaj()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            izvjestajController.PrikaziIzvjestaj(tenderId);
            consoleView.CekajEnter();
        }

        static void PrikaziDetaljanIzvjestaj()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            izvjestajController.PrikaziDetaljanIzvjestaj(tenderId);
            consoleView.CekajEnter();
        }

        static void PrikaziSazetak()
        {
            int tenderId = consoleView.UcitajBroj("Unesite ID tendera");
            izvjestajController.PrikaziSazetak(tenderId);
            consoleView.CekajEnter();
        }

        static void KreirajTestnePodatke()
        {
            try
            {
                consoleView.PrikaziNaslov("KREIRANJE TESTNIH PODATAKA");

                // Kreiranje test tendera
                PonderiKriterija ponderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);
                KriterijiTendera kriteriji = new KriterijiTendera(ponderi)
                {
                    MinimalnaCijena = 10000,
                    MaksimalnaCijena = 50000,
                    MaksimalanRok = 90,
                    MinimalnaGarancija = 12,
                    MinimalanKvalitet = 60
                };

                Tender tender1 = tenderController.KreirajTender(
                    "Nabavka računarske opreme",
                    "Tender za nabavku računara, monitora i periferija za potrebe fakulteta",
                    DateTime.Now.AddDays(30),
                    kriteriji
                );

                // Kreiranje ponuđača i ponuda
                Ponudjac ponudjac1 = new Ponudjac("Tech Solutions d.o.o.", "info@techsolutions.ba", "+387 33 123 456");
                ponudjac1.Id = 1;
                ponudaController.PrijaviPonudu(tender1.Id, ponudjac1, 35000, 45, 24, 85);

                Ponudjac ponudjac2 = new Ponudjac("IT Comp d.o.o.", "kontakt@itcomp.ba", "+387 33 234 567");
                ponudjac2.Id = 2;
                ponudaController.PrijaviPonudu(tender1.Id, ponudjac2, 42000, 30, 18, 90);

                Ponudjac ponudjac3 = new Ponudjac("Digital World", "info@digitalworld.ba", "+387 33 345 678");
                ponudjac3.Id = 3;
                ponudaController.PrijaviPonudu(tender1.Id, ponudjac3, 28000, 60, 12, 75);

                Ponudjac ponudjac4 = new Ponudjac("Hardware Plus", "office@hardwareplus.ba", "+387 33 456 789");
                ponudjac4.Id = 4;
                ponudaController.PrijaviPonudu(tender1.Id, ponudjac4, 55000, 40, 36, 95);

                // Kreiranje drugog tendera
                PonderiKriterija ponderi2 = new PonderiKriterija(0.5, 0.2, 0.2, 0.1);
                KriterijiTendera kriteriji2 = new KriterijiTendera(ponderi2)
                {
                    MinimalnaCijena = 5000,
                    MaksimalnaCijena = 20000,
                    MaksimalanRok = 60,
                    MinimalnaGarancija = 6,
                    MinimalanKvalitet = 50
                };

                Tender tender2 = tenderController.KreirajTender(
                    "Nabavka kancelarijskog materijala",
                    "Godišnja nabavka kancelarijskog materijala i potrošnog materijala za štampače",
                    DateTime.Now.AddDays(15),
                    kriteriji2
                );

                Ponudjac ponudjac5 = new Ponudjac("Office Pro", "sales@officepro.ba", "+387 33 567 890");
                ponudjac5.Id = 5;
                ponudaController.PrijaviPonudu(tender2.Id, ponudjac5, 12000, 30, 12, 70);

                Ponudjac ponudjac6 = new Ponudjac("Paper World", "info@paperworld.ba", "+387 33 678 901");
                ponudjac6.Id = 6;
                ponudaController.PrijaviPonudu(tender2.Id, ponudjac6, 15000, 20, 6, 80);

                // Kreiranje trećeg tendera
                PonderiKriterija ponderi3 = new PonderiKriterija(0.35, 0.25, 0.25, 0.15);
                KriterijiTendera kriteriji3 = new KriterijiTendera(ponderi3)
                {
                    MinimalnaCijena = 20000,
                    MaksimalnaCijena = 100000,
                    MaksimalanRok = 120,
                    MinimalnaGarancija = 24,
                    MinimalanKvalitet = 70
                };

                Tender tender3 = tenderController.KreirajTender(
                    "Održavanje IT infrastrukture",
                    "Godišnji ugovor o održavanju servera, mreže i IT opreme",
                    DateTime.Now.AddDays(45),
                    kriteriji3
                );

                Ponudjac ponudjac7 = new Ponudjac("Network Solutions", "contact@netsolutions.ba", "+387 33 789 012");
                ponudjac7.Id = 7;
                ponudaController.PrijaviPonudu(tender3.Id, ponudjac7, 45000, 90, 24, 85);

                Ponudjac ponudjac8 = new Ponudjac("IT Service Pro", "info@itservicepro.ba", "+387 33 890 123");
                ponudjac8.Id = 8;
                ponudaController.PrijaviPonudu(tender3.Id, ponudjac8, 38000, 100, 36, 90);

                Ponudjac ponudjac9 = new Ponudjac("Tech Support", "office@techsupport.ba", "+387 33 901 234");
                ponudjac9.Id = 9;
                ponudaController.PrijaviPonudu(tender3.Id, ponudjac9, 52000, 80, 30, 80);

                // Nevalidna ponuda za testiranje
                Ponudjac ponudjac10 = new Ponudjac("Budget IT", "info@budgetit.ba", "+387 33 012 345");
                ponudjac10.Id = 10;
                ponudaController.PrijaviPonudu(tender3.Id, ponudjac10, 15000, 150, 6, 50);

                consoleView.PrikaziPoruku("Testni podaci su uspješno kreirani!", TipPoruke.Uspjeh);
                Console.WriteLine();
                Console.WriteLine("Kreirano:");
                Console.WriteLine($"  - 3 tendera");
                Console.WriteLine($"  - 10 ponuđača");
                Console.WriteLine($"  - 10 ponuda");
                Console.WriteLine();

                consoleView.CekajEnter();
            }
            catch (Exception ex)
            {
                menuView.PrikaziGresku($"Greška prilikom kreiranja testnih podataka: {ex.Message}");
            }
        }

        static void MeniSacuvajTender()
        {
            bool nazad = false;

            while (!nazad)
            {
                int izbor = menuView.PrikaziMeniSacuvajTender();

                switch (izbor)
                {
                    case 1: // Sačuvaj tender
                        string pathSave = consoleView.UcitajTekst("Unesite putanju za čuvanje fajla");
                        tenderController.SacuvajTenderJSON(pathSave);
                        consoleView.PrikaziPoruku($"Tenderi su sačuvani u '{pathSave}'", TipPoruke.Uspjeh);
                        break;

                    case 2: // Učitaj tender
                        string pathLoad = consoleView.UcitajTekst("Unesite putanju fajla za učitavanje");
                        tenderController.UcitajTenderJSON(pathLoad);
                        consoleView.PrikaziPoruku($"Tenderi su učitani iz '{pathLoad}'", TipPoruke.Uspjeh);
                        break;

                    case 0:
                        nazad = true;
                        break;

                    default:
                        consoleView.PrikaziPoruku("Nepoznata opcija!", TipPoruke.Greska);
                        break;
                }
            }
        }



    }
}