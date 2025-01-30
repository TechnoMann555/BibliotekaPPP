using BibliotekaPPP.Models.DatabaseObjects;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class GradjaBO
    {
        public int GradjaId { get; set; }

        public string NaslovnaStranaPath { get; set; } = null!;

        public string Naslov { get; set; } = null!;

        public string? Opis { get; set; }

        public string Isbn { get; set; } = null!;

        public string Udk { get; set; } = null!;

        public IzdavanjeBO? Izdavanje { get; set; }

        public List<AutorBO> Autori { get; set; }

        public GradjaBO()
        {
            Autori = new List<AutorBO>();
        }
    }
}
