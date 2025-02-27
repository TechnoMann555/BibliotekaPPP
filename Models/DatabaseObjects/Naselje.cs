using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Naselje
{
    public int NaseljeId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Ogranak> Ogranaks { get; set; } = new List<Ogranak>();
}
