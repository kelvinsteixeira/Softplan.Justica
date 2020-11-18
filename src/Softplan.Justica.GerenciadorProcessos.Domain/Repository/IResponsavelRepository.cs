using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Repository
{
    public interface IResponsavelRepository
    {
        Responsavel ObterPorId(int id);

        IEnumerable<Responsavel> Obter(Expression<Func<Responsavel, bool>> predicate);

        IQueryable<Responsavel> Query();

        void Criar(Responsavel responsavel);

        int Count(Expression<Func<Responsavel, bool>> predicate);

        void Atualizar(Responsavel responsavel);

        void Remover(int id);
    }
}