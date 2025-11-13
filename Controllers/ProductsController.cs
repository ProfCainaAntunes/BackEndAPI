using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEndAPI.Data;
using BackEndAPI.Models;
using BackEndAPI.DTOs;

namespace BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly BackEndAPIContext _context;

        public ProductsController(BackEndAPIContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productDTO)
        {
            // Valida se o novo preço e quantidade são válidos.
            if (productDTO.Price < 0)
                return BadRequest($"Price value: {productDTO.Price} invalid.");

            if (productDTO.StockQuantity < 0)
                return BadRequest($"Stock quantity value: {productDTO.Price} invalid.");
            
            // Valida o Id fornecido existe no banco.
            Product? product = await _context.Product.FindAsync(id);

            if (product == null)
                return BadRequest($"ProductId: {id} not found.");

            // Atualiza os campos do Produto.
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Category = productDTO.Category;
            product.Price = productDTO.Price;
            product.StockQuantity = productDTO.StockQuantity;

            // Atualiza o objeto no banco.
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductDTO productDTO)
        {
            if (productDTO.Price < 0)
                return BadRequest($"Price value: {productDTO.Price} invalid.");

            if (productDTO.StockQuantity < 0)
                return BadRequest($"Stock quantity value: {productDTO.Price} invalid.");
                
            Product product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Category = productDTO.Category,
                StockQuantity = productDTO.StockQuantity,
                Price = productDTO.Price
            };
            
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
