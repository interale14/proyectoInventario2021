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
    
    public partial class Modulos
    {
        public Modulos()
        {
            this.RangosModulos = new HashSet<RangosModulos>();
        }
    
        public short PKModulosID { get; set; }
        public string DescripcionModulo { get; set; }
        public bool EstadoModulo { get; set; }
    
        public virtual ICollection<RangosModulos> RangosModulos { get; set; }
    }
}
