using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Models;
using System.Reflection;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.AppStart
{
    public class AddMappersExtensionTests
    {
        [Test]
        public void AddMappers_RegistersAllMappers()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var iMapperType = typeof(IMapper<>);
            var assembly = Assembly.GetAssembly(typeof(IMapper<>));
            var mappers = GetAllTypesImplementingInterface(iMapperType, assembly!);

            // Act
            serviceCollection.AddMappers();

            // Assert
            foreach (var mapper in mappers)
            {
                serviceCollection.Any(x => x.ImplementationType == mapper).Should().BeTrue();
            }
        }


        public static IEnumerable<Type> GetAllTypesImplementingInterface(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                   (y != null && y.IsGenericType &&
                   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                   (z.IsGenericType &&
                   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }
}
