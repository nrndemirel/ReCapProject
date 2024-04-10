namespace Ericsson.ReCapProject.Core.Entitites
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public void Update(Product entity)
        {
            Name = entity.Name;
            Price = entity.Price;
            StockQuantity = entity.StockQuantity;
        }

        public void AdjustStockQuantity(int quantityChange)
        {
            StockQuantity += quantityChange;
        }
    }
}