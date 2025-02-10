using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class ClanarinaController : Controller
    {
        private ClanarinaRepository clanarinaRepository = new ClanarinaRepository();

        #region Korisnicke akcije

        #region [SK6] Prikaz informacija o članarinama

        // [SK6] Prikaz informacija o članarinama
        [HttpGet]
        [Route("Clanarine")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Clanarine()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            List<ClanarinaBO>? clanarineBO = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID((int)korisnickiNalog.ClanId);

            return View(clanarineBO?.OrderByDescending(cl => cl.DatumPocetka).ToList());
        }

        #endregion

        #endregion

        #region Administratorske akcije

        #region [SK13] Prikaz podataka o članarinama člana

        // [SK13] Prikaz podataka o članarinama člana
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> ClanarineClana(int id)
        {
            List<ClanarinaBO>? clanarine = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID(id);

            if(clanarine == null)
                return NotFound();

            // Ako je pri otvaranju clanarine uneta pogresna vrednost za cenu,
            // ispisace se error poruka na pogledu
            if(TempData["PorukaGreskaUnosCene"] != null)
            {
                ViewBag.PorukaGreskaUnosCene = JsonSerializer.Deserialize<Poruka>(
                    TempData["PorukaGreskaUnosCene"].ToString()
                );
            }

            ViewBag.ClanID = id;

            return View(clanarine.OrderByDescending(cl => cl.DatumPocetka).ToList());
        }

        #endregion

        #region [SK16] Otvaranje nove članarine za određenog člana

        // [SK16] Otvaranje nove članarine za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> ProveriUsloveOtvaranjaClanarine(int id)
        {
            PrUslovaOtvClanarineResult rezultatProvere = await clanarinaRepository.ProveriUsloveOtvaranjaClanarine(id);

            if(rezultatProvere == PrUslovaOtvClanarineResult.IspunjeniUslovi)
                return PartialView("~/Views/Clanarina/_FormaNovaClanarina.cshtml", id);
            else
            {
                string tekst = "";

                if(rezultatProvere == PrUslovaOtvClanarineResult.PostojiTekucaClanarina)
                    tekst = "Član biblioteke već ima tekuću članarinu.";
                else if(rezultatProvere == PrUslovaOtvClanarineResult.PostojeNerazduzenePozajmice)
                    tekst = "Član biblioteke ima nerazdužene pozajmice.";

                Poruka poruka = new Poruka(tekst, TipPoruke.Greska);
                return PartialView("~/Views/Shared/_PorukaKorisniku.cshtml", poruka);
            }
        }

        // [SK16] Otvaranje nove članarine za određenog člana
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> OtvoriClanarinu(int id, decimal cena)
        {
            if(cena < 1)
            {
                Poruka errorPoruka = new Poruka("Unešena je nepravilna vrednost za cenu.", TipPoruke.Greska);
                TempData["PorukaGreskaUnosCene"] = JsonSerializer.Serialize<Poruka>(errorPoruka);
            }
            else
            {
                await clanarinaRepository.OtvoriClanarinu(id, cena);
            }
            
            return RedirectToAction("ClanarineClana", new { id });
        }

        #endregion

        #endregion
    }
}
