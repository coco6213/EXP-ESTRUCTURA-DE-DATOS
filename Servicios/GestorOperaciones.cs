namespace PrototipoCajaArequipa
{
    // Coordina las cuatro estructuras.
    // Registrar o anular una operación cambia varias estructuras a la vez; si cada opción
    // del menú lo hiciera por su cuenta, sería fácil olvidar un paso y dejar datos incoherentes.
    // Aquí esa coordinación está escrita en UN solo lugar.
    public class GestorOperaciones
    {
        public const decimal MontoMinimo = 0.01m;
        public const decimal MontoMaximo = 5000.00m;   // límite ficticio del prototipo

        private readonly ListaSimpleOperaciones registro = new ListaSimpleOperaciones();
        private readonly ListaDobleHistorial historial = new ListaDobleHistorial();
        private readonly ListaCircularPendientes pendientes = new ListaCircularPendientes();
        private readonly Pila<Operacion> anulables = new Pila<Operacion>();

        // Catálogo fijo de agencias ficticias. Aquí un arreglo es adecuado porque
        // nunca se insertan ni se eliminan agencias. Puno empieza sin conexión para la demostración.
        private readonly Agencia[] agencias =
        {
            new Agencia("Arequipa Centro", true),
            new Agencia("Cayma", true),
            new Agencia("Cusco", true),
            new Agencia("Puno", false),
            new Agencia("Tacna", true)
        };

        public ListaSimpleOperaciones Registro { get { return registro; } }
        public ListaDobleHistorial Historial { get { return historial; } }
        public ListaCircularPendientes Pendientes { get { return pendientes; } }
        public Pila<Operacion> Anulables { get { return anulables; } }
        public Agencia[] Agencias { get { return agencias; } }

        // ----- Validaciones -----

        // Devuelve un mensaje de error, o null si el número es válido.
        public string? ValidarNumero(string numeroOperacion)
        {
            if (string.IsNullOrWhiteSpace(numeroOperacion))
                return "El número de operación no puede estar vacío.";

            if (numeroOperacion.Trim().Contains(' '))
                return "El número de operación no puede tener espacios.";

            if (ExisteNumero(numeroOperacion))
                return $"Ya existe una operación con el número {numeroOperacion.Trim().ToUpper()}.";

            return null;
        }

        // Se revisan las DOS listas: una operación anulada ya no está en el registro de activas,
        // pero sigue en el historial, y su número no debe volver a usarse.
        public bool ExisteNumero(string numeroOperacion)
        {
            return registro.BuscarOperacion(numeroOperacion) != null ||
                   historial.BuscarPorNumero(numeroOperacion) != null;
        }

        public string? ValidarMonto(decimal monto)
        {
            if (monto < MontoMinimo || monto > MontoMaximo)
                return "El monto debe estar entre S/ 0.01 y S/ 5,000.00.";

            if (decimal.Round(monto, 2) != monto)
                return "El monto puede tener como máximo 2 decimales.";

            return null;
        }

        // ----- Registro (usa las 4 estructuras) -----

        // Devuelve un mensaje de error, o null si se registró correctamente.
        public string? Registrar(Operacion operacion)
        {
            string? error = ValidarNumero(operacion.NumeroOperacion);
            if (error != null)
                return error;

            error = ValidarMonto(operacion.Monto);
            if (error != null)
                return error;

            if (operacion.Tipo == TipoOperacion.Transferencia)
            {
                if (operacion.AgenciaDestino == null)
                    return "Una transferencia necesita una agencia que recibe.";

                if (operacion.AgenciaDestino == operacion.AgenciaOrigen)
                    return "La agencia que recibe debe ser distinta a la agencia donde se realizó.";
            }
            else if (operacion.AgenciaDestino != null)
            {
                return "Solo las transferencias tienen agencia que recibe.";
            }

            // El MISMO objeto se agrega a cada estructura (no se hacen copias).
            registro.AgregarOperacion(operacion);   // lista simple: operaciones activas
            historial.AgregarFinal(operacion);      // lista doble: historial completo
            pendientes.Agregar(operacion);          // lista circular: falta enviarla a la agencia

            if (operacion.EsAnulable)
                anulables.Push(operacion);          // pila: se podrá anular

            return null;
        }

        // Carga las operaciones de ejemplo. Devuelve cuántas se agregaron
        // (las que ya existían se omiten para no repetir números).
        public int CargarDatosEjemplo()
        {
            int cargadas = 0;

            foreach (Operacion operacion in DatosEjemplo.Crear())
            {
                if (Registrar(operacion) == null)
                    cargadas++;
            }

            return cargadas;
        }

        // ----- Anulación (pila) -----

        // Consulta la cima de la pila sin sacarla. Devuelve null si la pila está vacía.
        public Operacion? ProximaAnulable()
        {
            if (anulables.IsEmpty())
                return null;

            return anulables.Top();
        }

        // Saca la cima de la pila y deja coherentes las demás estructuras.
        // Devuelve null si no hay nada que anular. (Simulación: no mueve dinero real.)
        public Operacion? AnularUltima()
        {
            // Se revisa antes, porque Pop() con la pila vacía lanza una excepción.
            if (anulables.IsEmpty())
                return null;

            Operacion operacion = anulables.Pop();

            operacion.Estado = EstadoOperacion.Anulada;          // el historial ve este cambio automáticamente
            registro.EliminarOperacion(operacion.NumeroOperacion);   // ya no es una operación activa
            pendientes.Eliminar(operacion.NumeroOperacion);          // si estaba pendiente, ya no se envía

            return operacion;
        }

        // "Cerrar el día": las operaciones siguen registradas, pero ya no se pueden anular.
        public void CerrarDia()
        {
            anulables.Clear();
        }

        // ----- Envío a agencias (lista circular) -----

        // Revisa CADA operación pendiente exactamente una vez.
        // Antes de empezar se cuenta cuántas hay y se dan exactamente esos pasos.
        // No basta con "parar al volver al inicio": si el nodo inicial se elimina
        // durante la revisión, nunca se volvería a él y el recorrido no terminaría.
        public List<string> EnviarPendientes()
        {
            List<string> pasos = new List<string>();
            int total = pendientes.Contar();

            if (total == 0)
                return pasos;

            int enviadas = 0;

            for (int paso = 1; paso <= total; paso++)
            {
                Operacion? operacion = pendientes.ObtenerActual();
                if (operacion == null)
                    break;   // seguridad: la lista quedó vacía

                Agencia? agencia = BuscarAgencia(operacion.AgenciaOrigen);
                string inicio = $"  Paso {paso} de {total}: {operacion.NumeroOperacion} - {operacion.AgenciaOrigen}";

                if (agencia != null && agencia.Conectada)
                {
                    operacion.Estado = EstadoOperacion.Enviada;
                    pendientes.Eliminar(operacion.NumeroOperacion);   // el cursor pasa solo a la siguiente
                    enviadas++;
                    pasos.Add(inicio + " (conectada) -> ENVIADA");
                }
                else
                {
                    pendientes.Avanzar();   // se queda para la próxima revisión
                    pasos.Add(inicio + " (sin conexión) -> sigue pendiente");
                }
            }

            pasos.Add("");
            if (total == 1)
                pasos.Add("  Fin de la revisión: se revisó la única operación pendiente.");
            else
                pasos.Add($"  Fin de la revisión: se revisaron las {total} operaciones una sola vez.");
            pasos.Add($"  Enviadas: {enviadas}. Siguen pendientes: {total - enviadas}.");

            return pasos;
        }

        public Agencia? BuscarAgencia(string nombre)
        {
            foreach (Agencia agencia in agencias)
            {
                if (agencia.Nombre == nombre)
                    return agencia;
            }

            return null;
        }
    }
}
