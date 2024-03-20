using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get([FromQuery] FilterSearchModel model)
        {
            IEnumerable<ProductModel> products;

            if (model != null)
            {
                products = await _productService.GetByFilterAsync(model);
                return Ok(products);
            }

            products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product is null)
            {
                return NotFound(new { Message = $"Product with id: {id} was not found" });
            }

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ProductModel model)
        {
            try
            {
                await _productService.AddAsync(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
           
            return CreatedAtAction(nameof(Get), model);
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductModel model)
        {
            try
            {
                model.Id = id;
                await _productService.UpdateAsync(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/products/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetCategories()
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            return Ok(categories);
        }

        // POST: api/products/categories
        [HttpPost("categories")]
        public async Task<ActionResult> AddCategory([FromBody] ProductCategoryModel model)
        {
            await _productService.AddCategoryAsync(model);
            return CreatedAtAction(nameof(GetCategories), model);
        }

        // PUT: api/products/categories/1
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] ProductCategoryModel model)
        {
            model.Id = id;
            await _productService.UpdateCategoryAsync(model);
            return NoContent();
        }

        // DELETE: api/products/categories/1
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await _productService.RemoveCategoryAsync(id);
            return NoContent();
        }
    }
}
