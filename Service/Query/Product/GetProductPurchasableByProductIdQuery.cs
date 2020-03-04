using Core.Enum;
using Core.Model;
using Data.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Query.Product
{
    public class GetProductPurchasableByProductIdQuery : IRequest<ResponseModel<bool>>
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }

    public class GetProductPurchasableByProductIdQueryHandler : IRequestHandler<GetProductPurchasableByProductIdQuery, ResponseModel<bool>>
    {
        private readonly Entities _db;
        public GetProductPurchasableByProductIdQueryHandler(Entities db)
        {
            _db = db;
        }
        public async Task<ResponseModel<bool>> Handle(GetProductPurchasableByProductIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _db.Product.AsNoTracking().FirstOrDefaultAsync(q => q.Id == request.ProductId, cancellationToken);
            if (product.Count >= request.Count)
            {
                return new ResponseModel<bool> { Result = true };
            }

            return new ResponseModel<bool> { Result = false };
        }
    }
}
