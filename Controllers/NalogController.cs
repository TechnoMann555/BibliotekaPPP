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

        // [SK2] Registracija clana na platformu biblioteke 
        [HttpGet]
        public IActionResult RegistracijaClan()
        {
            return View();
        }

        // [SK2] Registracija clana na platformu biblioteke 
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

            (NalogBO? nalogBO, KorisnikLoginResult rezultat) loginRezultat = await nalogRepository.LoginClanKorisnik(
                email: loginPodaci.Email,
                lozinka: loginPodaci.Lozinka
            );

            if(loginRezultat.nalogBO == null)
            {
                switch(loginRezultat.rezultat)
                {
                    case KorisnikLoginResult.NalogNePostoji:
                        loginPodaci.PorukaKorisniku = new Poruka(
                            tekst: "Ne postoji korisnički nalog vezan za unetu e-mail adresu.",
                            tip: TipPoruke.Greska
                        );
                    break;
                    case KorisnikLoginResult.PogresnaLozinka:
                        loginPodaci.PorukaKorisniku = new Poruka(
                            tekst: "Pogrešna lozinka.",
                            tip: TipPoruke.Greska
                        );
                    break;
                }

                return View("Login", loginPodaci);
            }
            else
            {
                Response.Cookies.Append(
                    "Korisnik",
                    JsonSerializer.Serialize(loginRezultat.nalogBO),
                    new CookieOptions()
                    {
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddDays(7),
                        SameSite = SameSiteMode.Strict
                    }
                );

                return RedirectToAction("LicniPodaci", "Clan");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Korisnik");
            return RedirectToAction("Pretraga", "Gradja");
        }
    }
}
