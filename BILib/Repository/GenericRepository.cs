using DBModel.SqlModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BILib
{
    /// <summary>
    /// Ref: https://docs.microsoft.com/zh-tw/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#create-a-generic-repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly MWDBContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(MWDBContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// 搜尋 (若後續須變更資料，請用此 API)
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Get()
        {
            return _dbSet;
        }  
        
        /// <summary>
        /// 僅供不變更的單純搜尋；不追蹤 DB 資料變動，提高執行效能
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAsNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
