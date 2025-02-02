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

        // public Task<ClanBO> TraziClanaPoJCB(string JCB);
    }
}
