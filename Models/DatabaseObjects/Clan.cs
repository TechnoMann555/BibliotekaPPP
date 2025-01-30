using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Clan
{
    public int ClanId { get; set; }

    public string Jcb { get; set; } = null!;

    public DateOnly DatumUclanjenja { get; set; }

    public string BrLicneKarte { get; set; } = null!;

    public string ImePrezime { get; set; } = null!;

    public DateOnly DatumRodjenja { get; set; }

    public string ImeRoditelja { get; set; } = null!;

    public string AdresaStanovanja { get; set; } = null!;

    public string Zanimanje { get; set; } = null!;

    public string KontaktTelefon { get; set; } = null!;

    public string KontaktMejl { get; set; } = null!;

    public virtual ICollection<Clanarina> Clanarinas { get; set; } = new List<Clanarina>();

    public virtual ClanskiKorisnickiNalog? ClanskiKorisnickiNalog { get; set; }
}
