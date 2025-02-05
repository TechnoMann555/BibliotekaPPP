using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.ViewModels;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanRepository
    {
        // [SK4] Prikaz ličnih i članskih podataka
        public Task<ClanBO?> TraziClanaPoClanID(int clanID);

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        public Task<ClanBO?> TraziClanaPoJCB(string JCB);

        // [SK13] Upisivanje novog člana biblioteke
        public Task<(UpisivanjeClanaResult, int?)> UpisiClana(UpisClanaViewModel podaci);
    }
}
