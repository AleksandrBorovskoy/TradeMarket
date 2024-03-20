using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly DateTime minDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime maxDate = DateTime.Now;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CustomerModel model)
        {
            if (model == null)
            {
                throw new MarketException($"Parameter cannot be null: {nameof(model)}");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(model.Name)}");
            }

            if (string.IsNullOrEmpty(model.Surname))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(model.Surname)}");
            }

            if (model.BirthDate < minDate || model.BirthDate > maxDate)
            {
                throw new MarketException($"Birth date must be in range from {minDate} to {maxDate}");
            }

            if (model.DiscountValue < 0)
            {
                throw new MarketException($"Discount value cannot be less than zero: {nameof(model.DiscountValue)}");
            }

            var customerEntity = _mapper.Map<Customer>(model);
            await _uow.CustomerRepository.AddAsync(customerEntity);
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _uow.CustomerRepository.DeleteByIdAsync(modelId);
            await _uow.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customerEntities = await _uow.CustomerRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<CustomerModel>>(customerEntities);
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customerEntity = await _uow.CustomerRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<CustomerModel>(customerEntity);   
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var customerEntities = (await _uow.CustomerRepository.GetAllWithDetailsAsync())
                .Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)));

            return _mapper.Map<IEnumerable<CustomerModel>>(customerEntities);
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new MarketException($"Property cannot be null or empty: {model.Name}");
            }

            if (string.IsNullOrEmpty(model.Surname))
            {
                throw new MarketException($"Property cannot be null or empty: {model.Surname}");
            }

            if (model.BirthDate < minDate || model.BirthDate > maxDate)
            {
                throw new MarketException($"Birth date must be in range from {minDate} to {maxDate}");
            }

            if (model.DiscountValue < 0)
            {
                throw new MarketException($"Discount value cannot be less than zero: {model.DiscountValue}");
            }

            var customerEntity = _mapper.Map<Customer>(model);
            _uow.CustomerRepository.Update(customerEntity);
            await _uow.SaveAsync();
        }
    }
}
