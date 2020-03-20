using Microsoft.Xna.Framework;

namespace PacmanWars
{
    public static class Extensions
    {
        /// <summary>
        /// Returns the time passed between current and previous frame.
        /// </summary>
        /// <param name="gameTime">Reference to game time.</param>
        public static float DeltaTime(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        #region Point Class Extentions

        /// <summary>
        /// Add a Point to current instance of Point.
        /// </summary>
        /// <param name="point">Point to add to current Point.</param>
        /// <returns>Final Point after sum.</returns>
        public static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        /// <summary>
        /// Subtract a Point to current instance of Point.
        /// </summary>
        /// <param name="point">Point to subtract to current Point.</param>
        /// <returns>Final Point after subtraction.</returns>
        public static Point Subtract(this Point point, Point other)
        {
            return new Point(point.X - other.X, point.Y - other.Y);
        }

        /// <summary>
        /// Multiply current instance of Point by a given value.
        /// </summary>
        /// <param name="point">Value to multiply the point by.</param>
        /// <returns>Final Point after multiplication.</returns>
        public static Point Multiply(this Point point, int scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }

        /// <summary>
        /// Divide current instance of Point by a given value.
        /// </summary>
        /// <param name="point">Value to divide the point by.</param>
        /// <returns>Final Point after division.</returns>
        public static Point Divide(this Point point, int divisor)
        {
            return new Point(point.X / divisor, point.Y / divisor);
        }

        #endregion
    }
}