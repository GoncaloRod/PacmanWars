﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class PowerPellet : DrawableGameComponent
    {
        private static int _size = 8;
        private static int _points = 50;

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
            if (_destroyNextFrame)
            {
                _game.PowerPellets.Remove(this);
                _game.Components.Remove(this);

                return;
            }

            //TODO player property for its area
            Rectangle player1Area = new Rectangle(_game.Player1.Position, new Point(Game1.TileSize));
            Rectangle player2Area = new Rectangle(_game.Player2.Position, new Point(Game1.TileSize));

            Rectangle powerPelletArea = new Rectangle(_position.Multiply(Game1.TileSize).Add(new Point((Game1.TileSize - _size) / 2)), new Point(_size));

            bool isPlayer1Intersecting = powerPelletArea.Intersects(player1Area);
            bool isPlayer2Intersecting = powerPelletArea.Intersects(player2Area);

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
                    // Player are at the same distance from the powerPellet
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

            //TODO: Change Player mode so he can turn into power Pellet mod ang be able to eat enemies
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