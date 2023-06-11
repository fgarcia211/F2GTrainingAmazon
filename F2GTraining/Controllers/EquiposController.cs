using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Mvc;
using F2GTraining.Services;
using F2GTraining.Models;

namespace F2GTraining.Controllers
{
    public class EquiposController : Controller
    {
        private ServiceAPIF2GTraining service;
        private ServiceSQS serviceSQS;

        public EquiposController(ServiceAPIF2GTraining service, ServiceSQS serviceSQS)
        {
            this.serviceSQS = serviceSQS;
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> MenuEquipo()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");
            List<Equipo> equipos = await this.service.GetEquiposUser(token);

            if (equipos.Count == 0)
            {
                return View();
            }
            else
            {
                //CHANGE
                ViewData["JUGADORESUSUARIO"] = await this.service.GetJugadoresUsuario(token);
                return View(equipos);
            }

        }

        [AuthorizeUsers]
        public async Task<IActionResult> _PartialVistaEquipo(int idequipo)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            return PartialView(await this.service.GetEquipo(token, idequipo));
        }

        [AuthorizeUsers]
        public IActionResult CrearEquipo()
        {
            return View();
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> CrearEquipo(string nombre, IFormFile imagen)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            string extension = System.IO.Path.GetExtension(imagen.FileName);

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                using (Stream stream = imagen.OpenReadStream())
                {
                    int rand = new Random().Next(0, 10000);
                    string nombrearchivo = nombre.Replace(" ","") + rand.ToString() + ".png";
                    string statusCode = await this.service.InsertEquipo(nombrearchivo, stream, nombre, token);
                    ViewData["NOSE"] = statusCode;
                }

                return RedirectToAction("MenuEquipo","Equipos");
            }
            else
            {
                ViewData["ERROR"] = "ERROR: La imagen debe ser en formato PNG";
                return View();
            }
        
        }

        [AuthorizeUsers]
        public async Task<IActionResult> NotasEquipo()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            List<Nota> notas = await this.serviceSQS.ReceiveMessagesAsync();

            if (notas == null)
            {
                return View();
            }

            List<Nota> notasUsuario = this.serviceSQS.NotasXIdUsuario(notas, idusuario);

            if (notasUsuario != null && notasUsuario.Count > 0)
            {
                ViewData["NOTAS"] = notas;
            }

            return View();
        }


        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> NotasEquipo(Nota notaEnvio)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            notaEnvio.IdUsuario = idusuario;

            await this.serviceSQS.SendMessageAsync(notaEnvio);

            return RedirectToAction("NotasEquipo");
        }

    }
}
