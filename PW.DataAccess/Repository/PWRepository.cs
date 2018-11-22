using Common.Data;
using Common.Repository;


namespace PW.DataAccess
{
    public class PWRepository<TEntity> : BaseRepository<PWContext, TEntity>
        where TEntity : class, IEntityBase
    {
        public PWRepository(PWContext context)
            : base(context)
        {
        }
    }
}
