using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class UI : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private SpriteFont _namco;

        /// <summary>
        /// Creates an instance of UI.
        /// </summary>
        /// <param name="game">Reference to the game</param>
        public UI(Game1 game) : base(game)
        {
            DrawOrder = 101;

            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _namco = game.Content.Load<SpriteFont>("Namco");
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            // Draw player 1 lives
            for (int i = 0; i < _game.Player1.Lives - 1; i++)
            {
                _batch.Draw(
                    texture: _spriteSheet,
                    destinationRectangle: new Rectangle(new Point(i, _game.Board.Height).Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                    sourceRectangle: new Rectangle(new Point(1, 0).Multiply(16), new Point(16)),
                    color: Color.White
                );
            }

            // Draw player 1 points
            string pointsStr = $"{_game.Player1.Score} points";
            Vector2 strSize = _namco.MeasureString(pointsStr);
            Vector2 strPos = new Vector2(Game1.TileSize * (_game.Player1.Lives - 1), _game.Board.Height * Game1.TileSize + (Game1.TileSize - strSize.Y) / 2);

            _batch.DrawString(_namco, pointsStr, strPos, Color.White);

            // Draw player 2 lives
            for (int i = 0; i < _game.Player2.Lives - 1; i++)
            {
                _batch.Draw(
                    texture: _spriteSheet,
                    destinationRectangle: new Rectangle(new Point(_game.Board.Width - i - 1, _game.Board.Height).Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                    sourceRectangle: new Rectangle(new Point(1, 0).Multiply(16), new Point(16)),
                    color: Color.White
                );
            }

            // Draw player 2 points
            pointsStr = $"{_game.Player2.Score} points";
            strSize = _namco.MeasureString(pointsStr);
            strPos = new Vector2((_game.Board.Width - (_game.Player1.Lives - 1)) * Game1.TileSize - strSize.X , _game.Board.Height * Game1.TileSize + (Game1.TileSize - strSize.Y) / 2);

            _batch.DrawString(_namco, pointsStr, strPos, Color.White);

            _batch.End();
        }
    }
}