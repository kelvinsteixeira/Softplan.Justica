using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.DomainModels
{
    public class CpfTests
    {
        [Theory]
        [InlineData("39769512001", true)]
        [InlineData("81620047004", true)]
        [InlineData("760.380.660-71", true)]
        [InlineData("887.842.950-31", true)]
        [InlineData("33759572071", false)]
        [InlineData("81630067044", false)]
        [InlineData("730.350.360-71", false)]
        [InlineData("347.842.967-31", false)]
        public void ValidarCpf(string cpf, bool valido)
        {
            Util.Cpf sut = cpf;
            sut.Valido.Should().Be(valido);
        }

        [Theory]
        [InlineData("39769512001", "397.695.120-01")]
        [InlineData("81620047004", "816.200.470-04")]
        [InlineData("76038066071", "760.380.660-71")]
        [InlineData("88784295031", "887.842.950-31")]
        public void ValidarFormatacao(string cpf, string cpfFormatado)
        {
            Util.Cpf sut = cpf;
            sut.Formatado.Should().Be(cpfFormatado);
        }

        [Theory]
        [InlineData("39769512001", "397.695.120-01")]
        [InlineData("81620047004", "816.200.470-04")]
        [InlineData("76038066071", "760.380.660-71")]
        [InlineData("88784295031", "887.842.950-31")]
        public void ValidarSemFormatacao(string cpf, string cpfFormatado)
        {
            Util.Cpf sut = cpfFormatado;
            sut.ToString().Should().Be(cpf);
        }
    }
}