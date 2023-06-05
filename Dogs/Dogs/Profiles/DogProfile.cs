using AutoMapper;
using Core.Db;
using Core.Dtos;

namespace Dogs.Profiles
{
    public class DogProfile : Profile
    {
        public DogProfile()
        {
            CreateMap<DogInfo, Dog>().ReverseMap();
        }
    }
}
