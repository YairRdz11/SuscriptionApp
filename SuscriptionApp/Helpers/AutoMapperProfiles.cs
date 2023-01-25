using AutoMapper;
using SuscriptionApp.DTOs;
using SuscriptionApp.Entities;

namespace SuscriptionApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<KeyAPI, KeyDTO>();
        }
    }
}
