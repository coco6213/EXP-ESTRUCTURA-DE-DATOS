namespace PrototipoCajaArequipa
{
    // Nodo de la lista enlazada simple (basado en el Lab 1).
    // Guarda una referencia a la operación y una referencia al siguiente nodo.
    public class NodoSimple
    {
        public Operacion Dato { get; }
        public NodoSimple? Siguiente { get; set; }

        public NodoSimple(Operacion dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
