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
    public class TenderServiceTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public TenderServiceTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region KreirajTender Tests

        [Fact]
        [Trait("Category", "KreirajTender")]
        public void KreirajTender_ValidniPodaci_VracaTenderSaId()
        {
            // Arrange
            var service = new TenderService();
            var naziv = "Test Tender";
            var opis = "Test opis";
            var rok = DateTime.Now.AddDays(30);
            var kriteriji = _fixture.DefaultKriteriji;

            // Act
            var tender = service.KreirajTender(naziv, opis, rok, kriteriji);

            // Assert
            Assert.NotNull(tender);
            Assert.True(tender.Id > 0);
            Assert.Equal(naziv, tender.Naziv);
            Assert.Equal(StatusTendera.Otvoren, tender.Status);
        }

        [Fact]
        [Trait("Category", "KreirajTender")]
        public void KreirajTender_ViseKreiranja_RazlicitiIdevi()
        {
            // Arrange
            var service = new TenderService();
            var kriteriji = _fixture.DefaultKriteriji;

            // Act
            var tender1 = service.KreirajTender("Tender 1", "Opis 1", DateTime.Now.AddDays(30), kriteriji);
            var tender2 = service.KreirajTender("Tender 2", "Opis 2", DateTime.Now.AddDays(30), kriteriji);
            var tender3 = service.KreirajTender("Tender 3", "Opis 3", DateTime.Now.AddDays(30), kriteriji);

            // Assert
            Assert.NotEqual(tender1.Id, tender2.Id);
            Assert.NotEqual(tender2.Id, tender3.Id);
            Assert.NotEqual(tender1.Id, tender3.Id);
            Assert.Equal(1, tender1.Id);
            Assert.Equal(2, tender2.Id);
            Assert.Equal(3, tender3.Id);
        }

        [Theory]
        [Trait("Category", "KreirajTender")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void KreirajTender_PrazanNaziv_BacaValidationException(string naziv)
        {
            // Arrange
            var service = new TenderService();
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            Assert.Throws<ValidationException>(() =>
                service.KreirajTender(naziv, "Opis", DateTime.Now.AddDays(30), kriteriji)
            );
        }

        [Fact]
        [Trait("Category", "KreirajTender")]
        public void KreirajTender_NullKriteriji_BacaArgumentNullException()
        {
            // Arrange
            var service = new TenderService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.KreirajTender("Naziv", "Opis", DateTime.Now.AddDays(30), null)
            );
        }

        #endregion

        #region GetTender Tests

        [Fact]
        [Trait("Category", "GetTender")]
        public void GetTender_PostojeciId_VracaTender()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            // Act
            var result = service.GetTender(tender.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tender.Id, result.Id);
            Assert.Equal(tender.Naziv, result.Naziv);
        }

        [Fact]
        [Trait("Category", "GetTender")]
        public void GetTender_NepostojeciId_BacaTenderException()
        {
            // Arrange
            var service = new TenderService();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.GetTender(999)
            );
            Assert.Contains("ne postoji", exception.Message.ToLower());
        }

        #endregion

        #region GetSviTenderi Tests

        [Fact]
        [Trait("Category", "GetSviTenderi")]
        public void GetSviTenderi_BezTendera_VracaPraznuListu()
        {
            // Arrange
            var service = new TenderService();

            // Act
            var tenderi = service.GetSviTenderi();

            // Assert
            Assert.NotNull(tenderi);
            Assert.Empty(tenderi);
        }

        [Fact]
        [Trait("Category", "GetSviTenderi")]
        public void GetSviTenderi_SaTenderima_VracaSveTendere()
        {
            // Arrange
            var service = new TenderService();
            service.KreirajTender("Tender 1", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            service.KreirajTender("Tender 2", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            service.KreirajTender("Tender 3", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            // Act
            var tenderi = service.GetSviTenderi();

            // Assert
            Assert.NotNull(tenderi);
            Assert.Equal(3, tenderi.Count);
        }

        #endregion

        #region GetAktivniTenderi Tests

        [Fact]
        [Trait("Category", "GetAktivniTenderi")]
        public void GetAktivniTenderi_SamoOtvoreniTenderi_VracaAktivne()
        {
            // Arrange
            var service = new TenderService();
            var tender1 = service.KreirajTender("Tender 1", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            var tender2 = service.KreirajTender("Tender 2", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            // Dodaj ponudu i zatvori tender2
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender2, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender2.Kriteriji);
            tender2.DodajPonudu(ponuda);
            service.ZatvoriTender(tender2.Id);

            // Act
            var aktivni = service.GetAktivniTenderi();

            // Assert
            Assert.Single(aktivni);
            Assert.Contains(tender1, aktivni);
            Assert.DoesNotContain(tender2, aktivni);
        }

        [Fact]
        [Trait("Category", "GetAktivniTenderi")]
        public void GetAktivniTenderi_BezAktivnih_VracaPraznuListu()
        {
            // Arrange
            var service = new TenderService();

            // Act
            var aktivni = service.GetAktivniTenderi();

            // Assert
            Assert.NotNull(aktivni);
            Assert.Empty(aktivni);
        }

        #endregion

        #region ZatvoriTender Tests

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_OtvorenTenderSaPonudama_MjenjaStatusUZatvoren()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);

            // Act
            var result = service.ZatvoriTender(tender.Id);

            // Assert
            Assert.True(result);
            var zatvoreni = service.GetTender(tender.Id);
            Assert.Equal(StatusTendera.Zatvoren, zatvoreni.Status);
        }

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_NepostojeciId_BacaTenderException()
        {
            // Arrange
            var service = new TenderService();

            // Act & Assert
            Assert.Throws<TenderException>(() =>
                service.ZatvoriTender(999)
            );
        }

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_BezPonuda_BacaTenderException()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.ZatvoriTender(tender.Id)
            );
            Assert.Contains("ponuda", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_VecZatvoren_BacaTenderException()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);

            service.ZatvoriTender(tender.Id);

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.ZatvoriTender(tender.Id)
            );
            Assert.Contains("otvoren", exception.Message.ToLower());
        }

        #endregion
    }
}