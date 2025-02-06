using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class PozajmicaBO
    {
        public int ClanFk { get; set; }

        public int ClanarinaFk { get; set; }

        public int Rbr { get; set; }

        public DateOnly DatumPocetka { get; set; }

        public DateOnly RokRazduzenja { get; set; }

        public DateOnly? DatumRazduzenja { get; set; }

        public string NaslovGradje { get; set; } = null!;

        public string InventarniBrojPrimerka { get; set; } = null!;

        public int OgranakID { get; set; }

        public string NazivOgrankaPrimerka { get; set; } = null!;

        public PozajmicaBO() { }

        public PozajmicaBO(Pozajmica pozajmica)
        {
            this.ClanFk = pozajmica.ClanarinaClanFk;
            this.ClanarinaFk = pozajmica.ClanarinaFk;
            this.Rbr = pozajmica.Rbr;
            this.DatumPocetka = pozajmica.DatumPocetka;
            this.RokRazduzenja = pozajmica.RokRazduzenja;
            this.DatumRazduzenja = pozajmica.DatumRazduzenja;
            this.NaslovGradje = pozajmica.PrimerakGradje.GradjaFkNavigation.Naslov;
            this.InventarniBrojPrimerka = pozajmica.PrimerakGradje.InventarniBroj;
            this.OgranakID = pozajmica.PrimerakGradje.OgranakFkNavigation.OgranakId;
            this.NazivOgrankaPrimerka = pozajmica.PrimerakGradje.OgranakFkNavigation.Naziv;
        }
    }
}
