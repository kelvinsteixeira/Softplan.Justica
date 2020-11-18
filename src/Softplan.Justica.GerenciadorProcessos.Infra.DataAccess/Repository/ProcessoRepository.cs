using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Repository
{
    public class ProcessoRepository : IProcessoRepository
    {
        private readonly IGerenciadorProcessosDbContext context;

        public ProcessoRepository(IGerenciadorProcessosDbContext context)
        {
            this.context = context;
        }

        public void Atualizar(Processo processo)
        {
            this.context.Processo.Update(processo);
        }

        public int Count(Expression<Func<Processo, bool>> predicate = null)
        {
            return this.context.Processo.Count(predicate);
        }

        public void Criar(Processo processo)
        {
            this.context.Processo.Add(processo);
        }

        public IEnumerable<Processo> Obter(Expression<Func<Processo, bool>> predicate)
        {
            return this.context.Processo.Where(predicate);
        }

        public Processo ObterPorId(int id)
        {
            return this.context.Processo.Include(p => p.Situacao).Include(p => p.ProcessoResponsaveis).ThenInclude(pr => pr.Responsavel).FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Processo> Query()
        {
            return this.context.Processo.Include(p => p.ProcessoResponsaveis).ThenInclude(pr => pr.Responsavel);
        }

        public void Remover(int id)
        {
            this.context.Processo.Remove(this.ObterPorId(id));
        }
    }
}