using DoseLab.Api.Contracts;
using DoseLab.Application;
using Microsoft.AspNetCore.Mvc;

namespace DoseLab.Api.Controllers;

[ApiController]
[Route("api/medications")]
public sealed class MedicationsController : ControllerBase
{
    private readonly TaperPlanService _service;

    public MedicationsController(TaperPlanService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<MedicationResponse>> List()
    {
        return Ok(_service.Medications().Select(MedicationResponse.From).ToList());
    }
}
