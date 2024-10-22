using AutoMapper;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;

namespace BusinessCardAPI.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<BusinessCardDTO, BusinessCard>();
            CreateMap<BusinessCard, businessCard>();
        }
    }
}
