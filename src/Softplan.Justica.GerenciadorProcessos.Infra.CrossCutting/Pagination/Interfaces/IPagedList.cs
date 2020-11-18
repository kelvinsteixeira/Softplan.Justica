namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination.Interfaces
{
    public interface IPagedList
    {
        int CurrentPage { get; }

        int TotalPages { get; }

        int PageSize { get; }

        int TotalCount { get; }

        bool HasPrevious { get; }

        bool HasNext { get; }
    }
}