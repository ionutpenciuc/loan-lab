using LoanLab.Api;
using LoanLab.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<ILoanAccountRepository, InMemoryLoanAccountRepository>();
builder.Services.AddSingleton<LoanAccountService>();

var app = builder.Build();

LoanAccountSeed.Apply(app.Services.GetRequiredService<ILoanAccountRepository>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.MapControllers();
app.Run();

public partial class Program;
