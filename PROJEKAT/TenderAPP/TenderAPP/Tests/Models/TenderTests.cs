using System;
using Xunit;
using TenderiApp.Models;
using TenderiApp.Exceptions;
using TenderiApp.Tests.Fixtures;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Tests.Models
{
    [ExcludeFromCodeCoverage]
    public class TenderTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public TenderTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region Constructor Tests

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_ValidniPodaci_KreiraTender()
        {
            // Arrange
            var naziv = "Test Tender";
            var opis = "Test opis";
            var rok = DateTime.Now.AddDays(30);
            var kriteriji = _fixture.DefaultKriteriji;

            // Act
            var tender = new Tender(naziv, opis, rok, kriteriji);

            // Assert
            Assert.NotNull(tender);
            Assert.Equal(naziv, tender.Naziv);
            Assert.Equal(opis, tender.Opis);
            Assert.Equal(rok, tender.RokZaPrijavu);
            Assert.Equal(StatusTendera.Otvoren, tender.Status);
            Assert.Empty(tender.Ponude);
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_PrazanNaziv_BacaValidationException(string naziv)
        {
            // Arrange
            var opis = "Test opis";
            var rok = DateTime.Now.AddDays(30);
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Tender(naziv, opis, rok, kriteriji)
            );
            Assert.Contains("naziv", exception.Message.ToLower());
        }

        [Theory]
        [Trait("Category", "Constructor")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_PrazanOpis_BacaValidationException(string opis)
        {
            // Arrange
            var naziv = "Test Tender";
            var rok = DateTime.Now.AddDays(30);
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Tender(naziv, opis, rok, kriteriji)
            );
            Assert.Contains("opis", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_RokUProslosti_BacaValidationException()
        {
            // Arrange
            var naziv = "Test Tender";
            var opis = "Test opis";
            var rok = DateTime.Now.AddDays(-1);
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                new Tender(naziv, opis, rok, kriteriji)
            );
            Assert.Contains("budućnosti", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "Constructor")]
        public void Constructor_NullKriteriji_BacaArgumentNullException()
        {
            // Arrange
            var naziv = "Test Tender";
            var opis = "Test opis";
            var rok = DateTime.Now.AddDays(30);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Tender(naziv, opis, rok, null)
            );
        }

        #endregion

        #region DodajPonudu Tests

        [Fact]
        [Trait("Category", "DodajPonudu")]
        public void DodajPonudu_ValidnaPonuda_DodajePonuduUListu()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);

            // Act
            var result = tender.DodajPonudu(ponuda);

            // Assert
            Assert.True(result);
            Assert.Contains(ponuda, tender.Ponude);
            Assert.Single(tender.Ponude);
        }

        [Fact]
        [Trait("Category", "DodajPonudu")]
        public void DodajPonudu_NullPonuda_BacaArgumentNullException()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                tender.DodajPonudu(null)
            );
        }

  

        #endregion

        #region ZatvoriTender Tests

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_OtvorenTenderSaPonudama_StatusPostaneZatvoren()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);

            // Act
            tender.ZatvoriTender();

            // Assert
            Assert.Equal(StatusTendera.Zatvoren, tender.Status);
        }

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_BezPonuda_BacaTenderException()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                tender.ZatvoriTender()
            );
            Assert.Contains("bez ponuda", exception.Message.ToLower());
        }

        #endregion

        #region GetValidnePonude Tests

        [Fact]
        [Trait("Category", "GetValidnePonude")]
        public void GetValidnePonude_VracaSamoValidnePonude()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("Ponudjac 1", "p1@test.ba", "123");
            var ponudjac2 = new Ponudjac("Ponudjac 2", "p2@test.ba", "456");

            var validna = new Ponuda(tender, ponudjac1, 35000, 45, 24, 85);
            validna.Validiraj(tender.Kriteriji);

            var nevalidna = new Ponuda(tender, ponudjac2, 60000, 45, 24, 85); // Prekoračuje max cijenu
            nevalidna.Validiraj(tender.Kriteriji);

            tender.Ponude.Add(validna);
            tender.Ponude.Add(nevalidna);

            // Act
            var validnePonude = tender.GetValidnePonude();

            // Assert
            Assert.Single(validnePonude);
            Assert.Contains(validna, validnePonude);
            Assert.DoesNotContain(nevalidna, validnePonude);
        }

        #endregion

        #region Additional Coverage Tests

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_VecZatvoren_BacaTenderException()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);
            tender.ZatvoriTender(); // Zatvori prvi put

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                tender.ZatvoriTender() // Pokušaj zatvoriti ponovo
            );
            Assert.Contains("otvoren", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "GetValidnePonude")]
        public void GetValidnePonude_BezPonuda_VracaPraznuListu()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();

            // Act
            var validne = tender.GetValidnePonude();

            Assert.NotNull(validne);
            Assert.Empty(validne);
        }

        [Fact]
        [Trait("Category", "GetValidnePonude")]
        public void GetValidnePonude_SamoNevalidnePonude_VracaPraznuListu()
        {
            // Arrange
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "111");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "222");
            ponudjac2.Id = 2;

            var nevalidna1 = new Ponuda(tender, ponudjac1, 60000, 45, 24, 85); // Previsoka cijena
            nevalidna1.Validiraj(tender.Kriteriji);

            var nevalidna2 = new Ponuda(tender, ponudjac2, 35000, 100, 24, 85); // Predug rok
            nevalidna2.Validiraj(tender.Kriteriji);

            tender.Ponude.Add(nevalidna1);
            tender.Ponude.Add(nevalidna2);

            // Act
            var validne = tender.GetValidnePonude();

            // Assert
            Assert.Empty(validne);
        }

        #endregion
    }
}