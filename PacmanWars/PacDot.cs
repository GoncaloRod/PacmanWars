using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class PacDot : DrawableGameComponent
    {
        private static int _points = 10;
        private static int _size = 2;

        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _position;
        private bool _destroyNextFrame = false;

        /// <summary>
        /// Creates an instance of PacDot.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Position in cells of the Pac-Dot</param>
        public PacDot(Game1 game, Point position) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _position = position;
        }

        public override void Update(GameTime gameTime)
        {
            if (_destroyNextFrame)
            {
                _game.PacDots.Remove(this);
                _game.Components.Remove(this);

                return;
            }
            
            Rectangle pacDotArea = new Rectangle(_position.Multiply(Game1.TileSize).Add(new Point((Game1.TileSize - _size) / 2)), new Point(_size));

            bool isPlayer1Intersecting = pacDotArea.Intersects(_game.Player1.Area);
            bool isPlayer2Intersecting = pacDotArea.Intersects(_game.Player2.Area);

            // Special Case: if player 1 and player 2 intersect pacdot position at the exact same time
            if (isPlayer1Intersecting && isPlayer2Intersecting)
            {
                float player1DistanceToDot = Vector2.Distance(_game.Player1.PositionVec, _position.ToVector2());
                float player2DistanceToDot = Vector2.Distance(_game.Player2.PositionVec, _position.ToVector2());

                if (player1DistanceToDot < player2DistanceToDot)
                    _game.Player1.AddPoints(_points);
                else if (player2DistanceToDot < player1DistanceToDot)
                    _game.Player2.AddPoints(_points);
                else
                {
                    // Player are at the same distance from the pacdot
                    // The one with less points wins
                    if (_game.Player1.Score < _game.Player2.Score)
                        _game.Player1.AddPoints(_points);
                    else
                        _game.Player2.AddPoints(_points);
                }

                _destroyNextFrame = true;
            }
            else if (isPlayer1Intersecting)
	        {
                _game.Player1.AddPoints(_points);

                _destroyNextFrame = true;
            }
            else if (isPlayer2Intersecting)
	        {
                _game.Player2.AddPoints(_points);

                _destroyNextFrame = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            _batch.Draw(
                texture:_spriteSheet,
                destinationRectangle: new Rectangle(_position.Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(8 * 16, 6 * 16, 16, 16),
                color: Color.White
            );

            if (_destroyNextFrame)
            {
                // TODO: Play sound
            }
            
            _batch.End();
        }
    }
}