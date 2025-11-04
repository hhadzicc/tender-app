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

            if (kriteriji == null)
                throw new ArgumentNullException(nameof(kriteriji));

            if (!ponuda.JeValidna)
            {
                ponuda.UkupanSkor = 0;
                return 0;
            }

            // INLINE kod iz IzracunajSkorCijene()
            // double skorCijene = IzracunajSkorCijene(ponuda.Cijena, kriteriji); // Zakomentirano
            double skorCijene;
            double rasponCijene = kriteriji.MaksimalnaCijena - kriteriji.MinimalnaCijena;

            if (rasponCijene <= 0)
            {
                skorCijene = 100;
            }
            else
            {
                double normalizovanaCijena = (ponuda.Cijena - kriteriji.MinimalnaCijena) / rasponCijene;

                if (normalizovanaCijena < 0)
                    normalizovanaCijena = 0;
                else if (normalizovanaCijena > 1)
                    normalizovanaCijena = 1;

                skorCijene = (1 - normalizovanaCijena) * 100;
            }

            // INLINE kod iz IzracunajSkorRoka()
            // double skorRoka = IzracunajSkorRoka(ponuda.RokIsporuke, kriteriji); // Zakomentirano
            double skorRoka;
            if (kriteriji.MaksimalanRok <= 0)
            {
                skorRoka = 100;
            }
            else
            {
                double normalizovanRok = (double)ponuda.RokIsporuke / kriteriji.MaksimalanRok;

                if (normalizovanRok < 0)
                    normalizovanRok = 0;
                else if (normalizovanRok > 1)
                    normalizovanRok = 1;

                skorRoka = (1 - normalizovanRok) * 100;
            }

            // INLINE kod iz IzracunajSkorGarancije()
            // double skorGarancije = IzracunajSkorGarancije(ponuda.Garancija, kriteriji); // Zakomentirano
            double skorGarancije;
            if (kriteriji.MinimalnaGarancija <= 0)
            {
                skorGarancije = ponuda.Garancija > 0 ? 100 : 0;
            }
            else
            {
                double ratio = (double)ponuda.Garancija / kriteriji.MinimalnaGarancija;

                if (ratio > 2.0)
                {
                    ratio = 2.0;
                }

                skorGarancije = ratio * 50;
            }

            // INLINE kod iz IzracunajSkorKvaliteta()
            // double skorKvaliteta = IzracunajSkorKvaliteta(ponuda.Kvalitet); // Zakomentirano
            double skorKvaliteta;
            if (ponuda.Kvalitet < 0)
                skorKvaliteta = 0;
            else if (ponuda.Kvalitet > 100)
                skorKvaliteta = 100;
            else
                skorKvaliteta = ponuda.Kvalitet;

            double ukupanSkor = (skorCijene * kriteriji.Ponderi.PonderCijene) +
                               (skorRoka * kriteriji.Ponderi.PonderRoka) +
                               (skorGarancije * kriteriji.Ponderi.PonderGarancije) +
                               (skorKvaliteta * kriteriji.Ponderi.PonderKvaliteta);
            ponuda.UkupanSkor = ukupanSkor;
            return ukupanSkor;
        }

        private double IzracunajSkorCijene(double cijena, KriterijiTendera kriteriji)
        {
            double raspon = kriteriji.MaksimalnaCijena - kriteriji.MinimalnaCijena;

            if (raspon <= 0)
            {
                return 100;
            }

            double normalizovanaCijena = (cijena - kriteriji.MinimalnaCijena) / raspon;

            if (normalizovanaCijena < 0)
            {
                normalizovanaCijena = 0;
            }
            else if (normalizovanaCijena > 1)
            {
                normalizovanaCijena = 1;
            }

            double skor = (1 - normalizovanaCijena) * 100;

            return skor;
        }

        private double IzracunajSkorRoka(int rok, KriterijiTendera kriteriji)
        {
            if (kriteriji.MaksimalanRok <= 0)
            {
                return 100;
            }

            double normalizovanRok = (double)rok / kriteriji.MaksimalanRok;

            if (normalizovanRok < 0)
            {
                normalizovanRok = 0;
            }
            else if (normalizovanRok > 1)
            {
                normalizovanRok = 1;
            }

            double skor = (1 - normalizovanRok) * 100;

            return skor;
        }

        private double IzracunajSkorGarancije(int garancija, KriterijiTendera kriteriji)
        {
            if (kriteriji.MinimalnaGarancija <= 0)
            {
                return garancija > 0 ? 100 : 0;
            }

            double ratio = (double)garancija / kriteriji.MinimalnaGarancija;

            if (ratio > 2.0)
            {
                ratio = 2.0;
            }

            double skor = ratio * 50;

            return skor;
        }

        private double IzracunajSkorKvaliteta(int kvalitet)
        {
            if (kvalitet < 0)
            {
                return 0;
            }

            if (kvalitet > 100)
            {
                return 100;
            }

            return kvalitet;
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