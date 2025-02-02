using System.ComponentModel.DataAnnotations;

namespace BibliotekaPPP.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = null!;

        public Poruka? PorukaKorisniku { get; set; } = null;
    }
}
