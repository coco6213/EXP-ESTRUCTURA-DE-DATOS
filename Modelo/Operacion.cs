using System.Globalization;

namespace PrototipoCajaArequipa
{
    // Tipos de operación de la billetera. Un "enum" es una lista cerrada de valores:
    // si se escribe mal un valor, el compilador lo detecta.
    public enum TipoOperacion
    {
        Transferencia,
        Recarga,
        PagoServicio
    }

    // Estados por los que pasa una operación.
    public enum EstadoOperacion
    {
        PendienteEnvio,
        Enviada,
        Anulada
    }

    // Dato central del prototipo.
    // Por cada operación se crea UN solo objeto; los nodos de las cuatro estructuras
    // guardan una referencia a ese mismo objeto (no copias).
    public class Operacion
    {
        public string NumeroOperacion { get; }
        public TipoOperacion Tipo { get; }
        public string AgenciaOrigen { get; }
        public string? AgenciaDestino { get; }   // solo en transferencias; en los demás tipos es null
        public decimal Monto { get; }             // decimal evita errores de redondeo con dinero
        public EstadoOperacion Estado { get; set; }

        public Operacion(string numeroOperacion, TipoOperacion tipo,
                         string agenciaOrigen, string? agenciaDestino, decimal monto)
        {
            NumeroOperacion = numeroOperacion.Trim().ToUpper();
            Tipo = tipo;
            AgenciaOrigen = agenciaOrigen;
            AgenciaDestino = agenciaDestino;
            Monto = monto;
            Estado = EstadoOperacion.PendienteEnvio;   // toda operación nueva empieza pendiente de envío
        }

        // Regla del prototipo: los pagos de servicio no se pueden anular.
        public bool EsAnulable
        {
            get { return Tipo != TipoOperacion.PagoServicio; }
        }

        public string NombreTipo()
        {
            switch (Tipo)
            {
                case TipoOperacion.Transferencia: return "Transferencia";
                case TipoOperacion.Recarga: return "Recarga de saldo";
                default: return "Pago de servicio";
            }
        }

        public string NombreEstado()
        {
            switch (Estado)
            {
                case EstadoOperacion.PendienteEnvio: return "Pendiente de envío";
                case EstadoOperacion.Enviada: return "Enviada";
                default: return "ANULADA";
            }
        }

        public string MontoTexto()
        {
            return "S/ " + Monto.ToString("#,##0.00", CultureInfo.InvariantCulture);
        }

        public string AgenciasTexto()
        {
            if (AgenciaDestino == null)
                return AgenciaOrigen;

            return AgenciaOrigen + " -> " + AgenciaDestino;
        }

        // Una sola línea, para mostrar listas.
        public string Resumen()
        {
            return NumeroOperacion.PadRight(8) + " | " +
                   NombreTipo().PadRight(16) + " | " +
                   AgenciasTexto().PadRight(26) + " | " +
                   MontoTexto().PadLeft(11) + " | " +
                   NombreEstado();
        }

        // Varias líneas, para mostrar una operación completa.
        public string Detalle()
        {
            string texto =
                "  N.° de operación:         " + NumeroOperacion + "\n" +
                "  Tipo:                     " + NombreTipo() + "\n" +
                "  Agencia donde se realizó: " + AgenciaOrigen + "\n";

            if (AgenciaDestino != null)
                texto += "  Agencia que recibe:       " + AgenciaDestino + "\n";

            texto +=
                "  Monto:                    " + MontoTexto() + "\n" +
                "  Estado:                   " + NombreEstado();

            return texto;
        }

        // La pila usa ToString() para mostrar su contenido: así se ve "[ OP001 OP002 ] <- Tope".
        public override string ToString()
        {
            return NumeroOperacion;
        }
    }
}
