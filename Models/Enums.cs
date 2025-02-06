namespace BibliotekaPPP.Models
{
    public enum TipPoruke
    {
        Uspeh = 0,
        Obavestenje,
        Upozorenje,
        Greska
    }

    public enum KreiranjeNalogaResult
    {
        Uspeh = 0,
        ClanNePostoji,
        EmailNeOdgovara,
        NalogVecPostoji
    }

    public enum LoginResult
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

    public enum UpisivanjeClanaResult
    {
        Uspeh,
        BrLicneKartePostoji,
        BrTelefonaPostoji,
        KontaktMejlPostoji
    }

    public enum PrUslovaOtvClanarineResult
    {
        IspunjeniUslovi,
        PostojiTekucaClanarina,
        PostojeNerazduzenePozajmice
    }

    public enum KreiranjePozajmiceResult
    {
        Uspeh,
        NemaTekucuClanarinu,
        ImaMaksTekucihPozajmica,
        ImaZakasneleTekucePozajmice,
        ImaTekucuPozajmicuZaGradju
    }
}