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

        // [SK5] Prikaz informacija o članarinama
        [HttpGet]
        [Route("Clanarine")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Clanarine()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            List<ClanarinaBO> clanarineBO = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoClanID((int)korisnickiNalog.ClanId);

            return View(clanarineBO.OrderByDescending(cl => cl.DatumPocetka).ToList());
        }

        // [SK11] Prikaz podataka o članarinama člana
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> ClanarineClana(int id)
        {
            List<ClanarinaBO>? clanarine = (List<ClanarinaBO>?)await clanarinaRepository.TraziClanarinePoClanID(id);

            if(clanarine == null)
                return RedirectToAction("Pretraga", "Clan");

            return View(clanarine.OrderByDescending(cl => cl.DatumPocetka).ToList());
        }
    }
}
