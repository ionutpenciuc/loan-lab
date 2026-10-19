using DoseLab.Application;
using Npgsql;

namespace DoseLab.Postgres;

/// <summary>
/// Stores taper plans in PostgreSQL. Turn it on with <c>Lab:Storage = Postgres</c>.
/// The database gives each plan its sequence number, so reference numbers stay unique under parallel saves.
/// </summary>
public sealed class PostgresTaperPlanRepository(NpgsqlDataSource dataSource) : ITaperPlanRepository
{
    public void EnsureSchema()
    {
        using var command = dataSource.CreateCommand("""
            CREATE TABLE IF NOT EXISTS taper_plans (
                sequence_number         bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                id                      uuid    NOT NULL UNIQUE,
                patient_name            text    NOT NULL,
                medication_code         text    NOT NULL,
                starting_daily_dose_mg  numeric NOT NULL,
                week_count              integer NOT NULL,
                created_on              date    NOT NULL
            )
            """);
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<TaperPlan> List()
    {
        using var command = dataSource.CreateCommand("""
            SELECT sequence_number, id, patient_name, medication_code, starting_daily_dose_mg, week_count, created_on
            FROM taper_plans
            ORDER BY sequence_number
            """);
        using var reader = command.ExecuteReader();

        var plans = new List<TaperPlan>();
        while (reader.Read())
        {
            plans.Add(new TaperPlan(
                reader.GetGuid(1),
                ReferenceNumber(reader.GetInt64(0)),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetDecimal(4),
                reader.GetInt32(5),
                reader.GetFieldValue<DateOnly>(6)));
        }

        return plans;
    }

    public TaperPlan Add(NewTaperPlan draft)
    {
        var id = Guid.NewGuid();

        using var command = dataSource.CreateCommand("""
            INSERT INTO taper_plans (id, patient_name, medication_code, starting_daily_dose_mg, week_count, created_on)
            VALUES (@id, @patient_name, @medication_code, @starting_daily_dose_mg, @week_count, @created_on)
            RETURNING sequence_number
            """);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("patient_name", draft.PatientName);
        command.Parameters.AddWithValue("medication_code", draft.MedicationCode);
        command.Parameters.AddWithValue("starting_daily_dose_mg", draft.StartingDailyDoseMg);
        command.Parameters.AddWithValue("week_count", draft.WeekCount);
        command.Parameters.AddWithValue("created_on", draft.CreatedOn);

        var sequenceNumber = (long)command.ExecuteScalar()!;

        return new TaperPlan(
            id,
            ReferenceNumber(sequenceNumber),
            draft.PatientName,
            draft.MedicationCode,
            draft.StartingDailyDoseMg,
            draft.WeekCount,
            draft.CreatedOn);
    }

    private static string ReferenceNumber(long sequenceNumber) => $"TP-{sequenceNumber:0000}";
}
