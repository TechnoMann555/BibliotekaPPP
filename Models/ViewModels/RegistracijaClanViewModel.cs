using System.ComponentModel.DataAnnotations;

namespace BibliotekaPPP.Models.ViewModels
{
    public class RegistracijaClanViewModel
    {
        [Required]
        public string JCB { get; set; } = null!;

        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(40)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string Lozinka { get; set; } = null!;

        public Poruka? PorukaKorisniku { get; set; } = null;
    }
}
