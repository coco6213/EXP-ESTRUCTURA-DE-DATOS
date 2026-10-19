using System.Globalization;
using System.Text;

namespace PrototipoCajaArequipa
{
    // Menú del prototipo. Program solo pide datos y muestra resultados;
    // las reglas y el manejo de las estructuras están en GestorOperaciones.
    class Program
    {
        static GestorOperaciones gestor = new GestorOperaciones();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;   // para mostrar tildes y la ñ

            string opcion;

            do
            {
                Console.Clear();
                Console.WriteLine("==============================================");
                Console.WriteLine("   CAJA AREQUIPA - BILLETERA MÓVIL (PRUEBA)");
                Console.WriteLine("   Simulación para clase: los datos son ficticios");
                Console.WriteLine("==============================================");
                Console.WriteLine(" 1. Registrar una nueva operación");
                Console.WriteLine(" 2. Cargar operaciones de ejemplo (para la demostración)");
                Console.WriteLine(" 3. Ver todas las operaciones activas");
                Console.WriteLine(" 4. Buscar una operación por su número");
                Console.WriteLine(" 5. Ver el historial de movimientos");
                Console.WriteLine(" 6. Ver operaciones pendientes de envío a agencias");
                Console.WriteLine(" 7. Anular una operación registrada por error");
                Console.WriteLine(" 8. Ver resumen general");
                Console.WriteLine(" 0. Salir");
                Console.WriteLine("==============================================");
                Console.Write("Elija una opción: ");

                opcion = LeerTexto();

                switch (opcion)
                {
                    case "1": RegistrarOperacion(); break;
                    case "2": CargarEjemplos(); break;
                    case "3": VerOperacionesActivas(); break;
                    case "4": BuscarOperacion(); break;
                    case "5": MenuHistorial(); break;
                    case "6": MenuPendientes(); break;
                    case "7": MenuAnular(); break;
                    case "8": VerResumen(); break;
                    case "0":
                        Console.WriteLine("\nGracias por usar el prototipo. Programa finalizado.");
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida. Escriba un número del 0 al 8.");
                        Pausar();
                        break;
                }

            } while (opcion != "0");
        }

        // ==================== 1. REGISTRAR ====================

        static void RegistrarOperacion()
        {
            Console.Clear();
            Titulo("REGISTRAR UNA NUEVA OPERACIÓN");
            Console.WriteLine("(Para cancelar, deje el número vacío y presione ENTER)\n");

            string numero;
            while (true)
            {
                Console.Write("Número de operación (ejemplo: OP010): ");
                numero = LeerTexto().ToUpper();

                if (numero == "")
                {
                    Console.WriteLine("\nRegistro cancelado. No se guardó nada.");
                    Pausar();
                    return;
                }

                string? error = gestor.ValidarNumero(numero);
                if (error == null)
                    break;

                Console.WriteLine("  " + error + " Intente con otro.");
            }

            TipoOperacion tipo = PedirTipo();

            Console.WriteLine("\nAgencia donde se realizó la operación:");
            string origen = PedirAgencia(null);

            string? destino = null;
            if (tipo == TipoOperacion.Transferencia)
            {
                Console.WriteLine("\nAgencia que recibe la transferencia:");
                destino = PedirAgencia(origen);
            }

            decimal monto = PedirMonto();

            Operacion operacion = new Operacion(numero, tipo, origen, destino, monto);
            string? errorRegistro = gestor.Registrar(operacion);

            if (errorRegistro != null)
            {
                Console.WriteLine("\nNo se pudo registrar: " + errorRegistro);
            }
            else
            {
                Console.WriteLine("\nOperación registrada correctamente:");
                Console.WriteLine(operacion.Detalle());
                Console.WriteLine();

                if (operacion.EsAnulable)
                    Console.WriteLine("Si se registró por error, podrá anularla desde la opción 7.");
                else
                    Console.WriteLine("Aviso: los pagos de servicio NO se pueden anular.");

                Console.WriteLine($"Queda pendiente de envío a la agencia {origen} (opción 6).");
            }

            Pausar();
        }

        static TipoOperacion PedirTipo()
        {
            Console.WriteLine("\nTipo de operación:");
            Console.WriteLine("  1. Transferencia a otra agencia");
            Console.WriteLine("  2. Recarga de saldo");
            Console.WriteLine("  3. Pago de servicio (luz, agua, teléfono)");

            while (true)
            {
                Console.Write("Elija el tipo (1, 2 o 3): ");
                string texto = LeerTexto();

                if (texto == "1") return TipoOperacion.Transferencia;
                if (texto == "2") return TipoOperacion.Recarga;
                if (texto == "3") return TipoOperacion.PagoServicio;

                Console.WriteLine("  Opción no válida. Escriba 1, 2 o 3.");
            }
        }

        // Muestra las agencias numeradas. "excluir" evita elegir la misma agencia dos veces.
        static string PedirAgencia(string? excluir)
        {
            Agencia[] agencias = gestor.Agencias;

            for (int i = 0; i < agencias.Length; i++)
                Console.WriteLine($"  {i + 1}. {agencias[i].Nombre}");

            while (true)
            {
                Console.Write($"Elija la agencia (1 al {agencias.Length}): ");

                if (int.TryParse(LeerTexto(), out int numero) && numero >= 1 && numero <= agencias.Length)
                {
                    string nombre = agencias[numero - 1].Nombre;

                    if (nombre == excluir)
                    {
                        Console.WriteLine("  La agencia que recibe debe ser distinta a la agencia donde se realizó.");
                        continue;
                    }

                    return nombre;
                }

                Console.WriteLine($"  Opción no válida. Escriba un número del 1 al {agencias.Length}.");
            }
        }

        static decimal PedirMonto()
        {
            while (true)
            {
                Console.Write("\nMonto en soles (ejemplo: 150.50): ");
                string texto = LeerTexto().Replace(',', '.');   // se acepta 150,50 o 150.50

                if (!decimal.TryParse(texto, NumberStyles.AllowDecimalPoint,
                                      CultureInfo.InvariantCulture, out decimal monto))
                {
                    Console.WriteLine("  Monto no válido. Escriba solo números, por ejemplo 150.50");
                    continue;
                }

                string? error = gestor.ValidarMonto(monto);
                if (error == null)
                    return monto;

                Console.WriteLine("  " + error);
            }
        }

        // ==================== 2. EJEMPLOS ====================

        static void CargarEjemplos()
        {
            Console.Clear();
            Titulo("CARGAR OPERACIONES DE EJEMPLO");

            int total = DatosEjemplo.Crear().Length;
            int cargadas = gestor.CargarDatosEjemplo();

            if (cargadas == 0)
            {
                Console.WriteLine("Las operaciones de ejemplo ya estaban cargadas.");
                Console.WriteLine("No se agregó nada, para no repetir números de operación.");
            }
            else
            {
                Console.WriteLine($"Se cargaron {cargadas} operaciones de ejemplo.");
                if (cargadas < total)
                    Console.WriteLine($"({total - cargadas} ya existían y se omitieron para no repetir números.)");

                Console.WriteLine("\nOperaciones activas ahora:");
                gestor.Registro.ListarOperaciones();
                Console.WriteLine("\nNota: la agencia de Puno empieza SIN CONEXIÓN, para mostrar envíos que quedan pendientes.");
            }

            Pausar();
        }

        // ==================== 3. ACTIVAS ====================

        static void VerOperacionesActivas()
        {
            Console.Clear();
            Titulo("OPERACIONES ACTIVAS");
            Console.WriteLine("(No incluye las operaciones anuladas; esas se ven en el historial)\n");

            if (gestor.Registro.EstaVacia())
            {
                Console.WriteLine("No hay operaciones activas. Puede registrar una (opción 1) o cargar ejemplos (opción 2).");
            }
            else
            {
                gestor.Registro.ListarOperaciones();
                Console.WriteLine($"\nTotal de operaciones activas: {gestor.Registro.ContarOperaciones()}");
            }

            Pausar();
        }

        // ==================== 4. BUSCAR ====================

        static void BuscarOperacion()
        {
            Console.Clear();
            Titulo("BUSCAR UNA OPERACIÓN");
            Console.Write("Escriba el número de operación (ejemplo: OP001): ");
            string numero = LeerTexto().ToUpper();

            if (numero == "")
            {
                Console.WriteLine("\nNo escribió ningún número.");
            }
            else
            {
                Operacion? encontrada = gestor.Registro.BuscarOperacion(numero);

                if (encontrada != null)
                {
                    Console.WriteLine("\nOperación encontrada:");
                    Console.WriteLine(encontrada.Detalle());
                }
                else if (gestor.Historial.BuscarPorNumero(numero) != null)
                {
                    Console.WriteLine($"\nLa operación {numero} fue ANULADA. Ya no está entre las operaciones activas,");
                    Console.WriteLine("pero puede verla en el historial de movimientos (opción 5).");
                }
                else
                {
                    Console.WriteLine($"\nNo existe ninguna operación con el número {numero}.");
                }
            }

            Pausar();
        }

        // ==================== 5. HISTORIAL ====================

        static void MenuHistorial()
        {
            string opcion;

            do
            {
                Console.Clear();
                Titulo("HISTORIAL DE MOVIMIENTOS");
                Console.WriteLine("Aquí aparecen TODAS las operaciones registradas, también las anuladas.\n");
                Console.WriteLine(" a. Ver desde la primera operación hasta la más reciente");
                Console.WriteLine(" b. Ver desde la más reciente hasta la primera");
                Console.WriteLine(" c. Revisar las operaciones una por una (anterior / siguiente)");
                Console.WriteLine(" d. Volver");
                Console.Write("\nElija una opción: ");

                opcion = LeerTexto().ToLower();

                switch (opcion)
                {
                    case "a":
                        Console.WriteLine("\nDesde la primera hasta la más reciente:");
                        if (gestor.Historial.EstaVacia())
                            Console.WriteLine("  El historial está vacío. Todavía no se registraron operaciones.");
                        else
                            gestor.Historial.ImprimirAdelante();
                        Pausar();
                        break;

                    case "b":
                        Console.WriteLine("\nDesde la más reciente hasta la primera:");
                        if (gestor.Historial.EstaVacia())
                            Console.WriteLine("  El historial está vacío. Todavía no se registraron operaciones.");
                        else
                            gestor.Historial.ImprimirAtras();
                        Pausar();
                        break;

                    case "c":
                        NavegarHistorial();
                        break;

                    case "d":
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Escriba a, b, c o d.");
                        Pausar();
                        break;
                }

            } while (opcion != "d");
        }

        static void NavegarHistorial()
        {
            if (gestor.Historial.EstaVacia())
            {
                Console.WriteLine("\nEl historial está vacío. Todavía no se registraron operaciones.");
                Pausar();
                return;
            }

            string opcion;
            string mensaje = "";

            do
            {
                Console.Clear();
                Titulo("REVISAR OPERACIONES UNA POR UNA");

                Operacion? actual = gestor.Historial.ObtenerActual();
                Console.WriteLine($"Operación {gestor.Historial.PosicionActual()} de {gestor.Historial.Contar()}\n");
                if (actual != null)
                    Console.WriteLine(actual.Detalle());

                if (mensaje != "")
                {
                    Console.WriteLine("\n" + mensaje);
                    mensaje = "";
                }

                Console.WriteLine("\n a. Anterior   b. Siguiente   c. Volver");
                Console.Write("\nElija una opción: ");

                opcion = LeerTexto().ToLower();

                switch (opcion)
                {
                    case "a":
                        if (gestor.Historial.MoverAtras() == null)
                            mensaje = "Ya está en la primera operación. No hay una anterior.";
                        break;

                    case "b":
                        if (gestor.Historial.MoverAdelante() == null)
                            mensaje = "Ya está en la operación más reciente. No hay una siguiente.";
                        break;

                    case "c":
                        break;

                    default:
                        mensaje = "Opción no válida. Escriba a, b o c.";
                        break;
                }

            } while (opcion != "c");
        }

        // ==================== 6. PENDIENTES DE ENVÍO ====================

        static void MenuPendientes()
        {
            string opcion;

            do
            {
                Console.Clear();
                Titulo("OPERACIONES PENDIENTES DE ENVÍO A AGENCIAS");
                Console.WriteLine("Cada operación debe enviarse a su agencia. Si la agencia no tiene conexión,");
                Console.WriteLine("la operación espera hasta la siguiente revisión.\n");
                Console.WriteLine(" a. Ver qué operaciones faltan enviar");
                Console.WriteLine(" b. Intentar enviar todas las pendientes (una revisión completa)");
                Console.WriteLine(" c. Conectar o desconectar una agencia");
                Console.WriteLine(" d. Volver");
                Console.Write("\nElija una opción: ");

                opcion = LeerTexto().ToLower();

                switch (opcion)
                {
                    case "a": VerPendientes(); break;
                    case "b": EnviarPendientes(); break;
                    case "c": CambiarConexion(); break;
                    case "d": break;
                    default:
                        Console.WriteLine("Opción no válida. Escriba a, b, c o d.");
                        Pausar();
                        break;
                }

            } while (opcion != "d");
        }

        static void VerPendientes()
        {
            Console.WriteLine();

            if (gestor.Pendientes.EstaVacia())
            {
                Console.WriteLine("No hay operaciones pendientes de envío. Todo está al día.");
            }
            else
            {
                Console.WriteLine($"Operaciones que faltan enviar ({gestor.Pendientes.Contar()}):");
                gestor.Pendientes.Imprimir();
            }

            Console.WriteLine();
            MostrarAgencias();
            Pausar();
        }

        static void EnviarPendientes()
        {
            Console.WriteLine();
            List<string> pasos = gestor.EnviarPendientes();

            if (pasos.Count == 0)
            {
                Console.WriteLine("No hay operaciones pendientes de envío. Todo está al día.");
            }
            else
            {
                Console.WriteLine("Revisando las operaciones pendientes, una por una:\n");
                foreach (string linea in pasos)
                    Console.WriteLine(linea);
            }

            Pausar();
        }

        static void CambiarConexion()
        {
            Console.WriteLine();
            MostrarAgencias();

            Agencia[] agencias = gestor.Agencias;
            Console.Write($"\nElija la agencia a cambiar (1 al {agencias.Length}), o deje vacío para volver: ");
            string texto = LeerTexto();

            if (texto == "")
                return;

            if (int.TryParse(texto, out int numero) && numero >= 1 && numero <= agencias.Length)
            {
                Agencia agencia = agencias[numero - 1];
                agencia.Conectada = !agencia.Conectada;
                Console.WriteLine($"\nLa agencia {agencia.Nombre} ahora está {agencia.EstadoTexto().ToUpper()}.");
            }
            else
            {
                Console.WriteLine($"\nOpción no válida. Debe escribir un número del 1 al {agencias.Length}.");
            }

            Pausar();
        }

        // ==================== 7. ANULAR ====================

        static void MenuAnular()
        {
            string opcion;

            do
            {
                Console.Clear();
                Titulo("ANULAR UNA OPERACIÓN REGISTRADA POR ERROR");
                Console.WriteLine("Solo se puede anular la ÚLTIMA operación registrada que todavía sea anulable.");
                Console.WriteLine("Los pagos de servicio no se pueden anular.\n");
                Console.WriteLine(" a. Ver cuál sería la próxima operación en anularse");
                Console.WriteLine(" b. Anular esa operación (se pedirá confirmación)");
                Console.WriteLine(" c. Ver la lista de operaciones que aún se pueden anular");
                Console.WriteLine(" d. Cerrar el día (las operaciones de hoy ya no se podrán anular)");
                Console.WriteLine(" e. Volver");
                Console.Write("\nElija una opción: ");

                opcion = LeerTexto().ToLower();

                switch (opcion)
                {
                    case "a": VerProximaAnulable(); break;
                    case "b": AnularOperacion(); break;
                    case "c": VerListaAnulables(); break;
                    case "d": CerrarDia(); break;
                    case "e": break;
                    default:
                        Console.WriteLine("Opción no válida. Escriba a, b, c, d o e.");
                        Pausar();
                        break;
                }

            } while (opcion != "e");
        }

        static void VerProximaAnulable()
        {
            Console.WriteLine();
            Operacion? operacion = gestor.ProximaAnulable();

            if (operacion == null)
            {
                MensajeNadaQueAnular();
            }
            else
            {
                Console.WriteLine("La próxima operación que se anularía es:");
                Console.WriteLine(operacion.Detalle());
                Console.WriteLine("\n(Solo se está consultando: todavía no se anuló nada.)");
            }

            Pausar();
        }

        static void AnularOperacion()
        {
            Console.WriteLine();
            Operacion? proxima = gestor.ProximaAnulable();

            if (proxima == null)
            {
                MensajeNadaQueAnular();
                Pausar();
                return;
            }

            Console.WriteLine("Se anulará la siguiente operación:");
            Console.WriteLine(proxima.Detalle());

            if (!Confirmar("\n¿Confirma la anulación? (s/n): "))
            {
                Console.WriteLine("\nAnulación cancelada. No se realizó ningún cambio.");
                Pausar();
                return;
            }

            Operacion? anulada = gestor.AnularUltima();

            if (anulada != null)
            {
                Console.WriteLine($"\nLa operación {anulada.NumeroOperacion} fue ANULADA.");
                Console.WriteLine("  - Ya no aparece en 'Operaciones activas' (opción 3).");
                Console.WriteLine("  - En el historial (opción 5) sigue apareciendo, con estado ANULADA.");
                Console.WriteLine("  - Si estaba pendiente de envío, se retiró de la lista de pendientes.");
            }

            Pausar();
        }

        static void VerListaAnulables()
        {
            Console.WriteLine();

            if (gestor.Anulables.IsEmpty())
            {
                MensajeNadaQueAnular();
            }
            else
            {
                Console.WriteLine($"Operaciones que aún se pueden anular ({gestor.Anulables.Cantidad}):");
                Console.WriteLine("  " + gestor.Anulables);
            }

            Pausar();
        }

        static void CerrarDia()
        {
            Console.WriteLine();

            if (gestor.Anulables.IsEmpty())
            {
                Console.WriteLine("No hay operaciones que se puedan anular, así que no hace falta cerrar el día.");
                Pausar();
                return;
            }

            Console.WriteLine($"Después de cerrar el día, las {gestor.Anulables.Cantidad} operaciones que hoy se pueden anular");
            Console.WriteLine("ya NO se podrán anular. Las operaciones siguen registradas normalmente.");

            if (Confirmar("\n¿Desea cerrar el día? (s/n): "))
            {
                gestor.CerrarDia();
                Console.WriteLine("\nDía cerrado. Ya no hay operaciones que se puedan anular.");
            }
            else
            {
                Console.WriteLine("\nCierre cancelado. No se realizó ningún cambio.");
            }

            Pausar();
        }

        static void MensajeNadaQueAnular()
        {
            Console.WriteLine("No hay operaciones que se puedan anular.");
            Console.WriteLine("Esto pasa cuando: todavía no se registraron operaciones, ya se anularon todas,");
            Console.WriteLine("solo hay pagos de servicio (que no se anulan) o ya se cerró el día.");
        }

        // ==================== 8. RESUMEN ====================

        static void VerResumen()
        {
            Console.Clear();
            Titulo("RESUMEN GENERAL");
            Console.WriteLine($"  Operaciones activas ......................... {gestor.Registro.ContarOperaciones()}");
            Console.WriteLine($"  Movimientos en el historial ................. {gestor.Historial.Contar()}");
            Console.WriteLine($"  Operaciones pendientes de envío ............. {gestor.Pendientes.Contar()}");
            Console.WriteLine($"  Operaciones que aún se pueden anular ........ {gestor.Anulables.Cantidad}");
            Console.WriteLine();
            MostrarAgencias();

            Pausar();
        }

        // ==================== AYUDANTES ====================

        static void MostrarAgencias()
        {
            Console.WriteLine("Estado de conexión de las agencias:");

            Agencia[] agencias = gestor.Agencias;
            for (int i = 0; i < agencias.Length; i++)
                Console.WriteLine($"  {i + 1}. {agencias[i].Nombre.PadRight(16)} {agencias[i].EstadoTexto()}");
        }

        static bool Confirmar(string pregunta)
        {
            while (true)
            {
                Console.Write(pregunta);
                string respuesta = LeerTexto().ToLower();

                if (respuesta == "s") return true;
                if (respuesta == "n") return false;

                Console.WriteLine("  Responda solo con s (sí) o n (no).");
            }
        }

        static string LeerTexto()
        {
            return (Console.ReadLine() ?? "").Trim();
        }

        static void Titulo(string texto)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("  " + texto);
            Console.WriteLine("==============================================");
        }

        static void Pausar()
        {
            Console.WriteLine("\nPresione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}
