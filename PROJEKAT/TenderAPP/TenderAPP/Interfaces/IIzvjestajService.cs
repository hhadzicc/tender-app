using System.Collections.Generic;
using TenderiApp.Models;

namespace TenderiApp.Interfaces
{
    public interface IIzvjestajService
    {
        Izvjestaj GenerisiIzvjestaj(Tender tender);
        string GenerisiDetaljanIzvjestaj(Tender tender);
        string GenerisiSazetak(List<Ponuda> ponude);
    }
}