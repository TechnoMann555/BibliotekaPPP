using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IOgranakRepository
    {
        // [SK18] Kreiranje pozajmice za određenog člana
        public Task<IEnumerable<OgranakBO>> VratiOgrankeSaSlobodnimPrimercimaGradje(int gradjaID);
    }
}
