using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class ClanarinaBO
    {
        public int ClanFk { get; set; }

        public int Rbr { get; set; }

        public DateOnly DatumPocetka { get; set; }

        public DateOnly DatumZavrsetka { get; set; }

        public decimal Cena { get; set; }

        public List<int> ListaIDProcitaneGradje { get; set; }

        public ClanarinaBO()
        {
            this.ListaIDProcitaneGradje = new List<int>();
        }

        public ClanarinaBO(Clanarina clanarina)
        {
            this.ClanFk = clanarina.ClanFk;
            this.Rbr = clanarina.Rbr;
            this.DatumPocetka = clanarina.DatumPocetka;
            this.DatumZavrsetka = clanarina.DatumZavrsetka;
            this.Cena = clanarina.Cena;
            this.ListaIDProcitaneGradje = new List<int>();
        }

        public ClanarinaBO(Clanarina clanarina, List<int> gradja)
        {
            this.ClanFk = clanarina.ClanFk;
            this.Rbr = clanarina.Rbr;
            this.DatumPocetka = clanarina.DatumPocetka;
            this.DatumZavrsetka = clanarina.DatumZavrsetka;
            this.Cena = clanarina.Cena;
            this.ListaIDProcitaneGradje = gradja;
        }
    }
}
