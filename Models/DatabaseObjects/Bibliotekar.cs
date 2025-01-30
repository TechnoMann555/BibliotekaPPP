using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Bibliotekar
{
    public int BibliotekarId { get; set; }

    public string Jbb { get; set; } = null!;

    public string ImePrezime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int OgranakFk { get; set; }

    public virtual BibliotekarskiNalog? BibliotekarskiNalog { get; set; }

    public virtual Ogranak OgranakFkNavigation { get; set; } = null!;
}
