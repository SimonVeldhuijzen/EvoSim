using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Evolution.Models;

namespace Evolution.Helpers
{
    public class TwoDHelper
    {
        private Size BoardSize;

        public TwoDHelper(Size boardSize)
        {
            this.BoardSize = boardSize;
        }

        public IEnumerable<Entity> FindCollisions<T>(EntityList<T> entities, Entity b) where T : Entity
        {
            return entities.Where(e => e.EntityId != b.EntityId && Collides(e, b));
        }

        public bool Collides(Entity a, Entity b)
        {
            return Math.Abs(a.Location.X - b.Location.X) < Parameters.EntityRadius * 2
                && Math.Abs(a.Location.Y - b.Location.Y) < Parameters.EntityRadius * 2;
        }

        public List<T> Sees<T>(Blob blob, EntityList<T> entities) where T : Entity
        {
            var length = GetEyeSightLength(blob.EyeSight);
            var width = GetEyeSightWidth(blob.EyeSight);
            var result = new List<T>();

            foreach (var entity in entities.Where(e => e.EntityId != blob.EntityId))
            {
                var virtualPointB = ClosestVirtualPoint(blob, entity);
                var distance = Distance(blob, virtualPointB);
                if (distance > length)
                {
                    continue;
                }

                var xDist = virtualPointB.X - blob.Location.X;
                var yDist = (virtualPointB.Y - blob.Location.Y);
                var alpha = RadiansToDegrees(Math.Asin(yDist / distance));

                if (xDist < 0)
                {
                    alpha = 180 - alpha;
                }
                else if (yDist < 0)
                {
                    alpha = alpha + 360;
                }

                var beta = Math.Abs(alpha - blob.LookingDirection);
                if (beta > width / 2)
                {
                    continue;
                }

                result.Add(entity);
            }

            return result;
        }

        public int GetEyeSightLength(double eyeSight)
        {
            return (int)Math.Round(Parameters.BaseEyeSightLength + Parameters.MaxEyeSightLength * (eyeSight + 1), 0, MidpointRounding.AwayFromZero);
        }

        public int GetEyeSightWidth(double eyeSight)
        {
            return (int)Math.Round(Parameters.BaseEyeSightWidth + Parameters.MaxEyeSightWidth * (eyeSight * -1 + 1), 0, MidpointRounding.AwayFromZero);
        }

        public double Distance(Entity a, Point b)
        {
            int xDist = a.Location.X - b.X;
            int yDist = a.Location.Y - b.Y;
            return Hypothenuse(xDist, yDist);
        }

        private Point ClosestVirtualPoint(Entity a, Entity b)
        {
            var bPoints = new List<Point>();
            if (b.Location.X < this.BoardSize.Width / 2)
            {
                bPoints.Add(new Point(b.Location.X + this.BoardSize.Width, b.Location.Y));
            }
            else
            {
                bPoints.Add(new Point(b.Location.X - this.BoardSize.Width, b.Location.Y));
            }

            if (b.Location.Y < this.BoardSize.Height / 2)
            {
                bPoints.Add(new Point(b.Location.X, b.Location.Y + this.BoardSize.Height));
            }
            else
            {
                bPoints.Add(new Point(b.Location.X, b.Location.Y - this.BoardSize.Height));
            }

            var closest = b.Location;
            var distance = Distance(a, b.Location);

            foreach (var p in bPoints)
            {
                var dist = Distance(a, p);
                if (dist < distance)
                {
                    distance = dist;
                    closest = p;
                }
            }

            return closest;
        }

        public double Hypothenuse(double sideA, double sideB)
        {
            return Math.Sqrt(sideA * sideA + sideB * sideB);
        }

        public InputParams GetInputParams(Blob blob, Entity other)
        {
            var eyeSightLength = GetEyeSightLength(blob.EyeSight);

            var virtualPointB = ClosestVirtualPoint(blob, other);
            var distance = Distance(blob, virtualPointB);
            var xDistOrig = virtualPointB.X - blob.Location.X;
            var yDistOrig = (virtualPointB.Y - blob.Location.Y);
            var alpha = RadiansToDegrees(Math.Asin(yDistOrig / distance));

            if (xDistOrig < 0)
            {
                alpha = 180 - alpha;
            }
            else if (yDistOrig < 0)
            {
                alpha = alpha + 360;
            }

            var beta = alpha - blob.LookingDirection;
            while (beta < 0)
            {
                beta += 360;
            }

            var yDist = (distance * Math.Sin(DegreesToRadians(beta))) / eyeSightLength;
            var xDist = (distance * Math.Cos(DegreesToRadians(beta))) / eyeSightLength;

            var result = new InputParams(Distance(blob, virtualPointB) / eyeSightLength, xDist, yDist);

            return result;
        }

        public int GetAdjustedLookingDirection(int lookingDirection, double delta)
        {
            lookingDirection = (int)Math.Round(lookingDirection + delta * Parameters.MaxTurnPerTick, 0, MidpointRounding.AwayFromZero);
            while (lookingDirection < 0)
            {
                lookingDirection += 360;
            }

            return lookingDirection % 360;
        }

        public Point GetNewLocation(Blob blob, double xFactor, double yFactor)
        {
            var xOffset = xFactor * Parameters.BlobSpeed;
            var yOffset = yFactor * Parameters.BlobSpeed;

            // get distance and direction on blob-axes
            var distance = Hypothenuse(xOffset, yOffset);

            var direction = RadiansToDegrees(Math.Asin(yOffset / distance));
            if (xOffset < 0)
            {
                direction = 180 - direction;
            }
            else if (yOffset < 0)
            {
                direction = direction + 360;
            }

            // translate distance and direction to actual axes
            direction = (blob.LookingDirection + direction) % 360;

            // get actual differences in x and y
            var x = (int)Math.Round(blob.Location.X + distance * Math.Cos(DegreesToRadians(direction)), 0, MidpointRounding.AwayFromZero);
            var y = (int)Math.Round(blob.Location.Y + distance * Math.Sin(DegreesToRadians(direction)), 0, MidpointRounding.AwayFromZero);

            x = x % this.BoardSize.Width;
            y = y % this.BoardSize.Height;

            while (x < 0)
            {
                x += this.BoardSize.Width;
            }

            while (y < 0)
            {
                y += this.BoardSize.Height;
            }

            // get the new point
            var result = new Point(x, y);

            return result;
        }

        public double DegreesToRadians(double degrees)
        {
            return Math.PI * (degrees / 180.0);
        }

        public double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        public Point[] GetEdgeOfView(Blob blob)
        {
            var result = new Point[2];
            var distance = GetEyeSightLength(blob.EyeSight);
            var width = GetEyeSightWidth(blob.EyeSight);
            var lookingDirection = blob.LookingDirection;
            var anglePoint1 = (360 + lookingDirection - width / 2) % 360;
            var anglePoint2 = (lookingDirection + width / 2) % 360;

            var x1 = (int)Math.Round(blob.Location.X + distance * Math.Cos(DegreesToRadians(anglePoint1)), 0, MidpointRounding.AwayFromZero);
            var y1 = (int)Math.Round(blob.Location.Y + distance * Math.Sin(DegreesToRadians(anglePoint1)), 0, MidpointRounding.AwayFromZero);

            var x2 = (int)Math.Round(blob.Location.X + distance * Math.Cos(DegreesToRadians(anglePoint2)), 0, MidpointRounding.AwayFromZero);
            var y2 = (int)Math.Round(blob.Location.Y + distance * Math.Sin(DegreesToRadians(anglePoint2)), 0, MidpointRounding.AwayFromZero);
            result[0] = new Point(x1, y1);
            result[1] = new Point(x2, y2);

            return result;
        }
    }
}
