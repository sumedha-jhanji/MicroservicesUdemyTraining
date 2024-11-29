using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsByLikesQuery:BaseQuery
    {
        public int NumberOfLikes { get; set; }
    }
}
