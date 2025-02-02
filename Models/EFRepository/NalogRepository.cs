using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class NalogRepository : INalogRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [1.2.1.1] Registracija na platformu biblioteke
        public async Task<KreiranjeNalogaResult> KreirajKorisnickiNalog(
            string JCB,
            string email,
            string lozinka
        )
        {
            Clan? trazenClan = await bibliotekaContext.Clans.FirstOrDefaultAsync(clan => clan.Jcb == JCB);

            if(trazenClan == null)
                return KreiranjeNalogaResult.ClanNePostoji;

            if(trazenClan.KontaktMejl != email)
                return KreiranjeNalogaResult.EmailNeOdgovara;

            if(trazenClan.KorisnickiNalogFk != null)
                return KreiranjeNalogaResult.NalogVecPostoji;

            Nalog novNalog = new Nalog()
            {
                Lozinka = lozinka,
                Uloga = "Korisnik_Clan"
            };

            bibliotekaContext.Add(novNalog);
            await bibliotekaContext.SaveChangesAsync();

            trazenClan.KorisnickiNalogFk = novNalog.NalogId;
            await bibliotekaContext.SaveChangesAsync();

            return KreiranjeNalogaResult.Uspeh;
        }

        //public async Task<ClanBO> TraziClanaPoJCB(string JCB)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
