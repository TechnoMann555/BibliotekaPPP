using BibliotekaPPP.Filters;
using BibliotekaPPP.Models.BusinessObjects;
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

        [HttpGet]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            List<ClanarinaBO> clanarineBO = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoNalogID(korisnickiNalog.NalogId);

            ViewBag.Clanarine = clanarineBO.OrderByDescending(cl => cl.DatumPocetka).ToList();
            ViewBag.ClanId = korisnickiNalog.ClanId;
            return View();
        }

        [HttpPost]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice(PozajmiceViewModel clanarina)
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);

            List<ClanarinaBO> listaClanarina = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoNalogID(korisnickiNalog.NalogId);
            ViewBag.Clanarine = listaClanarina.OrderByDescending(cl => cl.DatumPocetka).ToList();
            ViewBag.ClanId = korisnickiNalog.ClanId;

            List<PozajmicaBO> listaPozajmica = (List<PozajmicaBO>)await pozajmicaRepository.TraziPozajmicePoClanarini(
                clanFK: clanarina.ClanId,
                rbrClanarine: clanarina.ClanarinaRbr
            );
            ViewBag.Pozajmice = listaPozajmica.OrderByDescending(poz => poz.DatumPocetka).ToList();

            return View(clanarina);
        }
    }
}
