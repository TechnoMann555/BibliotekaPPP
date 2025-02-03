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
        ClanarinaRepository clanarinaRepository = new ClanarinaRepository();
        PozajmicaRepository pozajmicaRepository = new PozajmicaRepository();

        [HttpGet]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            List<ClanarinaBO> clanarineBO = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoClanID((int)korisnickiNalog.ClanId);

            ViewBag.Clanarine = clanarineBO.OrderByDescending(cl => cl.DatumPocetka).ToList();
            return View();
        }

        [HttpPost]
        [Route("Pozajmice")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Pozajmice(PozajmiceViewModel clanarina)
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);

            List<ClanarinaBO> listaClanarina = (List<ClanarinaBO>)await clanarinaRepository.TraziClanarinePoClanID((int)korisnickiNalog.ClanId);
            ViewBag.Clanarine = listaClanarina.OrderByDescending(cl => cl.DatumPocetka).ToList();

            List<PozajmicaBO> listaPozajmica = (List<PozajmicaBO>)await pozajmicaRepository.TraziPozajmicePoClanarini(
                clanFK: (int)korisnickiNalog.ClanId,
                rbrClanarine: clanarina.ClanarinaRbr
            );
            if(listaPozajmica.Count > 0)
                ViewBag.Pozajmice = listaPozajmica.OrderByDescending(poz => poz.DatumPocetka).ToList();
            else
            {
                ViewBag.Pozajmice = listaPozajmica;
                clanarina.PorukaKorisniku = new Poruka(
                    tekst: "Ne postoje pozajmice vezane za izabranu članarinu.",
                    tip: TipPoruke.Upozorenje
                );
            }

            return View(clanarina);
        }
    }
}
