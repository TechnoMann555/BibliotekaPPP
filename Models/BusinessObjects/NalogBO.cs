using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class NalogBO
    {
        public int NalogId { get; set; }

        public string Uloga { get; set; } = null!;

        public int? ClanId { get; set; } = null;

        public int? BibliotekarId { get; set; } = null;

        public NalogBO() { }

        public NalogBO(Nalog nalog)
        {
            this.NalogId = nalog.NalogId;
            this.Uloga = nalog.Uloga;

            if(nalog.Clan != null)
                this.ClanId = nalog.Clan.ClanId;
            else if(nalog.Bibliotekar != null)
                this.BibliotekarId = nalog.Bibliotekar.BibliotekarId;
        }
    }
}
