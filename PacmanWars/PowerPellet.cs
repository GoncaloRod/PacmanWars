using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class PowerPellet : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Vector2 _position;

        /// <summary>
        /// Creates an instance of PowerPellet.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Position in cells of the Power Pellet</param>
        public PowerPellet(Game1 game, Vector2 position) : base(game)
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

        }
    }
}