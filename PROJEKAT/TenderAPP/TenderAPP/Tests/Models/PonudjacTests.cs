using System;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Exceptions;
using TenderiApp.Tests.Fixtures;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Tests.Models
{
    [ExcludeFromCodeCoverage]
    public class PonudjacTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public PonudjacTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPodaci_KreiraPonudjaca()
        {
            // Arrange & Act
            var ponudjac = new Ponudjac("Test Firma d.o.o.", "test@firma.ba", "+387 33 123 456");

            // Assert
            Assert.NotNull(ponudjac);
            Assert.Equal("Test Firma d.o.o.", ponudjac.Naziv);
            Assert.Equal("test@firma.ba", ponudjac.Email);
            Assert.Equal("+387 33 123 456", ponudjac.Telefon);
            Assert.Equal(0, ponudjac.BrojPonuda);
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_PrazanNaziv_BacaValidationException(string naziv)
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponudjac(naziv, "test@test.ba", "123")
            );
            Assert.Contains("naziv", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        [InlineData("nemaAt")]
        public void Constructor_NevaliданEmail_BacaValidationException(string email)
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponudjac("Naziv", email, "123")
            );
            Assert.Contains("email", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_PrazanTelefon_BacaValidationException(string telefon)
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponudjac("Naziv", "test@test.ba", telefon)
            );
            Assert.Contains("telefon", exception.Message.ToLower());
        }

        #endregion

        #region KreirajPonudu Tests

        [Fact]
        [Trait("Category", "KreirajPonudu")]
        public void KreirajPonudu_ValidniPodaci_VracaPonudu()
        {
            // Arrange
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var tender = _fixture.CreateOpenTender();

            // Act
            var ponuda = ponudjac.KreirajPonudu(tender, 35000, 45, 24, 85);

            // Assert
            Assert.NotNull(ponuda);
            Assert.Equal(tender.Id, ponuda.TenderId);
            Assert.Equal(ponudjac, ponuda.Ponudjac);
            Assert.Equal(1, ponudjac.BrojPonuda);
        }

        [Fact]
        [Trait("Category", "KreirajPonudu")]
        public void KreirajPonudu_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ponudjac.KreirajPonudu(null, 35000, 45, 24, 85)
            );
        }

        [Fact]
        [Trait("Category", "KreirajPonudu")]
        public void KreirajPonudu_TenderNijeOtvoren_BacaTenderException()
        {
            // Arrange
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var tender = _fixture.CreateOpenTender();

            var p1 = new Ponudjac("P1", "p1@test.ba", "111");
            p1.Id = 99;
            var ponuda1 = new Ponuda(tender, p1, 30000, 40, 18, 80);
            ponuda1.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda1);
            tender.ZatvoriTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                ponudjac.KreirajPonudu(tender, 35000, 45, 24, 85)
            );
            Assert.Contains("nije otvoren", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "KreirajPonudu")]
        public void KreirajPonudu_VisePozivaPodjePovecavaBrojPonuda()
        {
            // Arrange
            var ponudjac = new Ponudjac("Test", "test@test.ba", "123");
            var tender1 = _fixture.CreateOpenTender();
            var tender2 = _fixture.CreateOpenTender();

            // Act
            ponudjac.KreirajPonudu(tender1, 35000, 45, 24, 85);
            ponudjac.KreirajPonudu(tender2, 40000, 50, 18, 90);

            // Assert
            Assert.Equal(2, ponudjac.BrojPonuda);
        }

        #endregion
    }
}