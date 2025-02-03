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
        public Task<(NalogBO?, KorisnikLoginResult)> LoginClanKorisnik(string email, string lozinka);

        public NalogBO? TraziNalogPoID(int nalogID);

        // public Task<ClanBO> TraziClanaPoJCB(string JCB);
    }
}
