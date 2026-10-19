namespace PrototipoCajaArequipa
{
    // HISTORIAL DE MOVIMIENTOS (lista enlazada doble).
    // Adaptado de ListaDoble (Lab 2). Guarda TODAS las operaciones, también las anuladas.
    // A propósito no tiene métodos para eliminar: un historial que se puede borrar deja de ser confiable.
    public class ListaDobleHistorial
    {
        private NodoDoble? cabeza;
        private NodoDoble? cola;
        private NodoDoble? actualNavegacion;   // cursor para revisar una por una

        public ListaDobleHistorial()
        {
            cabeza = null;
            cola = null;
            actualNavegacion = null;
        }

        public void AgregarFinal(Operacion operacion)
        {
            NodoDoble nuevo = new NodoDoble(operacion);

            if (cabeza == null)
            {
                cabeza = nuevo;
                cola = nuevo;
                actualNavegacion = cabeza;
            }
            else
            {
                // Se conectan las dos flechas: cola -> nuevo y nuevo -> cola.
                cola!.Siguiente = nuevo;
                nuevo.Anterior = cola;
                cola = nuevo;
            }
        }

        public bool EstaVacia()
        {
            return cabeza == null;
        }

        public int Contar()
        {
            int contador = 0;
            NodoDoble? actual = cabeza;

            while (actual != null)
            {
                contador++;
                actual = actual.Siguiente;
            }

            return contador;
        }

        public Operacion? BuscarPorNumero(string numeroOperacion)
        {
            string numero = numeroOperacion.Trim().ToUpper();
            NodoDoble? actual = cabeza;

            while (actual != null)
            {
                if (actual.Dato.NumeroOperacion == numero)
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        // Recorre desde la cabeza (la primera operación) usando "Siguiente".
        public void ImprimirAdelante()
        {
            NodoDoble? actual = cabeza;
            int posicion = 1;

            while (actual != null)
            {
                Console.WriteLine($"  {posicion}. {actual.Dato.Resumen()}");
                actual = actual.Siguiente;
                posicion++;
            }
        }

        // Recorre desde la cola (la más reciente) usando "Anterior".
        // Solo es posible porque cada nodo guarda una referencia a su anterior.
        public void ImprimirAtras()
        {
            NodoDoble? actual = cola;
            int posicion = Contar();

            while (actual != null)
            {
                Console.WriteLine($"  {posicion}. {actual.Dato.Resumen()}");
                actual = actual.Anterior;
                posicion--;
            }
        }

        // ----- Navegación una por una (cursor) -----

        public Operacion? ObtenerActual()
        {
            if (actualNavegacion == null && cabeza != null)
                actualNavegacion = cabeza;

            return actualNavegacion?.Dato;
        }

        // Posición del cursor (1 = la primera operación). Devuelve 0 si la lista está vacía.
        public int PosicionActual()
        {
            int posicion = 1;
            NodoDoble? actual = cabeza;

            while (actual != null)
            {
                if (actual == actualNavegacion)
                    return posicion;

                actual = actual.Siguiente;
                posicion++;
            }

            return 0;
        }

        // Devuelve null si no hay una operación siguiente.
        public Operacion? MoverAdelante()
        {
            if (cabeza == null)
                return null;

            if (actualNavegacion == null)
                actualNavegacion = cabeza;

            if (actualNavegacion.Siguiente == null)
                return null;

            actualNavegacion = actualNavegacion.Siguiente;
            return actualNavegacion.Dato;
        }

        // Devuelve null si no hay una operación anterior.
        public Operacion? MoverAtras()
        {
            if (cabeza == null)
                return null;

            if (actualNavegacion == null)
                actualNavegacion = cabeza;

            if (actualNavegacion.Anterior == null)
                return null;

            actualNavegacion = actualNavegacion.Anterior;
            return actualNavegacion.Dato;
        }
    }
}
