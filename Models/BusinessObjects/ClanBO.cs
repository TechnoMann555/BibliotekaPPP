namespace BibliotekaPPP.Models.BusinessObjects
{
    public class ClanBO
    {
        public int ClanId { get; set; }

        // Clanski podaci
        public string Jcb { get; set; } = null!;

        public DateOnly DatumUclanjenja { get; set; }

        // Licni podaci
        public string BrLicneKarte { get; set; } = null!;

        public string ImePrezime { get; set; } = null!;

        public DateOnly DatumRodjenja { get; set; }

        public string ImeRoditelja { get; set; } = null!;

        public string AdresaStanovanja { get; set; } = null!;

        public string Zanimanje { get; set; } = null!;

        public string KontaktTelefon { get; set; } = null!;

        public string KontaktMejl { get; set; } = null!;

        public NalogBO? KorisnickiNalog { get; set; }
    }
}
