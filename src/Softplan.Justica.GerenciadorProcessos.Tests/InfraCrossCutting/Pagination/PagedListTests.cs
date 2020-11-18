using System.Linq;
using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.InfraCrossCutting.Pagination
{
    public class PagedListTests
    {
        [Theory]
        [InlineData(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 1, 5, 2, true, false)]
        [InlineData(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 2, 5, 2, false, true)]
        [InlineData(new int[] { 1, 2, 3 }, 2, 1, 3, true, true)]
        [InlineData(new int[] { }, 2, 1, 0, false, false)]
        public void ValidarPaginacao(int[] itens, int pageNumber, int pageSize, int totalPaginasEsperado, bool hasNextEsperado, bool hasPreviousEsperado)
        {
            // Arrange
            // Act
            var sut = PagedList<int>.Create(itens.AsQueryable(), pageNumber, pageSize);

            // Assert
            sut.CurrentPage.Should().Be(pageNumber);
            sut.TotalPages.Should().Be(totalPaginasEsperado);
            sut.PageSize.Should().Be(pageSize);
            sut.TotalCount.Should().Be(itens.Length);
            sut.HasPrevious.Should().Be(hasPreviousEsperado);
            sut.HasNext.Should().Be(hasNextEsperado);
        }
    }
}