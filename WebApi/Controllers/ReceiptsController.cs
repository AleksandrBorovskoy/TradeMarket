using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get()
        {
            var receipts = await _receiptService.GetAllAsync();
            return Ok(receipts);
        }

        // GET: api/receipts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);

            if (receipt is null)
            {
                return NotFound(new { Message = $"Receipt with id: {id} was not found" });
            }

            return Ok(receipt);
        }

        // GET: api/receipts/1/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetAllDetails(int id)
        {
            var details = await _receiptService.GetReceiptDetailsAsync(id);
            return Ok(details);
        }

        // GET: api/receipts/1/sum
        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetSum(int id)
        {
            var sum = await _receiptService.ToPayAsync(id);
            return Ok(sum);
        }

        // GET: api/receipts/period
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetByPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var receipts = await _receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
            return Ok(receipts);
        }

        // POST: api/receipts
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ReceiptModel model)
        {
            await _receiptService.AddAsync(model);
            return CreatedAtAction(nameof(Get), model);
        }

        // PUT: api/receipts/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReceiptModel model)
        {
            model.Id = id;
            await _receiptService.UpdateAsync(model);
            return NoContent();
        }

        // PUT: api/receipts/1/products/add/1/1
        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProduct(int id, int productId, int quantity)
        {
            await _receiptService.AddProductAsync(productId, id, quantity);
            return NoContent();
        }

        // PUT: api/receipts/1/products/remove/1/1
        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProduct(int id, int productId, int quantity)
        {
            await _receiptService.RemoveProductAsync(productId, id, quantity);
            return NoContent();
        }

        // PUT: api/receipts/1/checkout
        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> Checkout(int id)
        {
            await _receiptService.CheckOutAsync(id);
            return NoContent();
        }

        // DELETE: api/receipts/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _receiptService.DeleteAsync(id);
            return NoContent();
        }
    }
}
