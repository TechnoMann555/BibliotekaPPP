using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.DatabaseObjects;

public partial class BibliotekaContext : DbContext
{
    public BibliotekaContext()
    {
    }

    public BibliotekaContext(DbContextOptions<BibliotekaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Autor> Autors { get; set; }

    public virtual DbSet<Bibliotekar> Bibliotekars { get; set; }

    public virtual DbSet<BibliotekarskiNalog> BibliotekarskiNalogs { get; set; }

    public virtual DbSet<Clan> Clans { get; set; }

    public virtual DbSet<Clanarina> Clanarinas { get; set; }

    public virtual DbSet<ClanskiKorisnickiNalog> ClanskiKorisnickiNalogs { get; set; }

    public virtual DbSet<Gradja> Gradjas { get; set; }

    public virtual DbSet<IzdavanjeGradje> IzdavanjeGradjes { get; set; }

    public virtual DbSet<Nalog> Nalogs { get; set; }

    public virtual DbSet<Naselje> Naseljes { get; set; }

    public virtual DbSet<OcenaProcitaneGradje> OcenaProcitaneGradjes { get; set; }

    public virtual DbSet<Ogranak> Ogranaks { get; set; }

    public virtual DbSet<Pozajmica> Pozajmicas { get; set; }

    public virtual DbSet<PrimerakGradje> PrimerakGradjes { get; set; }

    public virtual DbSet<StatusPrimerka> StatusPrimerkas { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-RICUM2T;Database=Biblioteka;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.AutorId).HasName("PK__Autor__F58AE9099717862A");

            entity.ToTable("Autor");

            entity.HasIndex(e => e.ImePrezime, "UQ__Autor__1613B026BCF38ACA").IsUnique();

            entity.Property(e => e.AutorId)
                .ValueGeneratedNever()
                .HasColumnName("AutorID");
            entity.Property(e => e.ImePrezime).HasMaxLength(80);
        });

        modelBuilder.Entity<Bibliotekar>(entity =>
        {
            entity.HasKey(e => e.BibliotekarId).HasName("PK__Bibliote__ABB78DE61C58BDE7");

            entity.ToTable("Bibliotekar");

            entity.HasIndex(e => e.Jbb, "uq_JBB").IsUnique();

            entity.Property(e => e.BibliotekarId)
                .ValueGeneratedNever()
                .HasColumnName("BibliotekarID");
            entity.Property(e => e.Email).HasMaxLength(40);
            entity.Property(e => e.ImePrezime).HasMaxLength(80);
            entity.Property(e => e.Jbb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("JBB");
            entity.Property(e => e.OgranakFk).HasColumnName("OgranakFK");

            entity.HasOne(d => d.OgranakFkNavigation).WithMany(p => p.Bibliotekars)
                .HasForeignKey(d => d.OgranakFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bibliotekar_ogranak");
        });

        modelBuilder.Entity<BibliotekarskiNalog>(entity =>
        {
            entity.HasKey(e => new { e.NalogFk, e.BibliotekarFk }).HasName("PK__Bibliote__623812E21D317046");

            entity.ToTable("BibliotekarskiNalog");

            entity.HasIndex(e => e.BibliotekarFk, "uq_bibliotekar").IsUnique();

            entity.HasIndex(e => e.NalogFk, "uq_bibliotekarskinalog").IsUnique();

            entity.Property(e => e.NalogFk).HasColumnName("NalogFK");
            entity.Property(e => e.BibliotekarFk).HasColumnName("BibliotekarFK");

            entity.HasOne(d => d.BibliotekarFkNavigation).WithOne(p => p.BibliotekarskiNalog)
                .HasForeignKey<BibliotekarskiNalog>(d => d.BibliotekarFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bibliotekarskinalog_bibliotekar");

            entity.HasOne(d => d.NalogFkNavigation).WithOne(p => p.BibliotekarskiNalog)
                .HasForeignKey<BibliotekarskiNalog>(d => d.NalogFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bibliotekarskinalog_nalog");
        });

        modelBuilder.Entity<Clan>(entity =>
        {
            entity.HasKey(e => e.ClanId).HasName("PK__Clan__EC03AA4444A0FCAD");

            entity.ToTable("Clan");

            entity.HasIndex(e => e.BrLicneKarte, "uq_BrLicneKarte_clan").IsUnique();

            entity.HasIndex(e => e.Jcb, "uq_JCB").IsUnique();

            entity.HasIndex(e => e.KontaktMejl, "uq_KontaktMejl_clan").IsUnique();

            entity.HasIndex(e => e.KontaktTelefon, "uq_KontaktTelefon_clan").IsUnique();

            entity.Property(e => e.ClanId).HasColumnName("ClanID");
            entity.Property(e => e.AdresaStanovanja).HasMaxLength(80);
            entity.Property(e => e.BrLicneKarte)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ImePrezime).HasMaxLength(80);
            entity.Property(e => e.ImeRoditelja).HasMaxLength(40);
            entity.Property(e => e.Jcb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("JCB");
            entity.Property(e => e.KontaktMejl).HasMaxLength(40);
            entity.Property(e => e.KontaktTelefon)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Zanimanje).HasMaxLength(50);
        });

        modelBuilder.Entity<Clanarina>(entity =>
        {
            entity.HasKey(e => new { e.ClanFk, e.Rbr }).HasName("PK__Clanarin__C0AC8B8FADFA6760");

            entity.ToTable("Clanarina");

            entity.HasIndex(e => e.DatumPocetka, "uq_DatumPocetka_clanarina").IsUnique();

            entity.Property(e => e.ClanFk).HasColumnName("ClanFK");
            entity.Property(e => e.Rbr).ValueGeneratedOnAdd();
            entity.Property(e => e.Cena).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ClanFkNavigation).WithMany(p => p.Clanarinas)
                .HasForeignKey(d => d.ClanFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_clanarina_clan");
        });

        modelBuilder.Entity<ClanskiKorisnickiNalog>(entity =>
        {
            entity.HasKey(e => new { e.NalogFk, e.ClanFk }).HasName("PK__ClanskiK__06435399EEBB2607");

            entity.ToTable("ClanskiKorisnickiNalog");

            entity.HasIndex(e => e.ClanFk, "uq_clan").IsUnique();

            entity.HasIndex(e => e.NalogFk, "uq_clanskikorisnickinalog").IsUnique();

            entity.Property(e => e.NalogFk).HasColumnName("NalogFK");
            entity.Property(e => e.ClanFk).HasColumnName("ClanFK");

            entity.HasOne(d => d.ClanFkNavigation).WithOne(p => p.ClanskiKorisnickiNalog)
                .HasForeignKey<ClanskiKorisnickiNalog>(d => d.ClanFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_clanskikorisnickinalog_clan");

            entity.HasOne(d => d.NalogFkNavigation).WithOne(p => p.ClanskiKorisnickiNalog)
                .HasForeignKey<ClanskiKorisnickiNalog>(d => d.NalogFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_clanskikorisnickinalog_nalog");
        });

        modelBuilder.Entity<Gradja>(entity =>
        {
            entity.HasKey(e => e.GradjaId).HasName("PK__Gradja__8EF0C7DB3F9F56E7");

            entity.ToTable("Gradja");

            entity.Property(e => e.GradjaId)
                .ValueGeneratedNever()
                .HasColumnName("GradjaID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(17)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN");
            entity.Property(e => e.IzdavanjeFk).HasColumnName("IzdavanjeFK");
            entity.Property(e => e.Naslov).HasMaxLength(80);
            entity.Property(e => e.NaslovnaStranaPath).HasMaxLength(255);
            entity.Property(e => e.Opis)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("text");
            entity.Property(e => e.Udk)
                .HasMaxLength(150)
                .HasColumnName("UDK");

            entity.HasOne(d => d.IzdavanjeFkNavigation).WithMany(p => p.Gradjas)
                .HasForeignKey(d => d.IzdavanjeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gradja_izdavanjegradje");

            entity.HasMany(d => d.AutorFks).WithMany(p => p.GradjaFks)
                .UsingEntity<Dictionary<string, object>>(
                    "GradjaAutor",
                    r => r.HasOne<Autor>().WithMany()
                        .HasForeignKey("AutorFk")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_gradjaautor_autor"),
                    l => l.HasOne<Gradja>().WithMany()
                        .HasForeignKey("GradjaFk")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_gradjaautor_gradja"),
                    j =>
                    {
                        j.HasKey("GradjaFk", "AutorFk").HasName("PK__GradjaAu__D1A8738B86D9D785");
                        j.ToTable("GradjaAutor");
                        j.IndexerProperty<int>("GradjaFk").HasColumnName("GradjaFK");
                        j.IndexerProperty<int>("AutorFk").HasColumnName("AutorFK");
                    });
        });

        modelBuilder.Entity<IzdavanjeGradje>(entity =>
        {
            entity.HasKey(e => e.IzdavanjeId).HasName("PK__Izdavanj__689FB244228711B7");

            entity.ToTable("IzdavanjeGradje");

            entity.HasIndex(e => new { e.NaseljeIzdavanja, e.NazivIzdavaca, e.GodinaIzdavanja }, "uq_izdavanje").IsUnique();

            entity.Property(e => e.IzdavanjeId)
                .ValueGeneratedNever()
                .HasColumnName("IzdavanjeID");
            entity.Property(e => e.GodinaIzdavanja).HasColumnType("numeric(4, 0)");
            entity.Property(e => e.NaseljeIzdavanja).HasMaxLength(50);
            entity.Property(e => e.NazivIzdavaca).HasMaxLength(50);
        });

        modelBuilder.Entity<Nalog>(entity =>
        {
            entity.HasKey(e => e.NalogId).HasName("PK__Nalog__D8833317EE34946F");

            entity.ToTable("Nalog");

            entity.Property(e => e.NalogId)
                .ValueGeneratedNever()
                .HasColumnName("NalogID");
            entity.Property(e => e.Password).HasMaxLength(30);
            entity.Property(e => e.UserRoleFk).HasColumnName("UserRoleFK");

            entity.HasOne(d => d.UserRoleFkNavigation).WithMany(p => p.Nalogs)
                .HasForeignKey(d => d.UserRoleFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_nalog_userrole");
        });

        modelBuilder.Entity<Naselje>(entity =>
        {
            entity.HasKey(e => e.NaseljeId).HasName("PK__Naselje__1B116FA4CCA74004");

            entity.ToTable("Naselje");

            entity.HasIndex(e => e.Naziv, "uq_naziv_naselje").IsUnique();

            entity.Property(e => e.NaseljeId)
                .ValueGeneratedNever()
                .HasColumnName("NaseljeID");
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        modelBuilder.Entity<OcenaProcitaneGradje>(entity =>
        {
            entity.HasKey(e => new { e.GradjaFk, e.ClanskiKorisnickiNalogNalogFk, e.ClanskiKorisnickiNalogClanFk }).HasName("PK__OcenaPro__49E3613030744136");

            entity.ToTable("OcenaProcitaneGradje");

            entity.Property(e => e.GradjaFk).HasColumnName("GradjaFK");
            entity.Property(e => e.ClanskiKorisnickiNalogNalogFk).HasColumnName("ClanskiKorisnickiNalog_NalogFK");
            entity.Property(e => e.ClanskiKorisnickiNalogClanFk).HasColumnName("ClanskiKorisnickiNalog_ClanFK");

            entity.HasOne(d => d.GradjaFkNavigation).WithMany(p => p.OcenaProcitaneGradjes)
                .HasForeignKey(d => d.GradjaFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ocenaprocitanegradje_gradja");

            entity.HasOne(d => d.ClanskiKorisnickiNalog).WithMany(p => p.OcenaProcitaneGradjes)
                .HasForeignKey(d => new { d.ClanskiKorisnickiNalogNalogFk, d.ClanskiKorisnickiNalogClanFk })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ocenaprocitanegradje_clanskikorisnickinalog");
        });

        modelBuilder.Entity<Ogranak>(entity =>
        {
            entity.HasKey(e => e.OgranakId).HasName("PK__Ogranak__C185428282E78201");

            entity.ToTable("Ogranak");

            entity.HasIndex(e => new { e.NaseljeFk, e.RbrUokviruNaselja }, "uq_ogranak").IsUnique();

            entity.Property(e => e.OgranakId)
                .ValueGeneratedNever()
                .HasColumnName("OgranakID");
            entity.Property(e => e.Adresa).HasMaxLength(80);
            entity.Property(e => e.NaseljeFk).HasColumnName("NaseljeFK");
            entity.Property(e => e.Naziv).HasMaxLength(80);
            entity.Property(e => e.RbrUokviruNaselja).HasColumnName("RbrUOkviruNaselja");

            entity.HasOne(d => d.NaseljeFkNavigation).WithMany(p => p.Ogranaks)
                .HasForeignKey(d => d.NaseljeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ogranak_naselje");
        });

        modelBuilder.Entity<Pozajmica>(entity =>
        {
            entity.HasKey(e => new { e.ClanarinaClanFk, e.ClanarinaFk, e.Rbr }).HasName("PK__Pozajmic__DFC4A8CA59BDE580");

            entity.ToTable("Pozajmica");

            entity.Property(e => e.ClanarinaClanFk).HasColumnName("Clanarina_ClanFK");
            entity.Property(e => e.ClanarinaFk).HasColumnName("ClanarinaFK");
            entity.Property(e => e.Rbr).ValueGeneratedOnAdd();
            entity.Property(e => e.PrimerakGradjeFk).HasColumnName("PrimerakGradjeFK");
            entity.Property(e => e.PrimerakGradjeGradjaFk).HasColumnName("PrimerakGradje_GradjaFK");
            entity.Property(e => e.PrimerakGradjeOgranakFk).HasColumnName("PrimerakGradje_OgranakFK");

            entity.HasOne(d => d.Clanarina).WithMany(p => p.Pozajmicas)
                .HasForeignKey(d => new { d.ClanarinaClanFk, d.ClanarinaFk })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pozajmica_clanarina");

            entity.HasOne(d => d.PrimerakGradje).WithMany(p => p.Pozajmicas)
                .HasForeignKey(d => new { d.PrimerakGradjeGradjaFk, d.PrimerakGradjeOgranakFk, d.PrimerakGradjeFk })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pozajmica_primerakgradje");
        });

        modelBuilder.Entity<PrimerakGradje>(entity =>
        {
            entity.HasKey(e => new { e.GradjaFk, e.OgranakFk, e.RbrUokviruOgranka }).HasName("PK__Primerak__CD993853AF7948B4");

            entity.ToTable("PrimerakGradje");

            entity.HasIndex(e => e.InventarniBroj, "uq_inventarnibroj_primerka").IsUnique();

            entity.Property(e => e.GradjaFk).HasColumnName("GradjaFK");
            entity.Property(e => e.OgranakFk).HasColumnName("OgranakFK");
            entity.Property(e => e.RbrUokviruOgranka).HasColumnName("RbrUOkviruOgranka");
            entity.Property(e => e.InventarniBroj)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Signatura).HasMaxLength(80);
            entity.Property(e => e.StatusFk).HasColumnName("StatusFK");

            entity.HasOne(d => d.GradjaFkNavigation).WithMany(p => p.PrimerakGradjes)
                .HasForeignKey(d => d.GradjaFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_primerakgradje_gradja");

            entity.HasOne(d => d.OgranakFkNavigation).WithMany(p => p.PrimerakGradjes)
                .HasForeignKey(d => d.OgranakFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_primerakgradje_ogranak");

            entity.HasOne(d => d.StatusFkNavigation).WithMany(p => p.PrimerakGradjes)
                .HasForeignKey(d => d.StatusFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_primerakgradje_statusprimerka");
        });

        modelBuilder.Entity<StatusPrimerka>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusPr__C8EE2043EBEB4E2E");

            entity.ToTable("StatusPrimerka");

            entity.HasIndex(e => e.Naziv, "uq_naziv_statusa").IsUnique();

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.Naziv).HasMaxLength(20);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A556FAC99C4");

            entity.ToTable("UserRole");

            entity.HasIndex(e => e.Naziv, "uq_naziv_userrole").IsUnique();

            entity.Property(e => e.UserRoleId)
                .ValueGeneratedNever()
                .HasColumnName("UserRoleID");
            entity.Property(e => e.Naziv).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
