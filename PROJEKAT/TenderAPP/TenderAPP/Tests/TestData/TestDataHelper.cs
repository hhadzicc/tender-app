using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TenderiApp.Tests.TestData
{
    [ExcludeFromCodeCoverage]
    public static class TestDataHelper
    {
        private static string FindFile(string fileName)
        {
            // 1) prvo probaj output\TestData (ako je postavljen Copy to Output)
            var direct = Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
            if (File.Exists(direct)) return direct;

            // 2) penji se kroz roditelje i traži nekoliko tipičnih rasporeda
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            for (var up = 0; dir != null && up < 8; up++, dir = dir.Parent)
            {
                string Try(string[] parts) =>
                    Path.Combine(new[] { dir.FullName }.Concat(parts).ToArray());

                var candidates = new[]
                {
            Try(new[] { "TestData", fileName }),
            Try(new[] { "Tests", "TestData", fileName }),
            Try(new[] { "TenderAPP", "Tests", "TestData", fileName }),
            Try(new[] { "TenderAPP", "TenderAPP", "Tests", "TestData", fileName }) // ⟵ TVOJA STVARNOST
        };

                foreach (var p in candidates)
                    if (File.Exists(p)) return p;
            }

            throw new FileNotFoundException(
                $"Fajl '{fileName}' nije pronađen; start: {AppContext.BaseDirectory}");
        }


        public static IEnumerable<object[]> GetPonudeFromCsv()
        {
            var csvPath = FindFile("ponude.csv");
            var lines = File.ReadAllLines(csvPath).Skip(1); // Skip header

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                yield return new object[]
                {
                    double.Parse(values[0]),  // Cijena
                    int.Parse(values[1]),     // RokIsporuke
                    int.Parse(values[2]),     // Garancija
                    int.Parse(values[3]),     // Kvalitet
                    bool.Parse(values[4])     // OcekivanaValidnost
                };
            }
        }

        public static IEnumerable<object[]> GetKriterijiFromJson()
        {
            var jsonPath = FindFile("kriteriji.json");
            var json = File.ReadAllText(jsonPath);
            var kriteriji = JsonConvert.DeserializeObject<List<KriterijiDto>>(json);

            foreach (var k in kriteriji)
            {
                yield return new object[]
                {
                    k.MinimalnaCijena,
                    k.MaksimalnaCijena,
                    k.MaksimalanRok,
                    k.MinimalnaGarancija,
                    k.MinimalanKvalitet,
                    k.PonderCijene,
                    k.PonderRoka,
                    k.PonderGarancije,
                    k.PonderKvaliteta
                };
            }
        }

        private class KriterijiDto
        {
            public double MinimalnaCijena { get; set; }
            public double MaksimalnaCijena { get; set; }
            public int MaksimalanRok { get; set; }
            public int MinimalnaGarancija { get; set; }
            public int MinimalanKvalitet { get; set; }
            public double PonderCijene { get; set; }
            public double PonderRoka { get; set; }
            public double PonderGarancije { get; set; }
            public double PonderKvaliteta { get; set; }
        }
    }
}