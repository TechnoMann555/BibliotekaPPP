using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        // [SK3] Logovanje na korisnički nalog
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // [SK3] Logovanje na korisnički nalog
        [HttpPost]
        public async Task<IActionResult> LoginClan(LoginViewModel loginPodaci)
        {
            if(!ModelState.IsValid)
            {
                return View("Login", loginPodaci);
            }

            NalogBO? nalogZaLogovanje = await nalogRepository.LoginClanKorisnik(
                email: loginPodaci.Email,
                lozinka: loginPodaci.Lozinka
            );

            if(nalogZaLogovanje == null)
            {
                loginPodaci.PorukaKorisniku = new Poruka(
                    tekst: "Došlo je do greške pri logovanju na korisnički nalog.",
                    tip: TipPoruke.Greska
                );
            }
            else
            {
                Response.Cookies.Append(
                    "Korisnik",
                    JsonSerializer.Serialize(nalogZaLogovanje),
                    new CookieOptions()
                    {
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddDays(7),
                        SameSite = SameSiteMode.Strict
                    }
                );
            }

            // TODO: Preusmeriti korisnika na prikaz clanskih i licnih podataka
            return View("Login", loginPodaci);
        }
    }
}
