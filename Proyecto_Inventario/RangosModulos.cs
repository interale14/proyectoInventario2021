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
    
    public partial class RangosModulos
    {
        public int PKRangosModulos { get; set; }
        public short FKRangosID { get; set; }
        public short FKModulosID { get; set; }
    
        public virtual Modulos Modulos { get; set; }
        public virtual Rangos Rangos { get; set; }
    }
}
