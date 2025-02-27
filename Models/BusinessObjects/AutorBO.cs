using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class AutorBO
    {
        public int AutorId { get; set; }

        public string ImePrezime { get; set; } = null!;

        public AutorBO() { }

        public AutorBO(Autor autor)
        {
            this.AutorId = autor.AutorId;
            this.ImePrezime = autor.ImePrezime;
        }
    }
}
