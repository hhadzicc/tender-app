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
    public class IzvjestajServiceTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public IzvjestajServiceTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region GenerisiIzvjestaj Tests

        [Fact]
        [Trait("Category", "GenerisiIzvjestaj")]
        public void GenerisiIzvjestaj_EvaluiranTender_VracaIzvjestajSaPobjednikom()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            ponuda.UkupanSkor = 78.5;

            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Evaluiran;

            // Act
            var izvjestaj = service.GenerisiIzvjestaj(tender);

            // Assert
            Assert.NotNull(izvjestaj);
            Assert.Equal(tender.Id, izvjestaj.TenderId);
            Assert.Equal(tender.Naziv, izvjestaj.NazivTendera);
            Assert.Equal(1, izvjestaj.BrojPonuda);
            Assert.NotNull(izvjestaj.PobjednickaOonuda);
        }

        [Fact]
        [Trait("Category", "GenerisiIzvjestaj")]
        public void GenerisiIzvjestaj_OtvorenTender_BacaTenderException()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.GenerisiIzvjestaj(tender)
            );
            Assert.Contains("otvoren", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "GenerisiIzvjestaj")]
        public void GenerisiIzvjestaj_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var service = new IzvjestajService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.GenerisiIzvjestaj(null)
            );
        }

        #endregion

        #region GenerisiDetaljanIzvjestaj Tests

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_SaPonudama_VracaFormatiranString()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            ponuda.UkupanSkor = 78.5;

            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var izvjestaj = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.NotNull(izvjestaj);
            Assert.Contains("DETALJAN IZVJEŠTAJ", izvjestaj);
            Assert.Contains(tender.Naziv, izvjestaj);
            Assert.Contains(ponudjac.Naziv, izvjestaj);
            Assert.Contains("35000", izvjestaj);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var service = new IzvjestajService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.GenerisiDetaljanIzvjestaj(null)
            );
        }

        #endregion

        #region GenerisiSazetak Tests

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_SaValidnimPonudama_VracaStatistiku()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");

            var ponuda1 = new Ponuda(tender, ponudjac1, 30000, 40, 18, 80);
            ponuda1.JeValidna = true;

            var ponuda2 = new Ponuda(tender, ponudjac2, 40000, 50, 24, 85);
            ponuda2.JeValidna = true;

            var ponude = new System.Collections.Generic.List<Ponuda> { ponuda1, ponuda2 };

            // Act
            var sazetak = service.GenerisiSazetak(ponude);

            // Assert
            Assert.NotNull(sazetak);
            Assert.Contains("Ukupno ponuda: 2", sazetak);
            Assert.Contains("Validne ponude: 2", sazetak);
            Assert.Contains("STATISTIKA CIJENA", sazetak);
            Assert.Contains("35000", sazetak); // Prosječna cijena
        }

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_PraznaLista_VracaPorukuONemaPonuda()
        {
            // Arrange
            var service = new IzvjestajService();
            var ponude = new System.Collections.Generic.List<Ponuda>();

            // Act
            var sazetak = service.GenerisiSazetak(ponude);

            // Assert
            Assert.NotNull(sazetak);
            Assert.Contains("Nema ponuda", sazetak);
        }

        #endregion

        #region Additional Coverage Tests

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_RazlicitiStatusTendera_PrikazujeStatus()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Zatvoren", result);
            Assert.Contains("Status:", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_StatusEvaluiran_PrikazujeEvaluiran()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            tender.Status = StatusTendera.Evaluiran;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Evaluiran", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_StatusZavrsen_PrikazujeZavrsen()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            tender.Status = StatusTendera.Zavrsen;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Zavrsen", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_MaliRasponCijene_PrikazujeUzakRaspon()
        {
            // Arrange
            var service = new IzvjestajService();
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 15000, // Raspon = 5000 < 10000
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };
            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            tender.Id = 1;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Uzak raspon", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_SrednjiRasponCijene_PrikazujeSrednjiRaspon()
        {
            // Arrange
            var service = new IzvjestajService();
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 40000, // Raspon = 30000 (10k-50k)
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };
            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            tender.Id = 1;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Srednji raspon", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_VelikiRasponCijene_PrikazujeSirokRaspon()
        {
            // Arrange
            var service = new IzvjestajService();
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.15, 0.15);
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 100000, // Raspon = 90000 > 50000
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };
            var tender = new Tender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji);
            tender.Id = 1;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("irok raspon", result); // Širok
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_SkorVrloDobar_PrikazujeVrloDobar()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 80.0; // 75-90
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Vrlo dobar", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_SkorDobar_PrikazujeDobar()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 65.0; // 60-75
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Dobar", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_SkorPrihvatljiv_PrikazujePrihvatljiv()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 50.0; // < 60
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Prihvatljiv", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_NevalidnaPonudaCijenaVanRaspona_PrikazujeRazlog()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85); // Previsoka cijena
            ponuda.JeValidna = false;
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Cijena van dozvoljenog raspona", result);
        }



        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_NevalidnaPonudaGarancijaIspodMinimuma_PrikazujeRazlog()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 6, 85); // Garancija < 12
            ponuda.JeValidna = false;
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Garancija ispod minimuma", result);
        }

        [Fact]
        [Trait("Category", "GenerisiDetaljanIzvjestaj")]
        public void GenerisiDetaljanIzvjestaj_NevalidnaPonudaKvalitetIspodMinimuma_PrikazujeRazlog()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 50); // Kvalitet < 60
            ponuda.JeValidna = false;
            tender.Ponude.Add(ponuda);
            tender.Status = StatusTendera.Zatvoren;

            // Act
            var result = service.GenerisiDetaljanIzvjestaj(tender);

            // Assert
            Assert.Contains("Kvalitet ispod minimuma", result);
        }

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_SamoNevalidnePonude_NePrikazujeStatistiku()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85);
            ponuda.JeValidna = false;
            var ponude = new List<Ponuda> { ponuda };

            // Act
            var result = service.GenerisiSazetak(ponude);

            // Assert
            Assert.Contains("Ukupno ponuda: 1", result);
            Assert.Contains("Validne ponude: 0", result);
            Assert.DoesNotContain("STATISTIKA CIJENA", result);
        }

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_VelikiRasponCijena_TacnaProsjecnaVrijednost()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var p1 = new Ponudjac("P1", "p1@test.ba", "111");
            var p2 = new Ponudjac("P2", "p2@test.ba", "222");
            var p3 = new Ponudjac("P3", "p3@test.ba", "333");

            var ponuda1 = new Ponuda(tender, p1, 20000, 40, 18, 80);
            ponuda1.JeValidna = true;

            var ponuda2 = new Ponuda(tender, p2, 40000, 50, 24, 85);
            ponuda2.JeValidna = true;

            var ponuda3 = new Ponuda(tender, p3, 30000, 60, 12, 75);
            ponuda3.JeValidna = true;

            var ponude = new List<Ponuda> { ponuda1, ponuda2, ponuda3 };

            // Act
            var result = service.GenerisiSazetak(ponude);

            // Assert
            Assert.Contains("30000", result); // Prosječna cijena = (20000+40000+30000)/3 = 30000
            Assert.Contains("20000", result); // Min cijena
            Assert.Contains("40000", result); // Max cijena
        }

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_VelikiRasponRokova_TacnaProsjecnaVrijednost()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var p1 = new Ponudjac("P1", "p1@test.ba", "111");
            var p2 = new Ponudjac("P2", "p2@test.ba", "222");

            var ponuda1 = new Ponuda(tender, p1, 35000, 30, 18, 80);
            ponuda1.JeValidna = true;

            var ponuda2 = new Ponuda(tender, p2, 40000, 60, 24, 85);
            ponuda2.JeValidna = true;

            var ponude = new List<Ponuda> { ponuda1, ponuda2 };

            // Act
            var result = service.GenerisiSazetak(ponude);

            // Assert
            Assert.Contains("STATISTIKA ROKOVA", result);
            Assert.Contains("45", result); // Prosječan rok = (30+60)/2 = 45
            Assert.Contains("30", result); // Min rok
            Assert.Contains("60", result); // Max rok
        }

        [Fact]
        [Trait("Category", "GenerisiSazetak")]
        public void GenerisiSazetak_VelikiRasponGarancija_TacnaProsjecnaVrijednost()
        {
            // Arrange
            var service = new IzvjestajService();
            var tender = _fixture.CreateOpenTender();
            var p1 = new Ponudjac("P1", "p1@test.ba", "111");
            var p2 = new Ponudjac("P2", "p2@test.ba", "222");

            var ponuda1 = new Ponuda(tender, p1, 35000, 40, 12, 80);
            ponuda1.JeValidna = true;

            var ponuda2 = new Ponuda(tender, p2, 40000, 50, 36, 85);
            ponuda2.JeValidna = true;

            var ponude = new List<Ponuda> { ponuda1, ponuda2 };

            // Act
            var result = service.GenerisiSazetak(ponude);

            // Assert
            Assert.Contains("STATISTIKA GARANCIJA", result);
            Assert.Contains("24", result); // Prosječna garancija = (12+36)/2 = 24
            Assert.Contains("12", result); // Min garancija
            Assert.Contains("36", result); // Max garancija
        }

        #endregion
    }
}