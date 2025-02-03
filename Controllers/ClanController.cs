using BibliotekaPPP.Filters;
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
    }
}
