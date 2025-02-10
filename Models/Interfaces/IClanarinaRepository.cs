using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanarinaRepository
    {
        // [SK6] Prikaz informacija o članarinama
        // [SK7] Prikaz podataka o pozajmicama
        // [SK13] Prikaz podataka o članarinama člana
        // [SK14] Prikaz podataka o pozajmicama člana
        public Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoClanID(int clanID);

        // [SK16] Otvaranje nove članarine za određenog člana
        public Task<PrUslovaOtvClanarineResult> ProveriUsloveOtvaranjaClanarine(int clanID);

        // [SK16] Otvaranje nove članarine za određenog člana
        public Task OtvoriClanarinu(int clanID, decimal cena);
    }
}
