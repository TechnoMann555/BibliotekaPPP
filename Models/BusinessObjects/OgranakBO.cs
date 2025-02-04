namespace BibliotekaPPP.Models.BusinessObjects
{
    public class OgranakBO
    {
        public int OgranakId { get; set; }

        public string Naziv { get; set; } = null!;

        public string Adresa { get; set; } = null!;

        public int NaseljeFk { get; set; }

        public int RbrUokviruNaselja { get; set; }

    }
}
