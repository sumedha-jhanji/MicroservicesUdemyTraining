using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPostRepository _repository;

        public QueryHandler(IPostRepository postRepository)
        {
            _repository = postRepository;
        }
        public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
        {
            return await _repository.ListAllAsync();
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
        {
           var post = await _repository.GetByIdAsync(query.Id);
            return new List<PostEntity> { post };
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
        {
            return await _repository.ListByAuthorAsync(query.Author);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsByLikesQuery query)
        {
            return await _repository.ListWithLikesAsync(query.NumberOfLikes);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
        {
            return await _repository.ListWithCommentsAsync();
        }
    }
}
