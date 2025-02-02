using BibliotekaPPP.Models.BusinessObjects;

namespace BibliotekaPPP.Models.Interfaces
{
    public interface IGradjaRepository
    {
        // [SK1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        public Task<IEnumerable<GradjaBO>> TraziGradju(
            string? naslov = null,
            string? imePrezimeAutora = null,
            string? naseljeIzdavanja = null,
            string? nazivIzdavaca = null,
            decimal? godinaIzdavanja = null,
            string? udk = null,
            string? ogranak = null,
            bool statusDostupnosti = false
        );

        // [SK1] Prikaz podataka o specifičnoj građi
        public Task<GradjaBO?> TraziGradjuPoID(int gradjaID);
    }
}
