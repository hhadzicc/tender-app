using System;
using System.Linq;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Services;
using TenderiApp.Exceptions;
using TenderiApp.Tests.Fixtures;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Tests.Services
{
    [ExcludeFromCodeCoverage]
    public class EvaluacijaServiceTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public EvaluacijaServiceTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region IzracunajSkor Tests - Složeni algoritam

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_MinimalnaCijena_VracaMaksimalanSkorCijene()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(1.0, 0, 0, 0); // Samo cijena
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 10000, 45, 24, 85); // MIN cijena
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            Assert.Equal(100, skor, 2); // Delta 0.01 - floating point preciznost
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_MaksimalnaCijena_VracaMinimalanSkorCijene()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(1.0, 0, 0, 0); // Samo cijena
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 50000, 45, 24, 85); // MAX cijena
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            Assert.Equal(0, skor, 2);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_SrednjaCijena_VracaSrednjiSkor()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(1.0, 0, 0, 0); // Samo cijena
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 30000, 45, 24, 85); // SREDNJA cijena
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            // (30000 - 10000) / (50000 - 10000) = 20000 / 40000 = 0.5
            // Inverzija: 1 - 0.5 = 0.5
            // Skaliranje: 0.5 * 100 = 50
            Assert.Equal(50, skor, 2);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_MinimalanRok_VracaMaksimalanSkorRoka()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(0, 1.0, 0, 0); // Samo rok
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 1, 24, 85); // MIN rok (1 dan)
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            // 1 / 90 = 0.011...
            // 1 - 0.011 = 0.989
            // 0.989 * 100 = 98.9
            Assert.True(skor > 98 && skor < 100);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_DuplaMinimalnaGarancija_VracaMaksimalanSkorGarancije()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(0, 0, 1.0, 0); // Samo garancija
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85); // 2x minimalna garancija
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            // 24 / 12 = 2.0 (capped at 2.0)
            // 2.0 * 50 = 100
            Assert.Equal(100, skor, 2);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        [Trait("Complexity", "High")]
        public void IzracunajSkor_WeightedScoring_KombinujeSveKriterije()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            var ponudjac = _fixture.DefaultPonudjac;

            // Srednje vrijednosti za sve kriterije
            var ponuda = new Ponuda(tender, ponudjac, 30000, 45, 24, 80);
            ponuda.JeValidna = true;

            // Act
            var skor = service.IzracunajSkor(ponuda, kriteriji);

            // Assert
            // Skor cijene: (1 - (30000-10000)/(50000-10000)) * 100 = 50
            // Skor roka: (1 - 45/90) * 100 = 50
            // Skor garancije: (24/12) * 50 = 100 (capped)
            // Skor kvaliteta: 80
            // Ukupno: 50*0.4 + 50*0.3 + 100*0.15 + 80*0.15 = 20 + 15 + 15 + 12 = 62
            Assert.True(skor > 60 && skor < 65);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_NevalidnaPonuda_VracaNula()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = false;

            // Act
            var skor = service.IzracunajSkor(ponuda, tender.Kriteriji);

            // Assert
            Assert.Equal(0, skor);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_NullPonuda_BacaArgumentNullException()
        {
            // Arrange
            var service = new EvaluacijaService();
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.IzracunajSkor(null, kriteriji)
            );
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_NullKriteriji_BacaArgumentNullException()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.IzracunajSkor(ponuda, null)
            );
        }

        #endregion

        #region EvaluirajPonude Tests

        [Fact]
        [Trait("Category", "EvaluirajPonude")]
        public void EvaluirajPonude_SaValidnimPonudama_PostavljaSkoroveIVracaRangirane()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");

            var ponuda1 = new Ponuda(tender, ponudjac1, 35000, 45, 24, 85);
            var ponuda2 = new Ponuda(tender, ponudjac2, 28000, 60, 12, 75);
            var ponuda3 = new Ponuda(tender, ponudjac3, 42000, 30, 18, 90);

            ponuda1.Validiraj(tender.Kriteriji);
            ponuda2.Validiraj(tender.Kriteriji);
            ponuda3.Validiraj(tender.Kriteriji);

            tender.Ponude.Add(ponuda1);
            tender.Ponude.Add(ponuda2);
            tender.Ponude.Add(ponuda3);

            tender.ZatvoriTender();

            // Act
            var rangirane = service.EvaluirajPonude(tender);

            // Assert
            Assert.Equal(3, rangirane.Count);
            Assert.All(rangirane, p => Assert.True(p.UkupanSkor > 0));

            // Provjeri da su rangirane od najvećeg ka najmanjem skoru
            for (int i = 0; i < rangirane.Count - 1; i++)
            {
                Assert.True(rangirane[i].UkupanSkor >= rangirane[i + 1].UkupanSkor);
            }

            Assert.Equal(StatusTendera.Evaluiran, tender.Status);
        }

        [Fact]
        [Trait("Category", "EvaluirajPonude")]
        public void EvaluirajPonude_BezValidnihPonuda_BacaTenderException()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85); // Nevalidna
            ponuda.Validiraj(tender.Kriteriji);

            tender.Ponude.Add(ponuda);
            tender.ZatvoriTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.EvaluirajPonude(tender)
            );
            Assert.Contains("validnih ponuda", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "EvaluirajPonude")]
        public void EvaluirajPonude_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var service = new EvaluacijaService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.EvaluirajPonude(null)
            );
        }

        #endregion

        #region OdrediPobjednika Tests

        [Fact]
        [Trait("Category", "OdrediPobjednika")]
        public void OdrediPobjednika_VracaPonuduSaNajvisimSkorom()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");

            var ponuda1 = new Ponuda(tender, ponudjac1, 35000, 45, 24, 85);
            var ponuda2 = new Ponuda(tender, ponudjac2, 28000, 60, 12, 75);
            var ponuda3 = new Ponuda(tender, ponudjac3, 42000, 30, 18, 90);

            ponuda1.Validiraj(tender.Kriteriji);
            ponuda2.Validiraj(tender.Kriteriji);
            ponuda3.Validiraj(tender.Kriteriji);

            tender.Ponude.Add(ponuda1);
            tender.Ponude.Add(ponuda2);
            tender.Ponude.Add(ponuda3);

            tender.ZatvoriTender();
            service.EvaluirajPonude(tender);

            // Act
            var pobjednik = service.OdrediPobjednika(tender);

            // Assert
            Assert.NotNull(pobjednik);
            var maksimalanSkor = tender.GetValidnePonude().Max(p => p.UkupanSkor);
            Assert.Equal(maksimalanSkor, pobjednik.UkupanSkor);
            Assert.Equal(StatusTendera.Zavrsen, tender.Status);
        }

        [Fact]
        [Trait("Category", "OdrediPobjednika")]
        public void OdrediPobjednika_TenderNijeEvaluiran_BacaTenderException()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.OdrediPobjednika(tender)
            );
            Assert.Contains("evaluiran", exception.Message.ToLower());
        }

        #endregion

        #region RangirajPonude Tests

        [Fact]
        [Trait("Category", "RangirajPonude")]
        public void RangirajPonude_RazlicitiSkorovi_VracaRangiranuListu()
        {
            // Arrange
            var service = new EvaluacijaService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");

            var ponuda1 = new Ponuda(tender, ponudjac1, 35000, 45, 24, 85);
            ponuda1.UkupanSkor = 75.5;

            var ponuda2 = new Ponuda(tender, ponudjac2, 28000, 60, 12, 75);
            ponuda2.UkupanSkor = 82.3;

            var ponuda3 = new Ponuda(tender, ponudjac3, 42000, 30, 18, 90);
            ponuda3.UkupanSkor = 68.7;

            var ponude = new System.Collections.Generic.List<Ponuda> { ponuda1, ponuda2, ponuda3 };

            // Act
            var rangirane = service.RangirajPonude(ponude);

            // Assert
            Assert.Equal(3, rangirane.Count);
            Assert.Equal(82.3, rangirane[0].UkupanSkor);
            Assert.Equal(75.5, rangirane[1].UkupanSkor);
            Assert.Equal(68.7, rangirane[2].UkupanSkor);
        }

        [Fact]
        [Trait("Category", "RangirajPonude")]
        public void RangirajPonude_PraznaLista_VracaPraznuListu()
        {
            // Arrange
            var service = new EvaluacijaService();
            var ponude = new System.Collections.Generic.List<Ponuda>();

            // Act
            var rangirane = service.RangirajPonude(ponude);

            // Assert
            Assert.NotNull(rangirane);
            Assert.Empty(rangirane);
        }

        [Fact]
        [Trait("Category", "RangirajPonude")]
        public void RangirajPonude_NullLista_VracaPraznuListu()
        {
            // Arrange
            var service = new EvaluacijaService();

            // Act
            var rangirane = service.RangirajPonude(null);

            // Assert
            Assert.NotNull(rangirane);
            Assert.Empty(rangirane);
        }

        #endregion
    }
}