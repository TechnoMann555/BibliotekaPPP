using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    // [SK8] Ocenjivanje pročitane građe
    public interface IOcenaProcitaneGradjeRepository
    {
        public Task<OcenaProcitaneGradjeBO?> TraziOcenu(int gradjaID, int korisnickiNalogID);

        public Task<OcenjivanjeGradjeResult> OceniGradju(int gradjaID, int clanskiKorisnickiNalogID, int ocena);
    }
}
