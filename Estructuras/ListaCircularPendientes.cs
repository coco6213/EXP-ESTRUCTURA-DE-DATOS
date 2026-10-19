namespace PrototipoCajaArequipa
{
    // OPERACIONES PENDIENTES DE ENVÍO A LAS AGENCIAS (lista enlazada circular).
    // Adaptado de ListaCircular (Lab 3): el último nodo apunta otra vez al primero.
    // Regla: si head no es null, last tampoco lo es (por eso a veces se usa "!").
    public class ListaCircularPendientes
    {
        private NodoCircular? head;
        private NodoCircular? last;
        private NodoCircular? actual;   // operación que se revisará en el siguiente paso

        public ListaCircularPendientes()
        {
            head = null;
            last = null;
            actual = null;
        }

        public void Agregar(Operacion operacion)
        {
            NodoCircular nuevo = new NodoCircular(operacion);

            if (head == null)
            {
                head = nuevo;
                last = nuevo;
                nuevo.Siguiente = head;   // el único nodo apunta a sí mismo
                actual = head;
            }
            else
            {
                last!.Siguiente = nuevo;
                nuevo.Siguiente = head;   // el nuevo último vuelve al primero
                last = nuevo;
            }
        }

        // El recorrido termina al VOLVER al nodo inicial (do...while), así se evita el bucle infinito.
        public Operacion? Buscar(string numeroOperacion)
        {
            if (head == null)
                return null;

            string numero = numeroOperacion.Trim().ToUpper();
            NodoCircular recorrido = head;

            do
            {
                if (recorrido.Dato.NumeroOperacion == numero)
                    return recorrido.Dato;

                recorrido = recorrido.Siguiente;

            } while (recorrido != head);

            return null;
        }

        public bool Eliminar(string numeroOperacion)
        {
            if (head == null)
                return false;

            string numero = numeroOperacion.Trim().ToUpper();
            NodoCircular recorrido = head;
            NodoCircular anterior = last!;

            do
            {
                if (recorrido.Dato.NumeroOperacion == numero)
                {
                    // Caso: solo existe un nodo
                    if (head == last)
                    {
                        head = null;
                        last = null;
                        actual = null;
                    }
                    // Caso: eliminar el primer nodo
                    else if (recorrido == head)
                    {
                        head = head.Siguiente;
                        last!.Siguiente = head;

                        if (actual == recorrido)
                            actual = head;
                    }
                    // Caso: eliminar cualquier otro nodo
                    else
                    {
                        anterior.Siguiente = recorrido.Siguiente;

                        if (recorrido == last)
                            last = anterior;

                        if (actual == recorrido)
                            actual = recorrido.Siguiente;
                    }

                    return true;
                }

                anterior = recorrido;
                recorrido = recorrido.Siguiente;

            } while (recorrido != head);

            return false;
        }

        public Operacion? ObtenerActual()
        {
            return actual?.Dato;
        }

        // Pasa a la siguiente operación. Después de la última vuelve a la primera.
        public void Avanzar()
        {
            if (actual != null)
                actual = actual.Siguiente;
        }

        public int Contar()
        {
            if (head == null)
                return 0;

            int contador = 0;
            NodoCircular recorrido = head;

            do
            {
                contador++;
                recorrido = recorrido.Siguiente;
            } while (recorrido != head);

            return contador;
        }

        public bool EstaVacia()
        {
            return head == null;
        }

        public void Imprimir()
        {
            if (head == null)
                return;

            NodoCircular recorrido = head;
            int posicion = 1;

            do
            {
                Operacion op = recorrido.Dato;
                Console.WriteLine($"  {posicion}. {op.NumeroOperacion.PadRight(8)} | {op.AgenciaOrigen.PadRight(16)} | {op.MontoTexto()}");
                recorrido = recorrido.Siguiente;
                posicion++;
            } while (recorrido != head);

            Console.WriteLine($"  (después de la última se vuelve a la primera: {head.Dato.NumeroOperacion})");
        }
    }
}
