using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class ClanskiKorisnickiNalog
{
    public int NalogFk { get; set; }

    public int ClanFk { get; set; }

    public virtual Clan ClanFkNavigation { get; set; } = null!;

    public virtual Nalog NalogFkNavigation { get; set; } = null!;

    public virtual ICollection<OcenaProcitaneGradje> OcenaProcitaneGradjes { get; set; } = new List<OcenaProcitaneGradje>();
}
