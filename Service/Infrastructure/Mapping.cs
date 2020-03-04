using AutoMapper;
using Core.Domain.Basket;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure
{
    public static class Mapping
    {

        public static void Configure()
        {
            Mapper.Initialize(GetMappingConfigure());
        }

        /// <summary>
        /// ortak mapping tanımlamaları burada yapılabilir. 
        /// </summary>
        /// <returns>Action<IMapperConfigurationExpression/></returns>
        public static Action<IMapperConfigurationExpression> GetMappingConfigure()
        {
            return Configure;
        }

        public static void Configure(IMapperConfigurationExpression cfg)
        {

            cfg.CreateMap<BasketDto,Basket>().ReverseMap();
            cfg.CreateMap<Basket, BasketDto>().ReverseMap();


        }

    }
}
