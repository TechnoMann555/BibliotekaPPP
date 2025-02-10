using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.DatabaseObjects;
using BibliotekaPPP.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaPPP.Models.EFRepository
{
    public class OcenaProcitaneGradjeRepository : IOcenaProcitaneGradjeRepository
    {
        BibliotekaContext bibliotekaContext = new BibliotekaContext();

        // [SK8] Ocenjivanje pročitane građe
        public async Task<OcenaProcitaneGradjeBO?> TraziOcenu(int gradjaID, int korisnickiNalogID)
        {
            OcenaProcitaneGradje? ocena = await bibliotekaContext.OcenaProcitaneGradjes
                                                .Where(o =>
                                                    o.GradjaFk == gradjaID &&
                                                    o.ClanskiKorisnickiNalogFk == korisnickiNalogID
                                                )
                                                .FirstOrDefaultAsync();

            if(ocena == null)
                return null;

            OcenaProcitaneGradjeBO ocenaBO = new OcenaProcitaneGradjeBO(ocena);

            return ocenaBO;
        }

        // [SK8] Ocenjivanje pročitane građe
        public async Task<OcenjivanjeGradjeResult> OceniGradju(OcenaProcitaneGradjeBO ocena)
        {
            // Provera da li već postoji ocena
            OcenaProcitaneGradje? postojecaOcena = await bibliotekaContext.OcenaProcitaneGradjes
                                                   .Where(o => 
                                                       o.GradjaFk == ocena.GradjaFk &&
                                                       o.ClanskiKorisnickiNalogFk == ocena.ClanskiKorisnickiNalogFk
                                                   )
                                                   .FirstOrDefaultAsync();

            // Ocena ne postoji - kreira se nova ocena
            if(postojecaOcena == null)
            {
                OcenaProcitaneGradje novaOcena = new OcenaProcitaneGradje()
                {
                    GradjaFk = ocena.GradjaFk,
                    ClanskiKorisnickiNalogFk = ocena.ClanskiKorisnickiNalogFk,
                    Ocena = ocena.Ocena
                };

                await bibliotekaContext.OcenaProcitaneGradjes.AddAsync(novaOcena);
                await bibliotekaContext.SaveChangesAsync();

                return OcenjivanjeGradjeResult.OcenaKreirana;
            }
            // Ocena postoji - azurira se ili brise u zavisnosti od vrednosti ocene
            else
            {
                // Ako je ocena u opsegu 1 - 10, azurira se ocena
                if(ocena.Ocena >= 1 && ocena.Ocena <= 10)
                {
                    postojecaOcena.Ocena = ocena.Ocena;
                    await bibliotekaContext.SaveChangesAsync();

                    return OcenjivanjeGradjeResult.OcenaAzurirana;
                }
                // Ocena nije u opsegu od 1 - 10, brise se ocena
                else
                {
                    bibliotekaContext.OcenaProcitaneGradjes.Remove(postojecaOcena);
                    await bibliotekaContext.SaveChangesAsync();

                    return OcenjivanjeGradjeResult.OcenaIzbrisana;
                }
            }
        }
    }
}
