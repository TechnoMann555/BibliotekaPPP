using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IClanarinaRepository
    {
        // [SK5] Prikaz informacija o članarinama
        public Task<IEnumerable<ClanarinaBO>?> TraziClanarinePoClanID(int clanID);
    }
}
