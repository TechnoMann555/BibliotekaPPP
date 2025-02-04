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
            ViewBag.Tab = "KorisnikLogin";
            return View();
        }

        // [SK3] Logovanje na korisnički nalog
        [HttpPost]
        public async Task<IActionResult> LoginClan(string emailClan, string lozinkaClan)
        {
            Poruka porukaKorisniku = new Poruka();

            if(string.IsNullOrEmpty(emailClan) || string.IsNullOrEmpty(lozinkaClan))
            {
                porukaKorisniku.Tekst = "Email i lozinka su obavezni za unos.";
                porukaKorisniku.Tip = TipPoruke.Greska;
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "KorisnikLogin";

                return View("Login");
            }

            (NalogBO? nalogBO, LoginResult rezultat) loginRezultat = await nalogRepository.LoginKorisnikClan(
                email: emailClan,
                lozinka: lozinkaClan
            );

            if(loginRezultat.nalogBO == null)
            {
                switch(loginRezultat.rezultat)
                {
                    case LoginResult.NalogNePostoji:
                    porukaKorisniku.Tekst = "Ne postoji korisnički nalog vezan za unetu e-mail adresu.";
                    porukaKorisniku.Tip = TipPoruke.Greska;
                    break;
                    case LoginResult.PogresnaLozinka:
                    porukaKorisniku.Tekst = "Pogrešna lozinka.";
                    porukaKorisniku.Tip = TipPoruke.Greska;
                    break;
                }

                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "KorisnikLogin";

                return View("Login");
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

        // TODO: Izvuci zajednicku metodu iz dve login akcije
        // [SK8] Logovanje na administratorski nalog
        [HttpPost]
        public async Task<IActionResult> LoginBibliotekar(string emailBibliotekar, string lozinkaBibliotekar)
        {
            Poruka porukaKorisniku = new Poruka();

            if(string.IsNullOrEmpty(emailBibliotekar) || string.IsNullOrEmpty(lozinkaBibliotekar))
            {
                porukaKorisniku.Tekst = "Email i lozinka su obavezni za unos.";
                porukaKorisniku.Tip = TipPoruke.Greska;
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "AdminLogin";

                return View("Login");
            }

            (NalogBO? nalogBO, LoginResult rezultat) loginRezultat = await nalogRepository.LoginAdminBibliotekar(
                email: emailBibliotekar,
                lozinka: lozinkaBibliotekar
            );

            if(loginRezultat.nalogBO == null)
            {
                switch(loginRezultat.rezultat)
                {
                    case LoginResult.NalogNePostoji:
                    porukaKorisniku.Tekst = "Ne postoji administratorski nalog vezan za unetu e-mail adresu.";
                    porukaKorisniku.Tip = TipPoruke.Greska;
                    break;
                    case LoginResult.PogresnaLozinka:
                    porukaKorisniku.Tekst = "Pogrešna lozinka.";
                    porukaKorisniku.Tip = TipPoruke.Greska;
                    break;
                }

                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "AdminLogin";

                return View("Login");
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

                // TODO: Promeniti da bude pretraga clanova po JCB
                return RedirectToAction("Pretraga", "Gradja");
            }
        }

        // TODO: Dokumentovati Logout funkcionalnost u Larmanu
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Korisnik");
            return RedirectToAction("Pretraga", "Gradja");
        }
    }
}
