using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    /// <summary>
    /// This represents a Pac-Dot and it's responsible to handle intersections with players.
    /// </summary>
    public class PacDot : DrawableGameComponent
    {
        private const int Points = 10;
        private const int Size = 2;

        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _position;
        private SoundEffect _pickUpSound;
        private bool _destroyNextFrame = false;

        /// <summary>
        /// Creates an instance of PacDot.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Position in cells of the Pac-Dot</param>
        public PacDot(Game1 game, Point position) : base(game)
        {
            DrawOrder = 0;

            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _position = position;
            _pickUpSound = game.Content.Load<SoundEffect>("Pickup");
        }

        public Point Position => _position;

        protected override void UnloadContent()
        {
            _pickUpSound.Dispose();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (_destroyNextFrame)
            {
                _game.PacDots.Remove(this);
                _game.Components.Remove(this);

                return;
            }
            
            Rectangle pacDotArea = new Rectangle(_position.Multiply(Game1.TileSize) + new Point((Game1.TileSize - Size) / 2), new Point(Size));

            bool isPlayer1Intersecting = pacDotArea.Intersects(_game.Player1.Area);
            bool isPlayer2Intersecting = pacDotArea.Intersects(_game.Player2.Area);

            // Special Case: if player 1 and player 2 intersect Pac-Dot position at the exact same time
            if (isPlayer1Intersecting && isPlayer2Intersecting)
            {
                float player1DistanceToDot = Vector2.Distance(_game.Player1.PositionVec, _position.ToVector2());
                float player2DistanceToDot = Vector2.Distance(_game.Player2.PositionVec, _position.ToVector2());

                if (player1DistanceToDot < player2DistanceToDot)
                    _game.Player1.AddPoints(Points);
                else if (player2DistanceToDot < player1DistanceToDot)
                    _game.Player2.AddPoints(Points);
                else
                {
                    // Players are at the same distance from the Pac-Dot
                    // The one with less points wins
                    if (_game.Player1.Score < _game.Player2.Score)
                        _game.Player1.AddPoints(Points);
                    else
                        _game.Player2.AddPoints(Points);
                }

                _destroyNextFrame = true;
            }
            else if (isPlayer1Intersecting)
	        {
                _game.Player1.AddPoints(Points);

                _destroyNextFrame = true;
            }
            else if (isPlayer2Intersecting)
	        {
                _game.Player2.AddPoints(Points);

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
                _pickUpSound.Play();
            
            _batch.End();
        }
    }
}