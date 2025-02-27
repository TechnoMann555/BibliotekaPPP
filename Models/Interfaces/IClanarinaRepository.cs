using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanarinaRepository
    {
        // [SK7] Prikaz informacija o članarinama
        // [SK8] Prikaz podataka o pozajmicama
        // [SK14] Prikaz podataka o članarinama člana
        // [SK15] Prikaz podataka o pozajmicama člana
        public Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoClanID(int clanID);

        // [SK17] Otvaranje nove članarine za određenog člana
        public Task<PrUslovaOtvClanarineResult> ProveriUsloveOtvaranjaClanarine(int clanID);

        // [SK17] Otvaranje nove članarine za određenog člana
        public Task OtvoriClanarinu(int clanID, decimal cena);
    }
}
