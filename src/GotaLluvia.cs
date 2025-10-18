using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenU2
{
    // Particula de lluvia
    sealed class GotaLluvia : IReciclable
    {
        // columnas/filas, posición vertical en float, velocidad de caída
        public int x, y; public float fy; public float velocidad; bool activo;
        public bool Activo => activo;

        // implementan de IReciclable
        public void Activar() { activo = true; }
        public void Desactivar() { activo = false; }
        public void Reiniciar() 
        { 
            x = y = 0; 
            fy = 0f; 
            velocidad = 24f; 
        }

        // Actualiza la caída
        public void Paso(int alto, float dt) 
        {
            if (!activo)
            {
                return;
            }
            fy += velocidad * dt; 
            y = (int)fy;
            // Si "y" llega al final, llama a Desactivar()
            if (y >= alto - 1)
            {
                Desactivar();
            }
        }
    }
}
