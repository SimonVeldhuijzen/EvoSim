using System;
using System.Drawing;

namespace Evolution.Models
{
    public abstract class Entity
    {
        public long EntityId { get; set; }
        public Point Location { get; set; }
        public DateTime CreatedOn { get; set; }

        public Entity(Point location)
        {
            this.Location = location;
            this.CreatedOn = DateTime.Now;
        }

        public abstract void Draw(Graphics g);
    }
}
