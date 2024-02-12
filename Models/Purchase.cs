using System.Text.Json;
using System.Text.Json.Serialization;

namespace server.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }

        // public Customer Customer { get; set; }
        // [JsonIgnore]
        public ICollection<PurchaseProduct> PurchaseProducts { get; set; }
    }
}
