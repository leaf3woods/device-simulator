using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Wpf.ViewModels.SubVMs;

namespace DeviceSimulator.Wpf.Utillities.AutoMapperProfiles
{
    public class VMProfile : Profile
    {
        public VMProfile()
        {
            CreateMap<DeviceTypeVM, DeviceType>();
            CreateMap<DeviceGridVM, Device>();
            CreateMap<DeviceType, DeviceType>()
                .ForMember(dest => dest.Id, opts => opts.Ignore());
        }
    }
}
