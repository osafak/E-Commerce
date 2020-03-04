

using AutoMapper;

namespace Ecommerce.Api.Mapping
{
    public static class Mapping
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {

                Service.Infrastructure.Mapping.Configure(cfg);

            });
        }
    }
}
