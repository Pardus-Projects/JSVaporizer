using Microsoft.AspNetCore.Mvc;
using MyViewLib;

namespace MyExampleApplication.Controllers
{

    [ApiController]
    [Route("BarberAppointmentController")]
    public class BarberAppointmentController : Controller
    {

        [HttpPost("MyRequestHandler")]
        public ActionResult<string> MyRequestHandler([FromForm] string dtoJSON)
        {

            BarberAppointmentTransformer xformer = new();
            MyCoolTransformerDto dto = xformer.JsonToDto(dtoJSON);

            bool valid = xformer.ValidateDto(dto, out string errMessage);
            if (valid)
            {
                return "You just booked an apointment.";
            }
            else
            {
                return "You failed to book and appointment, because: " + errMessage; ;
            }
            
        }
    }
}
