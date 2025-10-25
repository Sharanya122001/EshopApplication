﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MinimalEshop.Application.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface IProduct
    {
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> SearchAsync(string keyword);
        //Task<List<Product>> GetByCategoryAsync(string categoryId);
        Task<Product> AddAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string productId);
    }
}
