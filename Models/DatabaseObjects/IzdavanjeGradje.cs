using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class IzdavanjeGradje
{
    public int IzdavanjeId { get; set; }

    public string? NaseljeIzdavanja { get; set; }

    public string? NazivIzdavaca { get; set; }

    public decimal? GodinaIzdavanja { get; set; }

    public virtual ICollection<Gradja> Gradjas { get; set; } = new List<Gradja>();
}
