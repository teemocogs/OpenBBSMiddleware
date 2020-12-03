using DBModel.SqlModels;

namespace BILib
{
    public class PostRankRepository : GenericRepository<PostRank>
    {
        public PostRankRepository(MWDBContext context) : base(context)
        {
        }
    }
}
