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
    
    public partial class Ventas_Devoluciones_Detalles
    {
        public long PKDevolucionDetalleID { get; set; }
        public long FKDevolucionID { get; set; }
        public long FKProductosID { get; set; }
    
        public virtual Productos Productos { get; set; }
        public virtual Ventas_Devoluciones Ventas_Devoluciones { get; set; }
    }
}