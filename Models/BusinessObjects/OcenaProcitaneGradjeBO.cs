using BibliotekaPPP.Models.DatabaseObjects;

namespace BibliotekaPPP.Models.BusinessObjects
{
    public class OcenaProcitaneGradjeBO
    {
        public int GradjaFk { get; set; }

        public int ClanskiKorisnickiNalogFk { get; set; }

        public int Ocena { get; set; }

        public OcenaProcitaneGradjeBO() { }

        public OcenaProcitaneGradjeBO(OcenaProcitaneGradje ocena)
        {
            this.GradjaFk = ocena.GradjaFk;
            this.ClanskiKorisnickiNalogFk = ocena.ClanskiKorisnickiNalogFk;
            this.Ocena = ocena.Ocena;
        }
    }
}
