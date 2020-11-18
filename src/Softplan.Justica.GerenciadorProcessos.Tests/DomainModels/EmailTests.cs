using FluentAssertions;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Xunit;

namespace Softplan.Justica.GerenciadorProcessos.Tests.DomainModels
{
    public class EmailTests
    {
        [Theory]
        [InlineData("mail@mail.com", true)]
        [InlineData("mail.mail@mail.com", true)]
        [InlineData("sdfsdf.com.fsdf", false)]
        [InlineData("sdfs@sdfs@sdf.sdf", false)]
        [InlineData(".sdfs@sdfs@sdf.sdf", false)]
        public void ValidarEmail(string email, bool valid)
        {
            Util.Email sut = email;
            sut.Valido.Should().Be(valid);
        }
    }
}