using AutoMapper;

namespace ReceptionBook.Tests.Helpers;

public static class TestMapper
{
    public static IMapper Create() =>
        new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
}
