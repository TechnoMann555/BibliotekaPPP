using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Nalog
{
    public int NalogId { get; set; }

    public string Password { get; set; } = null!;

    public int UserRoleFk { get; set; }

    public virtual BibliotekarskiNalog? BibliotekarskiNalog { get; set; }

    public virtual ClanskiKorisnickiNalog? ClanskiKorisnickiNalog { get; set; }

    public virtual UserRole UserRoleFkNavigation { get; set; } = null!;
}
