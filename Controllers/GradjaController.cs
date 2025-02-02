using Microsoft.AspNetCore.Mvc;
using BibliotekaPPP.Models.ViewModels;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.Identity.Client;
using BibliotekaPPP.Filters;

namespace BibliotekaPPP.Controllers
{
    public class GradjaController : Controller
    {
        GradjaRepository gradjaRepository = new GradjaRepository();

        // [SK1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        [HttpGet]
        public IActionResult Pretraga()
        {
            ViewBag.Gradja = null;
            return View("Pretraga", null);
        }

        // [SK1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        [HttpPost]
        public async Task<IActionResult> Pretraga(GradjaPretragaViewModel gradja)
        {
            List<GradjaBO> pretrazenaGradja = (List<GradjaBO>) await gradjaRepository.TraziGradju(
                naslov: gradja.Naslov,
                imePrezimeAutora: gradja.Autor,
                naseljeIzdavanja: gradja.NaseljeIzdavanja,
                nazivIzdavaca: gradja.NazivIzdavaca,
                godinaIzdavanja: gradja.GodinaIzdavanja,
                udk: gradja.Udk,
                ogranak: gradja.Ogranak,
                statusDostupnosti: gradja.StatusDostupnosti
            );

            ViewBag.Gradja = pretrazenaGradja;
            return View("Pretraga", gradja);
        }

        // [SK1] Prikaz podataka o specifičnoj građi
        [HttpGet]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> Prikaz(int gradjaID)
        {
            GradjaBO? trazenaGradja = await gradjaRepository.TraziGradjuPoID(gradjaID);

            if(trazenaGradja == null)
                return RedirectToAction("Pretraga");

            return View(trazenaGradja);
        }
    }
}
