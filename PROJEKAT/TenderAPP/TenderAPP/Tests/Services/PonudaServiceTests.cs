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
    public class PonudaServiceTests : IClassFixture<TenderTestFixture>
    {
        private readonly TenderTestFixture _fixture;

        public PonudaServiceTests(TenderTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region PrijaviPonudu Tests

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_ValidniPodaci_VracaPonuduSaId()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;

            // Act
            var ponuda = service.PrijaviPonudu(tender, ponudjac, 35000, 45, 24, 85);

            // Assert
            Assert.NotNull(ponuda);
            Assert.True(ponuda.Id > 0);
            Assert.Equal(tender.Id, ponuda.TenderId);
            Assert.Equal(ponudjac, ponuda.Ponudjac);
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_ValidnaPonuda_AutomatskiValidira()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;

            // Act
            var ponuda = service.PrijaviPonudu(tender, ponudjac, 35000, 45, 24, 85);

            // Assert
            Assert.True(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_NevalidnaPonuda_OznacavaKaoNevalidnu()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;

            // Act
            var ponuda = service.PrijaviPonudu(tender, ponudjac, 60000, 45, 24, 85); // Prekoračuje max cijenu

            // Assert
            Assert.False(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_NullTender_BacaArgumentNullException()
        {
            // Arrange
            var service = new PonudaService();
            var ponudjac = _fixture.DefaultPonudjac;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.PrijaviPonudu(null, ponudjac, 35000, 45, 24, 85)
            );
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_NullPonudjac_BacaArgumentNullException()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.PrijaviPonudu(tender, null, 35000, 45, 24, 85)
            );
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_TenderZatvoren_BacaTenderException()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            var ponudjac2 = _fixture.DefaultPonudjac;

            // Dodaj prvu ponudu i zatvori
            service.PrijaviPonudu(tender, ponudjac1, 35000, 45, 24, 85);
            tender.ZatvoriTender();

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.PrijaviPonudu(tender, ponudjac2, 40000, 50, 24, 80)
            );
            Assert.Contains("otvoren", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "PrijaviPonudu")]
        public void PrijaviPonudu_IstيPonudjacDvaPuta_BacaTenderException()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;

            service.PrijaviPonudu(tender, ponudjac, 35000, 45, 24, 85);

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.PrijaviPonudu(tender, ponudjac, 40000, 50, 24, 80)
            );
            Assert.Contains("već prijavio", exception.Message.ToLower());
        }

        #endregion

        #region ValidirajPonudu Tests

        [Fact]
        [Trait("Category", "ValidirajPonudu")]
        public void ValidirajPonudu_ValidnaPonuda_VracaTrue()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);

            // Act
            var result = service.ValidirajPonudu(ponuda, tender.Kriteriji);

            // Assert
            Assert.True(result);
            Assert.True(ponuda.JeValidna);
        }

        [Fact]
        [Trait("Category", "ValidirajPonudu")]
        public void ValidirajPonudu_NevalidnaPonuda_VracaFalse()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 60000, 45, 24, 85); // Nevalidna

            // Act
            var result = service.ValidirajPonudu(ponuda, tender.Kriteriji);

            // Assert
            Assert.False(result);
            Assert.False(ponuda.JeValidna);
        }

        #endregion

        #region GetPonudeZaTender Tests

        [Fact]
        [Trait("Category", "GetPonudeZaTender")]
        public void GetPonudeZaTender_BezPonuda_VracaPraznuListu()
        {
            // Arrange
            var service = new PonudaService();

            // Act
            var ponude = service.GetPonudeZaTender(1);

            // Assert
            Assert.NotNull(ponude);
            Assert.Empty(ponude);
        }

        [Fact]
        [Trait("Category", "GetPonudeZaTender")]
        public void GetPonudeZaTender_SaPonudama_VracaSamoPonudeZaTajTender()
        {
            // Arrange
            var service = new PonudaService();
            var tender1 = _fixture.DefaultTender;
            var tender2 = new Tender("Tender 2", "Opis 2", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            tender2.Id = 2;

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender1, ponudjac1, 35000, 45, 24, 85);
            service.PrijaviPonudu(tender1, ponudjac2, 40000, 50, 24, 80);
            service.PrijaviPonudu(tender2, ponudjac3, 30000, 40, 12, 75);

            // Act
            var ponudeZaTender1 = service.GetPonudeZaTender(tender1.Id);

            // Assert
            Assert.Equal(2, ponudeZaTender1.Count);
            Assert.All(ponudeZaTender1, p => Assert.Equal(tender1.Id, p.TenderId));
        }

        #endregion

        #region FiltrirajPonude Tests

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_BezKriterija_VracaSvePonude()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;

            service.PrijaviPonudu(tender, ponudjac1, 35000, 45, 24, 85);
            service.PrijaviPonudu(tender, ponudjac2, 40000, 50, 24, 80);

            // Act
            var ponude = service.FiltrirajPonude();

            // Assert
            Assert.Equal(2, ponude.Count);
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_PoTenderId_VracaSamoPonudeZaTajTender()
        {
            // Arrange
            var service = new PonudaService();
            var tender1 = _fixture.CreateOpenTender();
            var tender2 = new Tender("Tender 2", "Opis 2", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            tender2.Id = 2;

            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;

            service.PrijaviPonudu(tender1, ponudjac1, 35000, 45, 24, 85);
            service.PrijaviPonudu(tender2, ponudjac2, 40000, 50, 24, 80);

            // Act
            var ponude = service.FiltrirajPonude(tenderId: tender1.Id);

            // Assert
            Assert.Single(ponude);
            Assert.Equal(tender1.Id, ponude[0].TenderId);
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_PoMaxCijeni_VracaSamoJeftinijePonude()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender, ponudjac1, 30000, 45, 24, 85);
            service.PrijaviPonudu(tender, ponudjac2, 40000, 50, 24, 80);
            service.PrijaviPonudu(tender, ponudjac3, 45000, 60, 12, 75);

            // Act
            var ponude = service.FiltrirajPonude(maxCijena: 40000);

            // Assert
            Assert.Equal(2, ponude.Count);
            Assert.All(ponude, p => Assert.True(p.Cijena <= 40000));
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_PoMinCijeni_VracaSamoSkupljePonude()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender, ponudjac1, 30000, 45, 24, 85);
            service.PrijaviPonudu(tender, ponudjac2, 40000, 50, 24, 80);
            service.PrijaviPonudu(tender, ponudjac3, 45000, 60, 12, 75);

            // Act
            var ponude = service.FiltrirajPonude(minCijena: 35000);

            // Assert
            Assert.Equal(2, ponude.Count);
            Assert.All(ponude, p => Assert.True(p.Cijena >= 35000));
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_PoMaxRoku_VracaSamoPonudeSaBrzomIsporukom()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender, ponudjac1, 35000, 30, 24, 85);
            service.PrijaviPonudu(tender, ponudjac2, 40000, 45, 24, 80);
            service.PrijaviPonudu(tender, ponudjac3, 45000, 60, 12, 75);

            // Act
            var ponude = service.FiltrirajPonude(maxRok: 45);

            // Assert
            Assert.Equal(2, ponude.Count);
            Assert.All(ponude, p => Assert.True(p.RokIsporuke <= 45));
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_PoMinKvalitetu_VracaSamoVisokoKvalitetnePonude()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender, ponudjac1, 35000, 45, 24, 70);
            service.PrijaviPonudu(tender, ponudjac2, 40000, 50, 24, 85);
            service.PrijaviPonudu(tender, ponudjac3, 45000, 60, 12, 90);

            // Act
            var ponude = service.FiltrirajPonude(minKvalitet: 80);

            // Assert
            Assert.Equal(2, ponude.Count);
            Assert.All(ponude, p => Assert.True(p.Kvalitet >= 80));
        }

        [Fact]
        [Trait("Category", "FiltrirajPonude")]
        public void FiltrirajPonude_VisheKriterija_VracaPonudeKojeZadovoljavajuSveKriterije()
        {
            // Arrange
            var service = new PonudaService();
            var tender = _fixture.CreateOpenTender();
            var ponudjac1 = new Ponudjac("P1", "p1@test.ba", "123");
            ponudjac1.Id = 1;
            var ponudjac2 = new Ponudjac("P2", "p2@test.ba", "456");
            ponudjac2.Id = 2;
            var ponudjac3 = new Ponudjac("P3", "p3@test.ba", "789");
            ponudjac3.Id = 3;

            service.PrijaviPonudu(tender, ponudjac1, 35000, 45, 24, 85); // Zadovoljava
            service.PrijaviPonudu(tender, ponudjac2, 48000, 50, 24, 80); // Cijena previsoka
            service.PrijaviPonudu(tender, ponudjac3, 38000, 60, 12, 90); // Rok predug

            // Act
            var ponude = service.FiltrirajPonude(maxCijena: 40000, maxRok: 50, minKvalitet: 80);

            // Assert
            Assert.Single(ponude);
            Assert.Equal(ponudjac1.Id, ponude[0].Ponudjac.Id);
        }

        #endregion
    }
}