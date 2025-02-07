using BibliotekaPPP.Filters;
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

            Poruka poruka = new Poruka();

            switch(rezultat)
            {
                case KreiranjeNalogaResult.ClanNePostoji:
                poruka.Tekst = "Ne postoji član biblioteke sa unetim JČB.";
                break;
                
                case KreiranjeNalogaResult.EmailNeOdgovara:
                poruka.Tekst = "Uneta e-mail adresa ne odgovara e-mail adresi vezana za člana biblioteke.";
                break;
                
                case KreiranjeNalogaResult.NalogVecPostoji:
                poruka.Tekst = "Član biblioteke već ima otvoren članski korisnički nalog.";
                break;
                
                case KreiranjeNalogaResult.Uspeh:
                poruka.Tekst = "Uspešno je kreiran člasnki korisnički nalog.";
                break;
            }
            poruka.Tip = (rezultat == KreiranjeNalogaResult.Uspeh) ? TipPoruke.Uspeh : TipPoruke.Greska;

            ViewBag.PorukaKorisniku = poruka;

            return View(regPodaci);
        }

        // [SK3] Logovanje na korisnički nalog
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Tab = "KorisnikLogin";
            return View();
        }

        #region Neakcione metode za login

        [NonAction]
        private bool ProveriLoginPolja(string email, string lozinka, ref Poruka poruka)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
            {
                poruka.Tekst = "Email i lozinka su obavezni za unos.";
                poruka.Tip = TipPoruke.Greska;
                return false;
            }

            return true;
        }

        [NonAction]
        private bool ProveriLoginRezultat(
            NalogBO? nalog,
            LoginResult rezultat,
            ref Poruka poruka,
            bool admin = false
        )
        {
            if(nalog != null)
                return true;

            switch(rezultat)
            {
                case LoginResult.NalogNePostoji:
                poruka.Tekst = $"Ne postoji {((admin) ? "administratorski" : "korisnički")} nalog vezan za unetu e-mail adresu.";
                break;
                case LoginResult.PogresnaLozinka:
                poruka.Tekst = "Pogrešna lozinka.";
                break;
            }
            poruka.Tip = TipPoruke.Greska;

            return false;
        }

        [NonAction]
        private void KreirajCookie(NalogBO nalog)
        {
            Response.Cookies.Append(
                "Korisnik",
                JsonSerializer.Serialize(nalog),
                new CookieOptions()
                {
                    Secure = true,
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SameSite = SameSiteMode.Strict
                }
            );
        }

        #endregion

        // [SK3] Logovanje na korisnički nalog
        [HttpPost]
        public async Task<IActionResult> LoginClan(string emailClan, string lozinkaClan)
        {
            Poruka porukaKorisniku = new Poruka();

            if(!ProveriLoginPolja(emailClan, lozinkaClan, ref porukaKorisniku))
            {
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "KorisnikLogin";
                return View("Login");
            }

            (NalogBO? nalogBO, LoginResult rezultat) loginRezultat = await nalogRepository.LoginKorisnikClan(
                email: emailClan,
                lozinka: lozinkaClan
            );

            if(!ProveriLoginRezultat(loginRezultat.nalogBO, loginRezultat.rezultat, ref porukaKorisniku))
            {
                // Dupliranje bi moglo da se izbegne ako ovo stavim na kraj metode,
                // kreiram labelu i iskoristim 'goto', ali bih da to izbegnem.
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "KorisnikLogin";
                return View("Login");
            }
            else
            {
                // Ne moze biti null jer smo ustanovili u pozivu metode 'ProveriLoginRezultat' da nije null
                KreirajCookie(loginRezultat.nalogBO);
                return RedirectToAction("LicniPodaci", "Clan");
            }
        }

        // [SK8] Logovanje na administratorski nalog
        [HttpPost]
        public async Task<IActionResult> LoginBibliotekar(string emailBibliotekar, string lozinkaBibliotekar)
        {
            Poruka porukaKorisniku = new Poruka();

            if(!ProveriLoginPolja(emailBibliotekar, lozinkaBibliotekar, ref porukaKorisniku))
            {
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "AdminLogin";
                return View("Login");
            }

            (NalogBO? nalogBO, LoginResult rezultat) loginRezultat = await nalogRepository.LoginAdminBibliotekar(
                email: emailBibliotekar,
                lozinka: lozinkaBibliotekar
            );

            if(!ProveriLoginRezultat(loginRezultat.nalogBO, loginRezultat.rezultat, ref porukaKorisniku, true))
            {
                // Dupliranje bi moglo da se izbegne ako ovo stavim na kraj metode,
                // kreiram labelu i iskoristim 'goto', ali bih da to izbegnem.
                ViewBag.Poruka = porukaKorisniku;
                ViewBag.Tab = "AdminLogin";
                return View("Login");
            }
            else
            {
                // Ne moze biti null jer smo ustanovili u prethodnom pozivu metode da nije null
                KreirajCookie(loginRezultat.nalogBO);
                return RedirectToAction("Pretraga", "Clan");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> BrisiKorisnickiNalog(int id)
        {
            BrisanjeKorisnickogNalogaResult rezultatBrisanja = await nalogRepository.BrisiKorisnickiNalog(id);

            Poruka poruka = new Poruka();
            
            switch(rezultatBrisanja)
            {
                case BrisanjeKorisnickogNalogaResult.Uspeh:
                poruka.Tekst = "Uspešno je izbrisan članski korisnički nalog.";
                break;
                case BrisanjeKorisnickogNalogaResult.NemaKorisnickiNalog:
                poruka.Tekst = "Član biblioteke nema članski korisnički nalog.";
                break;
            }

            poruka.Tip = (rezultatBrisanja == BrisanjeKorisnickogNalogaResult.Uspeh) ? TipPoruke.Uspeh : TipPoruke.Greska;
            TempData["PorukaKorisniku"] = JsonSerializer.Serialize(poruka);

            return RedirectToAction("PrikazLicnihPodataka", "Clan", new { id });
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
