﻿using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
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
    }
}
