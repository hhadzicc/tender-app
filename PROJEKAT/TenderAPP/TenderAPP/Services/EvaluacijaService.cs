using System;
using System.Collections.Generic;
using System.Linq;
using TenderiApp.Models;
using TenderiApp.Exceptions;
using TenderiApp.Interfaces;

namespace TenderiApp.Services
{
    public class EvaluacijaService : IEvaluacijaService
    {
        public EvaluacijaService()
        {
        }

        public List<Ponuda> EvaluirajPonude(Tender tender)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (tender.Status != StatusTendera.Zatvoren && tender.Status != StatusTendera.Otvoren)
            {
                throw new TenderException("Tender mora biti zatvoren prije evaluacije.");
            }

            if (tender.Ponude.Count == 0)
            {
                throw new TenderException("Nema ponuda za evaluaciju.");
            }

            List<Ponuda> validnePonude = tender.GetValidnePonude();

            if (validnePonude.Count == 0)
            {
                throw new TenderException("Nema validnih ponuda za evaluaciju.");
            }

            foreach (var ponuda in validnePonude)
            {
                IzracunajSkor(ponuda, tender.Kriteriji);
            }

            tender.Status = StatusTendera.Evaluiran;

            return RangirajPonude(validnePonude);
        }

        public double IzracunajSkor(Ponuda ponuda, KriterijiTendera kriteriji)
        {
            if (ponuda == null)
                throw new ArgumentNullException(nameof(ponuda));

            return ponuda.IzracunajSkor(kriteriji);
        }

        public Ponuda? OdrediPobjednika(Tender tender)
        {
            if (tender == null)
            {
                throw new ArgumentNullException(nameof(tender));
            }

            if (tender.Status != StatusTendera.Evaluiran)
            {
                throw new TenderException("Tender mora biti evaluiran prije određivanja pobjednika.");
            }

            List<Ponuda> validnePonude = tender.GetValidnePonude();

            if (validnePonude.Count == 0)
            {
                return null;
            }

            Ponuda pobjednik = validnePonude.OrderByDescending(p => p.UkupanSkor).First();

            tender.Status = StatusTendera.Zavrsen;

            return pobjednik;
        }

        public List<Ponuda> RangirajPonude(List<Ponuda> ponude)
        {
            if (ponude == null || ponude.Count == 0)
            {
                return new List<Ponuda>();
            }

            return ponude.OrderByDescending(p => p.UkupanSkor).ToList();
        }
    }
}