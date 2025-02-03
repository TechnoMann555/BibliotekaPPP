using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class ClanarinaRepository : IClanarinaRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        public async Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoNalogID(int nalogID)
        {
            Nalog? korisnickiNalog = await bibliotekaContext.Nalogs
                                           .Include(n => n.Clan)
                                           .ThenInclude(c => c.Clanarinas)
                                           .FirstOrDefaultAsync(n => n.NalogId == nalogID);

            if(korisnickiNalog == null)
                return null;

            List<ClanarinaBO> listaClanarina = new List<ClanarinaBO>();

            foreach(Clanarina clanarina in korisnickiNalog.Clan.Clanarinas)
            {
                ClanarinaBO clanarinaBO = new ClanarinaBO(clanarina);
                listaClanarina.Add(clanarinaBO);
            }

            return listaClanarina;
        }
    }
}
