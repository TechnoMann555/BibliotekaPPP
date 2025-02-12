using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class PozajmicaController : Controller
    {
        ClanarinaRepository clanarinaRepository = new ClanarinaRepository();
        PozajmicaRepository pozajmicaRepository = new PozajmicaRepository();

        #region Pomocne neakcione metode

        // [SK7] Prikaz podataka o pozajmicama
        // [SK14] Prikaz podataka o pozajmicama člana
        [NonAction]
        private async Task<IActionResult?> PripremiClanarineClana(int clanID, bool adminView = false)
        {
            List<ClanarinaBO>? listaClanarina = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID(clanID);

            if(listaClanarina == null)
                return NotFound();

            if(listaClanarina.Count > 0)
                ViewBag.Clanarine = listaClanarina.OrderByDescending(cl => cl.DatumPocetka).ToList();
            else
            {
                ViewBag.Clanarine = listaClanarina;
                ViewBag.PorukaKorisniku = new Poruka(
                    tekst: "Ne postoje članarine za koje mogu biti vezane pozajmice.",
                    tip: TipPoruke.Upozorenje
                );

                return View((adminView) ? "PozajmiceClana" : "Pozajmice");
            }

            return null;
        }

        // [SK7] Prikaz podataka o pozajmicama
        // [SK14] Prikaz podataka o pozajmicama člana
        [NonAction]
        private async Task PripremiPozajmice(int clanFK, int rbrClanarine)
        {
            List<PozajmicaBO> listaPozajmica = (List<PozajmicaBO>)await pozajmicaRepository.TraziPozajmicePoClanarini(
                clanFK: clanFK,
                rbrClanarine: rbrClanarine
            );
            if(listaPozajmica.Count > 0)
                ViewBag.Pozajmice = listaPozajmica.Where(poz => poz.DatumRazduzenja == null)
                                    .OrderBy(poz => poz.RokRazduzenja)
                                    .Concat(
                                        listaPozajmica
                                        .Where(poz => poz.DatumRazduzenja != null)
                                        .OrderByDescending(poz => poz.RokRazduzenja)
                                    ).ToList();
            else
            {
                ViewBag.Pozajmice = listaPozajmica;
                ViewBag.PorukaKorisniku = new Poruka(
                    tekst: "Ne postoje pozajmice vezane za izabranu članarinu.",
                    tip: TipPoruke.Upozorenje
                );
            }
        }

        #endregion

        #region Korisnicke akcije

        #region [SK7] Prikaz podataka o pozajmicama

        // [SK7] Prikaz podataka o pozajmicama
        [HttpGet]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            await PripremiClanarineClana((int)korisnickiNalog.ClanId);

            return View();
        }

        // [SK7] Prikaz podataka o pozajmicama
        [HttpPost]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice(int clanarinaRbr)
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            IActionResult? pogled = await PripremiClanarineClana((int)korisnickiNalog.ClanId);

            // Ako nije null, znaci da pripremanje clanarina nije proslo
            if(pogled != null)
                return pogled;
            ViewBag.ClanarinaRbr = clanarinaRbr;

            await PripremiPozajmice((int)korisnickiNalog.ClanId, clanarinaRbr);

            return View();
        }

        #endregion

        #endregion

        #region Administratorske akcije

        #region [SK14] Prikaz podataka o pozajmicama člana

        // [SK14] Prikaz podataka o pozajmicama člana
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PozajmiceClana(int id)
        {
            IActionResult? pogled = await PripremiClanarineClana(id, true);

            if(pogled != null && pogled.GetType() == typeof(NotFoundResult))
                return NotFound();

            ViewBag.ClanID = id;
            if(TempData["PorukaGreskaRazduzivanje"] != null)
            {
                ViewBag.PorukaGreskaRazduzivanje = JsonSerializer.Deserialize<Poruka>(
                    TempData["PorukaGreskaRazduzivanje"].ToString()
                );
            }

            return View();
        }

        // [SK14] Prikaz podataka o pozajmicama člana
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PozajmiceClanaPrikaz(int id, int clanarinaRbr)
        {
            IActionResult? pogled = await PripremiClanarineClana(id, true);

            if(pogled != null)
                return pogled;

            await PripremiPozajmice(id, clanarinaRbr);
            
            ViewBag.ClanID = id;
            ViewBag.ClanarinaRbr = clanarinaRbr;

            return View("PozajmiceClana");
        }

        #endregion

        #region [SK17] Kreiranje pozajmice za određenog člana

        // [SK17] Kreiranje pozajmice za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PrikaziFormuKreiranjaPozajmice(int id)
        {
            OgranakRepository ogranakRepository = new OgranakRepository();

            List<OgranakBO> ogranciSaGradjom = (List<OgranakBO>)await ogranakRepository.VratiOgrankeSaSlobodnimPrimercimaGradje(id);

            if(ogranciSaGradjom.Count == 0)
            {
                Poruka poruka = new Poruka(
                    tekst: "Ne postoji ni jedan slobodan primerak građe.",
                    tip: TipPoruke.Upozorenje
                );
                return PartialView("~/Views/Shared/_PorukaKorisniku.cshtml", poruka);
            }
            else
            {
                return PartialView("~/Views/Pozajmica/_FormaNovaPozajmica.cshtml", (ogranciSaGradjom, id));
            }
        }

        // [SK17] Kreiranje pozajmice za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> KreirajPozajmicu(int ogranakID, int gradjaID, string clanJCB)
        {
            ClanRepository clanRepository = new ClanRepository();

            ClanBO? clan = await clanRepository.TraziClanaPoJCB(clanJCB);
            Poruka? porukaGreska = null;

            if(clan == null)
            {
                porukaGreska = new Poruka("Ne postoji član sa unetim JČB.", TipPoruke.Greska);
            }
            else
            {
                (KreiranjePozajmiceResult rezultat, int? clanarinaRbr) rezultatKreiranja = await pozajmicaRepository.KreirajPozajmicu(
                    ogranakID: ogranakID,
                    gradjaID: gradjaID,
                    clanID: clan.ClanId
                );

                if(rezultatKreiranja.rezultat == KreiranjePozajmiceResult.Uspeh)
                    return RedirectToAction("PozajmiceClanaPrikaz", new { id = clan.ClanId, clanarinaRbr = rezultatKreiranja.clanarinaRbr });

                porukaGreska = new Poruka();
                switch(rezultatKreiranja.rezultat)
                {
                    case KreiranjePozajmiceResult.NemaTekucuClanarinu:
                    porukaGreska.Tekst = "Član sa unetim JČB nema tekuću članarinu.";
                    break;
                    case KreiranjePozajmiceResult.ImaMaksTekucihPozajmica:
                    porukaGreska.Tekst = "Član sa unetim JČB ima maksimalni broj tekućih pozajmica.";
                    break;
                    case KreiranjePozajmiceResult.ImaZakasneleTekucePozajmice:
                    porukaGreska.Tekst = "Član sa unetim JČB ima tekuće pozajmice sa prekoračenim rokom razduženja.";
                    break;
                    case KreiranjePozajmiceResult.ImaTekucuPozajmicuZaGradju:
                    porukaGreska.Tekst = "Član sa unetim JČB već ima tekuću pozajmicu za željenu građu.";
                    break;
                }
                porukaGreska.Tip = TipPoruke.Greska;
            }

            if(porukaGreska != null)
            {
                TempData["PorukaGreska"] = JsonSerializer.Serialize<Poruka>(porukaGreska);
            }

            return RedirectToAction("Prikaz", "Gradja", new { id = gradjaID });
        }

        #endregion

        #region [SK18] Razduživanje pozajmice za određenog člana

        // [SK18] Razduživanje pozajmice za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PrikaziFormuRazduzivanjaPozajmice(int clanID, int clanarinaID, int pozajmicaRbr)
        {
            PozajmicaBO? pozajmicaBO = await pozajmicaRepository.TraziPozajmicuPoPK(clanID, clanarinaID, pozajmicaRbr);

            if(pozajmicaBO == null)
            {
                Poruka errorPoruka = new Poruka("Nije pronađena tražena pozajmica.", TipPoruke.Greska);
                return PartialView("~/Views/Shared/_PorukaKorisniku.cshtml", errorPoruka);
            }

            return PartialView("~/Views/Pozajmica/_FormaRazduzivanjePozajmice.cshtml", pozajmicaBO);
        }

        // [SK18] Razduživanje pozajmice za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr)
        {
            RazduzivanjePozajmiceResult rezultatRazduzivanja = await pozajmicaRepository.RazduziPozajmicu(
                clanID: clanID,
                clanarinaID: clanarinaID,
                pozajmicaRbr: pozajmicaRbr
            );

            if(rezultatRazduzivanja == RazduzivanjePozajmiceResult.DatumRazduzenjaJeDatumPocetka)
            {
                Poruka porukaGreske = new Poruka(
                    tekst: "Pozajmica se ne može razdužiti istog datuma kada je kreirana!",
                    tip: TipPoruke.Greska
                );
                TempData["PorukaGreskaRazduzivanje"] = JsonSerializer.Serialize(porukaGreske);
            }

            return RedirectToAction("PozajmiceClana", new { id = clanID });
        }

        #endregion

        #endregion
    }
}
