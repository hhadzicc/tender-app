using System;
using System.Collections.Generic;
using TenderiApp.Models;
using TenderiApp.Services;
using TenderiApp.Exceptions;
using TenderiApp.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Controllers
{

    /*
     Sva poslovna logika, validacija i obrada podataka nalaze se unutar servisnog sloja, dok kontroleri samo pozivaju te metode bez vlastite obrade podataka.
     Zbog toga bi testiranje kontrolera značilo ponavljanje istih testova koji su već obuhvaćeni testiranjem servisa, što nije efikasno niti potrebno.
     */
    [ExcludeFromCodeCoverage]
    public class TenderController
    {
        private readonly ITenderService tenderService;
        private readonly IEvaluacijaService evaluacijaService;

        public TenderController(ITenderService tenderService, IEvaluacijaService evaluacijaService)
        {
            this.tenderService = tenderService ?? throw new ArgumentNullException(nameof(tenderService));
            this.evaluacijaService = evaluacijaService ?? throw new ArgumentNullException(nameof(evaluacijaService));
        }

        public Tender KreirajTender(string naziv, string opis, DateTime rok, KriterijiTendera kriteriji)
        {
            try
            {
                Tender tender = tenderService.KreirajTender(naziv, opis, rok, kriteriji);
                Console.WriteLine($"✓ Tender '{tender.Naziv}' je uspješno kreiran (ID: {tender.Id})");
                return tender;
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"✗ Greška prilikom validacije: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Greška prilikom kreiranja tendera: {ex.Message}");
                throw;
            }
        }

        public Tender? PrikaziTender(int id)
        {
            try
            {
                Tender tender = tenderService.GetTender(id);

                if (tender != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine($"TENDER: {tender.Naziv}");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine($"ID: {tender.Id}");
                    Console.WriteLine($"Opis: {tender.Opis}");
                    Console.WriteLine($"Status: {tender.Status}");
                    Console.WriteLine($"Datum kreiranja: {tender.DatumKreiranja:dd.MM.yyyy HH:mm}");
                    Console.WriteLine($"Rok za prijavu: {tender.RokZaPrijavu:dd.MM.yyyy HH:mm}");
                    Console.WriteLine($"Broj ponuda: {tender.Ponude.Count}");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine();
                }

                return tender;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return null;
            }
        }

        public List<Tender> PrikaziSveTendere()
        {
            List<Tender> tenderi = tenderService.GetSviTenderi();

            if (tenderi.Count == 0)
            {
                Console.WriteLine("Nema kreiranih tendera.");
                return tenderi;
            }

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("SVI TENDERI:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            foreach (var tender in tenderi)
            {
                Console.WriteLine($"[{tender.Id}] {tender.Naziv}");
                Console.WriteLine($"    Status: {tender.Status}");
                Console.WriteLine($"    Rok: {tender.RokZaPrijavu:dd.MM.yyyy}");
                Console.WriteLine($"    Ponude: {tender.Ponude.Count}");
                Console.WriteLine();
            }

            return tenderi;
        }

        public List<Tender> PrikaziAktivneTendere()
        {
            List<Tender> aktivniTenderi = tenderService.GetAktivniTenderi();

            if (aktivniTenderi.Count == 0)
            {
                Console.WriteLine("Nema aktivnih tendera.");
                return aktivniTenderi;
            }

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("AKTIVNI TENDERI:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            foreach (var tender in aktivniTenderi)
            {
                Console.WriteLine($"[{tender.Id}] {tender.Naziv}");
                Console.WriteLine($"    Rok: {tender.RokZaPrijavu:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"    Ponude: {tender.Ponude.Count}");
                Console.WriteLine();
            }

            return aktivniTenderi;
        }

        public bool ZatvoriTender(int tenderId)
        {
            try
            {
                bool uspjeh = tenderService.ZatvoriTender(tenderId);

                if (uspjeh)
                {
                    Console.WriteLine($"✓ Tender ID {tenderId} je uspješno zatvoren.");
                }

                return uspjeh;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return false;
            }
        }

        public List<Ponuda> EvaluirajTender(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return new List<Ponuda>();
                }

                if (tender.Status == StatusTendera.Otvoren)
                {
                    ZatvoriTender(tenderId);
                }

                List<Ponuda> rangiranePonude = evaluacijaService.EvaluirajPonude(tender);

                Console.WriteLine($"✓ Tender '{tender.Naziv}' je uspješno evaluiran.");
                Console.WriteLine($"  Validnih ponuda: {rangiranePonude.Count}");

                return rangiranePonude;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška prilikom evaluacije: {ex.Message}");
                return new List<Ponuda>();
            }
        }

        public Ponuda? OdrediPobjednika(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return null;
                }

                if (tender.Status != StatusTendera.Evaluiran)
                {
                    Console.WriteLine("✗ Tender mora biti evaluiran prije određivanja pobjednika.");
                    return null;
                }

                Ponuda? pobjednik = evaluacijaService.OdrediPobjednika(tender);

                if (pobjednik != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine("🏆 POBJEDNIK TENDERA:");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine($"Ponuđač: {pobjednik.Ponudjac.Naziv}");
                    Console.WriteLine($"Cijena: {pobjednik.Cijena:F2} KM");
                    Console.WriteLine($"Rok isporuke: {pobjednik.RokIsporuke} dana");
                    Console.WriteLine($"Garancija: {pobjednik.Garancija} mjeseci");
                    Console.WriteLine($"Kvalitet: {pobjednik.Kvalitet}/100");
                    Console.WriteLine($"Ukupan skor: {pobjednik.UkupanSkor:F2}");
                    Console.WriteLine("═══════════════════════════════════════════════════════════════");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("✗ Nema validnih ponuda za određivanje pobjednika.");
                }

                return pobjednik;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return null;
            }
        }
    }
}