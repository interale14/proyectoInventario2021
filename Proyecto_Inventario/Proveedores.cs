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
    
    public partial class Proveedores
    {
        public Proveedores()
        {
            this.Compras = new HashSet<Compras>();
        }
    
        public int PKProveedorID { get; set; }
        public string NombreProveedor { get; set; }
        public string NombreContacto { get; set; }
        public int TelefonoContacto1 { get; set; }
        public Nullable<int> TelefonoContacto2 { get; set; }
        public string EmailContacto1 { get; set; }
        public string EmailContacto2 { get; set; }
        public string Direccion { get; set; }
        public bool Estado { get; set; }
    
        public virtual ICollection<Compras> Compras { get; set; }
    }
}