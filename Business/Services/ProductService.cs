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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ProductModel model)
        {
            if (model.Price < 0)
            {
                throw new MarketException($"Property cannot be less than 0: {nameof(model.Price)}");
            }

            if (string.IsNullOrEmpty(model.ProductName))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(model.ProductName)}");
            }

            var entity = _mapper.Map<Product>(model);
            await _uow.ProductRepository.AddAsync(entity);
            await _uow.SaveAsync();
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (string.IsNullOrEmpty(categoryModel.CategoryName))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(categoryModel.CategoryName)}");
            }

            var entity = _mapper.Map<ProductCategory>(categoryModel);
            await _uow.ProductCategoryRepository.AddAsync(entity);
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _uow.ProductRepository.DeleteByIdAsync(modelId);
            await _uow.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var entities = await _uow.ProductRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ProductModel>>(entities);
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var entities = await _uow.ProductCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductCategoryModel>>(entities);
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var entities = (await _uow.ProductRepository.GetAllWithDetailsAsync())
                .Where(p => p.ProductCategoryId == (filterSearch.CategoryId ?? p.ProductCategoryId) && p.Price >= (filterSearch.MinPrice ?? p.Price) && p.Price <= (filterSearch.MaxPrice ?? p.Price));

            return _mapper.Map<IEnumerable<ProductModel>>(entities);
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            var entity = await _uow.ProductRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ProductModel>(entity);
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await _uow.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await _uow.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            if (string.IsNullOrEmpty(model.ProductName))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(model.ProductName)}");
            }

            var entity = _mapper.Map<Product>(model);
            _uow.ProductRepository.Update(entity);
            await _uow.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (string.IsNullOrEmpty(categoryModel.CategoryName))
            {
                throw new MarketException($"Property cannot be null or empty: {nameof(categoryModel.CategoryName)}");
            }

            var entity = _mapper.Map<ProductCategory>(categoryModel);
            _uow.ProductCategoryRepository.Update(entity);
            await _uow.SaveAsync();
        }
    }
}
