using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components
{
    public static class PaginationHelper
    {
        public static async Task<TResultContainer> PageAsync<TResult, TResultContainer>(this IQueryable<TResult> queryable, IPaginatedQuery query,
                                                                                        TResultContainer resultContainer)
            where TResultContainer : PagedResult<TResult>
        {
            var result = await queryable.PageAsync(query.Page ?? 1, query.Size ?? 10);
            resultContainer.Count = result.Count;
            resultContainer.Size = result.Size;
            resultContainer.Page = result.Page;
            resultContainer.Result = result.Result;
            resultContainer.Pages = result.Pages;
            return resultContainer;
        }

        public static async Task<TResultContainer> PageAsync<TResult, TResultContainer>(this IQueryable<TResult> queryable, IPaginatedQuery query)
            where TResultContainer : PagedResult<TResult>, new()
        {
            var resultContainer = new TResultContainer();
            var result = await queryable.PageAsync(query.Page ?? 1, query.Size ?? 10);
            resultContainer.Count = result.Count;
            resultContainer.Size = result.Size;
            resultContainer.Page = result.Page;
            resultContainer.Result = result.Result;
            resultContainer.Pages = result.Pages;
            return resultContainer;
        }

        public static async Task<PagedResult<T>> PageAsync<T>(this IQueryable<T> queryable, int page, int size = 10, int maxSize = 50)
        {
            var normalizedPage = Math.Max(1, page) - 1;
            var normalizedSize = Math.Min(Math.Max(1, size), maxSize);

            var skipCount = normalizedPage * normalizedSize;
            var takeCount = normalizedSize;

            var count = await queryable.CountAsync();
            var pages = (int) Math.Ceiling((double) count / size);
            var result = await queryable.Skip(skipCount)
                                        .Take(takeCount)
                                        .ToListAsync();

            return new PagedResult<T>
            {
                Count = count,
                Page = normalizedPage + 1,
                Size = normalizedSize,
                Result = result,
                Pages = pages
            };
        }
    }

    public interface IPaginatedQuery
    {
        int? Page { get; set; }

        int? Size { get; set; }
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public int Count { get; set; }

        public int Pages { get; set; }

        public IList<T> Result { get; set; }
    }
}