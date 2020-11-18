namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination
{
    public class PaginationMetadata
    {
        public PaginationMetadata(int totalCount, int pageSize, int currentPage, int totalPages, string nextPage, string previousPage)
        {
            this.TotalCount = totalCount;
            this.PageSize = pageSize;
            this.CurrentPage = currentPage;
            this.TotalPages = totalPages;
            this.NextPage = nextPage;
            this.PreviousPage = previousPage;
        }

        public int TotalCount { get; }

        public int PageSize { get; }

        public int CurrentPage { get; }

        public int TotalPages { get; }

        public string NextPage { get; }

        public string PreviousPage { get; }
    }
}