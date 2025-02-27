using System.ComponentModel.DataAnnotations;

namespace BibliotekaPPP.Models.ViewModels
{
    public class GradjaPretragaViewModel
    {
        public string? Naslov { get; set; }

        public string? Autor { get; set; }
        
        public string? Udk { get; set; }

        public string? Ogranak { get; set; }

        public string? NaseljeIzdavanja { get; set; }

        public string? NazivIzdavaca { get; set; }

        public decimal? GodinaIzdavanja { get; set; }

        public bool StatusDostupnosti { get; set; } = false;
    }
}
