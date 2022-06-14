using Microsoft.AspNetCore.Mvc;

namespace SimpleFileSystemServer.Controllers;

[Route("/error")]
public class ErrorController : Controller {
    public IActionResult Error() {
        return Problem();
    }
}
