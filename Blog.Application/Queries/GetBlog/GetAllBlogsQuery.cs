using Blog.Application.Common;
using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Blog.Application.Queries.GetBlog
{
    public record GetAllBlogsQuery(QueryObject Query) : IRequest<List<BlogDto>>;

    public class GetAllBlogsQueryHandler : IRequestHandler<GetAllBlogsQuery, List<BlogDto>>
    {
        private readonly IPostRepository _postRepository;
        private readonly ICacheService _cacheService;
        public GetAllBlogsQueryHandler(IPostRepository postRepository, ICacheService cacheService)
        {
            _postRepository = postRepository;
            _cacheService = cacheService;
        }

        public async Task<List<BlogDto>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
        {
            var query = request.Query;

            // Kiểm tra xem đây có phải là truy vấn trang mặc định không
            var isDefaultQuery = string.IsNullOrEmpty(query.Search) 
                                 && string.IsNullOrEmpty(query.TagSlug) 
                                 && query.PageNumber == 1;

            //Nếu KHÔNG phải trang chủ mặc định -> Truy vấn SQL trực tiếp không qua cache
            if (!isDefaultQuery)
            {
                var getBlogs = await _postRepository.GetAllPostsAsync(query);
                return getBlogs.Select(x => x.toBlogDto()).ToList();
            }

            //Nếu LÀ trang chủ mặc định -> Áp dụng cache-aside với Key tĩnh
            var cacheKey = CacheKey.AllBlogs;

            var cachedBlogs = await _cacheService.GetAsync<List<BlogDto>>(cacheKey);
            if (cachedBlogs != null) 
            { 
                return cachedBlogs; 
            }

           
            var defaultBlogs = await _postRepository.GetAllPostsAsync(query);
            var blogDtos = defaultBlogs.Select(x => x.toBlogDto()).ToList();

           
            await _cacheService.SetAsync(cacheKey, blogDtos, TimeSpan.FromMinutes(30));
            return blogDtos;
        }
    }
}
