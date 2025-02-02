using BibliotekaPPP.Models.BusinessObjects;
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

        public async Task<NalogBO?> LoginClanKorisnik(string email, string lozinka)
        {
            Clan? trazenClan = await bibliotekaContext.Clans.FirstOrDefaultAsync(cl => cl.KontaktMejl == email);

            if(trazenClan == null)
                return null;

            if(trazenClan.KorisnickiNalogFk == null)
                return null;

            Nalog korisnickiNalog = await bibliotekaContext.Nalogs.FirstOrDefaultAsync(n => n.NalogId == trazenClan.KorisnickiNalogFk);

            if(korisnickiNalog.Lozinka != lozinka)
                return null;

            NalogBO ulogovanNalog = new NalogBO(korisnickiNalog);

            return ulogovanNalog;
        }

        public NalogBO? TraziNalogPoID(int nalogID)
        {
            Nalog? trazenNalog = bibliotekaContext.Nalogs.FirstOrDefault(n => n.NalogId == nalogID);

            if(trazenNalog == null)
                return null;

            NalogBO pronadjenNalog = new NalogBO(trazenNalog);

            return pronadjenNalog;
        }

        //public async Task<ClanBO> TraziClanaPoJCB(string JCB)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
