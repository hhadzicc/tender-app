using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Tests.Fixtures;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Tests.Models
{
    [ExcludeFromCodeCoverage]
    public class IzvjestajTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public IzvjestajTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPodaci_KreiraIzvjestaj()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 78,5;
            var ponude = new List<Ponuda> { ponuda };

            // Act
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Assert
            Assert.NotNull(izvjestaj);
            Assert.Equal(tender.Id, izvjestaj.TenderId);
            Assert.Equal(tender.Naziv, izvjestaj.NazivTendera);
            Assert.Equal(1, izvjestaj.BrojPonuda);
            Assert.NotNull(izvjestaj.PobjednickaOonuda);
            Assert.Equal(ponuda, izvjestaj.PobjednickaOonuda);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var ponude = new List<Ponuda>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Izvjestaj(null, ponude)
            );
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_BezPonuda_IzvjestajBezPobjednika()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponude = new List<Ponuda>();

            // Act
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Assert
            Assert.Equal(0, izvjestaj.BrojPonuda);
            Assert.Null(izvjestaj.PobjednickaOonuda);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullPonude_KreiraIzvjestajSaPraznomListom()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();

            // Act
            var izvjestaj = new Izvjestaj(tender, null);

            // Assert
            Assert.Equal(0, izvjestaj.BrojPonuda);
            Assert.Empty(izvjestaj.SvePonude);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_SamoNevalidnePonude_NemaPobjednika()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = false;
            var ponude = new List<Ponuda> { ponuda };

            // Act
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Assert
            Assert.Equal(1, izvjestaj.BrojPonuda);
            Assert.Null(izvjestaj.PobjednickaOonuda);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_VisePonuda_OdredjujePobjednikaSaNajvisimSkorom()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var p1 = new Ponudjac("P1", "p1@test.ba", "111");
            var p2 = new Ponudjac("P2", "p2@test.ba", "222");
            var p3 = new Ponudjac("P3", "p3@test.ba", "333");

            var ponuda1 = new Ponuda(tender, p1, 35000, 45, 24, 85);
            ponuda1.JeValidna = true;
            ponuda1.UkupanSkor = 75.0;

            var ponuda2 = new Ponuda(tender, p2, 30000, 50, 18, 90);
            ponuda2.JeValidna = true;
            ponuda2.UkupanSkor = 82.5; // Najviši

            var ponuda3 = new Ponuda(tender, p3, 40000, 40, 24, 80);
            ponuda3.JeValidna = true;
            ponuda3.UkupanSkor = 70.0;

            var ponude = new List<Ponuda> { ponuda1, ponuda2, ponuda3 };

            // Act
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Assert
            Assert.Equal(ponuda2, izvjestaj.PobjednickaOonuda);
            Assert.Equal(82.5, izvjestaj.PobjednickaOonuda.UkupanSkor);
        }

        #endregion

        #region PrikaziIzvjestaj Tests

        [Fact]
        [Trait("Category", "PrikaziIzvjestaj")]
        public void PrikaziIzvjestaj_VracaFormatiraniString()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Test Firma", "test@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 78.5;
            var ponude = new List<Ponuda> { ponuda };
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Act
            var result = izvjestaj.PrikaziIzvjestaj();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(tender.Naziv, result);
            Assert.Contains("Test Firma", result);
            Assert.Contains("78.5", result);
        }

        [Fact]
        [Trait("Category", "PrikaziIzvjestaj")]
        public void PrikaziIzvjestaj_SaPobjednikom_SadržiPobjednicku()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = new Ponudjac("Pobjednik", "win@test.ba", "123");
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.JeValidna = true;
            ponuda.UkupanSkor = 85.0;
            var ponude = new List<Ponuda> { ponuda };
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Act
            var result = izvjestaj.PrikaziIzvjestaj();

            // Assert
            Assert.Contains("Pobjednik", result);
            Assert.Contains("85", result);
        }

        [Fact]
        [Trait("Category", "PrikaziIzvjestaj")]
        public void PrikaziIzvjestaj_SaNevalidnimPonudama_PrikazujeNevalidne()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var p1 = new Ponudjac("Validna", "v@test.ba", "111");
            var p2 = new Ponudjac("Nevalidna", "n@test.ba", "222");

            var validna = new Ponuda(tender, p1, 35000, 45, 24, 85);
            validna.JeValidna = true;
            validna.UkupanSkor = 78.0;

            var nevalidna = new Ponuda(tender, p2, 60000, 45, 24, 85);
            nevalidna.JeValidna = false;

            var ponude = new List<Ponuda> { validna, nevalidna };
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Act
            var result = izvjestaj.PrikaziIzvjestaj();

            // Assert
            Assert.Contains("Validna", result);
            Assert.Contains("Nevalidna", result);
            Assert.Contains("Ne zadovoljava", result);
        }

        [Fact]
        [Trait("Category", "PrikaziIzvjestaj")]
        public void PrikaziIzvjestaj_BezPonuda_PrikazujePoruku()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponude = new List<Ponuda>();
            var izvjestaj = new Izvjestaj(tender, ponude);

            // Act
            var result = izvjestaj.PrikaziIzvjestaj();

            // Assert
            Assert.Contains(tender.Naziv, result);
            Assert.Contains("0", result);
        }

        #endregion
    }
}