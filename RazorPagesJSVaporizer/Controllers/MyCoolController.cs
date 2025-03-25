using Microsoft.AspNetCore.Mvc;
using JSVTransformer;

namespace RazorPagesJSVaporizer
{

    [ApiController]
    [Route("MyCoolController")]
    public class MyCoolController : Controller
    {

        [HttpPost("MyRequestHandler")]
        public ActionResult<string> MyRequestHandler([FromForm] string dtoJSON)
        {

            MyCoolTransformer xformer = new MyCoolTransformer();
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
