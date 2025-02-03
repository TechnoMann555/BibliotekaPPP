using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanRepository
    {
        // [SK4] Prikaz ličnih i članskih podataka
        public Task<ClanBO?> TraziClanaPoClanID(int clanID);
    }
}
