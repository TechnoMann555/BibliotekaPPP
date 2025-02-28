﻿using AspNetCoreGeneratedDocument;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class PozajmicaRepository : IPozajmicaRepository
    {
        BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [SK9] Ocenjivanje pročitane građe
        public async Task<bool> ClanProcitaoGradju(int gradjaID, int clanID)
        {
            bool procitao = await bibliotekaContext.Pozajmicas.AnyAsync(
                p => p.ClanarinaClanFk == clanID &&
                     p.PrimerakGradjeGradjaFk == gradjaID &&
                     p.DatumRazduzenja != null
            );

            return procitao;
        }

        // [SK8] Prikaz podataka o pozajmicama
        // [SK15] Prikaz podataka o pozajmicama člana
        public async Task<IEnumerable<PozajmicaBO>> TraziPozajmicePoClanarini(int clanFK, int rbrClanarine)
        {
            List<Pozajmica> pozajmice = await bibliotekaContext.Pozajmicas
                                              .Include(p => p.PrimerakGradje)
                                                  .ThenInclude(pg => pg.OgranakFkNavigation)
                                              .Include(p => p.PrimerakGradje)
                                                  .ThenInclude(pg => pg.GradjaFkNavigation)
                                              .Where(p => 
                                                  p.ClanarinaFk == rbrClanarine &&
                                                  p.ClanarinaClanFk == clanFK
                                              ).ToListAsync();

            List<PozajmicaBO> listaPozajmica = new List<PozajmicaBO>();

            foreach(Pozajmica pozajmica in pozajmice)
            {
                PozajmicaBO pozajmicaBO = new PozajmicaBO(pozajmica);
                listaPozajmica.Add(pozajmicaBO);
            }

            return listaPozajmica;
        }

        // [SK18] Kreiranje pozajmice za određenog člana
        public async Task<(KreiranjePozajmiceResult, int?)> KreirajPozajmicu(int ogranakID, int gradjaID, int clanID)
        {
            Clanarina? poslednjaClanarina = await bibliotekaContext.Clanarinas
                                                  .Include(cl => cl.Pozajmicas)
                                                  .Where(cl => cl.ClanFk == clanID)
                                                  .OrderByDescending(cl => cl.DatumPocetka)
                                                  .FirstOrDefaultAsync();
            DateOnly trenutniDatum = DateOnly.FromDateTime(DateTime.Now);

            // Provera da li clan nema tekucu clanarinu
            if(
                poslednjaClanarina == null ||
                poslednjaClanarina.DatumZavrsetka < trenutniDatum
            )
            {
                return (KreiranjePozajmiceResult.NemaTekucuClanarinu, null);
            }

            // Provera da li clan ima maks. broj tekucih pozajmica (deset)
            if(
                poslednjaClanarina.Pozajmicas
                .Where(poz => poz.DatumRazduzenja == null)
                .Count() >= 10
            )
            {
                return (KreiranjePozajmiceResult.ImaMaksTekucihPozajmica, null);
            }

            // Provera da li clan ima bar jednu tekucu pozajmicu za koju je prekoracen rok razduzenja
            if(
                poslednjaClanarina.Pozajmicas.Any(poz => 
                poz.RokRazduzenja < trenutniDatum &&
                poz.DatumRazduzenja == null
            ))
            {
                return (KreiranjePozajmiceResult.ImaZakasneleTekucePozajmice, null);
            }

            // Provera da li clan vec ima tekucu pozajmicu za izabranu gradju
            if(poslednjaClanarina.Pozajmicas.Any(poz =>
                poz.PrimerakGradjeGradjaFk == gradjaID &&
                poz.DatumRazduzenja == null
            ))
            {
                return (KreiranjePozajmiceResult.ImaTekucuPozajmicuZaGradju, null);
            }

            // Izbor primerka gradje koji ima najmanji broj pozajmica

            // Ne moze biti null, jer smo metodom PretraziOgrankePoGradjiID()
            // utvrdili ogranke u kojima postoji bar jedan slobodan primerak gradje
            PrimerakGradje? primerak = await bibliotekaContext.PrimerakGradjes
                                            .Where(pg =>
                                                pg.GradjaFk == gradjaID &&
                                                pg.OgranakFk == ogranakID &&
                                                pg.Status == "slobodan"
                                            )
                                            .OrderBy(pg => pg.Pozajmicas.Count)
                                            .ThenBy(pg => pg.InventarniBroj)
                                            .FirstOrDefaultAsync();

            // Kreiranje nove pozajmice
            Pozajmica novaPozajmica = new Pozajmica()
            {
                ClanarinaClanFk = clanID,
                ClanarinaFk = poslednjaClanarina.Rbr,
                PrimerakGradjeGradjaFk = primerak!.GradjaFk,
                PrimerakGradjeOgranakFk = primerak.OgranakFk,
                PrimerakGradjeFk = primerak.RbrUokviruOgranka,
                DatumPocetka = trenutniDatum,
                RokRazduzenja = trenutniDatum.AddDays(20),
                DatumRazduzenja = null
            };

            await bibliotekaContext.Pozajmicas.AddAsync(novaPozajmica);
            primerak.Status = "zauzet";
            await bibliotekaContext.SaveChangesAsync();

            return (KreiranjePozajmiceResult.Uspeh, poslednjaClanarina.Rbr);
        }

        // [SK19] Razduživanje pozajmice za određenog člana
        public async Task<PozajmicaBO?> TraziPozajmicuPoPK(int clanID, int clanarinaID, int pozajmicaRbr)
        {
            Pozajmica? pozajmica = await bibliotekaContext.Pozajmicas
                                         .Include(p => p.PrimerakGradje)
                                             .ThenInclude(pg => pg.GradjaFkNavigation)
                                         .Where(poz =>
                                            poz.ClanarinaClanFk == clanID &&
                                            poz.ClanarinaFk == clanarinaID &&
                                            poz.Rbr == pozajmicaRbr
                                         )
                                         .FirstOrDefaultAsync();

            if(pozajmica == null)
                return null;

            PozajmicaBO trazenaPozajmica = new PozajmicaBO(pozajmica);
            return trazenaPozajmica;
        }

        // [SK19] Razduživanje pozajmice za određenog člana
        public async Task<RazduzivanjePozajmiceResult> RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr)
        {
            Pozajmica pozajmica = await bibliotekaContext.Pozajmicas
                                        .Include(poz => poz.PrimerakGradje)
                                        .Where(poz =>
                                            poz.ClanarinaClanFk == clanID &&
                                            poz.ClanarinaFk == clanarinaID &&
                                            poz.Rbr == pozajmicaRbr
                                        )
                                        .FirstOrDefaultAsync();
            DateOnly trenutniDatum = DateOnly.FromDateTime(DateTime.Now);

            if(pozajmica.DatumPocetka == trenutniDatum)
                return RazduzivanjePozajmiceResult.DatumRazduzenjaJeDatumPocetka;

            pozajmica.DatumRazduzenja = trenutniDatum;
            pozajmica.PrimerakGradje.Status = "slobodan";

            await bibliotekaContext.SaveChangesAsync();

            return RazduzivanjePozajmiceResult.Uspeh;
        }
    }
}
