using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Softplan.Justica.GerenciadorProcessos.Domain.Command;
using Softplan.Justica.GerenciadorProcessos.Domain.Constants;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Validators
{
    public class AtualizarResponsavelCommandValidator : DomainValidatorBase<AtualizarResponsavelCommand>, IAtualizarResponsavelCommandValidator
    {
        private readonly IResponsavelRepository responsavelRepository;

        public AtualizarResponsavelCommandValidator(IResponsavelRepository responsavelRepository,
            INotificationContext notificationContext) : base(notificationContext)
        {
            this.responsavelRepository = responsavelRepository;

            this.RuleFor(p => p.Id)
                           .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Id: {ErrorMessages.ErroVazio}");

            this.RuleFor(p => p.Nome)
                           .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Nome: {ErrorMessages.ErroVazio}")
                           .MaximumLength(150).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Nome: {string.Format(ErrorMessages.ErroTamanhoMaximo, 150)}");

            this.RuleFor(p => p.Cpf)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Cpf: {ErrorMessages.ErroVazio}")
                .Custom((cpf, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(cpf) && !Util.ValidarCpf(cpf))
                    {
                        context.AddFailure(
                            new ValidationFailure("Cpf", string.Format(ErrorMessages.CpfInvalido, cpf))
                            {
                                ErrorCode = NotificationKeys.InvalidArgument
                            });
                    }
                });

            this.RuleFor(p => p.Email)
                .NotEmpty().WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Email: {ErrorMessages.ErroVazio}")
                .MaximumLength(400).WithErrorCode(NotificationKeys.InvalidArgument).WithMessage($"Email: {string.Format(ErrorMessages.ErroTamanhoMaximo, 400)}")
                .Custom((email, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(email) && !Util.ValidarEmail(email))
                    {
                        context.AddFailure(
                            new ValidationFailure("Email", string.Format(ErrorMessages.EmailInvalido, email))
                            {
                                ErrorCode = NotificationKeys.InvalidArgument
                            });
                    }
                });
        }

        public override Task<bool> ValidateModelAsync(AtualizarResponsavelCommand model)
        {
            var result = this.Validate(model);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    this.AddError(error.ErrorCode, error.ErrorMessage);
                }

                return Task.FromResult(false);
            }

            var responsavel = this.responsavelRepository.ObterPorId(model.Id.Value);
            if (responsavel == null)
            {
                this.AddError(NotificationKeys.InvalidArgument, ErrorMessages.ResponsavelNaoEncontrado);
                return Task.FromResult(false);
            }

            Util.Cpf cpf = model.Cpf;
            responsavel = this.responsavelRepository.Obter(r => r.Cpf == cpf.ToString()).FirstOrDefault();
            if (responsavel != null && responsavel.Id != model.Id)
            {
                this.AddError(NotificationKeys.InvalidArgument, string.Format(ErrorMessages.CpfEmUso, model.Cpf));
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}