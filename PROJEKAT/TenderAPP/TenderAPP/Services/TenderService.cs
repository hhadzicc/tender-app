using System;
using System.Collections.Generic;
using System.Linq;
using TenderiApp.Interfaces;
using TenderiApp.Models;
using TenderiApp.Exceptions;

namespace TenderiApp.Services
{
    public class TenderService : ITenderService
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

            // INLINE kod iz kriteriji.ValidirajKriterije()
            // kriteriji.ValidirajKriterije(); // Zakomentirano

            // Validacija kriterija - inline kod
            if (kriteriji.MinimalnaCijena < 0)
            {
                throw new ValidationException("Minimalna cijena ne može biti negativna.");
            }

            if (kriteriji.MaksimalnaCijena < kriteriji.MinimalnaCijena)
            {
                throw new ValidationException("Maksimalna cijena ne može biti manja od minimalne cijene.");
            }

            if (kriteriji.MaksimalanRok <= 0)
            {
                throw new ValidationException("Maksimalan rok mora biti pozitivan broj.");
            }

            if (kriteriji.MinimalnaGarancija < 0)
            {
                throw new ValidationException("Minimalna garancija ne može biti negativna.");
            }

            if (kriteriji.MinimalanKvalitet < 0 || kriteriji.MinimalanKvalitet > 100)
            {
                throw new ValidationException("Minimalan kvalitet mora biti između 0 i 100.");
            }

            // INLINE kod iz kriteriji.Ponderi.ValidirajPondere()
            if (kriteriji.Ponderi.PonderCijene < 0 || kriteriji.Ponderi.PonderRoka < 0 ||
                kriteriji.Ponderi.PonderGarancije < 0 || kriteriji.Ponderi.PonderKvaliteta < 0)
            {
                throw new ValidationException("Ponderi ne mogu biti negativni.");
            }

            double ukupno = kriteriji.Ponderi.PonderCijene + kriteriji.Ponderi.PonderRoka +
                            kriteriji.Ponderi.PonderGarancije + kriteriji.Ponderi.PonderKvaliteta;

            if (Math.Abs(ukupno - 1.0) > 0.001)
            {
                throw new ValidationException($"Ukupni zbir pondera mora biti 1.0. Trenutno: {ukupno}");
            }

            if (kriteriji.Ponderi.PonderCijene > 1 || kriteriji.Ponderi.PonderRoka > 1 ||
                kriteriji.Ponderi.PonderGarancije > 1 || kriteriji.Ponderi.PonderKvaliteta > 1)
            {
                throw new ValidationException("Nijedan ponder ne može biti veći od 1.");
            }

            Tender tender = new Tender(naziv, opis, rok, kriteriji)
            {
                Id = nextId++
            };

            tenderi.Add(tender);
            return tender;
        }

        // ZADRŽI pomoćne metode (ne briši!)
        // Metode u KriterijiTendera i PonderiKriterija ostaju nepromijenjene

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