namespace CarPooling
{
    public static class ViajeManager
    {
        // Listar todos los viajes disponibles
        public static IResult ListarViajes()
        {
            return Results.Ok(ViajeDatabase.Viajes);
        }

        // Obtener detalles de un viaje por ID
        public static IResult ObtenerViajePorId(int id)
        {
            foreach (var viaje in ViajeDatabase.Viajes)
            {
                if (viaje.Id == id)
                {
                    return Results.Ok(viaje);
                }
            }
            return Results.NotFound("Viaje no encontrado.");
        }

        // Publicar un nuevo viaje
        public static IResult PublicarViaje(Viaje nuevoViaje)
        {
            nuevoViaje.Id = ViajeDatabase.ContadorId++;
            ViajeDatabase.Viajes.Add(nuevoViaje);
            return Results.Created($"/viajes/{nuevoViaje.Id}", nuevoViaje);
        }

        // Reservar un asiento en un viaje
        public static IResult ReservarAsiento(int id, string pasajero)
        {
            foreach (var viaje in ViajeDatabase.Viajes)
            {
                if (viaje.Id == id)
                {
                    if (viaje.AsientosDisponibles > 0)
                    {
                        viaje.Pasajeros.Add(pasajero);
                        viaje.AsientosDisponibles--;
                        return Results.Ok("Asiento reservado exitosamente.");
                    }
                    return Results.BadRequest("No hay asientos disponibles.");
                }
            }
            return Results.NotFound("Viaje no encontrado.");
        }

        // Cancelar un viaje por ID
        public static IResult CancelarViaje(int id)
        {
            for (int i = 0; i < ViajeDatabase.Viajes.Count; i++)
            {
                if (ViajeDatabase.Viajes[i].Id == id)
                {
                    ViajeDatabase.Viajes.RemoveAt(i);
                    return Results.NoContent();
                }
            }
            return Results.NotFound("Viaje no encontrado.");
        }
    }
}

