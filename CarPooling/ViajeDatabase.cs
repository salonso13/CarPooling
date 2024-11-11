namespace CarPooling
{
    public static class ViajeDatabase
    {
        public static List<Viaje> Viajes { get; } = new List<Viaje>();
        public static int ContadorId = 1;  // Contador de ID secuencial
    }
}
