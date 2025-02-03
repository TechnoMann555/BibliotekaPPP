using BibliotekaPPP.Filters;
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

        [HttpGet]
        [Route("Clanarine")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Clanarine()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            List<ClanarinaBO> clanarineBO = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoNalogID(korisnickiNalog.NalogId);

            return View(clanarineBO.OrderByDescending(cl => cl.DatumPocetka).ToList());
        }
    }
}
