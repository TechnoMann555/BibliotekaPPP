using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class GradjaRepository : IGradjaRepository
    {
        private BibliotekaContext bibliotekaEntities = new BibliotekaContext();

        #region Metode za prevodjenje tipova

        private GradjaBO ConvertGradjaToGradjaBO(Gradja gradja)
        {
            // Ako postoji bar jedan slobodan primerak gradje, azuriraj status dostupnosti da je true
            bool statusDostupnosti = false;
            if(gradja.PrimerakGradjes.Any(p => p.StatusFkNavigation.Naziv == "slobodan"))
                statusDostupnosti = true;

            GradjaBO gradjaBO = new GradjaBO()
            {
                GradjaId = gradja.GradjaId,
                NaslovnaStranaPath = gradja.NaslovnaStranaPath,
                Naslov = gradja.Naslov,
                Opis = gradja.Opis,
                Isbn = gradja.Isbn,
                Udk = gradja.Udk,
                ImaSlobodnihPrimeraka = statusDostupnosti
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
            string? nazivIzdavaca = null,
            decimal? godinaIzdavanja = null,
            string? udk = null,
            string? ogranak = null,
            bool statusDostupnosti = false
        )
        {
            // Koriscenjem IQueryable<T> filtriranje se vrsi na bazi umesto u memoriji
            IQueryable<Gradja> gradjaQuery = bibliotekaEntities.Gradjas;

            // Ako naslov gradje sadrzi unet string, ukljuci gradju
            if(naslov != null)
                gradjaQuery = gradjaQuery.Where(g => g.Naslov.Contains(naslov));


            // Ako postoji bar jedan autor gradje cije ime i prezime sadrzi unet string, ukljuci gradju
            if(imePrezimeAutora != null)
                gradjaQuery = gradjaQuery.Where(g => g.AutorFks.Any(a => a.ImePrezime.Contains(imePrezimeAutora)));

            // Ako naselje izdavanja gradje sadrzi unet string, ukljuci gradju
            if(naseljeIzdavanja != null)
                gradjaQuery = gradjaQuery.Where(
                    g => g.IzdavanjeFkNavigation.NaseljeIzdavanja != null &&
                    g.IzdavanjeFkNavigation.NaseljeIzdavanja.Contains(naseljeIzdavanja)
                );

            // Ako naziv izdavaca gradje sadrzi unet string, ukljuci gradju
            if(nazivIzdavaca != null)
                gradjaQuery = gradjaQuery.Where(
                    g => g.IzdavanjeFkNavigation.NazivIzdavaca != null &&
                    g.IzdavanjeFkNavigation.NazivIzdavaca.Contains(nazivIzdavaca)
                );

            // Ako je godina izdavanja gradje jednaka unetom godinom, ukljuci gradju
            if(godinaIzdavanja != null)
                gradjaQuery = gradjaQuery.Where(g => g.IzdavanjeFkNavigation.GodinaIzdavanja == godinaIzdavanja);

            // Ako UDK broj gradje pocinje sa unetim stringom, ukljuci gradju
            if(udk != null)
                gradjaQuery = gradjaQuery.Where(g => g.Udk.StartsWith(udk));

            // TODO: Ne radi sa određenim slovima ćirilice (bar sa 'ć')
            // Ako postoji bar jedan primerak gradje koji se nalazi u ogranku sa nazivom koji sadrzi unet string, ukljuci gradju
            if(ogranak != null)
                gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p => p.OgranakFkNavigation.Naziv.Contains(ogranak)));

            // Ako postoji bar jedan primerak gradje koji je slobodan za pozajmljivanje, ukljuci gradju
            if(statusDostupnosti == true)
                gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p => p.StatusFkNavigation.Naziv == "slobodan"));

            // Ukljucivanje podataka o autoru
            gradjaQuery = gradjaQuery.Include(g => g.AutorFks);

            // Ukljucivanje podataka o primercima gradje i statusima primeraka gradje
            gradjaQuery = gradjaQuery.Include(g => g.PrimerakGradjes).ThenInclude(p => p.StatusFkNavigation);

            // Izvrsavanje upita
            List<Gradja> gradjaList = gradjaQuery.ToList();
            List<GradjaBO> gradjaBOList = new List<GradjaBO>();

            // Punjenje GradjaBO liste
            foreach(Gradja gradja in gradjaList)
            {
                GradjaBO gradjaBO = ConvertGradjaToGradjaBO(gradja);
                gradjaBOList.Add(gradjaBO);
            }

            return gradjaBOList;
        }

        // [1.1.1.2] Prikaz podataka o specifičnoj građi
        public GradjaBO? TraziGradjuPoID(int gradjaID)
        {
            Gradja? trazenaGradja = bibliotekaEntities.Gradjas
                                    .Include(g => g.AutorFks)
                                    .FirstOrDefault(g => g.GradjaId == gradjaID);

            if(trazenaGradja == null)
                return null;

            GradjaBO pronadjenaGradja = ConvertGradjaToGradjaBO(trazenaGradja);
            return pronadjenaGradja;
        }
    }
}
