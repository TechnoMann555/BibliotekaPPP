using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IPozajmicaRepository
    {
        public Task<bool> ClanProcitaoGradju(int gradjaID, int clanID);

        // [SK6] Prikaz podataka o pozajmicama
        public Task<IEnumerable<PozajmicaBO>> TraziPozajmicePoClanarini(int clanFK, int rbrClanarine);

        public Task<KreiranjePozajmiceResult> KreirajPozajmicu(int ogranakID, int gradjaID, int clanID);

        public Task<PozajmicaBO?> TraziPozajmicuPoPK(int clanID, int clanarinaID, int pozajmicaRbr);

        public Task RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr);
    }
}
