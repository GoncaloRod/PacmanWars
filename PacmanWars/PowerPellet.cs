using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class PowerPellet : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _position;
        private bool _destroyNextFrame = false;

        /// <summary>
        /// Creates an instance of PowerPellet.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Position in cells of the Power Pellet</param>
        public PowerPellet(Game1 game, Point position) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _position = position;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle(_position.Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(10 * 16, 6 * 16, 16, 16),
                color: Color.White
            );
            if (_destroyNextFrame)
            {
                // TODO: Play Power Pellet mode sound
            }
            _batch.End();
        }
    }
}