using System;

namespace TenderiApp.Views
{
    public class ConsoleView
    {
        public void PrikaziNaslov(string naslov)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║{CentrirajTekst(naslov, 71)}║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        public void PrikaziPoruku(string poruka, TipPoruke tip = TipPoruke.Info)
        {
            switch (tip)
            {
                case TipPoruke.Uspjeh:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ {poruka}");
                    break;
                case TipPoruke.Greska:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {poruka}");
                    break;
                case TipPoruke.Upozorenje:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠ {poruka}");
                    break;
                case TipPoruke.Info:
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"ℹ {poruka}");
                    break;
            }
            Console.ResetColor();
        }

        public void PrikaziLiniju()
        {
            Console.WriteLine("─────────────────────────────────────────────────────────────────────────");
        }

        public void PrikaziDvostrukuLiniju()
        {
            Console.WriteLine("═════════════════════════════════════════════════════════════════════════");
        }

        public string UcitajTekst(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        public int UcitajBroj(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string unos = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(unos, out int rezultat))
                {
                    return rezultat;
                }

                PrikaziPoruku("Neispravan unos! Molimo unesite cijeli broj.", TipPoruke.Greska);
            }
        }

        public double UcitajDecimalniBroj(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string unos = Console.ReadLine() ?? string.Empty;

                if (double.TryParse(unos.Replace(',', '.'), out double rezultat))
                {
                    return rezultat;
                }

                PrikaziPoruku("Neispravan unos! Molimo unesite decimalni broj.", TipPoruke.Greska);
            }
        }

        public DateTime UcitajDatum(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (format: dd.MM.yyyy): ");
                string unos = Console.ReadLine() ?? string.Empty;

                if (DateTime.TryParseExact(unos, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime rezultat))
                {
                    return rezultat;
                }

                PrikaziPoruku("Neispravan format datuma! Koristite format: dd.MM.yyyy", TipPoruke.Greska);
            }
        }

        public bool UcitajPotvrdu(string prompt)
        {
            Console.Write($"{prompt} (D/N): ");
            string unos = Console.ReadLine()?.ToUpper() ?? string.Empty;
            return unos == "D" || unos == "DA";
        }

        public void CekajEnter()
        {
            Console.WriteLine();
            Console.WriteLine("Pritisnite ENTER za nastavak...");
            Console.ReadLine();
        }

        private string CentrirajTekst(string tekst, int sirina)
        {
            if (tekst.Length >= sirina)
            {
                return tekst.Substring(0, sirina);
            }

            int paddingUkupno = sirina - tekst.Length;
            int paddingLijevo = paddingUkupno / 2;
            int paddingDesno = paddingUkupno - paddingLijevo;

            return new string(' ', paddingLijevo) + tekst + new string(' ', paddingDesno);
        }
    }

    public enum TipPoruke
    {
        Info,
        Uspjeh,
        Greska,
        Upozorenje
    }
}