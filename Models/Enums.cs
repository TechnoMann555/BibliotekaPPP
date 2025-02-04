﻿namespace BibliotekaPPP.Models
{
    public enum KreiranjeNalogaResult
    {
        Uspeh = 0,
        ClanNePostoji,
        EmailNeOdgovara,
        NalogVecPostoji
    }

    public enum KorisnikLoginResult
    {
        Uspeh = 0,
        NalogNePostoji,
        PogresnaLozinka
    }

    public enum OcenjivanjeGradjeResult
    {
        OcenaKreirana,
        OcenaAzurirana,
        OcenaIzbrisana,
        Greska
    }

    public enum TipPoruke
    {
        Uspeh = 0,
        Obavestenje,
        Upozorenje,
        Greska
    }
}