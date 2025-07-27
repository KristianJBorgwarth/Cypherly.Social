using AutoMapper;
using Cypherly.UserManagement.Domain.Aggregates;
using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

namespace Social.Application.Profiles;

public class UserProfileMappingProfiles : Profile
{
    public UserProfileMappingProfiles()
    {
        CreateMap<UserProfile, GetUserProfileDto>()
            .ForMember(dest => dest.UserTag, opt => opt.MapFrom(src => src.UserTag.Tag))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore()).ReverseMap();
        CreateMap<UserProfile, GetUserProfileByTagDto>()
            .ForMember(dest => dest.UserTag, opt => opt.MapFrom(src => src.UserTag.Tag))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore()).ReverseMap();
        CreateMap<UserProfile, UpdateUserProfileDisplayNameDto>().ReverseMap();
    }
}