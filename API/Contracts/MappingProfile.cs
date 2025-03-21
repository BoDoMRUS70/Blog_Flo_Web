using AutoMapper;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.ViewModels.Comments;
using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Blog_Flo_Web.Services_model.ViewModels.Tags;
using Blog_Flo_Web.Services_model.ViewModels.Users;

namespace API.Contracts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterViewModel, User>()
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
                .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.UserName));

            CreateMap<CommentCreateViewModel, Comment>();
            CreateMap<CommentEditViewModel, Comment>();
            CreateMap<PostCreateViewModel, Post>();
            CreateMap<PostEditViewModel, Post>();
            CreateMap<TagCreateViewModel, Tag>();
            CreateMap<TagEditViewModel, Tag>();
            CreateMap<UserEditViewModel, User>();
        }
    }
}
