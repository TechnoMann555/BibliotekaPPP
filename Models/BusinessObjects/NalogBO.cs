using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class NalogBO
    {
        public int NalogId { get; set; }

        public string Uloga { get; set; } = null!;

        public NalogBO() { }

        public NalogBO(Nalog nalog)
        {
            this.NalogId = nalog.NalogId;
            this.Uloga = nalog.Uloga;
        }
    }
}
