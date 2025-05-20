using System.Linq.Expressions;

using Bogus;

namespace Api.IntegrationTests.Infrastructure.Extensions;

public static class FakerExtensions
{
    public static Faker<T> RuleForList<T, TU>(this Faker<T> fakerT, Expression<Func<T, ICollection<TU>>> propertyOfListU, Func<Faker, ICollection<TU>> itemsGetter)
        where T : class
    {
        var func = propertyOfListU.Compile();

        fakerT.RuleFor(propertyOfListU, (f, t) =>
        {
            var list = func(t);
            var items = itemsGetter(f);

            foreach (var item in items)
            {
                list.Add(item);
            }

            return items;
        });

        return fakerT;
    }
}