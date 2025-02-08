using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    // [SK7] Ocenjivanje procitane gradje
    public interface IOcenaProcitaneGradjeRepository
    {
        public Task<OcenaProcitaneGradjeBO?> TraziOcenu(int gradjaID, int korisnickiNalogID);

        public Task<OcenjivanjeGradjeResult> OceniGradju(OcenaProcitaneGradjeBO ocena);
    }
}
