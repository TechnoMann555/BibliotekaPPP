using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class StatusPrimerka
{
    public int StatusId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<PrimerakGradje> PrimerakGradjes { get; set; } = new List<PrimerakGradje>();
}
