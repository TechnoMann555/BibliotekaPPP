using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class PozajmicaController : Controller
    {
        ClanRepository clanRepository = new ClanRepository();
        ClanarinaRepository clanarinaRepository = new ClanarinaRepository();
        PozajmicaRepository pozajmicaRepository = new PozajmicaRepository();
        OgranakRepository ogranakRepository = new OgranakRepository();

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

        // [SK6] Prikaz podataka o pozajmicama
        [HttpGet]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            await PripremiClanarineClana((int)korisnickiNalog.ClanId);

            return View();
        }

        // [SK6] Prikaz podataka o pozajmicama
        [HttpPost]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice(PozajmiceViewModel clanarina)
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            IActionResult? pogled = await PripremiClanarineClana((int)korisnickiNalog.ClanId);
            
            if(pogled != null)
                return pogled;

            List<PozajmicaBO> listaPozajmica = (List<PozajmicaBO>)await pozajmicaRepository.TraziPozajmicePoClanarini(
                clanFK: (int)korisnickiNalog.ClanId,
                rbrClanarine: clanarina.ClanarinaRbr
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

            return View(clanarina);
        }

        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PozajmiceClana(int id)
        {
            List<ClanarinaBO>? clanarineBO = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID(id);

            if(clanarineBO == null)
                return RedirectToAction("Pretraga", "Clan");

            ViewBag.Clanarine = clanarineBO.OrderByDescending(cl => cl.DatumPocetka).ToList();
            ViewBag.ClanID = id;

            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PozajmiceClana(int id, PozajmiceViewModel clanarina)
        {
            List<ClanarinaBO>? listaClanarina = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID(id);

            if(listaClanarina == null)
                return RedirectToAction("Pretraga", "Clan");

            ViewBag.Clanarine = listaClanarina.OrderByDescending(cl => cl.DatumPocetka).ToList();
            ViewBag.ClanID = id;

            List<PozajmicaBO> listaPozajmica = (List<PozajmicaBO>)await pozajmicaRepository.TraziPozajmicePoClanarini(
                clanFK: id,
                rbrClanarine: clanarina.ClanarinaRbr
            );

            ViewBag.Pozajmice = listaPozajmica
                                .Where(poz => poz.DatumRazduzenja == null)
                                .OrderBy(poz => poz.RokRazduzenja)
                                .Concat(
                                    listaPozajmica
                                    .Where(poz => poz.DatumRazduzenja != null)
                                    .OrderByDescending(poz => poz.RokRazduzenja)
                                ).ToList();
            
            if(listaPozajmica.Count == 0)
            {
                ViewBag.PorukaKorisniku = new Poruka(
                    tekst: "Ne postoje pozajmice vezane za izabranu članarinu.",
                    tip: TipPoruke.Upozorenje
                );
            }

            return View(clanarina);
        }

        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PrikaziFormuKreiranjaPozajmice(int id)
        {
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

        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> KreirajPozajmicu(int ogranakID, int gradjaID, string clanJCB)
        {
            ClanBO? clan = await clanRepository.TraziClanaPoJCB(clanJCB);
            Poruka? porukaGreska = null;

            if(clan == null)
            {
                porukaGreska = new Poruka("Ne postoji član sa unetim JČB.", TipPoruke.Greska);
            }
            else
            {
                KreiranjePozajmiceResult rezultat = await pozajmicaRepository.KreirajPozajmicu(
                    ogranakID: ogranakID,
                    gradjaID: gradjaID,
                    clanID: clan.ClanId
                );

                if(rezultat == KreiranjePozajmiceResult.Uspeh)
                    return RedirectToAction("PozajmiceClana", new { id = gradjaID });

                porukaGreska = new Poruka();
                switch(rezultat)
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
                return RedirectToAction("Prikaz", "Gradja", new { id = gradjaID });
            }

            return RedirectToAction("Prikaz", "Gradja", new { id = gradjaID });
        }

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

        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> RazduziPozajmicu(int clanID, int clanarinaID, int pozajmicaRbr)
        {
            await pozajmicaRepository.RazduziPozajmicu(clanID, clanarinaID, pozajmicaRbr);
            return RedirectToAction("PozajmiceClana", new { id = clanID });
        }
    }
}
