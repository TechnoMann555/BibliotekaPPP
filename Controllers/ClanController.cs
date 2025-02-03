using BibliotekaPPP.Filters;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
    public class ClanController : Controller
    {
        private ClanRepository clanRepository = new ClanRepository();

        [HttpGet]
        public async Task<IActionResult> LicniPodaci()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            ClanBO clanBO = await clanRepository.TraziClanaPoNalogID(korisnickiNalog.NalogId);

            return View(clanBO);
        }
    }
}
