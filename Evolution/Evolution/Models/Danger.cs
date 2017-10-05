using System.Drawing;

namespace Evolution.Models
{
    public class Danger : Entity
    {
        public Danger(Point location)
            : base(location)
        {
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Blue, this.Location.X - Parameters.EntityRadius, this.Location.Y - Parameters.EntityRadius, Parameters.EntityRadius * 2, Parameters.EntityRadius * 2);
        }
    }
}
