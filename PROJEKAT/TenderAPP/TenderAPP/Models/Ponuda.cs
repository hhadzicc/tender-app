using System;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class Ponuda
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public Ponudjac Ponudjac { get; set; }
        public double Cijena { get; set; }
        public int RokIsporuke { get; set; }
        public int Garancija { get; set; }
        public int Kvalitet { get; set; }
        public DateTime DatumPrijave { get; set; }
        public double UkupanSkor { get; set; }
        public bool JeValidna { get; set; }
        public Ponuda()
        {
            // potreban za JSON deserializaciju
        }

        public Ponuda(Tender tender, Ponudjac ponudjac, double cijena, int rok, int garancija, int kvalitet)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (ponudjac == null)
            {
                throw new ArgumentNullException(nameof(ponudjac));
            }

            if (cijena <= 0)
            {
                throw new ValidationException("Cijena mora biti pozitivna.");
            }

            if (rok <= 0)
            {
                throw new ValidationException("Rok isporuke mora biti pozitivan.");
            }

            if (garancija < 0)
            {
                throw new ValidationException("Garancija ne može biti negativna.");
            }

            if (kvalitet < 0 || kvalitet > 100)
            {
                throw new ValidationException("Kvalitet mora biti između 0 i 100.");
            }

            TenderId = tender.Id;
            Ponudjac = ponudjac;
            Cijena = cijena;
            RokIsporuke = rok;
            Garancija = garancija;
            Kvalitet = kvalitet;
            DatumPrijave = DateTime.Now;
            UkupanSkor = 0;
            JeValidna = false;
        }

        public double IzracunajSkor(KriterijiTendera kriteriji)
        {
            if (kriteriji == null)
                throw new ArgumentNullException(nameof(kriteriji));

            if (!JeValidna)
            {
                UkupanSkor = 0;
                return 0;
            }

            double rasponCijene = kriteriji.MaksimalnaCijena - kriteriji.MinimalnaCijena;
            double skorCijene = rasponCijene > 0
                ? (1 - Math.Clamp((Cijena - kriteriji.MinimalnaCijena) / rasponCijene, 0, 1)) * 100
                : 100;
            double skorRoka = kriteriji.MaksimalanRok > 0
                ? (1 - Math.Clamp((double)RokIsporuke / kriteriji.MaksimalanRok, 0, 1)) * 100
                : 100;
            double skorGarancije = kriteriji.MinimalnaGarancija > 0
                ? Math.Min((double)Garancija / kriteriji.MinimalnaGarancija, 2.0) * 50
                : (Garancija > 0 ? 100 : 0);
            double skorKvaliteta = Math.Clamp(Kvalitet, 0, 100);

            UkupanSkor = (skorCijene * kriteriji.Ponderi.PonderCijene) +
                        (skorRoka * kriteriji.Ponderi.PonderRoka) +
                        (skorGarancije * kriteriji.Ponderi.PonderGarancije) +
                        (skorKvaliteta * kriteriji.Ponderi.PonderKvaliteta);

            return UkupanSkor;
        }

        public bool Validiraj(KriterijiTendera kriteriji)
        {
            if (kriteriji == null)
            {
                throw new ArgumentNullException(nameof(kriteriji));
            }

            bool validna = true;

            if (Cijena < kriteriji.MinimalnaCijena || Cijena > kriteriji.MaksimalnaCijena)
            {
                validna = false;
            }

            if (RokIsporuke > kriteriji.MaksimalanRok)
            {
                validna = false;
            }

            if (Garancija < kriteriji.MinimalnaGarancija)
            {
                validna = false;
            }

            if (Kvalitet < kriteriji.MinimalanKvalitet)
            {
                validna = false;
            }

            JeValidna = validna;
            return validna;
        }
    }
}