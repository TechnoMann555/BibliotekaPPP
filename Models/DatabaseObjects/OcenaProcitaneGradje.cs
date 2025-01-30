using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class OcenaProcitaneGradje
{
    public int GradjaFk { get; set; }

    public int ClanskiKorisnickiNalogNalogFk { get; set; }

    public int ClanskiKorisnickiNalogClanFk { get; set; }

    public int Ocena { get; set; }

    public virtual ClanskiKorisnickiNalog ClanskiKorisnickiNalog { get; set; } = null!;

    public virtual Gradja GradjaFkNavigation { get; set; } = null!;
}
