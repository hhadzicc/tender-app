using System.Collections.Generic;
using TenderiApp.Models;

namespace TenderiApp.Interfaces
{
    public interface IEvaluacijaService
    {
        List<Ponuda> EvaluirajPonude(Tender tender);
        double IzracunajSkor(Ponuda ponuda, KriterijiTendera kriteriji);
        Ponuda? OdrediPobjednika(Tender tender);
        List<Ponuda> RangirajPonude(List<Ponuda> ponude);
    }
}