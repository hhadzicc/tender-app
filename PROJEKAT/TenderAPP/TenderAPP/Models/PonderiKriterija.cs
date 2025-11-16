using System;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class PonderiKriterija
    {
        public double PonderCijene { get; set; }
        public double PonderRoka { get; set; }
        public double PonderGarancije { get; set; }
        public double PonderKvaliteta { get; set; }

        public PonderiKriterija(double cijena, double rok, double garancija, double kvalitet)
        {
            PonderCijene = cijena;
            PonderRoka = rok;
            PonderGarancije = garancija;
            PonderKvaliteta = kvalitet;
        }

        public bool ValidirajPondere()
        {
            if (PonderCijene < 0 || PonderRoka < 0 || PonderGarancije < 0 || PonderKvaliteta < 0)
            {
                throw new ValidationException("Ponderi ne mogu biti negativni.");
            }

            double ukupno = UkupniPonder();

            if (Math.Abs(ukupno - 1.0) > 0.001)
            {
                throw new ValidationException($"Ukupni zbir pondera mora biti 1.0. Trenutno: {ukupno}");
            }

            if (PonderCijene > 1 || PonderRoka > 1 || PonderGarancije > 1 || PonderKvaliteta > 1)
            {
                throw new ValidationException("Nijedan ponder ne može biti veći od 1.");
            }

            return true;
        }

        public double UkupniPonder()
        {
            return PonderCijene + PonderRoka + PonderGarancije + PonderKvaliteta;
        }

        public PonderiKriterija()
        {
            // potreban za JSON deserializaciju
        }
    }
}