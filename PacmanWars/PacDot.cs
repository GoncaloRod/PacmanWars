using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class PacDot : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Vector2 _position;

        /// <summary>
        /// Creates an instance of PacDot.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Position in cells of the Pac-Dot</param>
        public PacDot(Game1 game, Vector2 position) : base(game)
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
                texture:_spriteSheet,
                destinationRectangle: new Rectangle((_position * Game1.TileSize).ToPoint(), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(8 * 16, 6 * 16, 16, 16),
                color: Color.White
                );
            
            _batch.End();
        }
    }
}