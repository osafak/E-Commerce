using Core.Domain.Basket;
using Core.Enum;
using Core.Model;
using Data.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.Query.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Query.Basket
{
    public class BasketSaveQuery : IRequest<ResponseModel<BasketDto>>
    {
        [JsonIgnore]
        public Guid UniqueId { get; set; }
        [JsonIgnore]
        public int? CustomerId { get; set; }
        public int? AddressId { get; set; }
        public int? OrderId { get; set; }
        public List<BasketSaveProductListModel> ProductList { get; set; }

    }

    public class BasketSaveQueryHandler : IRequestHandler<BasketSaveQuery, ResponseModel<BasketDto>>
    {
        private readonly Entities _db;
        private readonly IMediator _mediator;
        public BasketSaveQueryHandler(Entities db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }
        public async Task<ResponseModel<BasketDto>> Handle(BasketSaveQuery request, CancellationToken cancellationToken)
        {
            var isExist = await _db.Basket.Include(q => q.BasketProduct).FirstOrDefaultAsync(q => q.UniqueId == request.UniqueId, cancellationToken);
            if (isExist == null)
            {
                return await Insert(request, cancellationToken);
            }

            return await Update(isExist, request, cancellationToken);
        }

        private async Task<ResponseModel<BasketDto>> Insert(BasketSaveQuery request, CancellationToken cancellationToken)
        {
            var basketEntity = new Data.Entity.Basket
            {
                AddressId = request.AddressId,
                CreateDate = DateTime.Now,
                CustomerId = request.CustomerId,
                OrderId = request.OrderId,
                StatusId = (int)BasketStatus.Active,
                UniqueId = Guid.NewGuid(),
            };

            foreach (var product in request.ProductList)
            {
                var basketProductEntity = new Data.Entity.BasketProduct
                {
                    Count = product.Count,
                    ProductId = product.ProductId,
                    BasketId = basketEntity.Id
                };
                basketEntity.BasketProduct.Add(basketProductEntity);
            }
            await _db.Basket.AddAsync(basketEntity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return await _mediator.Send(new GetBasketByIdQuery
            {
                BasketId = basketEntity.Id
            });
        }

        private async Task<ResponseModel<BasketDto>> Update(Data.Entity.Basket basket, BasketSaveQuery request, CancellationToken cancellationToken)
        {
            basket.AddressId = request.AddressId;
            basket.CustomerId = request.CustomerId;
            basket.OrderId = request.OrderId;
            basket.StatusId = (int)BasketStatus.Edited;
            basket.UpdateDate = DateTime.Now;

            var basketProducts = _db.BasketProduct.Where(q => q.BasketId == basket.Id);

            _db.BasketProduct.RemoveRange(basketProducts);

            foreach (var product in request.ProductList)
            {
                var basketProduct = new BasketProduct
                {
                    ProductId = product.ProductId,
                    Count = product.Count
                };

                basket.BasketProduct.Add(basketProduct);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return await _mediator.Send(new GetBasketByIdQuery
            {
                BasketId = basket.Id
            });
        }
    }

    public class BasketSaveValidation : AbstractValidator<BasketSaveQuery>
    {
        private readonly IMediator _mediator;
        public BasketSaveValidation(IMediator mediator)
        {
            _mediator = mediator;
            RuleFor(q => q.ProductList).NotNull().WithMessage("Ürün listesi boş olamaz.");
            RuleFor(q => q.ProductList).MustAsync(CheckProductInDb).When(q => q.ProductList != null).WithMessage("Yeterli sayıda ürün yok.");
        }

        private async Task<bool> CheckProductInDb(List<BasketSaveProductListModel> ProductList, CancellationToken arg2)
        {
            foreach (var product in ProductList)
            {
                var isPurchasable = await _mediator.Send(new GetProductPurchasableByProductIdQuery { Count = product.Count, ProductId = product.ProductId });
                if (!isPurchasable.Result)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class BasketSaveProductListModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }

    }
}
