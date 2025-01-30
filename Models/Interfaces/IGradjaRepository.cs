using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IGradjaRepository
    {
        // [1.1.1.1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        public IEnumerable<GradjaBO> TraziGradju(
            string? naslov = null,
            string? imePrezimeAutora = null,
            string? naseljeIzdavanja = null,
            decimal? godinaIzdavanja = null,
            string? udk = null,
            string? ogranak = null,
            bool statusDostupnosti = false
        );
    }
}
