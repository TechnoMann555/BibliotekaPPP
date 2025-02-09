﻿using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using BibliotekaPPP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class ClanController : Controller
    {
        private ClanRepository clanRepository = new ClanRepository();

        #region Korisnicke akcije

        #region [SK4] Prikaz ličnih i članskih podataka

        // [SK4] Prikaz ličnih i članskih podataka
        [HttpGet]
        [Route("LicniClanskiPodaci")]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> LicniClanskiPodaci()
        {
            NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);
            ClanBO? clanBO = await clanRepository.TraziClanaPoClanID((int)korisnickiNalog.ClanId);

            if(clanBO == null)
                return NotFound();

            return View(clanBO);
        }

        #endregion

        #endregion

        #region Administratorske akcije

        #region [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public IActionResult Pretraga()
        {
            return View();
        }

        // [SK9] Pretraga članova biblioteke po „Jedinstvenom Članskom Broju“ (JČB)
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> Pretraga(string jcb)
        {
            ClanBO? clanBO = await clanRepository.TraziClanaPoJCB(jcb);
            ViewBag.IzvrsenaPretraga = true;

            if(clanBO == null)
            {
                ViewBag.PorukaKorisniku = new Poruka(
                    tekst: "Nije pronađen član biblioteke sa unetim Jedinstvenim Članskim Brojem.",
                    tip: TipPoruke.Upozorenje
                );
            }
            else
            {
                ViewBag.PretrazenClan = clanBO;
            }

            ViewBag.JCB = jcb;
            return View();
        }

        #endregion

        #region [SK10] Prikaz ličnih podataka o članu

        // [SK10] Prikaz ličnih podataka o članu
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> LicniClanskiPodaciClana(int id)
        {
            ClanBO? trazenClan = await clanRepository.TraziClanaPoClanID(id);

            if(trazenClan == null)
                return NotFound();

            // Poruka bibliotekaru vezana za brisanje clanskog korisnickog naloga
            if(TempData["PorukaKorisniku"] != null)
            {
                Poruka porukaKorisniku = JsonSerializer.Deserialize<Poruka>(TempData["PorukaKorisniku"].ToString());
                ViewBag.PorukaKorisniku = porukaKorisniku;
            }

            return View(trazenClan);
        }

        #endregion

        #region [SK13] Upisivanje novog člana biblioteke

        // [SK13] Upisivanje novog člana biblioteke
        [HttpGet]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public IActionResult UpisNovogClana()
        {
            return View(null);
        }

        // [SK13] Upisivanje novog člana biblioteke
        [HttpPost]
        [ServiceFilter(typeof(AdminBibliotekarRequiredFilter))]
        public async Task<IActionResult> UpisNovogClana(UpisClanaViewModel podaci)
        {
            if(!ModelState.IsValid)
            {
                return View(podaci);
            }    

            (UpisivanjeClanaResult rezultat, int? clanID) rezultatUpisa = await clanRepository.UpisiClana(podaci);

            Poruka poruka = new Poruka();
            switch(rezultatUpisa.rezultat)
            {
                case UpisivanjeClanaResult.BrLicneKartePostoji:
                poruka.Tekst = "Već postoji član biblioteke sa unetim brojem lične karte.";
                break;
                case UpisivanjeClanaResult.BrTelefonaPostoji:
                poruka.Tekst = "Već postoji član biblioteke sa unetim kontakt brojem telefona.";
                break;
                case UpisivanjeClanaResult.KontaktMejlPostoji:
                poruka.Tekst = "Već postoji član biblioteke sa unetom e-mail adresom.";
                break;
            }

            if(rezultatUpisa.rezultat != UpisivanjeClanaResult.Uspeh)
            {
                poruka.Tip = TipPoruke.Greska;
                ViewBag.PorukaKorisniku = poruka;
                return View(podaci);
            }
            else
            {
                return RedirectToAction("ClanarineClana", "Clanarina", new { id = rezultatUpisa.clanID });
            }
        }

        #endregion

        #endregion
    }
}
