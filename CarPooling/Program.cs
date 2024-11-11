using Microsoft.Data.Sqlite;
using CarPooling;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Conexión a la base de datos SQLite
string connectionString = "Data Source=CarPooling.db";

app.MapGet("/", () => "Hello World!");
app.MapGet("/status", () => Results.Ok("El servicio esta correcto y a la espera de obtener respuestas"));
app.MapGet("/cars", async () => 
{
    try
    {
        List<Car> coche = new List<Car>();
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Cars";
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    coche.Add(new Car
                    {
                        Id = reader.GetInt32(0),
                        Seats = reader.GetInt32(1),
                        AvSeats = reader.GetInt32(2)
                    });
                }
            }
        }
        return Results.Ok(coche);
    }
    catch (Exception e)
    {
        return Results.BadRequest("Error al intentar ejecutar la query" + e);
    }
});
app.MapPut("/cars", async (List<Car> body) =>
{
    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            for (int i = 0; i < body.Count; i++)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Cars SET Seats = @seat, AvailableSeats = @avseat WHERE Id = @id;";
                command.Parameters.AddWithValue("@id", body[i].Id);
                command.Parameters.AddWithValue("@seat", body[i].Seats);
                command.Parameters.AddWithValue("@avseat", body[i].AvSeats);
                await command.ExecuteNonQueryAsync();
            }
        }
        return Results.Ok("Los vehiculos fueron registrados correctamente");
    }
    catch (Exception e) 
    {
        return Results.BadRequest("Error al intentar ejecutar la query" + e);
    }
});
app.MapPost("/journey", (Group group) =>
{
    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Groups(Id, People) VALUES (@id,@people)";
            command.Parameters.AddWithValue("@id", group.Id);
            command.Parameters.AddWithValue("@people", group.People);
            command.ExecuteNonQuery();
        }
        return Results.Ok($"Grupo fue registrado correctamente");
    }
    catch (Exception e)
    {
        return Results.BadRequest("Error al intentar ejecutar la query ");
    }
});
app.MapPost("/dropoff/{id}", (int id) => 
{
    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Groups WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }
        return Results.Ok($"Grupo fue eliminado correctamente");
    }
    catch (Exception e)
    {
        return Results.BadRequest("Error al intentar ejecutar la query " + e);
    }
});
app.MapGet("/locate/{id}", async (int id) =>
{
    try
    {
        List<Journey> journey = new List<Journey>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT CarId FROM Journeys WHERE GroupId = @gid";
            command.Parameters.AddWithValue("@gid", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    journey.Add(new Journey
                    {
                        Id = reader.GetInt32(0),
                        CarId = reader.GetInt32(1),
                        GroupId = reader.GetInt32(2)
                    });
                }
            }
        }
        if (journey.Count == 0) 
        {
            return Results.NoContent();
        }
        return Results.Ok(journey);
    }
    catch (SqliteException e)
    {
        return Results.BadRequest("Error al intentar ejecutar la query: " + e.Message);
    }
    catch (Exception e)
    {
        return Results.NotFound("El grupo no fue encontrado: " + e.Message);
    }
});


//// Listado de Viajes Disponibles
//app.MapGet("/viajes", ViajeManager.ListarViajes);

//// Detalles de un Viaje por ID
//app.MapGet("/viajes/{id:int}", ViajeManager.ObtenerViajePorId);

//// Publicar un Viaje
//app.MapPost("/viajes", ViajeManager.PublicarViaje);

//// Reservar un Asiento en un Viaje
//app.MapPost("/viajes/reservar/{id:int}", ViajeManager.ReservarAsiento);

//// Cancelar un Viaje
//app.MapDelete("/viajes/{id:int}", ViajeManager.CancelarViaje);

app.Run();
