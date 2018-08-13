using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Data.Entities;
using App.ViewModels;
using AutoMapper;

namespace App.Data
{
    public class YerbaMappingProfile : Profile
    {
        public YerbaMappingProfile()
        {
            CreateMap<UserViewModel, StoreUserExtended>()
                .ForMember(o => o.PasswordHash, ex => ex.MapFrom(o => o.Password));

            CreateMap<CreateOrderViewModel, Order>()                
                .ForMember(o => o.UserExecutedBy, ex => ex.Ignore())
                .ForMember(o => o.UserMadeBy, ex => ex.Ignore())
                .ForMember(o => o.Items, ex => ex.MapFrom(o => o.Items));

            CreateMap<CreateOrderItemViewModel, OrderItem>()
                .ForMember(o => o.UserDetails, ex => ex.Ignore());

            CreateMap<User, UserViewModel>()
                .ForMember(o => o.Roles, ex => ex.MapFrom(o => o.UserRoles));

            CreateMap<UserViewModel, User>()
                .ForMember(o => o.UserRoles, ex => ex.MapFrom(o => o.Roles));

            CreateMap<OrderItemViewModel, OrderItem>()
                .ForMember(o => o.UserDetails, ex => ex.Ignore());

        }

    }
}
