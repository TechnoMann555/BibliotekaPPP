using System;
using System.Collections.Generic;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Nalog> Nalogs { get; set; } = new List<Nalog>();
}
