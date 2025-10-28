using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TenderiApp.Models;
using TenderiApp.Exceptions;

namespace TenderiApp.Services
{
    public class IzvjestajService
    {
        public IzvjestajService()
        {
        }

        public Izvjestaj GenerisiIzvjestaj(Tender tender)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (tender.Status == StatusTendera.Otvoren)
            {
                throw new TenderException("Ne možete generisati izvještaj za tender koji je još uvijek otvoren.");
            }

            List<Ponuda> ponude = tender.Ponude;

            Izvjestaj izvjestaj = new Izvjestaj(tender, ponude);

            return izvjestaj;
        }

        public string GenerisiDetaljanIzvjestaj(Tender tender)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║              DETALJAN IZVJEŠTAJ O TENDERU                             ║");
            sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();

            sb.AppendLine($"Naziv tendera: {tender.Naziv}");
            sb.AppendLine($"Opis: {tender.Opis}");
            sb.AppendLine($"Datum kreiranja: {tender.DatumKreiranja:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Rok za prijavu: {tender.RokZaPrijavu:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Status: {tender.Status}");
            sb.AppendLine();

            sb.AppendLine("─────────────────────────────────────────────────────────────────────────");
            sb.AppendLine("KRITERIJI TENDERA:");
            sb.AppendLine("─────────────────────────────────────────────────────────────────────────");
            sb.AppendLine($"  Cijena: {tender.Kriteriji.MinimalnaCijena:F2} - {tender.Kriteriji.MaksimalnaCijena:F2} KM");
            sb.AppendLine($"  Maksimalan rok isporuke: {tender.Kriteriji.MaksimalanRok} dana");
            sb.AppendLine($"  Minimalna garancija: {tender.Kriteriji.MinimalnaGarancija} mjeseci");
            sb.AppendLine($"  Minimalan kvalitet: {tender.Kriteriji.MinimalanKvalitet}/100");
            sb.AppendLine();
            sb.AppendLine("  Ponderi:");
            sb.AppendLine($"    - Cijena: {tender.Kriteriji.Ponderi.PonderCijene:P0}");
            sb.AppendLine($"    - Rok: {tender.Kriteriji.Ponderi.PonderRoka:P0}");
            sb.AppendLine($"    - Garancija: {tender.Kriteriji.Ponderi.PonderGarancije:P0}");
            sb.AppendLine($"    - Kvalitet: {tender.Kriteriji.Ponderi.PonderKvaliteta:P0}");
            sb.AppendLine();

            sb.AppendLine("─────────────────────────────────────────────────────────────────────────");
            sb.AppendLine($"PONUDE (Ukupno: {tender.Ponude.Count}):");
            sb.AppendLine("─────────────────────────────────────────────────────────────────────────");

            if (tender.Ponude.Count == 0)
            {
                sb.AppendLine("  Nema prijavljenih ponuda.");
            }
            else
            {
                var validnePonude = tender.Ponude.Where(p => p.JeValidna).OrderByDescending(p => p.UkupanSkor).ToList();
                var nevalidnePonude = tender.Ponude.Where(p => !p.JeValidna).ToList();

                if (validnePonude.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("VALIDNE PONUDE:");
                    sb.AppendLine();

                    for (int i = 0; i < validnePonude.Count; i++)
                    {
                        var ponuda = validnePonude[i];
                        sb.AppendLine($"  [{i + 1}] {ponuda.Ponudjac.Naziv}");
                        sb.AppendLine($"      Cijena: {ponuda.Cijena:F2} KM");
                        sb.AppendLine($"      Rok isporuke: {ponuda.RokIsporuke} dana");
                        sb.AppendLine($"      Garancija: {ponuda.Garancija} mjeseci");
                        sb.AppendLine($"      Kvalitet: {ponuda.Kvalitet}/100");
                        sb.AppendLine($"      Datum prijave: {ponuda.DatumPrijave:dd.MM.yyyy HH:mm}");
                        sb.AppendLine($"      ★ UKUPAN SKOR: {ponuda.UkupanSkor:F2}");

                        if (i == 0)
                        {
                            sb.AppendLine($"      🏆 POBJEDNIČKA PONUDA");
                        }

                        sb.AppendLine();
                    }
                }

                if (nevalidnePonude.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("NEVALIDNE PONUDE:");
                    sb.AppendLine();

                    foreach (var ponuda in nevalidnePonude)
                    {
                        sb.AppendLine($"  ✗ {ponuda.Ponudjac.Naziv}");
                        sb.AppendLine($"      Cijena: {ponuda.Cijena:F2} KM");
                        sb.AppendLine($"      Rok isporuke: {ponuda.RokIsporuke} dana");
                        sb.AppendLine($"      Garancija: {ponuda.Garancija} mjeseci");
                        sb.AppendLine($"      Kvalitet: {ponuda.Kvalitet}/100");
                        sb.AppendLine($"      Razlog: Ne zadovoljava kriterije tendera");
                        sb.AppendLine();
                    }
                }
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        public string GenerisiSazetak(List<Ponuda> ponude)
        {
            if (ponude == null || ponude.Count == 0)
            {
                return "Nema ponuda za prikaz.";
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SAŽETAK PONUDA:");
            sb.AppendLine("─────────────────────────────────────────────────────────────────────────");

            var validnePonude = ponude.Where(p => p.JeValidna).ToList();
            var nevalidnePonude = ponude.Where(p => !p.JeValidna).ToList();

            sb.AppendLine($"Ukupno ponuda: {ponude.Count}");
            sb.AppendLine($"Validne ponude: {validnePonude.Count}");
            sb.AppendLine($"Nevalidne ponude: {nevalidnePonude.Count}");
            sb.AppendLine();

            if (validnePonude.Count > 0)
            {
                double prosjecnaCijena = validnePonude.Average(p => p.Cijena);
                double minCijena = validnePonude.Min(p => p.Cijena);
                double maxCijena = validnePonude.Max(p => p.Cijena);

                sb.AppendLine("STATISTIKA CIJENA:");
                sb.AppendLine($"  Prosječna cijena: {prosjecnaCijena:F2} KM");
                sb.AppendLine($"  Najniža cijena: {minCijena:F2} KM");
                sb.AppendLine($"  Najviša cijena: {maxCijena:F2} KM");
                sb.AppendLine();

                double prosjecanRok = validnePonude.Average(p => p.RokIsporuke);
                int minRok = validnePonude.Min(p => p.RokIsporuke);
                int maxRok = validnePonude.Max(p => p.RokIsporuke);

                sb.AppendLine("STATISTIKA ROKOVA:");
                sb.AppendLine($"  Prosječan rok: {prosjecanRok:F1} dana");
                sb.AppendLine($"  Najkraći rok: {minRok} dana");
                sb.AppendLine($"  Najduži rok: {maxRok} dana");
                sb.AppendLine();

                double prosjecnaGarancija = validnePonude.Average(p => p.Garancija);
                int minGarancija = validnePonude.Min(p => p.Garancija);
                int maxGarancija = validnePonude.Max(p => p.Garancija);

                sb.AppendLine("STATISTIKA GARANCIJA:");
                sb.AppendLine($"  Prosječna garancija: {prosjecnaGarancija:F1} mjeseci");
                sb.AppendLine($"  Najkraća garancija: {minGarancija} mjeseci");
                sb.AppendLine($"  Najduža garancija: {maxGarancija} mjeseci");
            }

            return sb.ToString();
        }
    }
}