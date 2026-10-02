using DoseLab.Api;
using DoseLab.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.IsLoopback)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var latencyMs = builder.Configuration.GetValue("Lab:SimulatedLatencyMs", 3);
builder.Services.AddSingleton(new SimulatedLatency(TimeSpan.FromMilliseconds(latencyMs)));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ITaperPlanRepository, InMemoryTaperPlanRepository>();
builder.Services.AddSingleton<IMedicationCatalog, InMemoryMedicationCatalog>();
builder.Services.AddSingleton<TaperPlanService>();

var app = builder.Build();

DoseLabSeed.Apply(app.Services.GetRequiredService<ITaperPlanRepository>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.MapControllers();
app.Run();

public partial class Program;
