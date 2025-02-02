using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace BibliotekaPPP.Filters
{
    public class KorisnikClanRequiredFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var korisnik = context.HttpContext.Request.Cookies["Korisnik"];
            if(string.IsNullOrEmpty(korisnik))
            {
                context.Result = new RedirectToActionResult("Login", "Nalog", null);
            }
            else
            {
                NalogRepository nalogRepository = new NalogRepository();
                NalogBO korisnickiNalog = JsonSerializer.Deserialize<NalogBO>(korisnik);

                if(korisnickiNalog.Uloga != "Korisnik_Clan")
                {
                    context.Result = new RedirectToActionResult("Login", "Nalog", null);
                }
            }
        }
    }
}
