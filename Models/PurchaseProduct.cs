namespace server.Models
{
    [PrimaryKey(nameof(PurchaseId), nameof(ProductId))]
    public class PurchaseProduct
    {
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // public Product Product { get; set; }
        // public Purchase Purchase { get; set; }
    }
}

