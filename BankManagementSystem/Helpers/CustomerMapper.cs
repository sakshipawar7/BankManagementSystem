using AutoMapper;
using BankManagementSystem.Models;
using BankManagementSystem.DAL;
namespace BankManagementSystem.Helpers
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<CustomerAccountHybridModel, Customer>();
            CreateMap<UpdateCustomerModel, Customer>().ReverseMap();
            CreateMap<Customer,DisplayCustomerModel>().ReverseMap();
            CreateMap<Account, DisplayAccountsModel>()
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.ToString())); // Enum to String
            CreateMap<DisplayAccountsModel, Account>()
                 .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => Enum.Parse<AccountType>(src.AccountType))); // String to Enum
            CreateMap<Transaction, TransactionModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString())); //Enum to String
           
        }
    }
}
