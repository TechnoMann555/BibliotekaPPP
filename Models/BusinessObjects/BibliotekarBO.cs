namespace BibliotekaPPP.Models.BusinessObjects
{
    public class BibliotekarBO
    {
        public int BibliotekarId { get; set; }

        public string Jbb { get; set; } = null!;

        public string ImePrezime { get; set; } = null!;

        public string Email { get; set; } = null!;

        public OgranakBO OgranakRadnoMesto { get; set; }

        public NalogBO AdminNalog { get; set; }

        #warning Refaktorisati kada bude bilo bitno
        public BibliotekarBO()
        {
            this.OgranakRadnoMesto = new OgranakBO();
            this.AdminNalog = new NalogBO();
        }

    }
}
