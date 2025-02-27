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

    public int AdminNalogFk { get; set; }

    public virtual Nalog AdminNalogFkNavigation { get; set; } = null!;

    public virtual Ogranak OgranakFkNavigation { get; set; } = null!;
}
