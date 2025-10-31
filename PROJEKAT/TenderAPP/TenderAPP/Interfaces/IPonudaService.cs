using System.Collections.Generic;
using TenderiApp.Models;

namespace TenderiApp.Interfaces
{
    public interface IPonudaService
    {
        Ponuda PrijaviPonudu(Tender tender, Ponudjac ponudjac, double cijena, int rok, int garancija, int kvalitet);
        bool ValidirajPonudu(Ponuda ponuda, KriterijiTendera kriteriji);
        List<Ponuda> GetPonudeZaTender(int tenderId);
        List<Ponuda> FiltrirajPonude(int? tenderId = null, double? minCijena = null, double? maxCijena = null, int? maxRok = null, int? minKvalitet = null);
    }
}