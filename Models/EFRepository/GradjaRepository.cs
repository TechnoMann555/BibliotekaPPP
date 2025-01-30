using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;

namespace BibliotekaPPP.Models.EFRepository
{
    public class GradjaRepository : IGradjaRepository
    {
        private BibliotekaContext bibliotekaEntities = new BibliotekaContext();

        #region Metode za prevodjenje tipova

        private GradjaBO ConvertGradjaToGradjaBO(Gradja gradja)
        {
            GradjaBO gradjaBO = new GradjaBO()
            {
                GradjaId = gradja.GradjaId,
                NaslovnaStranaPath = gradja.NaslovnaStranaPath,
                Naslov = gradja.Naslov,
                Opis = gradja.Opis,
                Isbn = gradja.Isbn,
                Udk = gradja.Udk
            };

            IzdavanjeGradje? izdavanje = bibliotekaEntities.IzdavanjeGradjes.FirstOrDefault(iz => iz.IzdavanjeId == gradja.IzdavanjeFk);

            if(izdavanje == null)
                gradjaBO.Izdavanje = null;
            else
                gradjaBO.Izdavanje = new IzdavanjeBO(izdavanje);

            foreach(Autor autor in gradja.AutorFks)
            {
                AutorBO autorBO = new AutorBO(autor);
                gradjaBO.Autori.Add(autorBO);
            }
            
            return gradjaBO;
        }

        #endregion

        // [1.1.1.1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        public IEnumerable<GradjaBO> TraziGradju(
            string? naslov = null,
            string? imePrezimeAutora = null,
            string? naseljeIzdavanja = null,
            decimal? godinaIzdavanja = null,
            string? udk = null,
            string? ogranak = null,
            bool statusDostupnosti = false
        )
        {
            // Koriscenjem IQueryable<T> filtriranje se vrsi na bazi umesto u memoriji
            IQueryable<Gradja> gradjaQuery = bibliotekaEntities.Gradjas;

            if(naslov != null)
                gradjaQuery = gradjaQuery.Where(g => g.Naslov.Contains(naslov));

            if(imePrezimeAutora != null)
                gradjaQuery = gradjaQuery.Where(g => g.AutorFks.Any(a => a.ImePrezime.Contains(imePrezimeAutora)));

            if(naseljeIzdavanja != null)
                gradjaQuery = gradjaQuery.Where(g => g.IzdavanjeFkNavigation.NaseljeIzdavanja == naseljeIzdavanja);

            if(godinaIzdavanja != null)
                gradjaQuery = gradjaQuery.Where(g => g.IzdavanjeFkNavigation.GodinaIzdavanja == godinaIzdavanja);

            if(udk != null)
                gradjaQuery = gradjaQuery.Where(g => g.Udk.StartsWith(udk));

            if(ogranak != null)
                gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p => p.OgranakFkNavigation.Naziv == ogranak));

            if(statusDostupnosti == true)
                gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p => p.StatusFkNavigation.Naziv == "slobodan"));

            List<Gradja> gradjaList = gradjaQuery.ToList();
            List<GradjaBO> gradjaBOList = new List<GradjaBO>();

            foreach(Gradja gradja in gradjaList)
            {
                GradjaBO gradjaBO = ConvertGradjaToGradjaBO(gradja);
                gradjaBOList.Add(gradjaBO);
            }

            return gradjaBOList;
        }
    }
}
