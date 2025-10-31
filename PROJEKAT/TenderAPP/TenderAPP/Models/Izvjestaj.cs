using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenderiApp.Models
{
    public class Izvjestaj
    {
        public int TenderId { get; set; }
        public string NazivTendera { get; set; }
        public DateTime DatumGenerisanja { get; set; }
        public int BrojPonuda { get; set; }
        public Ponuda? PobjednickaOonuda { get; set; }
        public List<Ponuda> SvePonude { get; set; }
        public string Sazetak { get; set; }

        public Izvjestaj(Tender tender, List<Ponuda> ponude)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            TenderId = tender.Id;
            NazivTendera = tender.Naziv;
            DatumGenerisanja = DateTime.Now;
            SvePonude = ponude ?? new List<Ponuda>();
            BrojPonuda = SvePonude.Count;

            if (SvePonude.Any(p => p.JeValidna))
            {
                PobjednickaOonuda = SvePonude.Where(p => p.JeValidna).OrderByDescending(p => p.UkupanSkor).FirstOrDefault();
            }

            Sazetak = GenerisiSazetak();
        }

        private string GenerisiSazetak()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Izvještaj za tender: {NazivTendera}");
            sb.AppendLine($"Datum generisanja: {DatumGenerisanja:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Ukupno ponuda: {BrojPonuda}");
            sb.AppendLine($"Validne ponude: {SvePonude.Count(p => p.JeValidna)}");

            if (PobjednickaOonuda != null)
            {
                sb.AppendLine($"Pobjednička ponuda: {PobjednickaOonuda.Ponudjac.Naziv} (Skor: {PobjednickaOonuda.UkupanSkor:F2})");
            }
            else
            {
                sb.AppendLine("Nema pobjedničke ponude.");
            }

            return sb.ToString();
        }

        public string PrikaziIzvjestaj()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine(Sazetak);
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine();

            if (SvePonude.Count > 0)
            {
                sb.AppendLine("Sve ponude:");
                sb.AppendLine("-".PadRight(60, '-'));

                var rangiranjePonude = SvePonude.Where(p => p.JeValidna).OrderByDescending(p => p.UkupanSkor).ToList();

                for (int i = 0; i < rangiranjePonude.Count; i++)
                {
                    var ponuda = rangiranjePonude[i];
                    sb.AppendLine($"{i + 1}. {ponuda.Ponudjac.Naziv}");
                    sb.AppendLine($"   Cijena: {ponuda.Cijena:F2} KM");
                    sb.AppendLine($"   Rok: {ponuda.RokIsporuke} dana");
                    sb.AppendLine($"   Garancija: {ponuda.Garancija} mjeseci");
                    sb.AppendLine($"   Kvalitet: {ponuda.Kvalitet}/100");
                    sb.AppendLine($"   Ukupan skor: {ponuda.UkupanSkor:F2}");
                    sb.AppendLine();
                }

                var nevalidnePonude = SvePonude.Where(p => !p.JeValidna).ToList();
                if (nevalidnePonude.Count > 0)
                {
                    sb.AppendLine("Nevalidne ponude:");
                    foreach (var ponuda in nevalidnePonude)
                    {
                        sb.AppendLine($"- {ponuda.Ponudjac.Naziv} (Ne zadovoljava kriterije)");
                    }
                }
            }

            return sb.ToString();
        }
    }
}