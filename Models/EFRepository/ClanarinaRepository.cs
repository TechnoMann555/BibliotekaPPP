using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class ClanarinaRepository : IClanarinaRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [SK5] Prikaz informacija o članarinama
        public async Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoClanID(int clanID)
        {
            Clan? clan = await bibliotekaContext.Clans
                               .Include(c => c.Clanarinas)
                               .FirstOrDefaultAsync(c => c.ClanId == clanID);

            if(clan == null)
                return null;

            List<ClanarinaBO> listaClanarina = new List<ClanarinaBO>();

            foreach(Clanarina clanarina in clan.Clanarinas)
            {
                List<int> procitanaGradja = await bibliotekaContext.Pozajmicas
                                            .Where(p => p.ClanarinaClanFk == clanID &&
                                                        p.ClanarinaFk == clanarina.Rbr &&
                                                        p.DatumRazduzenja != null)
                                            .Select(p => p.PrimerakGradjeGradjaFk)
                                            .ToListAsync();
                                               
                ClanarinaBO clanarinaBO = new ClanarinaBO(clanarina, procitanaGradja);

                listaClanarina.Add(clanarinaBO);
            }

            return listaClanarina;
        }
    }
}
