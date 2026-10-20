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

        #region Additional Coverage Tests

        [Fact]
        [Trait("Category", "KreirajTender")]
        public void KreirajTender_RokUProslosti_BacaValidationException()
        {
            // Arrange
            var service = new TenderService();
            var kriteriji = _fixture.DefaultKriteriji;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(-1), kriteriji)
            );
            Assert.Contains("budućnosti", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "KreirajTender")]
        public void KreirajTender_NevalidniKriteriji_BacaValidationException()
        {
            // Arrange
            var service = new TenderService();
            var ponderi = new PonderiKriterija(0.5, 0.3, 0.1, 0.05); // Zbir = 0.95 != 1.0
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 10000,
                MaksimalnaCijena = 50000,
                MaksimalanRok = 90,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 60
            };

            // Act & Assert
            Assert.Throws<ValidationException>(() =>
                service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), kriteriji)
            );
        }

        [Fact]
        [Trait("Category", "ZatvoriTender")]
        public void ZatvoriTender_VecEvaluiran_BacaTenderException()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);

            service.ZatvoriTender(tender.Id);
            tender.Status = StatusTendera.Evaluiran; // Promijeni u evaluiran

            // Act & Assert
            var exception = Assert.Throws<TenderException>(() =>
                service.ZatvoriTender(tender.Id)
            );
            Assert.Contains("otvoren", exception.Message.ToLower());
        }

        [Fact]
        [Trait("Category", "GetAktivniTenderi")]
        public void GetAktivniTenderi_SviZatvoreni_VracaPraznuListu()
        {
            // Arrange
            var service = new TenderService();
            var tender = service.KreirajTender("Test", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

            var ponudjac = _fixture.DefaultPonudjac;
            var ponuda = new Ponuda(tender, ponudjac, 35000, 45, 24, 85);
            ponuda.Validiraj(tender.Kriteriji);
            tender.DodajPonudu(ponuda);
            service.ZatvoriTender(tender.Id);

            // Act
            var aktivni = service.GetAktivniTenderi();

            // Assert
            Assert.Empty(aktivni);
        }

        //ZA potrebe task 4
        [Fact]
        public void Test_SortiranjePonuda()
        {
            // Arrange

            // 1. Kreiramo ponder-e
            var ponderi = new PonderiKriterija(0.4, 0.2, 0.2, 0.2);

            // 2. Kreiramo kriterije tendera
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 50,
                MaksimalnaCijena = 150,
                MinimalnaGarancija = 6,
                MaksimalanRok = 30,
                MinimalanKvalitet = 50
            };

            // 3. Kreiramo tender
            var tender = new Tender("Tender1", "Opis", DateTime.Now.AddDays(10), kriteriji);

            // 4. Kreiramo ponuđače
            var ponudjacA = new Ponudjac("FirmaA", "a@firma.com", "061123456");
            var ponudjacB = new Ponudjac("FirmaB", "b@firma.com", "061654321");
            var ponudjacC = new Ponudjac("FirmaC", "c@firma.com", "061987654");

            // 5. Kreiramo ponude preko ponuđača
            var ponudaA = ponudjacA.KreirajPonudu(tender, 80, 10, 12, 70);
            var ponudaB = ponudjacB.KreirajPonudu(tender, 60, 5, 12, 90);
            var ponudaC = ponudjacC.KreirajPonudu(tender, 100, 8, 6, 60);

            // 6. Validiramo ponude
            ponudaA.Validiraj(kriteriji);
            ponudaB.Validiraj(kriteriji);
            ponudaC.Validiraj(kriteriji);

            // 7. Izračunavamo UkupanSkor
            ponudaA.IzracunajSkor(kriteriji);
            ponudaB.IzracunajSkor(kriteriji);
            ponudaC.IzracunajSkor(kriteriji);

            // 8. Dodajemo ponude u tender
            tender.Ponude.Add(ponudaA);
            tender.Ponude.Add(ponudaB);
            tender.Ponude.Add(ponudaC);

            // 9. Kreiramo servis
            var service = new TenderService();

            // Act – sortiranje ponuda
            service.SortirajPonude(tender);

            // Assert – provjera da je lista sortirana po UkupanSkor
            Assert.True(tender.Ponude[0].UkupanSkor >= tender.Ponude[1].UkupanSkor);
            Assert.True(tender.Ponude[1].UkupanSkor >= tender.Ponude[2].UkupanSkor);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(42, 3)]
        public void UcitajTenderJSON_NovoKreiranje_NastavljaNakonNajvecegId(int prviId, int drugiId)
        {
            var original = new TenderService();
            var kriteriji = _fixture.DefaultKriteriji;
            original.KreirajTender("Prvi", "Opis", DateTime.Now.AddDays(30), kriteriji).Id = prviId;
            original.KreirajTender("Drugi", "Opis", DateTime.Now.AddDays(30), kriteriji).Id = drugiId;
            string path = Path.GetTempFileName();

            try
            {
                original.SacuvajTenderJSON(path);
                var ucitani = new TenderService();
                ucitani.UcitajTenderJSON(path);

                var novi = ucitani.KreirajTender("Novi", "Opis", DateTime.Now.AddDays(30), kriteriji);

                Assert.Equal(Math.Max(prviId, drugiId) + 1, novi.Id);
                Assert.Equal(3, ucitani.GetSviTenderi().Select(t => t.Id).Distinct().Count());
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void UcitajTenderJSON_PraznaLista_NovoKreiranjePocinjeOdJedan()
        {
            var service = new TenderService();
            service.KreirajTender("Postojeci", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);
            string path = Path.GetTempFileName();

            try
            {
                File.WriteAllText(path, "[]");
                service.UcitajTenderJSON(path);

                var novi = service.KreirajTender("Novi", "Opis", DateTime.Now.AddDays(30), _fixture.DefaultKriteriji);

                Assert.Single(service.GetSviTenderi());
                Assert.Equal(1, novi.Id);
            }
            finally
            {
                File.Delete(path);
            }
        }

        //ZA potrebe task 4 JSON TESTOVI 
        [Fact]
        public void JsonUpisICitanje_ShouldWorkCorrectly()
        {
            // Arrange
            var service = new TenderService();

            // Kreiramo pondere
            var ponderi = new PonderiKriterija(0.4, 0.3, 0.2, 0.1);

            // Kreiramo kriterije za tender
            var kriteriji = new KriterijiTendera(ponderi)
            {
                MinimalnaCijena = 1000,
                MaksimalnaCijena = 5000,
                MaksimalanRok = 30,
                MinimalnaGarancija = 12,
                MinimalanKvalitet = 70
            };

            // Kreiramo testni tender
            var tenderOriginal = service.KreirajTender(
                naziv: "Testni Tender",
                opis: "Ovo je JSON test",
                rok: DateTime.Now.AddDays(10),
                kriteriji: kriteriji
            );

            // Putanja za testni JSON (koristi temp folder)
            string filePath = Path.Combine(Path.GetTempPath(), "tender_test.json");

            // Act 1: Snimi JSON
            service.SacuvajTenderJSON(filePath);

            // Act 2: Učitaj JSON u novi servis
            var noviService = new TenderService();
            noviService.UcitajTenderJSON(filePath);

            var ucitaniTenderi = noviService.GetSviTenderi();

            // Assert
            Assert.Single(ucitaniTenderi);
            var t = ucitaniTenderi[0];

            Assert.Equal(tenderOriginal.Naziv, t.Naziv);
            Assert.Equal(tenderOriginal.Opis, t.Opis);
            Assert.Equal(tenderOriginal.RokZaPrijavu.Date, t.RokZaPrijavu.Date);

            // Poređenje kriterija
            Assert.Equal(kriteriji.MinimalnaCijena, t.Kriteriji.MinimalnaCijena);
            Assert.Equal(kriteriji.MaksimalnaCijena, t.Kriteriji.MaksimalnaCijena);
            Assert.Equal(kriteriji.MaksimalanRok, t.Kriteriji.MaksimalanRok);
            Assert.Equal(kriteriji.MinimalnaGarancija, t.Kriteriji.MinimalnaGarancija);
            Assert.Equal(kriteriji.MinimalanKvalitet, t.Kriteriji.MinimalanKvalitet);

            // Poređenje pondera
            Assert.Equal(ponderi.PonderCijene, t.Kriteriji.Ponderi.PonderCijene);
            Assert.Equal(ponderi.PonderRoka, t.Kriteriji.Ponderi.PonderRoka);
            Assert.Equal(ponderi.PonderGarancije, t.Kriteriji.Ponderi.PonderGarancije);
            Assert.Equal(ponderi.PonderKvaliteta, t.Kriteriji.Ponderi.PonderKvaliteta);

            // Cleanup
            if (File.Exists(filePath))
                File.Delete(filePath);
        }








        #endregion
    }
}