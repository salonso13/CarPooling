namespace CarPooling
{
    public class Viaje
    {
        public int Id { get; set; }
        public string Conductor { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaHora { get; set; }
        public int AsientosDisponibles { get; set; }
        public List<string> Pasajeros { get; set; } = new List<string>();   
    }
}
