using System;
using System.Collections.Generic;
using System.Linq;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class Tender
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime RokZaPrijavu { get; set; }
        public KriterijiTendera Kriteriji { get; set; }
        public List<Ponuda> Ponude { get; set; }
        public StatusTendera Status { get; set; }

        public Tender(string naziv, string opis, DateTime rok, KriterijiTendera kriteriji)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                throw new ValidationException("Naziv tendera ne može biti prazan.");
            }

            if (string.IsNullOrWhiteSpace(opis))
            {
                throw new ValidationException("Opis tendera ne može biti prazan.");
            }

            if (rok <= DateTime.Now)
            {
                throw new ValidationException("Rok za prijavu mora biti u budućnosti.");
            }

            if (kriteriji == null)
            {
                throw new ArgumentNullException(nameof(kriteriji));
            }

            kriteriji.ValidirajKriterije();

            Naziv = naziv;
            Opis = opis;
            DatumKreiranja = DateTime.Now;
            RokZaPrijavu = rok;
            Kriteriji = kriteriji;
            Ponude = new List<Ponuda>();
            Status = StatusTendera.Otvoren;
        }

        public bool DodajPonudu(Ponuda ponuda)
        {
            if (ponuda == null)
            {
                throw new ArgumentNullException(nameof(ponuda));
            }

            if (Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije otvoren za nove ponude.");
            }

            if (DateTime.Now > RokZaPrijavu)
            {
                throw new TenderException("Rok za prijavu je istekao.");
            }

            if (Ponude.Any(p => p.Ponudjac.Id == ponuda.Ponudjac.Id))
            {
                throw new TenderException("Ponuđač je već prijavio ponudu za ovaj tender.");
            }

            ponuda.Validiraj(Kriteriji);

            Ponude.Add(ponuda);
            return true;
        }

        public void ZatvoriTender()
        {
            if (Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije u statusu 'Otvoren'.");
            }

            if (Ponude.Count == 0)
            {
                throw new TenderException("Ne možete zatvoriti tender bez ponuda.");
            }

            Status = StatusTendera.Zatvoren;
        }

        public List<Ponuda> GetValidnePonude()
        {
            return Ponude.Where(p => p.JeValidna).ToList();
        }

        public Tender()
        {
            // Potreban za JSON deserializaciju
        }

    }
}