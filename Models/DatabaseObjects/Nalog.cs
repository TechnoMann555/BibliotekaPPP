using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Nalog
{
    public int NalogId { get; set; }

    public string Lozinka { get; set; } = null!;

    public string Uloga { get; set; } = null!;

    public virtual Bibliotekar? Bibliotekar { get; set; }

    public virtual Clan? Clan { get; set; }

    public virtual ICollection<OcenaProcitaneGradje> OcenaProcitaneGradjes { get; set; } = new List<OcenaProcitaneGradje>();
}
