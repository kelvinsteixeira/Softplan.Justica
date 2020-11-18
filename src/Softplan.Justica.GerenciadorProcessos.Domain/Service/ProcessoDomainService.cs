using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Service
{
    public class ProcessoDomainService : IProcessoDomainService
    {
        private readonly IProcessoRepository processoRepository;

        public ProcessoDomainService(IProcessoRepository processoRepository)
        {
            this.processoRepository = processoRepository;
        }

        public bool ValidarHierarquiaQuantidade(Processo processo)
        {
            var hierarquia = this.ObterHierarquia(processo);
            return hierarquia?.Count() <= 4;
        }

        public bool ValidarNaoExistenteNaHierarquia(Processo processo)
        {
            if (processo?.ProcessoVinculadoId == null || processo?.ProcessoVinculado == null)
            {
                return true;
            }

            var processoPai = this.processoRepository.ObterPorId(processo.ProcessoVinculadoId.Value);
            var processoFilho = this.processoRepository.Obter(p => p.ProcessoVinculadoId == processoPai.Id);

            return processoFilho == null;
        }

        private Processo ObterProcessoNeto(Processo processo)
        {
            var processoAnterior = this.processoRepository.Obter(p => p.ProcessoVinculadoId == processo.Id).FirstOrDefault();
            return processoAnterior != null ? this.ObterProcessoNeto(processoAnterior) : processo;
        }

        public IEnumerable<Processo> ObterHierarquia(Processo processo)
        {
            if (processo == null)
            {
                return default;
            }

            var processoNeto = this.ObterProcessoNeto(processo) ?? processo;
            Queue<Processo> hierarquia = new Queue<Processo>();

            void ProcessarHierarquia(Processo processoAtual)
            {
                hierarquia.Enqueue(processoAtual);
                var processoPai = processoAtual.ProcessoVinculadoId.HasValue ? this.processoRepository.ObterPorId(processoAtual.ProcessoVinculadoId.Value) : null;

                if (processoPai != null)
                {
                    ProcessarHierarquia(processoPai);
                }
            }

            ProcessarHierarquia(processoNeto);
            return hierarquia.ToList();
        }
    }
}