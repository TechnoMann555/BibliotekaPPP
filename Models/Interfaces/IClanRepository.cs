using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.ViewModels;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanRepository
    {
        // [SK6] Prikaz ličnih i članskih podataka
        // [SK13] Prikaz ličnih i članskih podataka o članu
        // [SK16] Upisivanje novog člana biblioteke
        public Task<ClanBO?> TraziClanaPoClanID(int clanID);

        // [SK12] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        // [SK18] Kreiranje pozajmice za određenog člana
        public Task<ClanBO?> TraziClanaPoJCB(string JCB);

        // [SK16] Upisivanje novog člana biblioteke
        public Task<(UpisivanjeClanaResult, int?)> UpisiClana(UpisClanaViewModel podaci);
    }
}
