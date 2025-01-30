using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class Gradja
{
    public int GradjaId { get; set; }

    public string NaslovnaStranaPath { get; set; } = null!;

    public string Naslov { get; set; } = null!;

    public string? Opis { get; set; }

    public string Isbn { get; set; } = null!;

    public string Udk { get; set; } = null!;

    public int IzdavanjeFk { get; set; }

    public virtual IzdavanjeGradje IzdavanjeFkNavigation { get; set; } = null!;

    public virtual ICollection<OcenaProcitaneGradje> OcenaProcitaneGradjes { get; set; } = new List<OcenaProcitaneGradje>();

    public virtual ICollection<PrimerakGradje> PrimerakGradjes { get; set; } = new List<PrimerakGradje>();

    public virtual ICollection<Autor> AutorFks { get; set; } = new List<Autor>();
}
