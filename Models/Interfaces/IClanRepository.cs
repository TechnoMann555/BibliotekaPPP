using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.ViewModels;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanRepository
    {
        // [SK5] Prikaz ličnih i članskih podataka
        // [SK12] Prikaz ličnih podataka o članu
        public Task<ClanBO?> TraziClanaPoClanID(int clanID);

        // [SK11] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        public Task<ClanBO?> TraziClanaPoJCB(string JCB);

        // [SK15] Upisivanje novog člana biblioteke
        public Task<(UpisivanjeClanaResult, int?)> UpisiClana(UpisClanaViewModel podaci);
    }
}
