using System;
using System.Collections.Generic;
using TenderiApp.Exceptions;

namespace TenderiApp.Models
{
    public class Ponudjac
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public int BrojPonuda { get; set; }

        public Ponudjac(string naziv, string email, string telefon)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                throw new ValidationException("Naziv ponuđača ne može biti prazan.");
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ValidationException("Email mora biti validan.");
            }

            if (string.IsNullOrWhiteSpace(telefon))
            {
                throw new ValidationException("Telefon ne može biti prazan.");
            }

            Naziv = naziv;
            Email = email;
            Telefon = telefon;
            BrojPonuda = 0;
        }

        public Ponuda KreirajPonudu(Tender tender, double cijena, int rok, int garancija, int kvalitet)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (tender.Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije otvoren za prijave.");
            }

            if (DateTime.Now > tender.RokZaPrijavu)
            {
                throw new TenderException("Rok za prijavu je istekao.");
            }

            Ponuda ponuda = new Ponuda(tender, this, cijena, rok, garancija, kvalitet);
            BrojPonuda++;

            return ponuda;
        }
    }
}