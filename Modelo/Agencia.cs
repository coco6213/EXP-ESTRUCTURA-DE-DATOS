namespace PrototipoCajaArequipa
{
    // Agencia ficticia. "Conectada" permite simular fallas de conexión
    // al enviar las operaciones pendientes.
    public class Agencia
    {
        public string Nombre { get; }
        public bool Conectada { get; set; }

        public Agencia(string nombre, bool conectada)
        {
            Nombre = nombre;
            Conectada = conectada;
        }

        public string EstadoTexto()
        {
            return Conectada ? "conectada" : "SIN CONEXIÓN";
        }
    }
}
