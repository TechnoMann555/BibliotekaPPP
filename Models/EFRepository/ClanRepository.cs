using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class ClanRepository : IClanRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        #region Metode za prevodjenje tipova

        private async Task<ClanBO> ConvertClanToClanBO(Clan clan)
        {
            ClanBO clanBO = new ClanBO()
            {
                ClanId = clan.ClanId,
                Jcb = clan.Jcb,
                DatumUclanjenja = clan.DatumUclanjenja,
                BrLicneKarte = clan.BrLicneKarte,
                ImePrezime = clan.ImePrezime,
                DatumRodjenja = clan.DatumRodjenja,
                ImeRoditelja = clan.ImeRoditelja,
                AdresaStanovanja = clan.AdresaStanovanja,
                Zanimanje = clan.Zanimanje,
                KontaktTelefon = clan.KontaktTelefon,
                KontaktMejl = clan.KontaktMejl
            };

            Nalog? nalog = await bibliotekaContext.Nalogs.FirstOrDefaultAsync(n => n.NalogId == clan.KorisnickiNalogFk);
            clanBO.KorisnickiNalog = (nalog == null) ? null : new NalogBO(nalog);
            
            return clanBO;
        }

        #endregion

        // [SK4] Prikaz ličnih i članskih podataka
        public async Task<ClanBO?> TraziClanaPoClanID(int clanID)
        {
            Clan? clan = await bibliotekaContext.Clans.FirstOrDefaultAsync(c => c.ClanId == clanID);

            if(clan == null)
                return null;

            ClanBO nadjenClan = await ConvertClanToClanBO(clan);

            return nadjenClan;
        }

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        public async Task<ClanBO?> TraziClanaPoJCB(string JCB)
        {
            Clan? clan = await bibliotekaContext.Clans.FirstOrDefaultAsync(c => c.Jcb == JCB);

            if(clan == null)
                return null;

            ClanBO nadjenClan = await ConvertClanToClanBO(clan);

            return nadjenClan;
        }

        private async Task<string> KreirajNovJCB()
        {
            string? poslednjiJCB = await bibliotekaContext.Clans.MaxAsync(c => c.Jcb);
            int noviRedniBroj;

            if(poslednjiJCB == null)
            {
                noviRedniBroj = 1;
            }
            else
            {
                string godinaPoslednjegJCB = poslednjiJCB.Substring(poslednjiJCB.IndexOf('/') + 1);
                if(int.Parse(godinaPoslednjegJCB) < DateTime.Now.Year)
                {
                    noviRedniBroj = 1;
                }
                else
                {
                    int poslednjiRedniBroj = Convert.ToInt32(poslednjiJCB.Substring(0, poslednjiJCB.IndexOf('/')));
                    noviRedniBroj = poslednjiRedniBroj + 1;
                }
            }

            return $"{noviRedniBroj}/{DateTime.Now.Year}";
        }

        public async Task<UpisivanjeClanaResult> UpisiClana(UpisClanaViewModel podaci)
        {
            // Da li postoji clan sa istim brojem licne karte?
            bool licnaKartaPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.BrLicneKarte == podaci.BrLicneKarte);
            if(licnaKartaPostoji)
                return UpisivanjeClanaResult.BrLicneKartePostoji;

            // Da li postoji clan sa istim brojem telefona?
            bool brojTelefonaPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.KontaktTelefon == podaci.KontaktTelefon);
            if(brojTelefonaPostoji)
                return UpisivanjeClanaResult.BrTelefonaPostoji;

            // Da li postoji clan sa istom e-mail adresom?
            bool emailPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.KontaktMejl == podaci.KontaktMejl);
            if(emailPostoji)
                return UpisivanjeClanaResult.KontaktMejlPostoji;

            // Kreiranje JCB za novog clana
            string noviJCB = await KreirajNovJCB();

            Clan noviClan = new Clan()
            {
                Jcb = noviJCB,
                DatumUclanjenja = DateOnly.FromDateTime(DateTime.Now),
                BrLicneKarte = podaci.BrLicneKarte,
                ImePrezime = podaci.ImePrezime,
                DatumRodjenja = podaci.DatumRodjenja,
                ImeRoditelja = podaci.ImeRoditelja,
                AdresaStanovanja = podaci.AdresaStanovanja,
                Zanimanje = podaci.Zanimanje,
                KontaktTelefon = podaci.KontaktTelefon,
                KontaktMejl = podaci.KontaktMejl,
                KorisnickiNalogFk = null
            };
            bibliotekaContext.Clans.Add(noviClan);
            await bibliotekaContext.SaveChangesAsync();

            return UpisivanjeClanaResult.Uspeh;
        }
    }
}
