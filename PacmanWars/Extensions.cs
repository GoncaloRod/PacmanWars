using Microsoft.Xna.Framework;

namespace PacmanWars
{
    /// <summary>
    /// Extension methods for external classes.
    /// </summary>
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