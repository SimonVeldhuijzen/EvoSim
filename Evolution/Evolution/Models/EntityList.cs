using System.Collections.Generic;
using System.Linq;

namespace Evolution.Models
{
    public class EntityList<T> : List<T> where T : Entity
    {
        private long HighestId = 0;
        private long LowestId = 1;

        public T GetById(long id)
        {
            return this.SingleOrDefault(e => e.EntityId == id);
        }

        public long AddEntity(T entity)
        {
            this.HighestId++;
            entity.EntityId = this.HighestId;
            this.Add(entity);
            return this.HighestId;
        }

        public List<T2> Get<T2>() where T2 : T
        {
            return this.OfType<T2>().ToList();
        }

        public T GetOldest()
        {
            if (this.Any(e => e.EntityId == this.LowestId))
            {
                return this.GetById(this.LowestId);
            }

            else
            {
                this.LowestId = this.Min(e => e.EntityId);
                return GetOldest();
            }
        }
    }
}
