using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;


namespace Evaluacion_3.Modelos.Modelos
{
    public class Estudiante
    {
        public string? NombreCompleto { get; set; }
        public string? Email { get; set; }
        public int? Edad { get; set; }
        public string? Curso { get; set; }


    }
}
