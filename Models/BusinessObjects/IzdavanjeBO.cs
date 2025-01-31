using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class IzdavanjeBO
    {
        public int IzdavanjeId { get; set; }

        public string? NaseljeIzdavanja { get; set; }

        public string? NazivIzdavaca { get; set; }

        public decimal? GodinaIzdavanja { get; set; }

        public IzdavanjeBO() { }

        public IzdavanjeBO(IzdavanjeGradje izdavanje)
        {
            this.IzdavanjeId = izdavanje.IzdavanjeId;
            this.NaseljeIzdavanja = izdavanje.NaseljeIzdavanja;
            this.NazivIzdavaca = izdavanje.NazivIzdavaca;
            this.GodinaIzdavanja = izdavanje.GodinaIzdavanja;
        }
    }
}
