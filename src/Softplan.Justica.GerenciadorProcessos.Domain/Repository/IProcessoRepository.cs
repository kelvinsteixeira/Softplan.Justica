using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Repository
{
    public interface IProcessoRepository
    {
        Processo ObterPorId(int id);

        IEnumerable<Processo> Obter(Expression<Func<Processo, bool>> predicate);

        IQueryable<Processo> Query();

        void Atualizar(Processo processo);

        void Criar(Processo processo);

        void Remover(int id);

        int Count(Expression<Func<Processo, bool>> predicate = null);
    }
}