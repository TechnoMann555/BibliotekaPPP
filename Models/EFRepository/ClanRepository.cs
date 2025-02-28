﻿using BibliotekaPPP.Models.BusinessObjects;
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

        // [SK6] Prikaz ličnih i članskih podataka
        // [SK13] Prikaz ličnih i članskih podataka o članu
        // [SK16] Upisivanje novog člana biblioteke
        public async Task<ClanBO?> TraziClanaPoClanID(int clanID)
        {
            Clan? clan = await bibliotekaContext.Clans.FirstOrDefaultAsync(c => c.ClanId == clanID);

            if(clan == null)
                return null;

            ClanBO nadjenClan = await ConvertClanToClanBO(clan);

            return nadjenClan;
        }

        // [SK12] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        // [SK18] Kreiranje pozajmice za određenog člana
        public async Task<ClanBO?> TraziClanaPoJCB(string JCB)
        {
            Clan? clan = await bibliotekaContext.Clans.FirstOrDefaultAsync(c => c.Jcb == JCB);

            if(clan == null)
                return null;

            ClanBO nadjenClan = await ConvertClanToClanBO(clan);

            return nadjenClan;
        }

        // [SK16] Upisivanje novog člana biblioteke
        private async Task<string> KreirajNovJCB()
        {
            string? poslednjiJCB = await bibliotekaContext.Clans
                                         .OrderByDescending(c => c.DatumUclanjenja)
                                         // Posto je JCB tipa string, moramo sortirati po duzini JCB
                                         // da bismo pravilno sortirali prema rednom broju
                                         .ThenByDescending(c => c.Jcb.Length)
                                         .ThenByDescending(c => c.Jcb)
                                         .Select(c => c.Jcb)
                                         .FirstOrDefaultAsync();

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

        // [SK16] Upisivanje novog člana biblioteke
        public async Task<(UpisivanjeClanaResult, int?)> UpisiClana(UpisClanaViewModel podaci)
        {
            // Da li postoji clan sa istim brojem licne karte?
            bool licnaKartaPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.BrLicneKarte == podaci.BrLicneKarte);
            if(licnaKartaPostoji)
                return (UpisivanjeClanaResult.BrLicneKartePostoji, null);

            // Da li postoji clan sa istim brojem telefona?
            bool brojTelefonaPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.KontaktTelefon == podaci.KontaktTelefon);
            if(brojTelefonaPostoji)
                return (UpisivanjeClanaResult.BrTelefonaPostoji, null);

            // Da li postoji clan sa istom e-mail adresom?
            bool emailPostoji = await bibliotekaContext.Clans.AnyAsync(c => c.KontaktMejl == podaci.KontaktMejl);
            if(emailPostoji)
                return (UpisivanjeClanaResult.KontaktMejlPostoji, null);

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
            await bibliotekaContext.Clans.AddAsync(noviClan);
            await bibliotekaContext.SaveChangesAsync();

            return (UpisivanjeClanaResult.Uspeh, noviClan.ClanId);
        }
    }
}
