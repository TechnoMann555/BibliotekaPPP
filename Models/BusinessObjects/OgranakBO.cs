using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class OgranakBO
    {
        public int OgranakId { get; set; }

        public string Naziv { get; set; } = null!;

        public string Adresa { get; set; } = null!;

        public string NaseljeNaziv { get; set; } = null!;

        public OgranakBO() { }

        public OgranakBO(Ogranak ogranak)
        {
            this.OgranakId = ogranak.OgranakId;
            this.Naziv = ogranak.Naziv;
            this.Adresa = ogranak.Adresa;
            this.NaseljeNaziv = ogranak.NaseljeFkNavigation.Naziv;
        }
    }
}
