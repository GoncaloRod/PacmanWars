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
            //Variables
            Rectangle player1Area;
            Rectangle player2Area;
            float player1DistanceToDot, player2DistanceToDot;

            //Players position
            player1Area = new Rectangle((_game.Player1.Position).ToPoint(), new Point(Game1.TileSize));
            player2Area = new Rectangle((_game.Player2.Position).ToPoint(), new Point(Game1.TileSize));
           
            Rectangle pacDotArea = new Rectangle((_position).ToPoint(), new Point(Game1.TileSize / 2));

            //Special Case: if player 1 and player 2 intersect pacdot position at the exact same time
            if (pacDotArea.Intersects(player1Area) && pacDotArea.Intersects(player2Area))
            {
                player1DistanceToDot = Vector2.Distance(_game.Player1.Position, _position);
                player2DistanceToDot = Vector2.Distance(_game.Player2.Position, _position);
    
                if (player1DistanceToDot < player2DistanceToDot)
                    _game.Player1.AddPoints(10);

                if (player2DistanceToDot < player1DistanceToDot)
	                _game.Player2.AddPoints(10);

                //If the distance is equal, attributte points to the lowest scored player
                else if (_game.Player1.Score < _game.Player2.Score && player1DistanceToDot == player2DistanceToDot)
                        _game.Player1.AddPoints(10);
                else
                    _game.Player2.AddPoints(10); 

                _game.PacDots.Remove(this);
                _game.Components.Remove(this);
            }

            if (pacDotArea.Intersects(player1Area))
	        {
                _game.Player1.AddPoints(10);
                _game.PacDots.Remove(this);
                _game.Components.Remove(this);
	        }

            if (pacDotArea.Intersects(player2Area))
	        {
                _game.Player2.AddPoints(10);
                _game.PacDots.Remove(this);
                _game.Components.Remove(this);
	        }
                
	        
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