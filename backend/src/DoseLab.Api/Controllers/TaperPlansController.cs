using DoseLab.Api.Contracts;
using DoseLab.Api.Security;
using DoseLab.Application;
using Microsoft.AspNetCore.Mvc;

namespace DoseLab.Api.Controllers;

[ApiController]
[Route("api/taper-plans")]
public sealed class TaperPlansController : ControllerBase
{
    private readonly TaperPlanService _service;

    public TaperPlansController(TaperPlanService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<TaperPlanResponse>> List()
    {
        var plans = _service.List().Select(TaperPlanResponse.From).ToList();
        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<TaperPlanResponse> Get(Guid id)
    {
        var plan = _service.Find(id);
        if (plan is null)
            return NotFound(new ErrorResponse("Taper plan not found."));

        return Ok(TaperPlanResponse.From(plan));
    }

    [HttpPost("preview")]
    public ActionResult<ScheduleResponse> Preview(PreviewScheduleRequest request)
    {
        try
        {
            var schedule = _service.Preview(request.MedicationCode, request.StartingDailyDoseMg, request.WeekCount);
            return Ok(ScheduleResponse.From(schedule));
        }
        catch (TaperValidationException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }

    [HttpPost]
    [RequireApiKey]
    public ActionResult<TaperPlanResponse> Create(CreateTaperPlanRequest request)
    {
        try
        {
            var plan = _service.Create(
                request.PatientName,
                request.MedicationCode,
                request.StartingDailyDoseMg,
                request.WeekCount);
            var body = TaperPlanResponse.From(plan);
            return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
        }
        catch (TaperValidationException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }
}
