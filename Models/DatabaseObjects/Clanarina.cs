using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Clanarina
{
    public int ClanFk { get; set; }

    public int Rbr { get; set; }

    public DateOnly DatumPocetka { get; set; }

    public DateOnly DatumZavrsetka { get; set; }

    public decimal Cena { get; set; }

    public virtual Clan ClanFkNavigation { get; set; } = null!;

    public virtual ICollection<Pozajmica> Pozajmicas { get; set; } = new List<Pozajmica>();
}
