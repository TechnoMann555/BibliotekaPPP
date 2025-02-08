using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanarinaRepository
    {
        // [SK5] Prikaz informacija o članarinama
        // [SK6] Prikaz podataka o pozajmicama
        // [SK11] Prikaz podataka o članarinama člana
        public Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoClanID(int clanID);

        // [SK14] Otvaranje nove članarine za određenog člana
        public Task<PrUslovaOtvClanarineResult> ProveriUsloveOtvaranjaClanarine(int clanID);

        // [SK14] Otvaranje nove članarine za određenog člana
        public Task OtvoriClanarinu(int clanID, decimal cena);
    }
}
