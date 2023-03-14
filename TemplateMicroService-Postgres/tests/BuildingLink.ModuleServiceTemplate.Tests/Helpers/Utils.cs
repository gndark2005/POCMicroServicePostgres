using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Tests.Helpers
{
    public static class Utils
    {
        public static IQueryable<T> CreateAsyncIQueryable<T>(IEnumerable<T> list)
        {
            var queryable = list.AsQueryable();

            var mockIQueryable = new Mock<IQueryable<T>>();
            mockIQueryable.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<T>(queryable.GetEnumerator()));

            mockIQueryable.Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

            mockIQueryable.Setup(m => m.Expression).Returns(queryable.Expression);
            mockIQueryable.Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockIQueryable.Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return mockIQueryable.Object;
        }
    }
}
