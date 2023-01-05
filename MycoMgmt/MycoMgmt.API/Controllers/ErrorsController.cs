using Microsoft.AspNetCore.Mvc;

namespace MycoMgmt.API.Controllers;

public class ErrorsController : ControllerBase
{
   [Route("/error")]
   public IActionResult Error()
   {
      return Problem();
   }
}