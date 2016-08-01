using AutoMapper;

namespace Ks.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        #region Category

        //public static CategoryModel ToModel(this Category entity)
        //{
        //    return entity.MapTo<Category, CategoryModel>();
        //}

        //public static Category ToEntity(this CategoryModel model)
        //{
        //    return model.MapTo<CategoryModel, Category>();
        //}

        //public static Category ToEntity(this CategoryModel model, Category destination)
        //{
        //    return model.MapTo(destination);
        //}

        #endregion
    }
}