using System;
using System.Linq;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Services;
using TenderiApp.Exceptions;
using TenderiApp.Tests.Fixtures;

namespace TenderiApp.Tests.Services
{
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
    }
}