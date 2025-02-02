using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface INalogRepository
    {
        // [1.2.1.1] Registracija na platformu biblioteke
        public Task<KreiranjeNalogaResult> KreirajKorisnickiNalog(
            string JCB,
            string email,
            string lozinka
        );

        public Task<NalogBO?> LoginClanKorisnik(string email, string lozinka);

        public NalogBO? TraziNalogPoID(int nalogID);

        // public Task<ClanBO> TraziClanaPoJCB(string JCB);
    }
}
