using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IOcenaProcitaneGradjeRepository
    {
        public Task<OcenaProcitaneGradjeBO?> TraziOcenu(int gradjaID, int korisnickiNalogID);

        public Task<OcenjivanjeGradjeResult> OceniGradju(OcenaProcitaneGradjeBO ocena);
    }
}
