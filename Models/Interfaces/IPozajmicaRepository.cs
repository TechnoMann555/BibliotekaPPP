using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IPozajmicaRepository
    {
        // [SK7] Ocenjivanje procitane gradje
        public Task<bool> ClanProcitaoGradju(int gradjaID, int clanID);

        // [SK6] Prikaz podataka o pozajmicama
        // [SK12] Prikaz podataka o pozajmicama člana
        public Task<IEnumerable<PozajmicaBO>> TraziPozajmicePoClanarini(int clanFK, int rbrClanarine);

        // [SK15] Kreiranje pozajmice za određenog člana
        public Task<KreiranjePozajmiceResult> KreirajPozajmicu(int ogranakID, int gradjaID, int clanID);

        // [SK16] Razduživanje pozajmice za određenog člana
        public Task<PozajmicaBO?> TraziPozajmicuPoPK(int clanID, int clanarinaID, int pozajmicaRbr);

        // [SK16] Razduživanje pozajmice za određenog člana
        public Task<RazduzivanjePozajmiceResult> RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr);
    }
}
