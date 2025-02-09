using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class OgranakRepository : IOgranakRepository
    {
        BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [SK15] Kreiranje pozajmice za određenog člana
        public async Task<IEnumerable<OgranakBO>> VratiOgrankeSaSlobodnimPrimercimaGradje(int gradjaID)
        {
            // Vrati sve ogranke uz naselja tako da u njima postoji bar jedan primerak gradje
            // koji je slobodan za pozajmljivanje i odgovara trazenoj gradji 
            List<Ogranak> ogranci = await bibliotekaContext.Ogranaks
                                          .Include(og => og.NaseljeFkNavigation)
                                          .Where(og =>
                                              og.PrimerakGradjes.Any(pg =>
                                                  pg.GradjaFk == gradjaID &&
                                                  pg.Status == "slobodan"
                                              )
                                          ).ToListAsync();

            List<OgranakBO> listaOgranaka = new List<OgranakBO>();

            foreach(Ogranak ogranak in ogranci)
            {
                OgranakBO ogranakBO = new OgranakBO(ogranak);
                listaOgranaka.Add(ogranakBO);
            }

            return listaOgranaka;
        }
    }
}
