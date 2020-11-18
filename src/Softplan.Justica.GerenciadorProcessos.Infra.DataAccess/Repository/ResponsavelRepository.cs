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
    public class ResponsavelRepository : IResponsavelRepository
    {
        private readonly IGerenciadorProcessosDbContext context;

        public ResponsavelRepository(IGerenciadorProcessosDbContext context)
        {
            this.context = context;
        }

        public void Atualizar(Responsavel responsavel)
        {
            this.context.Responsavel.Update(responsavel);
        }

        public int Count(Expression<Func<Responsavel, bool>> predicate)
        {
            return this.context.Responsavel.Count(predicate);
        }

        public void Criar(Responsavel responsavel)
        {
            this.context.Responsavel.Add(responsavel);
        }

        public IEnumerable<Responsavel> Obter(Expression<Func<Responsavel, bool>> predicate)
        {
            return this.context.Responsavel.Where(predicate);
        }

        public Responsavel ObterPorId(int id)
        {
            return this.context.Responsavel.Include(r => r.ProcessoResponsaveis).ThenInclude(pr => pr.Responsavel).FirstOrDefault(r => r.Id == id);
        }

        public IQueryable<Responsavel> Query()
        {
            return this.context.Responsavel;
        }

        public void Remover(int id)
        {
            this.context.Responsavel.Remove(this.ObterPorId(id));
        }
    }
}