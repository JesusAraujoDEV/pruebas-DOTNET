namespace InventarioConsolaApp
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int Cantidad { get; set; }

        // Constructor para inicializar un nuevo Producto
        public Producto(int id, string nombre, double precio, int cantidad)
        {
            Id = id;
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre}, Precio: ${Precio:F2}, Cantidad: {Cantidad}";
        }
    }
}