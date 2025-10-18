using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenU2
{
    // Motor de consola (Singleton)
    sealed class MotorConsola
    {
        static readonly Lazy<MotorConsola> _inst = new Lazy<MotorConsola>(() => new MotorConsola());
        public static MotorConsola Instancia => _inst.Value;

        int ancho = 80, alto = 28, fps = 30; // tamaño deseado y fps
        double msObjetivo; 
        bool corriendo;
        bool teclaArriba, teclaAbajo, teclaEsc; // estados de teclas

        MotorConsola() 
        { 
            msObjetivo = 1000.0 / fps; // ms por fotograma
        }

        public void Ejecutar(Action<float> actualizar, Action dibujar, Func<bool> pedirSalir)
        {
            try
            {
                Console.CursorVisible = false; // Oculta cursor
                Console.SetWindowSize(Math.Min(ancho, Console.LargestWindowWidth), Math.Min(alto, Console.LargestWindowHeight));
                Console.SetBufferSize(ancho, alto);
            }
            catch 
            {
                // Ignorar errores al cambiar el tamaño de la consola (solo si es mayor al del sistema)
            }

            // inicia cronómetro y bucle
            var cron = Stopwatch.StartNew(); 
            double previo = cron.Elapsed.TotalMilliseconds;
            corriendo = true;

            while (corriendo)
            {
                // Lectura de input
                teclaArriba = teclaAbajo = false;
                while (Console.KeyAvailable)
                {
                    // asignación de teclas
                    var k = Console.ReadKey(true).Key;
                    if (k == ConsoleKey.UpArrow) teclaArriba = true;
                    if (k == ConsoleKey.DownArrow) teclaAbajo = true;
                    if (k == ConsoleKey.Escape) teclaEsc = true;
                }

                if (pedirSalir()) break; // si devuelve true, salir del bucle

                double ahora = cron.Elapsed.TotalMilliseconds;
                // calcula dt entre frames con Stopwatch
                float dt = (float)((ahora - previo) / 1000.0); previo = ahora;

                actualizar(dt); // lógica del juego

                Console.SetCursorPosition(0, 0);
                Console.Clear();
                dibujar(); // crea pantalla, gotas y suelo

                int dormir = (int)(msObjetivo - (Stopwatch.GetTimestamp() - cron.ElapsedTicks) * 1000.0 / Stopwatch.Frequency);
                if (dormir > 0)
                {
                    Thread.Sleep(dormir);
                }
            }
            try 
            { 
                Console.CursorVisible = true; // hace visible el cursor al salir
            } 
            catch 
            {
                // Ignorar errores al cambiar la visibilidad del cursor
            }
        }

        // Acciones
        public bool Arriba => teclaArriba;
        public bool Abajo => teclaAbajo;
        public bool Esc => teclaEsc; 
        public void ConfirmarEsc() 
        { 
            teclaEsc = false; 
        }

        // Dimensiones
        public int Ancho => ancho; 
        public int Alto => alto;

        // escribe texto en una fila/columna (recorta si se sale)
        public void EscribirEn(int x, int y, string s)
        {
            if (y < 0 || y >= alto)
            {
                return;
            }
                if (x < 0) x = 0;
            { 
                if (x + s.Length > ancho) s = s.Substring(0, Math.Max(0, ancho - x)); 
            }
            Console.SetCursorPosition(x, y); Console.Write(s);
        }

        // escribe un solo carácter en coordenadas
        public void Poner(int x, int y, char c)
        {
            if (x < 0 || x >= ancho || y < 0 || y >= alto)
            {
                return;
            }
            Console.SetCursorPosition(x, y); Console.Write(c);
        }
    }
}
