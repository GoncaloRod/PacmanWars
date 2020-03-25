using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class Enemy : DrawableGameComponent
    {
        private static float _speed = 2.0f;
        private static float _runAwaySpeed = 1.5f;
        private static float _runAwayTime = 5.0f;
        private static float _cooldownAfterDie = 2.0f;
        private static int _eatenGhostsP1 = 0;
        private static int _eatenGhostsP2 = 0;

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
        private Dictionary<Direction, Vector2> _spritePositions;
        private int _type;
        private int _frame = 0;
        private float _cooldown;
        private float _runAwayTimer = 0.0f;
        private bool _isRunningAway = false;

        /// <summary>
        /// Creates an instance of Enemy.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="position">Starting position in cells of the Enemy</param>
        /// <param name="type">Type of the Enemy (Used to change colors)</param>
        /// <param name="cooldown">Starting cooldown of the Enemy</param>
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

            _spritePositions = new Dictionary<Direction, Vector2>
            {
                [Direction.Up] = new Vector2(4, 4 + _type),
                [Direction.Down] = new Vector2(6, 4 + _type),
                [Direction.Right] = new Vector2(0, 4 +_type),
                [Direction.Left] = new Vector2(2, 4 + _type),
            };

            PowerPellet.OnPowerPelletPickUp += () =>
            {
                _eatenGhostsP1 = _eatenGhostsP2 = 0;
                _runAwayTimer = _runAwayTime;
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

            // Reduce run away timer
            _isRunningAway = _runAwayTimer > 0.0f;

            if (_isRunningAway)
                _runAwayTimer -= gameTime.DeltaTime();

            Move();

            Intersections();
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle(_position, new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(((_spritePositions[_direction] + Vector2.UnitX * _frame) * 16).ToPoint(), (Vector2.One * 16).ToPoint()),
                color: Color.White
            );

            _batch.End();
        }

        /// <summary>
        /// Handle enemy movement.
        /// </summary>
        private void Move()
        {
            if (_targetPosition == _position)
            {
                // Can I still follow the last direction??
                if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[_direction])] == ' ')
                {
                    // TODO(Goncalo): Run away behavior

                    // Yes, I can! :) but should I change????

                    List<Direction> availableDirections = new List<Direction>
                    {
                        _direction
                    };

                    if (_direction == Direction.Up || _direction == Direction.Down)
                    {
                        // Can I go left or right?
                        if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[Direction.Left])] == ' ')
                            availableDirections.Add(Direction.Left);

                        if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[Direction.Right])] == ' ')
                            availableDirections.Add(Direction.Right);
                    }
                    else
                    {
                        // Can I go up or down?
                        if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[Direction.Up])] == ' ')
                            availableDirections.Add(Direction.Up);

                        if (_game.Board[_position.Divide(Game1.TileSize).Add(_neighbors[Direction.Down])] == ' ')
                            availableDirections.Add(Direction.Down);
                    }

                    _direction = availableDirections[Game1.Rnd.Next(availableDirections.Count)];

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

                float speed = _isRunningAway ? _runAwaySpeed : _speed;

                _position = (_position.ToVector2() + (vec * speed)).ToPoint();

                if ((_position.X + _position.Y) % 8 == 0)
                {
                    _frame++;
                    if (_frame > 1)
                        _frame = 0;
                }
            }
        }

        /// <summary>
        /// Handle intersections with players.
        /// </summary>
        private void Intersections()
        {
            Rectangle enemyArea = new Rectangle(_position, new Point(Game1.TileSize));

            if (_isRunningAway)
            {
                if (enemyArea.Intersects(_game.Player1.Area))
                {
                    _game.Player1.AddPoints((int)Math.Pow(2, ++_eatenGhostsP1) * 100);

                    Die();
                }
                else if (enemyArea.Intersects(_game.Player2.Area))
                {
                    _game.Player2.AddPoints((int)Math.Pow(2, ++_eatenGhostsP2) * 100);

                    Die();
                }
            }
            else
            {
                if (enemyArea.Intersects(_game.Player1.Area))
                {
                    _game.Player1.Die();
                }
                else if (enemyArea.Intersects(_game.Player2.Area))
                {
                    _game.Player2.Die();
                }
            }
        }

        /// <summary>
        /// Kill enemy, set a cooldown and reset run away timer.
        /// Enemy will be automatically moved to his origin.
        /// </summary>
        private void Die()
        {
            _cooldown = _cooldownAfterDie;
            _runAwayTimer = 0.0f;
            _position = _origin;
            _targetPosition = _position.Add(new Point(0, -2).Multiply(Game1.TileSize));
        }
    }
}
