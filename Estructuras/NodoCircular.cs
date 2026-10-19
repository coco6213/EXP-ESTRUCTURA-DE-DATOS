namespace PrototipoCajaArequipa
{
    // Nodo de la lista circular (basado en el Lab 3).
    public class NodoCircular
    {
        public Operacion Dato { get; }
        public NodoCircular Siguiente { get; set; }

        public NodoCircular(Operacion dato)
        {
            Dato = dato;
            // En una lista circular nunca hay "null": un nodo solo se apunta a sí mismo.
            Siguiente = this;
        }
    }
}
