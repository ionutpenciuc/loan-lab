using DoseLab.Api;
using DoseLab.Application;
using DoseLab.Postgres;
using Npgsql;

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

// "InMemory" (default) or "Postgres". Postgres needs a database: see tools/docker-and-postgres.md.
if (builder.Configuration.GetValue("Lab:Storage", "InMemory") == "Postgres")
{
    var connectionString = builder.Configuration.GetConnectionString("DoseLab")
        ?? throw new InvalidOperationException("ConnectionStrings:DoseLab is required when Lab:Storage is Postgres.");
    builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
    builder.Services.AddSingleton<ITaperPlanRepository, PostgresTaperPlanRepository>();
}
else
{
    builder.Services.AddSingleton<ITaperPlanRepository, InMemoryTaperPlanRepository>();
}

builder.Services.AddSingleton<IMedicationCatalog, InMemoryMedicationCatalog>();
builder.Services.AddSingleton<TaperPlanService>();

var app = builder.Build();

var repository = app.Services.GetRequiredService<ITaperPlanRepository>();
if (repository is PostgresTaperPlanRepository postgres)
    postgres.EnsureSchema();
if (repository.List().Count == 0)
    DoseLabSeed.Apply(repository);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.MapControllers();
app.Run();

public partial class Program;
