using Core.Domain.Basket;
using Core.Exception;
using Core.Model;
using Data.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Query.Basket
{
    public class GetBasketByIdQuery : IRequest<ResponseModel<BasketDto>>
    {
        public int BasketId { get; set; }
    }
    public class GetBasketByIdQueryHandler : IRequestHandler<GetBasketByIdQuery, ResponseModel<BasketDto>>
    {
        private readonly Entities _db;
        public GetBasketByIdQueryHandler(Entities db)
        {
            _db = db;
        }
        public async Task<ResponseModel<BasketDto>> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
        {
            var basket = await _db.Basket.Include(q => q.BasketProduct).AsNoTracking().FirstOrDefaultAsync(q => q.Id == request.BasketId, cancellationToken);

            if (basket == null)
            {
                throw new NotFoundException("Sepet bilgisi bulunamadı.");
            }

            var basketDto = AutoMapper.Mapper.Map<BasketDto>(basket);
            foreach (var product in basket.BasketProduct)
            {
                var basketProduct = new BasketProductDto
                {
                    ProductId = product.ProductId,
                    Count = product.Count
                };
                basketDto.ProductList.Add(basketProduct);
            }

            return new ResponseModel<BasketDto>
            {
                Result = basketDto
            };
        }
    }

    public class GetBasketByIdQueryValidator : AbstractValidator<GetBasketByIdQuery>
    {

    }
}
