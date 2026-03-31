using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoMSD.Pages
{
    [AllowAnonymous]
    public class AccesoPendienteModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
