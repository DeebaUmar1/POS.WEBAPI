using AutoMapper;
using POS.Models.DTO;
using POS.Models.Entities;

namespace PointOfSaleWebAPIs.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() {

            //CreateMap<Employee,EmployeeDTO>();

            //CreateMap<Employee, EmployeeDTO>()
            //.ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            CreateMap <LoginDTO, User>()
             .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name ))
             .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.password));


            CreateMap<RegisterDTO, User>()
             .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
             .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.password))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.email))
            .ForMember(dest => dest.role, opt => opt.MapFrom(src => src.role));

            CreateMap<User, LoginDTO>()
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.password))
            .ForMember(dest => dest.role, opt => opt.MapFrom(src => src.role));
            CreateMap<UserRoleDTO, UserRole>();

            CreateMap<User, UserRoleDTO>()
            .ForMember(dest => dest.role, opt=>opt.MapFrom(src => src.role));
            // Add other mappings as needed
            CreateMap<User, RegisterDTO>(); // If

            CreateMap<ProductDTO, Product>()
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Price))
             .ForMember(dest => dest.category, opt => opt.MapFrom(src => src.Category))
             .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type));

     /*       CreateMap<PurchaseProductsDTO, PurchaseProducts>()
           .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.price))
            .ForMember(dest => dest.category, opt => opt.MapFrom(src => src.category))
            .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.quantity))
           .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.type));

            CreateMap<PurchaseProducts, PurchaseProductsDTO>()
          .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
           .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.price))
           .ForMember(dest => dest.category, opt => opt.MapFrom(src => src.category))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.quantity))
          .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.type));*/

            CreateMap<SaleProducts, SaleProductsDTO>();
            CreateMap<FinalReceipt, FinalReceiptDTO>()
             .ForMember(dest => dest.Receipt, opt => opt.MapFrom(src => src.Receipt))
             .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));

            // Map from Receipt to ReceiptDTO
            CreateMap<Receipt, ReceiptDTO>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));

            CreateMap<UpdateProductDTO, Product>()
          .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Price))
           .ForMember(dest => dest.category, opt => opt.MapFrom(src => src.Category))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Quantity))
          .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type));

        }
    }
}
