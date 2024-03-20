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
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        // GET: api/statistics/popularProducts?productCount=2
        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopularProducts([FromQuery] int productCount)
        {
            var products = await _statisticService.GetMostPopularProductsAsync(productCount);
            return Ok(products);
        }

        // GET: api/statistics/customer/1/1
        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetCustomerMostPopularProducts(int id, int productCount)
        {
            var products = await _statisticService.GetCustomersMostPopularProductsAsync(productCount, id);
            return Ok(products);
        }

        // GET: api/statistics/activity/1?startDate=2020-7-21&endDate=2020-7-22
        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostActiveCustomersInPeriod(int customerCount, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var activityModels = await _statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate);
            return Ok(activityModels);
        }

        // GET: api/statistics/income/1/?startDate=2020-7-21&endDate=2020-7-22
        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeOfCategoryInPeriod(int categoryId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var income = await _statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate);
            return Ok(income);
        } 
    }
}
