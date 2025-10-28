using System;
using System.Collections.Generic;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class Administrator
    {
        public int Id { get; set; }
        public string KorisnickoIme { get; set; }
        public string Email { get; set; }

        public Administrator(string korisnickoIme, string email)
        {
            if (string.IsNullOrWhiteSpace(korisnickoIme))
            {
                throw new ValidationException("Korisničko ime ne može biti prazno.");
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ValidationException("Email mora biti validan.");
            }

            KorisnickoIme = korisnickoIme;
            Email = email;
        }

        public Tender KreirajTender(string naziv, string opis, DateTime rok, KriterijiTendera kriteriji)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                throw new ValidationException("Naziv tendera ne može biti prazan.");
            }

            if (kriteriji == null)
            {
                throw new ArgumentNullException(nameof(kriteriji));
            }

            Tender tender = new Tender(naziv, opis, rok, kriteriji);
            return tender;
        }

        public void OdobriPonudu(Ponuda ponuda)
        {
            if (ponuda == null)
            {
                throw new ArgumentNullException(nameof(ponuda));
            }

            if (!ponuda.JeValidna)
            {
                throw new ValidationException("Ponuda nije validna i ne može biti odobrena.");
            }

            Console.WriteLine($"Ponuda #{ponuda.Id} od ponuđača {ponuda.Ponudjac.Naziv} je odobrena.");
        }

        public Izvjestaj GenerisiIzvjestaj(Tender tender, List<Ponuda> ponude)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            Izvjestaj izvjestaj = new Izvjestaj(tender, ponude);
            return izvjestaj;
        }
    }
}