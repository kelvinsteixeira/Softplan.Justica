using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.DomainModels
{
    public class NumeroProcessoTests
    {
        [Theory]
        [InlineData("896.984.616.484-51", "89698461648451")]
        [InlineData("34987948576", "34987948576")]
        public void ValidarNumeroProcesso(string numeroProcesso, string numeroProcessoEsperado)
        {
            Util.NumeroProcesso nProcesso = numeroProcesso;
            nProcesso.ToString().Should().Be(numeroProcessoEsperado);
        }
    }
}