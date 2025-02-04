using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface INalogRepository
    {
        // [SK2] Registracija clana na platformu biblioteke 
        public Task<KreiranjeNalogaResult> KreirajKorisnickiNalog(
            string JCB,
            string email,
            string lozinka
        );

        // [SK3] Logovanje na korisnički nalog
        public Task<(NalogBO?, LoginResult)> LoginKorisnikClan(string email, string lozinka);

        // [SK8] Logovanje na administratorski nalog
        public Task<(NalogBO?, LoginResult)> LoginAdminBibliotekar(string email, string lozinka);

        public NalogBO? TraziNalogPoID(int nalogID);


        // public Task<ClanBO> TraziClanaPoJCB(string JCB);
    }
}
