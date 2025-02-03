using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanarinaRepository
    {
        public Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoNalogID(int nalogID);
    }
}
