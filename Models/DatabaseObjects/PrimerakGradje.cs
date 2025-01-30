using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class PrimerakGradje
{
    public int GradjaFk { get; set; }

    public int OgranakFk { get; set; }

    public int RbrUokviruOgranka { get; set; }

    public string Signatura { get; set; } = null!;

    public string InventarniBroj { get; set; } = null!;

    public int StatusFk { get; set; }

    public virtual Gradja GradjaFkNavigation { get; set; } = null!;

    public virtual Ogranak OgranakFkNavigation { get; set; } = null!;

    public virtual ICollection<Pozajmica> Pozajmicas { get; set; } = new List<Pozajmica>();

    public virtual StatusPrimerka StatusFkNavigation { get; set; } = null!;
}
