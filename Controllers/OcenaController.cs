﻿using BibliotekaPPP.Filters;
using BibliotekaPPP.Models;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BibliotekaPPP.Controllers
{
    public class OcenaController : Controller
    {
        OcenaProcitaneGradjeRepository ocenaRepository = new OcenaProcitaneGradjeRepository();

        #region Korisnicke akcije

        #region [SK7] Ocenjivanje procitane gradje

        // [SK7] Ocenjivanje procitane gradje
        [HttpPost]
        [ServiceFilter(typeof(KorisnikClanRequiredFilter))]
        public async Task<IActionResult> OceniGradju(int gradjaID, int ocenaGradje)
        {
            NalogBO nalogBO = JsonSerializer.Deserialize<NalogBO>(Request.Cookies["Korisnik"]);

            OcenaProcitaneGradjeBO ocena = new OcenaProcitaneGradjeBO()
            {
                GradjaFk = gradjaID,
                ClanskiKorisnickiNalogFk = nalogBO.NalogId,
                Ocena = ocenaGradje
            };

            OcenjivanjeGradjeResult rezultatOcenjivanja = await ocenaRepository.OceniGradju(ocena);
            Poruka porukaKorisniku = new Poruka();

            switch(rezultatOcenjivanja)
            {
                case OcenjivanjeGradjeResult.OcenaKreirana:
                porukaKorisniku.Tekst = "Ocena je uspešno uneta.";
                porukaKorisniku.Tip = TipPoruke.Uspeh;
                break;
                case OcenjivanjeGradjeResult.OcenaAzurirana:
                porukaKorisniku.Tekst = "Ocena je uspešno ažurirana.";
                porukaKorisniku.Tip = TipPoruke.Obavestenje;
                break;
                case OcenjivanjeGradjeResult.OcenaIzbrisana:
                porukaKorisniku.Tekst = "Ocena je izbrisana.";
                porukaKorisniku.Tip = TipPoruke.Upozorenje;
                break;
            }

            return PartialView("~/Views/Shared/_PorukaKorisniku.cshtml", porukaKorisniku);
        }

        #endregion

        #endregion
    }
}
