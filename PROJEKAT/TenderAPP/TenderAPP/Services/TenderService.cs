using System;
using System.Collections.Generic;
using System.Linq;
using TenderiApp.Models;
using TenderiApp.Exceptions;

namespace TenderiApp.Services
{
    public class TenderService
    {
        private List<Tender> tenderi;
        private int nextId;

        public TenderService()
        {
            tenderi = new List<Tender>();
            nextId = 1;
        }

        public Tender KreirajTender(string naziv, string opis, DateTime rok, KriterijiTendera kriteriji)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                throw new ValidationException("Naziv tendera ne može biti prazan.");
            }

            if (kriteriji == null)
            {
                throw new ArgumentNullException(nameof(kriteriji));
            }

            kriteriji.ValidirajKriterije();

            Tender tender = new Tender(naziv, opis, rok, kriteriji)
            {
                Id = nextId++
            };

            tenderi.Add(tender);
            return tender;
        }

        public Tender? GetTender(int id)
        {
            var tender = tenderi.FirstOrDefault(t => t.Id == id);

            if (tender == null)
            {
                throw new TenderException($"Tender sa ID {id} ne postoji.");
            }

            return tender;
        }

        public List<Tender> GetSviTenderi()
        {
            return new List<Tender>(tenderi);
        }

        public List<Tender> GetAktivniTenderi()
        {
            return tenderi.Where(t => t.Status == StatusTendera.Otvoren && t.RokZaPrijavu > DateTime.Now).ToList();
        }

        public bool ZatvoriTender(int tenderId)
        {
            var tender = GetTender(tenderId);

            if (tender == null)
            {
                throw new TenderException($"Tender sa ID {tenderId} ne postoji.");
            }

            if (tender.Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije u statusu 'Otvoren' i ne može biti zatvoren.");
            }

            if (tender.Ponude.Count == 0)
            {
                throw new TenderException("Tender ne može biti zatvoren jer nema ponuda.");
            }

            tender.ZatvoriTender();
            return true;
        }
    }
}