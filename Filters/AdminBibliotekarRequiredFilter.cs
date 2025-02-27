using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace BibliotekaPPP.Filters
{
    public class AdminBibliotekarRequiredFilter : IAuthorizationFilter
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
                NalogBO adminNalog = JsonSerializer.Deserialize<NalogBO>(korisnik);

                if(adminNalog.Uloga != "Admin_Bibliotekar")
                {
                    context.Result = new RedirectToActionResult("Login", "Nalog", null);
                }
            }
        }
    }
}
