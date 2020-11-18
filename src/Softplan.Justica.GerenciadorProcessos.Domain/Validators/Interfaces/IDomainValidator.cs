using System.Threading.Tasks;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces
{
    public interface IDomainValidator<TModel>
    {
        Task<bool> ValidateModelAsync(TModel t);
    }
}