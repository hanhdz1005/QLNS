using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Common
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
            this IQueryable<TEntity> query,
            int pageIndex,
            int pageSize,
            IMapper mapper)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = mapper.Map<IReadOnlyList<TDto>>(items)
            };
        }
    }
}
