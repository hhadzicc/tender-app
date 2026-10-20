using System;
using System.Collections.Generic;
using System.Linq;
using TenderiApp.Models;
using TenderiApp.Exceptions;
using TenderiApp.Interfaces;

namespace TenderiApp.Services
{
    public class PonudaService : IPonudaService
    {
        private readonly ITenderService? tenderService;
        private readonly List<Tender> tenderi = new List<Tender>();

        public PonudaService(ITenderService? tenderService = null)
        {
            this.tenderService = tenderService;
        }

        private List<Ponuda> GetSvePonude()
        {
            var sviTenderi = tenderService?.GetSviTenderi() ?? tenderi;
            return sviTenderi.SelectMany(t => t.Ponude).ToList();
        }

        public Ponuda PrijaviPonudu(Tender tender, Ponudjac ponudjac, double cijena, int rok, int garancija, int kvalitet)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (ponudjac == null)
            {
                throw new ArgumentNullException(nameof(ponudjac));
            }

            if (tender.Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije otvoren za prijave.");
            }

            if (DateTime.Now > tender.RokZaPrijavu)
            {
                throw new TenderException("Rok za prijavu je istekao.");
            }

            if (tenderService == null && !tenderi.Contains(tender))
            {
                tenderi.Add(tender);
            }

            var svePonude = GetSvePonude();
            if (ponudjac.Id <= 0)
            {
                var postojeciPonudjac = svePonude.Select(p => p.Ponudjac)
                    .FirstOrDefault(p => string.Equals(p.Email.Trim(), ponudjac.Email.Trim(), StringComparison.OrdinalIgnoreCase));
                ponudjac.Id = postojeciPonudjac != null && postojeciPonudjac.Id > 0
                    ? postojeciPonudjac.Id
                    : svePonude.Select(p => p.Ponudjac.Id).DefaultIfEmpty(0).Max() + 1;
            }

            if (svePonude.Any(p => p.TenderId == tender.Id &&
                (p.Ponudjac.Id == ponudjac.Id ||
                 string.Equals(p.Ponudjac.Email.Trim(), ponudjac.Email.Trim(), StringComparison.OrdinalIgnoreCase))))
            {
                throw new TenderException("Ponuđač je već prijavio ponudu za ovaj tender.");
            }

            Ponuda ponuda = new Ponuda(tender, ponudjac, cijena, rok, garancija, kvalitet)
            {
                Id = svePonude.Select(p => p.Id).DefaultIfEmpty(0).Max() + 1
            };

            // INLINE kod iz ValidirajPonudu()
            // bool validna = ValidirajPonudu(ponuda, tender.Kriteriji); // Zakomentirano

            // Validacija ponude - inline kod
            bool validna = true;

            if (ponuda.Cijena < tender.Kriteriji.MinimalnaCijena)
            {
                validna = false;
            }

            if (ponuda.Cijena > tender.Kriteriji.MaksimalnaCijena)
            {
                validna = false;
            }

            if (ponuda.RokIsporuke > tender.Kriteriji.MaksimalanRok)
            {
                validna = false;
            }

            if (ponuda.Garancija < tender.Kriteriji.MinimalnaGarancija)
            {
                validna = false;
            }

            if (ponuda.Kvalitet < tender.Kriteriji.MinimalanKvalitet)
            {
                validna = false;
            }

            ponuda.JeValidna = validna;

            tender.DodajPonudu(ponuda);

            return ponuda;
        }

        

        public bool ValidirajPonudu(Ponuda ponuda, KriterijiTendera kriteriji)
        {
            if (ponuda == null)
            {
                throw new ArgumentNullException(nameof(ponuda));
            }

            if (kriteriji == null)
            {
                throw new ArgumentNullException(nameof(kriteriji));
            }

            bool validna = true;

            if (ponuda.Cijena < kriteriji.MinimalnaCijena)
            {
                validna = false;
            }

            if (ponuda.Cijena > kriteriji.MaksimalnaCijena)
            {
                validna = false;
            }

            if (ponuda.RokIsporuke > kriteriji.MaksimalanRok)
            {
                validna = false;
            }

            if (ponuda.Garancija < kriteriji.MinimalnaGarancija)
            {
                validna = false;
            }

            if (ponuda.Kvalitet < kriteriji.MinimalanKvalitet)
            {
                validna = false;
            }

            ponuda.JeValidna = validna;
            return validna;
        }

        public List<Ponuda> GetPonudeZaTender(int tenderId)
        {
            return GetSvePonude().Where(p => p.TenderId == tenderId).ToList();
        }

        public List<Ponuda> FiltrirajPonude(int? tenderId = null, double? minCijena = null, double? maxCijena = null,
                                            int? maxRok = null, int? minKvalitet = null)
        {
            var rezultat = GetSvePonude().AsQueryable();

            if (tenderId.HasValue)
            {
                rezultat = rezultat.Where(p => p.TenderId == tenderId.Value);
            }

            if (minCijena.HasValue)
            {
                rezultat = rezultat.Where(p => p.Cijena >= minCijena.Value);
            }

            if (maxCijena.HasValue)
            {
                rezultat = rezultat.Where(p => p.Cijena <= maxCijena.Value);
            }

            if (maxRok.HasValue)
            {
                rezultat = rezultat.Where(p => p.RokIsporuke <= maxRok.Value);
            }

            if (minKvalitet.HasValue)
            {
                rezultat = rezultat.Where(p => p.Kvalitet >= minKvalitet.Value);
            }

            return rezultat.ToList();
        }
    }
}