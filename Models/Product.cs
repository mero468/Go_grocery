namespace Go_grocery.Models
{
    public class Product
    { 
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public double Weight { get; set; }
            public int CategoryId { get; set; }
            public int ShopOwnerID { get; set; }
    }
}
