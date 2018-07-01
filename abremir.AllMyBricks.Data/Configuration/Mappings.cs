using abremir.AllMyBricks.Data.Models;
using ExpressMapper;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Configuration
{
    public static class Mappings
    {
        public static void Configure()
        {
            Mapper.Register<Managed.Category, Category>();
            Mapper.Register<Category, Managed.Category>();

            Mapper.Register<Managed.Image, Image>();
            Mapper.Register<Image, Managed.Image>();

            Mapper.Register<Managed.Instruction, Instruction>();
            Mapper.Register<Instruction, Managed.Instruction>();

            Mapper.Register<Managed.PackagingType, PackagingType>();
            Mapper.Register<PackagingType, Managed.PackagingType>();

            Mapper.Register<Managed.Price, Price>();
            Mapper.Register<Price, Managed.Price>();

            Mapper.Register<Managed.RatingItem, RatingItem>();
            Mapper.Register<RatingItem, Managed.RatingItem>();

            Mapper.Register<Managed.Review, Review>();
            Mapper.Register<Review, Managed.Review>();

            Mapper.Register<Managed.Set, Set>();
            Mapper.Register<Set, Managed.Set>();

            Mapper.Register<Managed.Subtheme, Subtheme>();
            Mapper.Register<Subtheme, Managed.Subtheme>();

            Mapper.Register<Managed.Tag, Tag>();
            Mapper.Register<Tag, Managed.Tag>();

            Mapper.Register<Managed.Theme, Theme>();
            Mapper.Register<Theme, Managed.Theme>();

            Mapper.Register<Managed.ThemeGroup, ThemeGroup>();
            Mapper.Register<ThemeGroup, Managed.ThemeGroup>();

            Mapper.Register<Managed.YearSetCount, YearSetCount>();
            Mapper.Register<YearSetCount, Managed.YearSetCount>();
        }
    }
}