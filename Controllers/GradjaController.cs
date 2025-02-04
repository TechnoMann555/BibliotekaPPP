using Microsoft.AspNetCore.Mvc;
using BibliotekaPPP.Models.ViewModels;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.Identity.Client;
using BibliotekaPPP.Filters;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class GradjaController : Controller
    {
        GradjaRepository gradjaRepository = new GradjaRepository();
        PozajmicaRepository pozajmicaRepository = new PozajmicaRepository();
        OcenaProcitaneGradjeRepository ocenaRepository = new OcenaProcitaneGradjeRepository();

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
        public async Task<IActionResult> Prikaz(int id)
        {
            GradjaBO? trazenaGradja = await gradjaRepository.TraziGradjuPoID(id);

            if(trazenaGradja == null)
                return RedirectToAction("Pretraga");

            // [SK7] Ocenjivanje procitane gradje
            if(Request.Cookies.TryGetValue("Korisnik", out _))
            {
                NalogBO nalogBO = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
                
                if(nalogBO.Uloga == "Korisnik_Clan")
                {
                    ViewBag.GradjaProcitana = await pozajmicaRepository.ClanProcitaoGradju(
                        gradjaID: id,
                        clanID: (int)nalogBO.ClanId
                    );

                    OcenaProcitaneGradjeBO? ocenaGradje = await ocenaRepository.TraziOcenu(
                        gradjaID: id,
                        korisnickiNalogID: nalogBO.NalogId
                    );
                    trazenaGradja.Ocena = ocenaGradje;
                }
            }

            return View(trazenaGradja);
        }
    }
}
