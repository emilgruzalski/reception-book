using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace ReceptionBook.Presentation.Controllers;

[Route("api/dashboard")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IServiceManager _service;

    public DashboardController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _service.DashboardService.GetDashboardAsync();

        return Ok(dashboard);
    }
}
