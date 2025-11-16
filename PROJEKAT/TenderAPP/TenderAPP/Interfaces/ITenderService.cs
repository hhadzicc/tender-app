using System;
using System.Collections.Generic;
using TenderiApp.Models;

namespace TenderiApp.Interfaces
{
    public interface ITenderService
    {
        Tender KreirajTender(string naziv, string opis, DateTime rok, KriterijiTendera kriteriji);
        Tender? GetTender(int id);
        List<Tender> GetSviTenderi();
        List<Tender> GetAktivniTenderi();
        bool ZatvoriTender(int tenderId);
        void SacuvajTenderJSON(string path); // Za potrebe 
        void UcitajTenderJSON(string path);
    }
}