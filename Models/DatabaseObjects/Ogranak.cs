using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Ogranak
{
    public int OgranakId { get; set; }

    public string Naziv { get; set; } = null!;

    public string Adresa { get; set; } = null!;

    public int NaseljeFk { get; set; }

    public int RbrUokviruNaselja { get; set; }

    public virtual ICollection<Bibliotekar> Bibliotekars { get; set; } = new List<Bibliotekar>();

    public virtual Naselje NaseljeFkNavigation { get; set; } = null!;

    public virtual ICollection<PrimerakGradje> PrimerakGradjes { get; set; } = new List<PrimerakGradje>();
}
