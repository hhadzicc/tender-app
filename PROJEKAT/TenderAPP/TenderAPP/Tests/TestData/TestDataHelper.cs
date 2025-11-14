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
        public static IEnumerable<object[]> GetPonudeFromCsv()
        {
            var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "ponude.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException($"CSV fajl ne postoji: {csvPath}");
            }
            var lines = File.ReadAllLines(csvPath).Skip(1); // Skip header

            foreach (var line in lines)
            {
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
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "kriteriji.json");
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