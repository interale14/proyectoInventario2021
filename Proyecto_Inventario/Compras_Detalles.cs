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
    
    public partial class Compras_Detalles
    {
        public long PKCompraDetalleID { get; set; }
        public long FKCompraID { get; set; }
        public long FKProductoID { get; set; }
        public decimal PrecioUnidad { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalProducto { get; set; }
        public string Estatus { get; set; }
    
        public virtual Compras Compras { get; set; }
        public virtual Productos Productos { get; set; }
    }
}