namespace PrototipoCajaArequipa
{
    // Nodo de la lista doble (basado en el Lab 2).
    // Tiene dos referencias: al nodo anterior y al siguiente.
    public class NodoDoble
    {
        public Operacion Dato { get; }
        public NodoDoble? Siguiente { get; set; }
        public NodoDoble? Anterior { get; set; }

        public NodoDoble(Operacion dato)
        {
            Dato = dato;
            Siguiente = null;
            Anterior = null;
        }
    }
}
