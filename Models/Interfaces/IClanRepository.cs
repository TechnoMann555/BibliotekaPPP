using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanRepository
    {
        public Task<ClanBO?> TraziClanaPoNalogID(int nalogID);
    }
}
