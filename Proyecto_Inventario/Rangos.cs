//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Inventario
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rangos
    {
        public Rangos()
        {
            this.Empleados = new HashSet<Empleados>();
            this.RangosModulos = new HashSet<RangosModulos>();
        }
    
        public short PKRangoID { get; set; }
        public string DescripcionRango { get; set; }
        public bool EstadoRango { get; set; }
    
        public virtual ICollection<Empleados> Empleados { get; set; }
        public virtual ICollection<RangosModulos> RangosModulos { get; set; }
    }
}
