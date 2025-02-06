using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IOgranakRepository
    {
        public Task<IEnumerable<OgranakBO>> VratiOgrankeSaSlobodnimPrimercimaGradje(int gradjaID);
    }
}
