using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class ClanController : Controller
    {
        private ClanRepository clanRepository = new ClanRepository();

        // [SK4] Prikaz ličnih i članskih podataka
        [HttpGet]
        [Route("LicniPodaci")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> LicniPodaci()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            ClanBO clanBO = await clanRepository.TraziClanaPoClanID((int)korisnickiNalog.ClanId);

            return View(clanBO);
        }

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public IActionResult Pretraga()
        {
            return View();
        }

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> Pretraga(string jcb)
        {
            ClanBO? clanBO = await clanRepository.TraziClanaPoJCB(jcb);

            if(clanBO == null)
                return PartialView("~/Views/Shared/_PorukaKorisniku.cshtml", new Poruka(
                    tekst: "Nije pronađen član biblioteke sa unetim Jedinstvenim Članskim Brojem.",
                    tip: TipPoruke.Upozorenje
                ));

            return PartialView("~/Views/Clan/_AdminClanPanel.cshtml", clanBO);
        }

        // [SK9] Prikaz ličnih podataka o članu
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> PrikazLicnihPodataka(int id)
        {
            ClanBO? trazenClan = await clanRepository.TraziClanaPoClanID(id);

            if(trazenClan == null)
                return RedirectToAction("Pretraga");

            return View(trazenClan);
        }
    }
}
