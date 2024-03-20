using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            var receipts = (await _uow.ReceiptRepository.GetAllWithDetailsAsync()).Where(r => r.CustomerId == customerId);

            var products = receipts.SelectMany(r => r.ReceiptDetails).OrderByDescending(rd => rd.Quantity)
                .Select(rd => rd.Product)
                .Take(productCount);

            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            return (await _uow.ReceiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .Sum(rd => rd.Quantity * rd.DiscountUnitPrice);
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            var products = (await _uow.ReceiptDetailRepository.GetAllWithDetailsAsync())
                .OrderByDescending(rd => rd.Quantity)
                .Select(rd => rd.Product)
                .Take(productCount);

            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            return (await _uow.ReceiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .Select(r => new 
                { r.CustomerId, CustomerName = r.Customer.Person.Name + ' ' + r.Customer.Person.Surname, ReceiptSum = r.ReceiptDetails.Sum(rd => rd.Quantity * rd.DiscountUnitPrice) })
                .GroupBy(r =>  new { r.CustomerId, r.CustomerName },
                    r => r.ReceiptSum,
                    (key, sums) => new CustomerActivityModel
                    {
                        CustomerId = key.CustomerId,
                        CustomerName = key.CustomerName,
                        ReceiptSum = sums.Sum()
                    })
                .OrderByDescending(cam => cam.ReceiptSum)
                .Take(customerCount);
        }
    }
}
