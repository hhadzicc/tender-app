using System;
using TenderiApp.Models;

namespace TenderiApp.Tests.Fixtures
{
    public class TenderTestFixture : IDisposable
    {
        public PonderiKriterija DefaultPonderi { get; }
        public KriterijiTendera DefaultKriteriji { get; }
        public Tender DefaultTender { get; }
        public Ponudjac DefaultPonudjac { get; }

        public TenderTestFixture()
        {
            // Setup zajednički za sve testove
            DefaultPonderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);

            DefaultKriteriji = new KriterijiTendera(DefaultPonderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            DefaultTender = new Tender(
                "Test Tender",
                "Opis test tendera",
                DateTime.Now.AddDays(30),
                DefaultKriteriji
            );
            DefaultTender.Id = 1;

            DefaultPonudjac = new Ponudjac(
                "Test Ponudjac d.o.o.",
                "test@test.ba",
                "+387 33 123 456"
            );
            DefaultPonudjac.Id = 1;
        }

        public Tender CreateOpenTender()
        {
            return new Tender(
                "Test Tender",
                "Opis",
                DateTime.Now.AddDays(30),
                DefaultKriteriji
            )
            {
                Id = new Random().Next(1, 1000)
            };
        }

        public void Dispose()
        {
            // Cleanup ako treba
        }
    }
}