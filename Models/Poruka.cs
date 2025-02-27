namespace BibliotekaPPP.Models
{
    public class Poruka
    {
        public string Tekst { get; set; }

        public TipPoruke Tip { get; set; }

        public Poruka()
        {
            this.Tekst = "### PRAZNA PORUKA ###";
            this.Tip = TipPoruke.Greska;
        }

        public Poruka(string tekst, TipPoruke tip)
        {
            this.Tekst = tekst;
            this.Tip = tip;
        }
    }
}
