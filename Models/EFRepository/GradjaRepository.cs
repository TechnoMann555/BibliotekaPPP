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

        private async Task<GradjaBO> ConvertGradjaToGradjaBO(Gradja gradja, string? nazivOgranka = null)
        {
            bool statusDostupnosti = false;

            if(nazivOgranka == null)
            {
                if(gradja.PrimerakGradjes.Any(p => p.Status == "slobodan"))
                    statusDostupnosti = true;
            }
            else
            {
                if(gradja.PrimerakGradjes.Any(p =>
                    p.Status == "slobodan" &&
                    p.OgranakFkNavigation.Naziv.Contains(nazivOgranka))
                )
                    statusDostupnosti = true;
            }

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

            IzdavanjeGradje? izdavanje = await bibliotekaEntities.IzdavanjeGradjes
                                               .FirstOrDefaultAsync(iz => iz.IzdavanjeId == gradja.IzdavanjeFk);

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

        // [SK1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        public async Task<IEnumerable<GradjaBO>> TraziGradju(
            string? naslov = null,
            string? imePrezimeAutora = null,
            string? naseljeIzdavanja = null,
            string? nazivIzdavaca = null,
            decimal? godinaIzdavanja = null,
            string? udk = null,
            string? nazivOgranka = null,
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

            // Filtriranje gradje po ogranku i statusu dostupnosti primeraka gradje
            if(nazivOgranka != null || statusDostupnosti == true)
            {
                // Ako postoji bar jedan primerak gradje koji je slobodan za pozajmljivanje
                // i nalazi se u ogranku ciji naziv pocinje sa unetim stringom, ukljuci gradju
                if(nazivOgranka != null && statusDostupnosti == true)
                {
                    gradjaQuery = gradjaQuery.Where(g =>
                                      g.PrimerakGradjes.Any(pg =>
                                          pg.OgranakFkNavigation.Naziv.Contains(nazivOgranka) &&
                                          pg.Status == "slobodan"
                                      )
                                  );
                }
                // Ako postoji bar jedan primerak gradje koji se nalazi u ogranku ciji naziv
                // pocinje sa unetim stringom, ukljuci gradju
                else if(nazivOgranka != null)
                {
                    gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p =>
                            p.OgranakFkNavigation.Naziv.Contains(nazivOgranka)
                        )
                    );
                }
                // Ako postoji bar jedan primerak gradje koji je slobodan za pozajmljivanje,
                // ukljuci gradju
                else
                {
                    gradjaQuery = gradjaQuery.Where(g => g.PrimerakGradjes.Any(p => p.Status == "slobodan"));
                }
            }

            // Ukljucivanje podataka o autoru
            gradjaQuery = gradjaQuery.Include(g => g.AutorFks);

            // Ako je vrsena pretraga po nazivu ogranka gde se nalaze primerci gradje,
            // ukljuci podatke o primeracima gradje uz podatke o njihovim ograncima
            if(nazivOgranka != null)
            {
                gradjaQuery = gradjaQuery
                              .Include(g => g.PrimerakGradjes)
                              .ThenInclude(pg => pg.OgranakFkNavigation);
            }
            // Ako nije vrsena pretraga po nazivu ogranka, ukljuci samo podatke o primercima gradje
            else
            {
                gradjaQuery = gradjaQuery.Include(g => g.PrimerakGradjes);
            }

            // Izvrsavanje upita
            List<Gradja> gradjaList = await gradjaQuery.ToListAsync();
            
            // Kreiranje i punjenje GradjaBO liste
            List<GradjaBO> gradjaBOList = new List<GradjaBO>();
            foreach(Gradja gradja in gradjaList)
            {
                GradjaBO gradjaBO = await ConvertGradjaToGradjaBO(gradja, (nazivOgranka != null) ? nazivOgranka : null);
                gradjaBOList.Add(gradjaBO);
            }

            return gradjaBOList;
        }

        // [SK1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        public async Task<GradjaBO?> TraziGradjuPoID(int gradjaID)
        {
            Gradja? trazenaGradja = await bibliotekaEntities.Gradjas
                                    .Include(g => g.AutorFks)
                                    .FirstOrDefaultAsync(g => g.GradjaId == gradjaID);

            if(trazenaGradja == null)
                return null;

            GradjaBO pronadjenaGradja = await ConvertGradjaToGradjaBO(trazenaGradja);
            return pronadjenaGradja;
        }
    }
}
