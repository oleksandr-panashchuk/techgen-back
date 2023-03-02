using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Domain.Entities.RoadmapEntity;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Post;
using Techgen.Models.ResponseModels.Session;

namespace Techgen.Services
{
    public class AutoMapperProfileConfiguration : AutoMapper.Profile
    {
        public AutoMapperProfileConfiguration()
        : this("MyProfile")
        {
        }

        protected AutoMapperProfileConfiguration(string profileName)
        : base(profileName)
        {
            #region User model

            CreateMap<Profile, UserResponseModel>()
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User != null ? x.User.Email : ""))
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.User != null ? x.User.PhoneNumber : ""))
                .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => x.User != null ? !x.User.IsActive : false));

            CreateMap<ApplicationUser, UserBaseResponseModel>()
               .IncludeAllDerived();

            CreateMap<ApplicationUser, UserResponseModel>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(x => x.Profile.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(x => x.Profile.LastName))
                .ForMember(x => x.IsBlocked, opt => opt.MapFrom(x => !x.IsActive))
                .IncludeAllDerived();



            CreateMap<ApplicationUser, UserRoleResponseModel>();

            #endregion

            #region Comment model

            CreateMap<Comment, CommentResponseModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Text, opt => opt.MapFrom(x => x.Text))
                .ForMember(x => x.AnswersToComment, opt => opt.MapFrom(x => x.Answers))
                .ForMember(x => x.ParentCommentId, opt => opt.MapFrom(x => x.ParentCommentId))
                .ForMember(x => x.AuthorId, opt => opt.MapFrom(x => x.UserId));

            #endregion

            #region Roadmap model

            CreateMap<Roadmap, RoadmapResponseModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Markdown, opt => opt.MapFrom(x => x.Markdown))
                .ForMember(x => x.Path, opt => opt.MapFrom(x => x.Path))
                .ForMember(x => x.ImageName, opt => opt.MapFrom(x => x.ImageName));
            #endregion

            #region Post model

            CreateMap<Post, PostResponseModel>()
                .ForMember(x => x.LikesCount, opt => opt.MapFrom(x => x.Likes.Count))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Text, opt => opt.MapFrom(x => x.Text))
                .ForMember(x => x.Comments, opt => opt.MapFrom(x => x.Comments));

            CreateMap<Post, SmallPostResponseModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.LikesCount, opt => opt.MapFrom(x => x.Likes.Count));

            #endregion

        }
    }
}
