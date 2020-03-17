using Microsoft.Xna.Framework;

namespace PacmanWars
{
    public static class Extensions
    {
        public static float DeltaTime(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static Point Multiply(this Point point, int scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }
    }
}