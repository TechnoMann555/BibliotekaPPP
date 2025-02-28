﻿using BibliotekaPPP.Models.DatabaseObjects;

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

        public string? NaslovGradje { get; set; } = null;

        public string? InventarniBrojPrimerka { get; set; } = null;

        public string SignaturaPrimerka { get; set; } = null!;

        public int? OgranakID { get; set; } = null;

        public string? NazivOgrankaPrimerka { get; set; } = null;

        public PozajmicaBO() { }

        public PozajmicaBO(Pozajmica pozajmica)
        {
            this.ClanFk = pozajmica.ClanarinaClanFk;
            this.ClanarinaFk = pozajmica.ClanarinaFk;
            this.Rbr = pozajmica.Rbr;
            this.DatumPocetka = pozajmica.DatumPocetka;
            this.RokRazduzenja = pozajmica.RokRazduzenja;
            this.DatumRazduzenja = pozajmica.DatumRazduzenja;
            
            if(pozajmica.PrimerakGradje != null)
            {
                this.InventarniBrojPrimerka = pozajmica.PrimerakGradje.InventarniBroj;
                this.SignaturaPrimerka = pozajmica.PrimerakGradje.Signatura;

                if(pozajmica.PrimerakGradje.GradjaFkNavigation != null)
                {
                    this.NaslovGradje = pozajmica.PrimerakGradje.GradjaFkNavigation.Naslov;
                }

                if(pozajmica.PrimerakGradje.OgranakFkNavigation != null)
                {
                    this.OgranakID = pozajmica.PrimerakGradje.OgranakFkNavigation.OgranakId;
                    this.NazivOgrankaPrimerka = pozajmica.PrimerakGradje.OgranakFkNavigation.Naziv;
                }
            }
        }
    }
}
