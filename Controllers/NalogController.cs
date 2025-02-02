using BibliotekaPPP.Models;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaPPP.Controllers
{
    public class NalogController : Controller
    {
        NalogRepository nalogRepository = new NalogRepository();

        [HttpGet]
        public IActionResult RegistracijaClan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistracijaClan(RegistracijaClanViewModel regPodaci)
        {
            if(!ModelState.IsValid)
            {
                return View(regPodaci);
            }

            KreiranjeNalogaResult rezultat = await nalogRepository.KreirajKorisnickiNalog(
                JCB: regPodaci.JCB,
                email: regPodaci.Email,
                lozinka: regPodaci.Lozinka
            );

            switch(rezultat)
            {
                case KreiranjeNalogaResult.ClanNePostoji:
                regPodaci.PorukaKorisniku = new Poruka(
                    tekst: "Ne postoji član biblioteke sa unetim JČB.",
                    tip: TipPoruke.Greska
                );
                break;
                case KreiranjeNalogaResult.EmailNeOdgovara:
                regPodaci.PorukaKorisniku = new Poruka(
                    tekst: "Uneta e-mail adresa ne odgovara adresi vezana za člana biblioteke.",
                    tip: TipPoruke.Greska
                );
                break;
                case KreiranjeNalogaResult.NalogVecPostoji:
                regPodaci.PorukaKorisniku = new Poruka(
                    tekst: "Član biblioteke već ima otvoren članski korisnički nalog.",
                    tip: TipPoruke.Greska
                );
                break;
                case KreiranjeNalogaResult.Uspeh:
                regPodaci.PorukaKorisniku = new Poruka(
                    tekst: "Uspešno je kreiran člasnki korisnički nalog.",
                    tip: TipPoruke.Uspeh
                );
                break;
            }

            return View(regPodaci);
        }
    }
}
