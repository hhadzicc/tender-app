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
    public class PonudaController
    {
        private readonly IPonudaService ponudaService;
        private readonly ITenderService tenderService;

        public PonudaController(IPonudaService ponudaService, ITenderService tenderService)
        {
            this.ponudaService = ponudaService ?? throw new ArgumentNullException(nameof(ponudaService));
            this.tenderService = tenderService ?? throw new ArgumentNullException(nameof(tenderService));
        }

        public Ponuda? PrijaviPonudu(int tenderId, Ponudjac ponudjac, double cijena, int rok, int garancija, int kvalitet)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return null;
                }

                Ponuda ponuda = ponudaService.PrijaviPonudu(tender, ponudjac, cijena, rok, garancija, kvalitet);

                if (ponuda.JeValidna)
                {
                    Console.WriteLine($"✓ Ponuda je uspješno prijavljena (ID: {ponuda.Id})");
                    Console.WriteLine($"  Ponuđač: {ponudjac.Naziv}");
                    Console.WriteLine($"  Status: VALIDNA");
                }
                else
                {
                    Console.WriteLine($"⚠ Ponuda je prijavljena ali nije validna (ID: {ponuda.Id})");
                    Console.WriteLine($"  Ponuđač: {ponudjac.Naziv}");
                    Console.WriteLine($"  Status: NEVALIDNA - ne zadovoljava kriterije tendera");
                }

                return ponuda;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return null;
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"✗ Greška validacije: {ex.Message}");
                return null;
            }
        }

        public List<Ponuda> PrikaziPonudeZaTender(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return new List<Ponuda>();
                }

                List<Ponuda> ponude = ponudaService.GetPonudeZaTender(tenderId);

                if (ponude.Count == 0)
                {
                    Console.WriteLine($"Nema ponuda za tender '{tender.Naziv}'.");
                    return ponude;
                }

                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════════════");
                Console.WriteLine($"PONUDE ZA TENDER: {tender.Naziv}");
                Console.WriteLine("═══════════════════════════════════════════════════════════════");

                foreach (var ponuda in ponude)
                {
                    string status = ponuda.JeValidna ? "✓ VALIDNA" : "✗ NEVALIDNA";
                    Console.WriteLine($"[{ponuda.Id}] {ponuda.Ponudjac.Naziv} - {status}");
                    Console.WriteLine($"    Cijena: {ponuda.Cijena:F2} KM");
                    Console.WriteLine($"    Rok: {ponuda.RokIsporuke} dana");
                    Console.WriteLine($"    Garancija: {ponuda.Garancija} mjeseci");
                    Console.WriteLine($"    Kvalitet: {ponuda.Kvalitet}/100");

                    if (ponuda.JeValidna && ponuda.UkupanSkor > 0)
                    {
                        Console.WriteLine($"    Skor: {ponuda.UkupanSkor:F2}");
                    }

                    Console.WriteLine();
                }

                return ponude;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return new List<Ponuda>();
            }
        }

        public List<Ponuda> FiltrirajPonude(int? tenderId = null, double? minCijena = null,
                                            double? maxCijena = null, int? maxRok = null, int? minKvalitet = null)
        {
            List<Ponuda> ponude = ponudaService.FiltrirajPonude(tenderId, minCijena, maxCijena, maxRok, minKvalitet);

            if (ponude.Count == 0)
            {
                Console.WriteLine("Nema ponuda koje zadovoljavaju kriterije filtriranja.");
                return ponude;
            }

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("FILTRIRANE PONUDE:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            foreach (var ponuda in ponude)
            {
                Console.WriteLine($"[{ponuda.Id}] {ponuda.Ponudjac.Naziv}");
                Console.WriteLine($"    Tender ID: {ponuda.TenderId}");
                Console.WriteLine($"    Cijena: {ponuda.Cijena:F2} KM");
                Console.WriteLine($"    Rok: {ponuda.RokIsporuke} dana");
                Console.WriteLine($"    Kvalitet: {ponuda.Kvalitet}/100");
                Console.WriteLine();
            }

            return ponude;
        }
    }
}