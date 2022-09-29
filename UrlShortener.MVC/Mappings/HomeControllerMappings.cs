using AutoMapper;

namespace UrlShortener.MVC.Mappings;

public class HomeControllerMappings : Profile
{
    public HomeControllerMappings()
    {
        // Url
        CreateMap<ShortenedUrl, UrlModel>()
            .ForMember(m => m.Username, o => o.MapFrom(su => su.User != null ? su.User.UserName : null));

        // User
        CreateMap<User, UserModel>()
            .ForMember(m => m.Username, o => o.MapFrom(u => u.UserName));
    }
}