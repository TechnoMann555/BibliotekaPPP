using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class ClanRepository : IClanRepository
    {
        private BibliotekaContext bibliotekaContext = new BibliotekaContext();

        #region Metode za prevodjenje tipova

        private async Task<ClanBO> ConvertClanToClanBO(Clan clan, NalogBO? korisnickiNalog = null)
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

            if(korisnickiNalog != null)
                clanBO.KorisnickiNalog = korisnickiNalog;
            else
            {
                Nalog? nalog = await bibliotekaContext.Nalogs.FirstOrDefaultAsync(n => n.NalogId == clan.KorisnickiNalogFk);
                clanBO.KorisnickiNalog = (nalog == null) ? null : new NalogBO(nalog);
            }

            return clanBO;
        }

        #endregion

        public async Task<ClanBO?> TraziClanaPoNalogID(int nalogID)
        {
            Nalog? korisnickiNalog = await bibliotekaContext.Nalogs.Include(n => n.Clan).FirstOrDefaultAsync(n => n.NalogId == nalogID);

            if(korisnickiNalog == null)
                return null;

            ClanBO nadjenClan = await ConvertClanToClanBO(korisnickiNalog.Clan, new NalogBO(korisnickiNalog));

            return nadjenClan;
        }
    }
}
