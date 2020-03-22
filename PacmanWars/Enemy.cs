using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class Enemy : DrawableGameComponent
    {
        private enum Direction
        {
            Up, Down, Right, Left
        }

        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _origin;
        private Point _position;
        private Point _targetPosition;
        private Direction _direction = Direction.Up;
        private Dictionary<Direction, Point> _neighbors;
        private int _type;
        private float _cooldown;

        public Enemy(Game1 game, Point position, int type, float cooldown = 0.0f) : base(game)
        {
            DrawOrder = 99;

            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _origin = _position = position.Multiply(Game1.TileSize);
            _targetPosition = _origin.Add(new Point(0, -2).Multiply(Game1.TileSize));
            _type = type;
            _cooldown = cooldown;

            _neighbors = new Dictionary<Direction, Point>
            {
                [Direction.Up] = new Point(0, -1),
                [Direction.Down] = new Point(0, 1),
                [Direction.Right] = new Point(1, 0),
                [Direction.Left] = new Point(-1, 0),
            };
        }

        public override void Update(GameTime gameTime)
        {
            // Reduce cooldown
            if (_cooldown > 0.0f)
            {
                _cooldown -= gameTime.DeltaTime();
                return;
            }

            if (_targetPosition == _position)
            {
                // Can I still follow the last direction??
                if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[_direction])] == ' ')
                {
                    // Yes, I can! :) but should I change????
                    _targetPosition = _position.Add(_neighbors[_direction].Multiply(Game1.TileSize));
                }
                else
                {
                    // No :( Find another direction
                    List<Direction> availableDirections = new List<Direction>();

                    foreach (var dir in _neighbors)
                        if (_game.Board[_position.Divide(Game1.TileSize).Add(dir.Value)] == ' ')
                            availableDirections.Add(dir.Key);

                    // This shouldn't happen, but better safe than sorry
                    if (availableDirections.Count == 0) return;

                    _direction = availableDirections[Game1.Rnd.Next(availableDirections.Count)];
                    _targetPosition = _position.Add(_neighbors[_direction].Multiply(Game1.TileSize));
                }
            }
            else
            {
                // Move enemy
                Vector2 vec = _targetPosition.ToVector2() - _position.ToVector2();
                vec.Normalize();

                _position = (_position.ToVector2() + vec).ToPoint();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle(_position, new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(new Point(0, 4 + _type).Multiply(16), new Point(16)),
                color: Color.White
            );

            _batch.End();
        }
    }
}
