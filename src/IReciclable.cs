using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenU2
{
    // Object Pool 
    interface IReciclable 
    { 
        bool Activo { get; } // indica si el objeto está en uso
        void Activar(); 
        void Desactivar(); 
        void Reiniciar(); // deja el objeto en estado inicial
    }
    sealed class PiscinaObjetos<T> where T : class, IReciclable, new()
    {
        readonly List<T> todos; 
        readonly Stack<T> libres;
        public PiscinaObjetos(int capacidad)
        {
            todos = new List<T>(capacidad); //objetos del pool
            libres = new Stack<T>(capacidad); //disponibles para "prestar"
            for (int i = 0; i < capacidad; i++) 
            { 
                var o = new T(); 
                o.Desactivar(); 
                o.Reiniciar(); 
                todos.Add(o); 
                libres.Push(o); 
            }
        }
        public T Tomar() //saca 1 objeto de libres
        { 
            return libres.Count > 0 ? libres.Pop() : null; 
        }
        public void Devolver(T o) //desactiva, reinicia y lo regresa a libres
        {
            if (o == null)
            {
                return;
            }
            o.Desactivar(); 
            o.Reiniciar(); 
            libres.Push(o); 
        }
        // expone la lista completa (todos) para poder dibujar los activos.
        public IEnumerable<T> Elementos() => todos;
    }
}
