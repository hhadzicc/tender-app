using System;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class KriterijiTendera
    {
        public double MinimalnaCijena { get; set; }
        public double MaksimalnaCijena { get; set; }
        public int MaksimalanRok { get; set; }
        public int MinimalnaGarancija { get; set; }
        public int MinimalanKvalitet { get; set; }
        public PonderiKriterija Ponderi { get; set; }

        public KriterijiTendera(PonderiKriterija ponderi)
        {
            Ponderi = ponderi ?? throw new ArgumentNullException(nameof(ponderi));
            MinimalnaCijena = 0;
            MaksimalnaCijena = double.MaxValue;
            MaksimalanRok = int.MaxValue;
            MinimalnaGarancija = 0;
            MinimalanKvalitet = 0;
        }

        public bool ValidirajKriterije()
        {
            if (MinimalnaCijena < 0)
            {
                throw new ValidationException("Minimalna cijena ne može biti negativna.");
            }

            if (MaksimalnaCijena < MinimalnaCijena)
            {
                throw new ValidationException("Maksimalna cijena ne može biti manja od minimalne cijene.");
            }

            if (MaksimalanRok <= 0)
            {
                throw new ValidationException("Maksimalan rok mora biti pozitivan broj.");
            }

            if (MinimalnaGarancija < 0)
            {
                throw new ValidationException("Minimalna garancija ne može biti negativna.");
            }

            if (MinimalanKvalitet < 0 || MinimalanKvalitet > 100)
            {
                throw new ValidationException("Minimalan kvalitet mora biti između 0 i 100.");
            }

            Ponderi.ValidirajPondere();

            return true;
        }
    }
}