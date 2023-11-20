using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.AppStart
{
    public class AddMappersExtensionTests
    {
        // test to see if addTransient is called for each concrete implmentation of IMapper<T>
        [Test]
        public void AddMappers_RegistersAllMappers()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var iMapperType = typeof(IMapper<>);
            var assembly = Assembly.GetAssembly(typeof(IMapper<>));
            var mappers = GetAllTypesImplementingInterface(iMapperType, assembly);

            // Act
            serviceCollection.AddMappers();

            // Assert
            foreach (var mapper in mappers)
            {
                var interfaceType = mapper.GetInterfaces().First();
                serviceCollection.Any(x => x.ImplementationType == mapper).Should().BeTrue();

                //mockServiceCollection.Verify(m => m.AddTransient(interfaceType, mapper), Times.Once);
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
