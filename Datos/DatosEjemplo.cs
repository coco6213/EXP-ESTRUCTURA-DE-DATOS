namespace PrototipoCajaArequipa
{
    // Operaciones FICTICIAS para la demostración en clase.
    // OP003 es un pago de servicio (no se puede anular) y OP003/OP005 son de Puno,
    // que empieza sin conexión: así quedan pendientes en la primera revisión.
    public static class DatosEjemplo
    {
        public static Operacion[] Crear()
        {
            return new Operacion[]
            {
                new Operacion("OP001", TipoOperacion.Transferencia, "Arequipa Centro", "Cusco", 250.00m),
                new Operacion("OP002", TipoOperacion.Recarga, "Cayma", null, 50.00m),
                new Operacion("OP003", TipoOperacion.PagoServicio, "Puno", null, 120.40m),
                new Operacion("OP004", TipoOperacion.Transferencia, "Tacna", "Arequipa Centro", 800.00m),
                new Operacion("OP005", TipoOperacion.Recarga, "Puno", null, 30.00m)
            };
        }
    }
}
