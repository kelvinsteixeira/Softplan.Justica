using System;
using System.Collections.Generic;
using System.Linq;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination.Interfaces;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Pagination
{
    public class PagedList<T> : List<T>, IPagedList
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => this.TotalPages > 0 && this.CurrentPage > 1;

        public bool HasNext => this.CurrentPage < TotalPages;

        private PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            this.TotalCount = count;
            this.CurrentPage = pageNumber;
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        // Used after converting domain entities to any other type
        public static PagedList<T> Create(List<T> currentItems, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedList<T>(currentItems, totalCount, pageNumber, pageSize);
        }
    }
}