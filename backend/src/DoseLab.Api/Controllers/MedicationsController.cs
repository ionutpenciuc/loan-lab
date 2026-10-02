using DoseLab.Api.Contracts;
using DoseLab.Application;
using Microsoft.AspNetCore.Mvc;

namespace DoseLab.Api.Controllers;

[ApiController]
[Route("api/medications")]
public sealed class MedicationsController(TaperPlanService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<MedicationResponse>> List()
    {
        return Ok(service.Medications().Select(MedicationResponse.From).ToList());
    }
}
