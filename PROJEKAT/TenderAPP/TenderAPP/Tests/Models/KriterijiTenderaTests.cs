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
    public class KriterijiTenderaTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public KriterijiTenderaTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPonderi_KreiraKriterije()
        {
            // Arrange
            var ponderi = _fixture.DefaultPonderi;

            // Act
            var kriteriji = new KriterijiTendera(ponderi);

            // Assert
            Assert.NotNull(kriteriji);
            Assert.Equal(ponderi, kriteriji.Ponderi);
            Assert.Equal(0, kriteriji.MinimalnaCijena);
            Assert.Equal(double.MaxValue, kriteriji.MaksimalnaCijena);
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullPonderi_BacaArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new KriterijiTendera(null)
            );
        }

        #endregion

        #region ValidirajKriterije Tests - DATA DRIVEN (JSON)

        

        [Fact]
        [Trait("Category", "Validation")]
        public void ValidirajKriterije_NegativnaMinimalnaCijena_BacaValidationException()
        {
            // Arrange
            var ponderi = _fixture.DefaultPonderi;
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = -1000
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                kriteriji.ValidirajKriterije()
            );
            Assert.Contains("negativna", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "Validation")]
        public void ValidirajKriterije_MaxCijenaManjаOdMin_BacaValidationException()
        {
            // Arrange
            var ponderi = _fixture.DefaultPonderi;
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 50000,
                MaksimalnaCijena = 30000
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                kriteriji.ValidirajKriterije()
            );
            Assert.Contains("manja", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Validation")]
        [InlineData(0)]
        [InlineData(-10)]
        public void ValidirajKriterije_NevaliданRok_BacaValidationException(int rok)
        {
            // Arrange
            var ponderi = _fixture.DefaultPonderi;
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MaksimalanRok = rok
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                kriteriji.ValidirajKriterije()
            );
            Assert.Contains("pozitivan", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Validation")]
        [InlineData(-1)]
        [InlineData(101)]
        public void ValidirajKriterije_KvalitetVanRasponа_BacaValidationException(int kvalitet)
        {
            // Arrange
            var ponderi = _fixture.DefaultPonderi;
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalanKvalitet = kvalitet
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                kriteriji.ValidirajKriterije()
            );
            Assert.Contains("između", exception.Message.ToLower());
        }

        #endregion
    }
}