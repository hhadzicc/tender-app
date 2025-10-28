using System;
using System.Collections.Generic;
using TenderiApp.Models;
using TenderiApp.Services;
using TenderiApp.Exceptions;

namespace TenderiApp.Controllers
{
    public class IzvjestajController
    {
        private readonly IzvjestajService izvjestajService;
        private readonly TenderService tenderService;

        public IzvjestajController(IzvjestajService izvjestajService, TenderService tenderService)
        {
            this.izvjestajService = izvjestajService ?? throw new ArgumentNullException(nameof(izvjestajService));
            this.tenderService = tenderService ?? throw new ArgumentNullException(nameof(tenderService));
        }

        public Izvjestaj? GenerisiIzvjestaj(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return null;
                }

                Izvjestaj izvjestaj = izvjestajService.GenerisiIzvjestaj(tender);

                Console.WriteLine($"✓ Izvještaj za tender '{tender.Naziv}' je uspješno generisan.");

                return izvjestaj;
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
                return null;
            }
        }

        public void PrikaziIzvjestaj(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return;
                }

                Izvjestaj izvjestaj = izvjestajService.GenerisiIzvjestaj(tender);

                Console.WriteLine();
                Console.WriteLine(izvjestaj.PrikaziIzvjestaj());
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
            }
        }

        public void PrikaziDetaljanIzvjestaj(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return;
                }

                string detaljanIzvjestaj = izvjestajService.GenerisiDetaljanIzvjestaj(tender);

                Console.WriteLine();
                Console.WriteLine(detaljanIzvjestaj);
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
            }
        }

        public void PrikaziSazetak(int tenderId)
        {
            try
            {
                Tender tender = tenderService.GetTender(tenderId);

                if (tender == null)
                {
                    Console.WriteLine($"✗ Tender sa ID {tenderId} ne postoji.");
                    return;
                }

                List<Ponuda> ponude = tender.Ponude;

                string sazetak = izvjestajService.GenerisiSazetak(ponude);

                Console.WriteLine();
                Console.WriteLine(sazetak);
            }
            catch (TenderException ex)
            {
                Console.WriteLine($"✗ Greška: {ex.Message}");
            }
        }
    }
}