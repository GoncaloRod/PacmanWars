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
            // TODO: Refactor this ugly thing

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
            string message = $"{_game.Player1.Score} points";
            Vector2 messageSize = _namco.MeasureString(message);
            Vector2 messagePos = new Vector2(Game1.TileSize * (_game.Player1.Lives - 1), _game.Board.Height * Game1.TileSize + (Game1.TileSize - messageSize.Y) / 2);

            _batch.DrawString(_namco, message, messagePos, Color.White);

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
            message = $"{_game.Player2.Score} points";
            messageSize = _namco.MeasureString(message);
            messagePos = new Vector2((_game.Board.Width - (_game.Player2.Lives - 1)) * Game1.TileSize - messageSize.X , _game.Board.Height * Game1.TileSize + (Game1.TileSize - messageSize.Y) / 2);

            _batch.DrawString(_namco, message, messagePos, Color.White);

            // Game Over message
            if (_game.LoserPlayer != null)
            {
                message = $"player {(_game.LoserPlayer.Number == 1 ? 2 : 1)} wins!";
                messageSize = _namco.MeasureString(message);
                messagePos = (_game.ScreenSize - messageSize) / 2.0f;

                if (gameTime.TotalGameTime.Seconds % 2 == 0)
                    _batch.DrawString(_namco, message, messagePos, Color.White);
            }

            _batch.End();
        }
    }
}