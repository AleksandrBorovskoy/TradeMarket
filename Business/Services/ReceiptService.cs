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
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            var entity = _mapper.Map<Receipt>(model);
            await _uow.ReceiptRepository.AddAsync(entity);
            await _uow.SaveAsync();
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            ReceiptDetail detail;

            if (receipt == null)
            {
                throw new MarketException($"Receipt with id {receiptId} was not found");
            }

            if (receipt.ReceiptDetails != null)
            {
                detail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

                if (detail != null)
                {
                    detail.Quantity += quantity;

                    await _uow.SaveAsync();
                    return;
                }
            }

            var product = await _uow.ProductRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new MarketException($"Product with id: {productId} was not found");
            }

            var receiptDetail = new ReceiptDetail
            {
                ReceiptId = receiptId,
                ProductId = productId,
                DiscountUnitPrice = product.Price - (receipt.Customer.DiscountValue * product.Price / 100),
                UnitPrice = product.Price,
                Quantity = quantity
            };

            await _uow.ReceiptDetailRepository.AddAsync(receiptDetail);

            await _uow.SaveAsync();
        }

        public async Task CheckOutAsync(int receiptId)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdAsync(receiptId);
            receipt.IsCheckedOut = true;
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

            foreach (var receiptDetail in receipt.ReceiptDetails)
            {
                _uow.ReceiptDetailRepository.Delete(receiptDetail);
            }

            await _uow.ReceiptRepository.DeleteByIdAsync(modelId);
            await _uow.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            var entities = await _uow.ReceiptRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReceiptModel>>(entities);
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            var entity = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ReceiptModel>(entity);
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            return _mapper.Map<IEnumerable<ReceiptDetailModel>>(receipt.ReceiptDetails);
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var entities = (await _uow.ReceiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);

            return _mapper.Map<IEnumerable<ReceiptModel>>(entities);
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            ReceiptDetail detail;

            if (receipt == null)
            {
                throw new MarketException($"Receipt with id {receiptId} was not found");
            }

            if (receipt.ReceiptDetails != null)
            {
                detail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

                if (detail != null)
                {
                    detail.Quantity -= quantity;

                    if (detail.Quantity <= 0)
                    {
                        _uow.ReceiptDetailRepository.Delete(detail);
                    }

                    await _uow.SaveAsync();
                }
            }



        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            var receipt = await _uow.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            decimal sum = 0;

            foreach (var receiptDetail in receipt.ReceiptDetails)
            {
                sum += receiptDetail.DiscountUnitPrice * receiptDetail.Quantity;
            }

            return sum;
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            _uow.ReceiptRepository.Update(_mapper.Map<Receipt>(model));
            await _uow.SaveAsync();
        }
    }
}
