using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class PozajmicaRepository : IPozajmicaRepository
    {
        BibliotekaContext bibliotekaContext = new BibliotekaContext();

        public async Task<bool> ClanProcitaoGradju(int gradjaID, int clanID)
        {
            bool procitao = await bibliotekaContext.Pozajmicas.AnyAsync(
                p => p.ClanarinaClanFk == clanID &&
                     p.PrimerakGradjeGradjaFk == gradjaID &&
                     p.DatumRazduzenja != null
            );

            return procitao;
        }

        public async Task<IEnumerable<PozajmicaBO>> TraziPozajmicePoClanarini(int clanFK, int rbrClanarine)
        {
            List<Pozajmica> pozajmice = await bibliotekaContext.Pozajmicas
                .Include(p => p.PrimerakGradje)
                    .ThenInclude(pg => pg.OgranakFkNavigation)
                .Include(p => p.PrimerakGradje)
                    .ThenInclude(pg => pg.GradjaFkNavigation)
                .Where(p => 
                p.ClanarinaFk == rbrClanarine && 
                p.ClanarinaClanFk == clanFK
            ).ToListAsync();

            List<PozajmicaBO> listaPozajmica = new List<PozajmicaBO>();

            foreach(Pozajmica pozajmica in pozajmice)
            {
                PozajmicaBO pozajmicaBO = new PozajmicaBO(pozajmica);
                listaPozajmica.Add(pozajmicaBO);
            }

            return listaPozajmica;
        }
    }
}
