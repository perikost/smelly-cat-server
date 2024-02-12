using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/purchases")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly DataContext _context;

        public PurchaseController(DataContext context)
        {
            _context = context;
        }

        // GET: api/purchases
        [HttpGet]
        public async Task<ActionResult<List<Purchase>>> GetPurchases()
        {
            // include the PurchaseProducts junction table
            return await _context.Purchases
                .Include(p => p.PurchaseProducts)
                .ToListAsync();
        }

        // GET: api/purchases/{{id}}
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        // POST: api/purchases
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                //Create the Purchase record
                var purchaseToSave = new Purchase
                {
                    CustomerId = purchase.CustomerId,
                    PurchaseDate = purchase.PurchaseDate,
                    TotalAmount = 0
                };

                _context.Purchases.Add(purchaseToSave);
                await _context.SaveChangesAsync();

                // Populate PurchaseProducts junction table
                foreach (var pp in purchase.PurchaseProducts)
                {
                    var product = await _context.Products.FindAsync(pp.ProductId);

                    if (product == null)
                    {
                        // Product not found, rollback transaction
                        transaction.Rollback();
                        return NotFound($"Product with ID {pp.ProductId} not found.");
                    }

                    if (product.QuantityInStock < pp.Quantity)
                    {
                        // Out of stock, rollback transaction
                        transaction.Rollback();
                        return BadRequest($"Product {product.Name} is out of stock.");
                    }

                    // Update product quantity
                    product.QuantityInStock -= pp.Quantity;

                    // Calculate product total price
                    var productTotalAmount = product.Price * pp.Quantity;
                    purchaseToSave.TotalAmount += productTotalAmount;

                    var purchaseProduct = new PurchaseProduct
                    {
                        PurchaseId = purchaseToSave.Id,
                        ProductId = pp.ProductId,
                        Quantity = pp.Quantity
                    };

                    _context.PurchasesProducts.Add(purchaseProduct);
                }

                // Increase customer loyalty points
                var customer = await _context.Customers.FindAsync(purchase.CustomerId);
                if (customer != null)
                {
                    customer.LoyaltyPoints += 10;
                }

                // Save changes
                await _context.SaveChangesAsync();

                // Commit the transaction
                transaction.Commit();

                return CreatedAtAction("PostPurchase", new { id = purchaseToSave.Id }, purchaseToSave);
            }
            catch (Exception)
            {
                // Rollback transaction in case of any error
                transaction.Rollback();
                throw;
            }
        }

        // PUT: api/purchases/{{id}}
        [HttpPut("{id}")]
        public async Task<ActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!PurchaseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(purchase);
        }

        // DELETE: api/purchases/{{id}}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
