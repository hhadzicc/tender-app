using Xunit;
using TenderiApp.Models;
using TenderiApp.Exceptions;

namespace TenderiApp.Tests.Models
{
    public class PonderiKriterijaTests
    {
        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPonderi_KreiraPondere()
        {
            // Arrange & Act
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.2, 0.1);

            // Assert
            Assert.Equal(0.4, ponderi.PonderCijene);
            Assert.Equal(0.3, ponderi.PonderRoka);
            Assert.Equal(0.2, ponderi.PonderGarancije);
            Assert.Equal(0.1, ponderi.PonderKvaliteta);
        }

        #endregion

        #region ValidirajPondere Tests

        [Fact]
        [Trait("Category", "Validation")]
        public void ValidirajPondere_ZbirJedan_VracaTrue()
        {
            // Arrange
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.2, 0.1);

            // Act
            var result = ponderi.ValidirajPondere();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [Trait("Category", "Validation")]
        [InlineData(-0.1, 0.4, 0.4, 0.3)]
        [InlineData(0.4, -0.1, 0.4, 0.3)]
        [InlineData(0.4, 0.4, -0.1, 0.3)]
        [InlineData(0.4, 0.4, 0.3, -0.1)]
        public void ValidirajPondere_NegativniPonder_BacaValidationException(
            double pCijene, double pRoka, double pGarancije, double pKvaliteta)
        {
            // Arrange
            var ponderi = new PonderiKriterija(pCijene, pRoka, pGarancije, pKvaliteta);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                ponderi.ValidirajPondere()
            );
            Assert.Contains("negativni", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Validation")]
        [InlineData(0.5, 0.3, 0.1, 0.05)]  // Zbir = 0.95
        [InlineData(0.5, 0.3, 0.2, 0.2)]   // Zbir = 1.2
        [InlineData(0.25, 0.25, 0.25, 0.2)] // Zbir = 0.95
        public void ValidirajPondere_ZbirNijeJedan_BacaValidationException(
            double pCijene, double pRoka, double pGarancije, double pKvaliteta)
        {
            // Arrange
            var ponderi = new PonderiKriterija(pCijene, pRoka, pGarancije, pKvaliteta);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                ponderi.ValidirajPondere()
            );
            Assert.Contains("1.0", exception.Message);
        }

        [Theory]
        [Trait("Category", "Validation")]
        [InlineData(1.1, 0, 0, 0)]
        [InlineData(0, 1.1, 0, 0)]
        [InlineData(0, 0, 1.1, 0)]
        [InlineData(0, 0, 0, 1.1)]
        public void ValidirajPondere_PonderVeciOdJedan_BacaValidationException(
            double pCijene, double pRoka, double pGarancije, double pKvaliteta)
        {
            // Arrange
            var ponderi = new PonderiKriterija(pCijene, pRoka, pGarancije, pKvaliteta);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                ponderi.ValidirajPondere()
            );
            Assert.Contains("veći", exception.Message.ToLower());
        }

        #endregion

        #region UkupniPonder Tests

        [Theory]
        [Trait("Category", "Calculation")]
        [InlineData(0.25, 0.25, 0.25, 0.25, 1.0)]
        [InlineData(0.4, 0.3, 0.2, 0.1, 1.0)]
        [InlineData(0.5, 0.2, 0.2, 0.1, 1.0)]
        public void UkupniPonder_RazlicitiPonderi_VracaTacanZbir(
            double pCijene, double pRoka, double pGarancije, double pKvaliteta, double ocekivaniZbir)
        {
            // Arrange
            var ponderi = new PonderiKriterija(pCijene, pRoka, pGarancije, pKvaliteta);

            // Act
            var zbir = ponderi.UkupniPonder();

            // Assert
            Assert.Equal(ocekivaniZbir, zbir, 3); // 3 decimale preciznosti
        }

        #endregion
    }
}