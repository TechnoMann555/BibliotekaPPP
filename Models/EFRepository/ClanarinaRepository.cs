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
        // [SK6] Prikaz podataka o pozajmicama
        // [SK11] Prikaz podataka o članarinama člana
        // [SK12] Prikaz podataka o pozajmicama člana
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
                ClanarinaBO clanarinaBO = new ClanarinaBO(clanarina);
                listaClanarina.Add(clanarinaBO);
            }

            return listaClanarina;
        }

        // [SK14] Otvaranje nove članarine za određenog člana
        public async Task<PrUslovaOtvClanarineResult> ProveriUsloveOtvaranjaClanarine(int clanID)
        {
            bool imaTekucuClanarinu = await bibliotekaContext.Clanarinas.AnyAsync(cl =>
                cl.ClanFk == clanID &&
                cl.DatumZavrsetka >= DateOnly.FromDateTime(DateTime.Now)
            );

            if(imaTekucuClanarinu)
                return PrUslovaOtvClanarineResult.PostojiTekucaClanarina;

            bool imaNerazduzenihPozajmica = await bibliotekaContext.Pozajmicas.AnyAsync(poz =>
                poz.ClanarinaClanFk == clanID &&
                poz.DatumRazduzenja == null
            );

            if(imaNerazduzenihPozajmica)
                return PrUslovaOtvClanarineResult.PostojeNerazduzenePozajmice;

            return PrUslovaOtvClanarineResult.IspunjeniUslovi;
        }

        // [SK14] Otvaranje nove članarine za određenog člana
        public async Task OtvoriClanarinu(int clanID, decimal cena)
        {
            Clanarina novaClanarina = new Clanarina()
            {
                ClanFk = clanID,
                DatumPocetka = DateOnly.FromDateTime(DateTime.Now),
                DatumZavrsetka = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                Cena = cena
            };

            await bibliotekaContext.Clanarinas.AddAsync(novaClanarina);
            await bibliotekaContext.SaveChangesAsync();
        }
    }
}
