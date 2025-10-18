using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenU2
{
    // Lógica del juego
    sealed class JuegoLluvia
    {
        readonly PiscinaObjetos<GotaLluvia> pool = new PiscinaObjetos<GotaLluvia>(700); //capacidad
        readonly Random azar = new Random(); // para posiciones y pequeñas variaciones
        float tasaGotas = 50f; // gotas por segundo
        float acumulador = 0f; // integra la tasa continua para spawnear en entero

        public void Iniciar()
        {
            var M = MotorConsola.Instancia;

            M.Ejecutar(
                actualizar: (dt) => 
                {
                    if (M.Arriba) tasaGotas = Math.Min(tasaGotas + 10f, 600f);
                    if (M.Abajo) tasaGotas = Math.Max(tasaGotas - 10f, 0f);

                    acumulador += tasaGotas * dt;
                    while (acumulador >= 1f)
                    {
                        acumulador -= 1f;
                        var g = pool.Tomar();
                        if (g != null)
                        {
                            g.x = azar.Next(0, M.Ancho);
                            g.fy = 1; 
                            g.y = 1;
                            g.velocidad = 16f + azar.Next(0, 6);
                            g.Activar();
                        }
                    }

                    // Actualizar todas las gotas activas
                    foreach (var g in pool.Elementos())
                        if (g.Activo)
                        {
                            g.Paso(M.Alto, dt);
                            if (!g.Activo)
                            {
                                pool.Devolver(g); // devuelve al pool si llegó al suelo
                            }
                        }
                },

                // Dibuja la pantalla
                dibujar: () => 
                {
                    M.EscribirEn(0, 0, $"Lluvia por Consola | Intensidad: {(int)tasaGotas}/s   (Arriba/Abajo)   ESC: salir");
                    foreach (var g in pool.Elementos())
                    { 
                        if (g.Activo) M.Poner(g.x, g.y, '|'); 
                    }
                    for (int x = 0; x < M.Ancho; x++)
                    {
                        M.Poner(x, M.Alto - 1, '_');
                    }
                },

                // Solo funcionará si tecleamos ESC
                pedirSalir: () => 
                {
                    if (M.Esc) 
                    { 
                        M.ConfirmarEsc(); 
                        return true; 
                    }
                    return false;
                }
            );
        }
    }
}
