using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IPozajmicaRepository
    {
        // [SK8] Ocenjivanje pročitane građe
        public Task<bool> ClanProcitaoGradju(int gradjaID, int clanID);

        // [SK7] Prikaz podataka o pozajmicama
        // [SK14] Prikaz podataka o pozajmicama člana
        public Task<IEnumerable<PozajmicaBO>> TraziPozajmicePoClanarini(int clanFK, int rbrClanarine);

        // [SK17] Kreiranje pozajmice za određenog člana
        public Task<(KreiranjePozajmiceResult, int?)> KreirajPozajmicu(int ogranakID, int gradjaID, int clanID);

        // [SK18] Razduživanje pozajmice za određenog člana
        public Task<PozajmicaBO?> TraziPozajmicuPoPK(int clanID, int clanarinaID, int pozajmicaRbr);

        // [SK18] Razduživanje pozajmice za određenog člana
        public Task<RazduzivanjePozajmiceResult> RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr);
    }
}
