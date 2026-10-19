namespace PrototipoCajaArequipa
{
    // REGISTRO DE OPERACIONES ACTIVAS (lista enlazada simple).
    // Adaptado de ListaEnlazadaTickets (Lab 1): la lógica de los enlaces es la misma,
    // pero cada nodo guarda una Operacion y los métodos devuelven true/false
    // en lugar de escribir mensajes (los mensajes los muestra Program).
    public class ListaSimpleOperaciones
    {
        private NodoSimple? head;
        private NodoSimple? cola;   // último nodo: permite insertar al final en un solo paso

        public ListaSimpleOperaciones()
        {
            head = null;
            cola = null;
        }

        // Inserta al final. No permite números vacíos ni repetidos.
        public bool AgregarOperacion(Operacion operacion)
        {
            if (string.IsNullOrWhiteSpace(operacion.NumeroOperacion))
                return false;

            if (BuscarOperacion(operacion.NumeroOperacion) != null)
                return false;

            NodoSimple nuevo = new NodoSimple(operacion);

            if (head == null)
            {
                head = nuevo;
                cola = nuevo;
                return true;
            }

            // No hace falta mover ningún elemento: solo se enlaza el nuevo nodo al final.
            cola!.Siguiente = nuevo;
            cola = nuevo;
            return true;
        }

        // Recorre desde el primer nodo hasta encontrar el número o llegar a null.
        public Operacion? BuscarOperacion(string numeroOperacion)
        {
            if (string.IsNullOrWhiteSpace(numeroOperacion))
                return null;

            string numero = numeroOperacion.Trim().ToUpper();
            NodoSimple? actual = head;

            while (actual != null)
            {
                if (actual.Dato.NumeroOperacion == numero)
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        // Quita una operación del registro (se usa al anular).
        public bool EliminarOperacion(string numeroOperacion)
        {
            if (head == null)
                return false;

            string numero = numeroOperacion.Trim().ToUpper();

            // Caso 1: la operación está en el primer nodo.
            if (head.Dato.NumeroOperacion == numero)
            {
                head = head.Siguiente;

                if (head == null)
                    cola = null;   // la lista quedó vacía

                return true;
            }

            // Caso 2: está en otra posición. Se busca el nodo ANTERIOR para "saltar" el eliminado.
            NodoSimple actual = head;

            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.Dato.NumeroOperacion == numero)
                {
                    if (actual.Siguiente == cola)
                        cola = actual;   // se eliminó el último: el anterior es la nueva cola

                    actual.Siguiente = actual.Siguiente.Siguiente;
                    return true;
                }

                actual = actual.Siguiente;
            }

            return false;
        }

        public int ContarOperaciones()
        {
            int contador = 0;
            NodoSimple? actual = head;

            while (actual != null)
            {
                contador++;
                actual = actual.Siguiente;
            }

            return contador;
        }

        public bool EstaVacia()
        {
            return head == null;
        }

        public void ListarOperaciones()
        {
            NodoSimple? actual = head;
            int posicion = 1;

            while (actual != null)
            {
                Console.WriteLine($"  {posicion}. {actual.Dato.Resumen()}");
                actual = actual.Siguiente;
                posicion++;
            }
        }
    }
}
