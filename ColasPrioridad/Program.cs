using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace EjercicioEstructura
{
    public class Cliente
    {
        public int ID { get; set; }
        public DateTime HoraLlegada { get; set; }
        public int Prioridad { get; set; }
        public Cliente SIG { get; set; }
        public Cliente ANT { get; set; }
    }

    public class ColaPrioridades
    {
        private Cliente Frente;
        private Cliente Fin;
        private int numClientes;

        public ColaPrioridades()
        {
            Frente = null;
            Fin = null;
            numClientes = 0;
        }

        public bool ColaLlena(int maxClientes)
        {
            return numClientes >= maxClientes;
        }

        public bool ColaVacia()
        {
            return numClientes == 0;
        }

        public void MeterEnLaCola(int maxClientes)
        {
            if (ColaLlena(maxClientes))
            {
                Console.WriteLine("La cola está llena");
            }
            else
            {
                Random random = new Random();
                int nuevoID = 0;
                bool ID_OK = false;

                while (!ID_OK)
                {
                    nuevoID = random.Next(1, maxClientes + 1);
                    ID_OK = VerificarID(nuevoID);
                }

                int prioridad = 1; // Por defecto
                if (nuevoID % 5 == 0)
                {
                    prioridad = random.Next(3, 5); // Prioridad alta (3 o 4)
                }
                else
                {
                    prioridad = random.Next(1, 3); // Prioridad baja (1 o 2)
                }

                Cliente nuevoCliente = new Cliente
                {
                    ID = nuevoID,
                    HoraLlegada = DateTime.Now,
                    Prioridad = prioridad
                };

                if (ColaVacia() || nuevoCliente.Prioridad > Fin.Prioridad)
                {
                    if (ColaVacia())
                    {
                        Frente = Fin = nuevoCliente;
                    }
                    else
                    {
                        Fin.SIG = nuevoCliente;
                        nuevoCliente.ANT = Fin;
                        Fin = nuevoCliente;
                    }
                }
                else
                {
                    Cliente actual = Frente;
                    while (actual != null && actual.Prioridad >= nuevoCliente.Prioridad)
                    {
                        actual = actual.SIG;
                    }

                    if (actual != null)
                    {
                        nuevoCliente.SIG = actual;
                        nuevoCliente.ANT = actual.ANT;
                        if (actual.ANT != null)
                        {
                            actual.ANT.SIG = nuevoCliente;
                        }
                        actual.ANT = nuevoCliente;
                    }
                    else
                    {
                        nuevoCliente.SIG = actual;
                        nuevoCliente.ANT = actual.ANT;
                        actual.ANT.SIG = nuevoCliente;
                        actual.ANT = nuevoCliente;
                    }
                }

                numClientes++;
                Console.WriteLine($"Cliente {nuevoCliente.ID} ha sido añadido a la cola a las {nuevoCliente.HoraLlegada:HH:mm:ss}");
            }
        }

        public void SacarDeLaCola()
        {
            if (ColaVacia())
            {
                Console.WriteLine("La cola está vacía");
            }
            else
            {
                Cliente clienteAtendido = Frente;
                Frente = Frente.SIG;
                if (Frente != null)
                {
                    Frente.ANT = null;
                }
                else
                {
                    Fin = null;
                }
                numClientes--;
                Console.WriteLine($"Cliente {clienteAtendido.ID} ha sido atendido a las {DateTime.Now:HH:mm:ss}");
            }
        }

        public void MostrarLaCola()
        {
            if (ColaVacia())
            {
                Console.WriteLine("La cola está vacía");
            }
            else
            {
                Console.WriteLine("Clientes en la cola:");
                Cliente actual = Frente;
                while (actual != null)
                {
                    Console.WriteLine($"Cliente {actual.ID} - Hora de llegada: {actual.HoraLlegada:HH:mm:ss}");
                    actual = actual.SIG;
                }
            }
        }

        public void VaciarLaCola()
        {
            if (ColaVacia())
            {
                Console.WriteLine("La cola está vacía");
            }
            else
            {
                while (!ColaVacia())
                {
                    SacarDeLaCola();
                }
            }
        }

        public void CasosEspeciales()
        {
            if (ColaVacia())
            {
                Console.WriteLine("No hay clientes en la cola para atender (no FIFO).");
            }
            else
            {
                Cliente cliente = Fin;
                if (cliente.ANT != null)
                {
                    cliente.ANT.SIG = null;
                    Fin = cliente.ANT;
                }
                else
                {
                    Frente = Fin = null;
                }
                numClientes--;
                Console.WriteLine($"{cliente.ID} ha sido atendido (no FIFO) y sacado de la cola.");
            }
        }

        public void Resultados(int maxClientes)
        {
            Console.WriteLine("Resultados finales:");
            Console.WriteLine($"Número total de clientes atendidos: {maxClientes - numClientes}");
            Console.WriteLine($"Número de clientes en la cola: {numClientes}");
            Console.WriteLine("Simulación finalizada. Resultados:");
            if (numClientes > 0)
            {
                Console.WriteLine($"Clientes restantes en la cola: {numClientes}");
                MostrarLaCola();
            }
            else
            {
                Console.WriteLine("No hay clientes restantes en la cola.");
            }
        }

        private bool VerificarID(int id)
        {
            Cliente actual = Frente;
            while (actual != null)
            {
                if (actual.ID == id)
                {
                    return false;
                }
                actual = actual.SIG;
            }
            return true;
        }
    }

    public class Program
    {
        static ColaPrioridades cola = new ColaPrioridades();
        static int maxClientes = 30;

        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese la hora para iniciar el programa (HH:MM): ");
            DateTime hInicial = DateTime.ParseExact(Console.ReadLine(), "HH:mm", CultureInfo.InvariantCulture);
            Console.WriteLine("Ingrese la hora para terminar el programa (HH:MM): ");
            DateTime hFinal = DateTime.ParseExact(Console.ReadLine(), "HH:mm", CultureInfo.InvariantCulture);

            Random random = new Random();
            DateTime hActual = DateTime.Now;

            while (hActual.TimeOfDay >= hInicial.TimeOfDay && hActual.TimeOfDay <= hFinal.TimeOfDay)
            {
                int op = random.Next(1, 6);

                switch (op)
                {
                    case 1:
                        cola.MeterEnLaCola(maxClientes);
                        break;
                    case 2:
                        cola.SacarDeLaCola();
                        break;
                    case 3:
                        cola.MostrarLaCola();
                        break;
                    case 4:
                        cola.VaciarLaCola();
                        break;
                    case 5:
                        cola.CasosEspeciales();
                        break;
                }

                Thread.Sleep(1000);
                hActual = DateTime.Now;
            }

            cola.Resultados(maxClientes);
            Console.ReadKey();
        }
    }
}
