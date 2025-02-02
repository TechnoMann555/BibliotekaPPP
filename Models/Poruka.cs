namespace BibliotekaPPP.Models
{
    public class Poruka
    {
        public string Tekst { get; set; }

        public TipPoruke Tip { get; set; }

        public Poruka(string tekst, TipPoruke tip)
        {
            this.Tekst = tekst;
            this.Tip = tip;
        }
    }
}
