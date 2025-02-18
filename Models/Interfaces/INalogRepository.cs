using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface INalogRepository
    {
        // [SK2] Registracija člana na platformu biblioteke 
        public Task<KreiranjeNalogaResult> KreirajKorisnickiNalog(string JCB, string email, string lozinka);

        // [SK4] Logovanje na korisnički nalog
        public Task<(NalogBO?, LoginResult)> LoginKorisnikClan(string email, string lozinka);

        // [SK10] Logovanje na administratorski nalog
        public Task<(NalogBO?, LoginResult)> LoginAdminBibliotekar(string email, string lozinka);

        // [SK20] Brisanje korisničkog naloga određenog člana
        public Task<bool> BrisiKorisnickiNalog(int clanID);
    }
}
