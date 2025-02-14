using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class NalogRepository : INalogRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [SK2] Registracija člana na platformu biblioteke 
        public async Task<KreiranjeNalogaResult> KreirajKorisnickiNalog(string JCB, string email, string lozinka)
        {
            Clan? trazenClan = await bibliotekaContext.Clans.FirstOrDefaultAsync(clan => clan.Jcb == JCB);

            // Ne postoji clan sa unetim JCB
            if(trazenClan == null)
                return KreiranjeNalogaResult.ClanNePostoji;

            // Email ne odgovara pronadjenom clanu
            if(trazenClan.KontaktMejl != email)
                return KreiranjeNalogaResult.EmailNeOdgovara;

            // Clan vec ima korisnicki nalog
            if(trazenClan.KorisnickiNalogFk != null)
                return KreiranjeNalogaResult.NalogVecPostoji;

            // Kreiranje novog korisnickog naloga
            Nalog novNalog = new Nalog()
            {
                Lozinka = lozinka,
                Uloga = "Korisnik_Clan"
            };
            await bibliotekaContext.AddAsync(novNalog);
            await bibliotekaContext.SaveChangesAsync();

            // Vezivanje novog korisnickog naloga za clana biblioteke
            trazenClan.KorisnickiNalogFk = novNalog.NalogId;
            await bibliotekaContext.SaveChangesAsync();

            return KreiranjeNalogaResult.Uspeh;
        }

        // [SK3] Logovanje na korisnički nalog
        public async Task<(NalogBO?, LoginResult)> LoginKorisnikClan(string email, string lozinka)
        {
            Clan? trazenClan = await bibliotekaContext.Clans.FirstOrDefaultAsync(cl => cl.KontaktMejl == email);

            if(trazenClan == null || (trazenClan != null && trazenClan.KorisnickiNalogFk == null))
                return (null, LoginResult.NalogNePostoji);

            // Dohvatanje korisnickog naloga clana
            Nalog korisnickiNalog = await bibliotekaContext.Nalogs
                                          .Include(n => n.Clan)
                                          .FirstOrDefaultAsync(n => n.NalogId == trazenClan.KorisnickiNalogFk);

            // Uneta lozinka se ne poklapa sa lozinkom naloga
            if(korisnickiNalog.Lozinka != lozinka)
                return (null, LoginResult.PogresnaLozinka);

            NalogBO ulogovanNalog = new NalogBO(korisnickiNalog);

            return (ulogovanNalog, LoginResult.Uspeh);
        }

        // [SK9] Logovanje na administratorski nalog
        public async Task<(NalogBO?, LoginResult)> LoginAdminBibliotekar(string email, string lozinka)
        {
            Bibliotekar? trazenBibliotekar = await bibliotekaContext.Bibliotekars
                                             .FirstOrDefaultAsync(bib => bib.Email == email);

            if(trazenBibliotekar == null)
                return (null, LoginResult.NalogNePostoji);

            // Dohvatanje administratorskog naloga bibliotekara
            Nalog adminNalog = await bibliotekaContext.Nalogs
                                     .Include(n => n.Bibliotekar)
                                     .FirstOrDefaultAsync(n => n.NalogId == trazenBibliotekar.AdminNalogFk);

            // Uneta lozinka se ne poklapa sa lozinkom naloga
            if(adminNalog.Lozinka != lozinka)
                return (null, LoginResult.PogresnaLozinka);

            NalogBO ulogovanNalog = new NalogBO(adminNalog);

            return (ulogovanNalog, LoginResult.Uspeh);
        }

        // [SK19] Brisanje korisničkog naloga određenog člana
        public async Task<bool> BrisiKorisnickiNalog(int clanID)
        {
            Clan? clan = await bibliotekaContext.Clans
                               .Include(c => c.KorisnickiNalogFkNavigation)
                                   .ThenInclude(n => n.OcenaProcitaneGradjes)
                               .Where(c => c.ClanId == clanID)
                               .FirstOrDefaultAsync();

            if(clan == null)
                return false;
            if(clan.KorisnickiNalogFkNavigation == null)
                return false;

            clan.KorisnickiNalogFkNavigation.OcenaProcitaneGradjes.Clear();
            bibliotekaContext.Nalogs.Remove(clan.KorisnickiNalogFkNavigation);
            
            await bibliotekaContext.SaveChangesAsync();

            return true;
        }
    }
}
