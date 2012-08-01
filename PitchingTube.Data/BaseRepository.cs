using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Data;
using System.Linq.Expressions;

namespace PitchingTube.Data
{
    public class BaseRepository<T> where T : EntityObject
    {
        protected PitchingTubeEntities _context;
        protected IObjectSet<T> _objectSet;

        public BaseRepository()
        {
            _context = new PitchingTubeEntities();
            _objectSet = _context.CreateObjectSet<T>();
        }

        public virtual void Insert(T newEntity)
        {
            _objectSet.AddObject(newEntity);
            _context.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            _objectSet.AddObject(entity);
            _context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            _context.SaveChanges();
        }

        public virtual List<T> ToList()
        {
            return _objectSet.ToList();
        }
        public virtual IEnumerable<T> Query(Expression<Func<T, bool>> query)
        {
            return _objectSet.Where(query).ToList();

        }
        public virtual T FirstOrDefault(Expression<Func<T, bool>> query)
        {
            return _objectSet.FirstOrDefault(query);
        }
    }
}
