using System.ComponentModel.DataAnnotations;

namespace BibliotekaPPP.Models.ViewModels
{
    public class UpisClanaViewModel
    {
        [Required(ErrorMessage = "Broj lične karte je obavezan.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Broj lične karte je neispravnog formata.")]
        [Length(9, 9, ErrorMessage = "Broj lične karte mora sadržati 9 cifara.")]
        public string BrLicneKarte { get; set; } = null!;

        [Required(ErrorMessage = "Ime i prezime je obavezno.")]
        [MaxLength(80)]
        public string ImePrezime { get; set; } = null!;

        [Required(ErrorMessage = "Datum rođenja je obavezno.")]
        [DataType(DataType.Date)]
        public DateOnly DatumRodjenja { get; set; }

        [Required(ErrorMessage = "Ime roditelja je obavezno.")]
        [MaxLength(40)]
        public string ImeRoditelja { get; set; } = null!;

        [Required(ErrorMessage = "Adresa stanovanja je obavezna.")]
        [MaxLength(80)]
        public string AdresaStanovanja { get; set; } = null!;

        [Required(ErrorMessage = "Zanimanje je obavezno.")]
        [MaxLength(50)]
        public string Zanimanje { get; set; } = null!;

        [Required(ErrorMessage = "Kontakt telefon je obavezan.")]
        [RegularExpression(@"^(\+381)(6\d)(\d{6,7})$", ErrorMessage = "Kontakt telefon je neispravnog formata.")]
        public string KontaktTelefon { get; set; } = null!;

        [Required(ErrorMessage = "Kontakt mejl je obavezan.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Kontakt mejl je neispravnog formata.")]
        public string KontaktMejl { get; set; } = null!;

    }
}
