using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Autor
{
    public int AutorId { get; set; }

    public string ImePrezime { get; set; } = null!;

    public virtual ICollection<Gradja> GradjaFks { get; set; } = new List<Gradja>();
}
