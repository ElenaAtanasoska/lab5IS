﻿using EShop.Domain.Domain;
using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.ProductInOrders)
                .Include(z => z.Owner)
                .ToList();
        }

        public Order GetDetailsForOrder(BaseEntity id)
        {
           
                return entities
                .Include(z => z.ProductInOrders)
                    .ThenInclude(m => m.OrderedProduct)
                    .ThenInclude(m => m.Concert)
                .Include(z => z.Owner)
                .SingleOrDefaultAsync(z => z.Id == id.Id).Result;
        }
    }
}
