using LoanLab.Api.Contracts;
using LoanLab.Application;
using Microsoft.AspNetCore.Mvc;

namespace LoanLab.Api.Controllers;

[ApiController]
[Route("api/loan-accounts")]
public sealed class LoanAccountsController : ControllerBase
{
    private readonly LoanAccountService _service;

    public LoanAccountsController(LoanAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<LoanAccountResponse>> List()
    {
        var accounts = _service.List().Select(LoanAccountResponse.From).ToList();
        return Ok(accounts);
    }

    [HttpPost("preview")]
    public ActionResult<ScheduleResponse> Preview(PreviewScheduleRequest request)
    {
        try
        {
            var schedule = _service.Preview(request.Amount, request.AnnualInterestRate, request.InstallmentCount);
            return Ok(ScheduleResponse.From(schedule));
        }
        catch (LoanValidationException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }

    [HttpPost]
    public ActionResult<LoanAccountResponse> Create(CreateLoanAccountRequest request)
    {
        try
        {
            var account = _service.Create(
                request.CustomerName,
                request.Amount,
                request.AnnualInterestRate,
                request.InstallmentCount);
            var body = LoanAccountResponse.From(account);
            return Created($"/api/loan-accounts/{account.Id}", body);
        }
        catch (LoanValidationException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message));
        }
    }
}
