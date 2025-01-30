using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class BibliotekarskiNalog
{
    public int NalogFk { get; set; }

    public int BibliotekarFk { get; set; }

    public virtual Bibliotekar BibliotekarFkNavigation { get; set; } = null!;

    public virtual Nalog NalogFkNavigation { get; set; } = null!;
}
