using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Diseño.Net
{
    public class Estudiante
    {
        public string nombre { get; set; }
        public string apellido1 { get; set; }
        public string correo { get; set; }
        public string password { get; set; }
    }

    public class Coursesjson
    {
        public bool success { get; set; }
        public string menssage { get; set; }
        
        public Courseslist data {  get; set; }
    }

    public class Courseslist
    {
        public List<CourseInfo> courses {  get; set; }
    }
    public class CourseInfo
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int codigo { get; set; }
        public int grupo { get; set; }
        public List<TareaInfo> tareas { get; set; }
    }

    public class TareaInfo
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string adjunto { get; set; }
        public string fechaEntrega { get; set; }
    }

    public class Err
    {
        public bool success { get; set; }
        public string message { get; set; }

        public object errors { get; set; }
    }
    public class Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public string token { get; set; }
    }
}
