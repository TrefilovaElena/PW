using AutoMapper;
using PW.DataModel.Entities;
using PW.ViewModels;


namespace PW.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
        }
    }
}
