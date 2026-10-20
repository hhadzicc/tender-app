using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TenderiApp.Exceptions;
using TenderiApp.Interfaces;
using TenderiApp.Models;

namespace TenderiApp.Services
{
    public class TenderService : ITenderService
    {
        private List<Tender> tenderi;
        private int nextId;

        public TenderService()
        {
            tenderi = new List<Tender>();
            nextId = 1;
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

            Tender tender = new Tender(naziv, opis, rok, kriteriji)
            {
                Id = nextId++
            };

            tenderi.Add(tender);
            return tender;
        }

        public Tender? GetTender(int id)
        {
            var tender = tenderi.FirstOrDefault(t => t.Id == id);

            if (tender == null)
            {
                throw new TenderException($"Tender sa ID {id} ne postoji.");
            }

            return tender;
        }

        public List<Tender> GetSviTenderi()
        {
            return new List<Tender>(tenderi);
        }

        public List<Tender> GetAktivniTenderi()
        {
            return tenderi.Where(t => t.Status == StatusTendera.Otvoren && t.RokZaPrijavu > DateTime.Now).ToList();
        }

        public bool ZatvoriTender(int tenderId)
        {
            var tender = GetTender(tenderId);

            if (tender == null)
            {
                throw new TenderException($"Tender sa ID {tenderId} ne postoji.");
            }

            if (tender.Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender nije u statusu 'Otvoren' i ne može biti zatvoren.");
            }

            if (tender.Ponude.Count == 0)
            {
                throw new TenderException("Tender ne može biti zatvoren jer nema ponuda.");
            }

            tender.ZatvoriTender();
            return true;
        }

        public void SortirajPonude(Tender tender)
        {
            if (tender == null)
                throw new ArgumentNullException(nameof(tender));

            // Provjeravamo da ponude postoje
            if (tender.Ponude == null || tender.Ponude.Count == 0)
                throw new TenderException("Tender nema ponuda za sortiranje.");

            // Prvo izračunavamo UkupanSkor za sve ponude
            foreach (var ponuda in tender.Ponude)
            {
                ponuda.IzracunajSkor(tender.Kriteriji);
            }

            // Sortiramo po UkupanSkor opadajuće
            tender.Ponude = tender.Ponude
                .OrderByDescending(p => p.UkupanSkor)
                .ToList();
        }

        // Čuvanje tendera u JSON fajl
        public void SacuvajTenderJSON(string path)
        {
            string json = JsonSerializer.Serialize(tenderi, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public void UcitajTenderJSON(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("JSON fajl nije pronađen", path);

            string json = File.ReadAllText(path);
            tenderi = JsonSerializer.Deserialize<List<Tender>>(json) ?? new List<Tender>();
            nextId = tenderi.Count == 0 ? 1 : tenderi.Max(t => t.Id) + 1;
        }

    }
}