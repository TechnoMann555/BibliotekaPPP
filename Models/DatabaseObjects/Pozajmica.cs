using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Pozajmica
{
    public int ClanarinaClanFk { get; set; }

    public int ClanarinaFk { get; set; }

    public int Rbr { get; set; }

    public int PrimerakGradjeGradjaFk { get; set; }

    public int PrimerakGradjeOgranakFk { get; set; }

    public int PrimerakGradjeFk { get; set; }

    public DateOnly DatumPocetka { get; set; }

    public DateOnly RokRazduzenja { get; set; }

    public DateOnly? DatumRazduzenja { get; set; }

    public virtual Clanarina Clanarina { get; set; } = null!;

    public virtual PrimerakGradje PrimerakGradje { get; set; } = null!;
}
