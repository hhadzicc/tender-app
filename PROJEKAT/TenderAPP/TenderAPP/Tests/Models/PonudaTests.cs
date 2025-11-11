using System;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Exceptions;
using TenderiApp.Tests.Fixtures;
using TenderiApp.Tests.TestData;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Tests.Models
{
    [ExcludeFromCodeCoverage]
    public class PonudaTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public PonudaTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPodaci_KreiraPonudu()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;

            // Act
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Assert
            Assert.NotNull(ponuda);
            Assert.Equal(tender.Id, ponuda.TenderId);
            Assert.Equal(ponudjac, ponuda.Ponudjac);
            Assert.Equal(35000, ponuda.Cijena);
            Assert.Equal(45, ponuda.RokIsporuke);
            Assert.Equal(24, ponuda.Garancija);
            Assert.Equal(85, ponuda.Kvalitet);
            Assert.False(ponuda.JeValidna); // Default je false dok se ne validira
            Assert.Equal(0, ponuda.UkupanSkor);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Ponuda(null, ponudjac, 35000, 45, 24, 85)
            );
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullPonudjac_BacaArgumentNullException()
        {
            // Arrange
            var tender = _fixture.DefaultTender;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Ponuda(tender, null, 35000, 45, 24, 85)
            );
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(0)]
        [InlineData(-100)]
        [InlineData(-0.01)]
        public void Constructor_NevaliднaCijena_BacaValidationException(double cijena)
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponuda(tender, ponudjac, cijena, 45, 24, 85)
            );
            Assert.Contains("pozitivna", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(0)]
        [InlineData(-10)]
        public void Constructor_NevaliданRok_BacaValidationException(int rok)
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponuda(tender, ponudjac, 35000, rok, 24, 85)
            );
            Assert.Contains("pozitivan", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NegativnaGarancija_BacaValidationException()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponuda(tender, ponudjac, 35000, 45, -12, 85)
            );
            Assert.Contains("negativna", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(-1)]
        [InlineData(101)]
        [InlineData(150)]
        public void Constructor_KvalitetVanRaspona_BacaValidationException(int kvalitet)
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Ponuda(tender, ponudjac, 35000, 45, 24, kvalitet)
            );
            Assert.Contains("između", exception.Message.ToLower());
        }

        #endregion

        #region Validiraj Tests - DATA DRIVEN (CSV)

        

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_ValidnaPonuda_VracaTrue()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.True(result);
            Assert.True(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_CijenaPrekoracujeMaksimum_VracaFalse()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85); // Max je 50000

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_CijenaIspodMinimuma_VracaFalse()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 5000, 45, 24, 85); // Min je 10000

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_RokPrekoracujeMaksimum_VracaFalse()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 100, 24, 85); // Max rok je 90

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_GarancijaIspodMinimuma_VracaFalse()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 6, 85); // Min garancija je 12

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_KvalitetIspodMinimuma_VracaFalse()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 50); // Min kvalitet je 60

            // Act
            var result = ponuda.Validiraj(tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void Validiraj_NullKriteriji_BacaArgumentNullException()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ponuda.Validiraj(null)
            );
        }

        #endregion

        #region IzracunajSkor Tests

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_ValidnaPonuda_PostavljaSkor()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);

            // Act
            var skor = ponuda.IzracunajSkor(tender.Kriteriji);

            // Assert
            Assert.True(skor > 0);
            Assert.Equal(skor, ponuda.UkupanSkor);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_NevalidnaPonuda_VracaNula()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85); // Nevalidna
            ponuda.Validiraj(tender.Kriteriji);

            // Act
            var skor = ponuda.IzracunajSkor(tender.Kriteriji);

            // Assert
            Assert.Equal(0, skor);
            Assert.Equal(0, ponuda.UkupanSkor);
        }

        [Fact]
        [Trait("Category", "IzracunajSkor")]
        public void IzracunajSkor_NullKriteriji_BacaArgumentNullException()
        {
            // Arrange
            var tender = _fixture.DefaultTender;
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ponuda.IzracunajSkor(null)
            );
        }

        #endregion
    }
}