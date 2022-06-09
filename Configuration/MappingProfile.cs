using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Task3.Store.Models;
using Task3.DtoModels;
using Task3.ViewModels;

namespace Task3.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Section, SectionViewModel>();
            CreateMap<Section, SectionEditViewModel>();
            CreateMap<Section, SectionDeleteViewModel>();
            CreateMap<SectionCreateViewModel, Section>();

            CreateMap<Section, SectionDto>();
            CreateMap<SectionAddDto, Section>();

            CreateMap<Section, ModeratedSections>()
                .ForMember(x => x.Section, opt => opt.MapFrom(src => src));

            CreateMap<ModeratedSections, IdentityUser>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.User.UserName));
            
            CreateMap<Topic, TopicViewModel>();
            CreateMap<Topic, TopicEditViewModel>();
            CreateMap<Topic, TopicDeleteViewModel>();
            CreateMap<TopicCreateViewModel, Topic>();

            CreateMap<Section, TopicCreateViewModel>()
                .ForMember(x => x.Name, opt => opt.Ignore())
                .ForMember(x => x.Description, opt => opt.Ignore())
                .ForMember(x => x.Section, opt => opt.MapFrom(src => src))
                .ForMember(x => x.SectionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<IdentityUser, AccountViewModel>()
                .ForMember(x => x.User, opt => opt.MapFrom(src => src));
            CreateMap<Message, MessageViewModel>();
            CreateMap<Message, MessageEditViewModel>();
            CreateMap<Message, MessageDeleteViewModel>();
            CreateMap<MessageCreateViewModel, Message>()
                .ForMember(x => x.Attachments, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<Topic, MessageCreateViewModel>()
                .ForMember(x => x.Topic, opt => opt.MapFrom(src => src))
                .ForMember(x => x.TopicId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Attachment, AttachmentViewModel>();
        }
    }
}