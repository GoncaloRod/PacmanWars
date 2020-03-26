using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    /// <summary>
    /// This represents an enemy and it's responsible handle intersections with players and autonomous movement.
    /// </summary>
    public class Enemy : DrawableGameComponent
    {
        private const float NormalSpeed = 2.0f;
        private const float RunAwaySpeed = 1.8f;
        private const float RunAwayTime = 5.0f;
        private const float CooldownAfterDie = 1.0f;

        private static int _eatenGhostsP1 = 0;
        private static int _eatenGhostsP2 = 0;

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
        private float _statCooldown;
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
            DrawOrder = 1;

            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _origin = _position = position.Multiply(Game1.TileSize);
            _targetPosition = _origin + new Point(0, -2).Multiply(Game1.TileSize);
            _type = type;
            _statCooldown = _cooldown = cooldown;

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
                [Direction.Right] = new Vector2(0, 4 + _type),
                [Direction.Left] = new Vector2(2, 4 + _type),
            };

            PowerPellet.OnPowerPelletPickUp += () =>
            {
                _eatenGhostsP1 = _eatenGhostsP2 = 0;
                _runAwayTimer = RunAwayTime;
            };
        }

        public override void Update(GameTime gameTime)
        {
            // Reduce run away timer
            _isRunningAway = _runAwayTimer > 0.0f;

            if (_isRunningAway)
                _runAwayTimer -= gameTime.DeltaTime();

            // Reduce cooldown
            if (_cooldown > 0.0f)
            {
                _cooldown -= gameTime.DeltaTime();
                return;
            }

            Move();

            Intersections();
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            if (!_isRunningAway)
            {
                _batch.Draw(
                    texture: _spriteSheet,
                    destinationRectangle: new Rectangle(_position, new Point(Game1.TileSize)),
                    sourceRectangle: new Rectangle(((_spritePositions[_direction] + Vector2.UnitX * _frame) * 16).ToPoint(), (Vector2.One * 16).ToPoint()),
                    color: Color.White
                );
            }
            else
            {
                _batch.Draw(
                    texture: _spriteSheet,
                    destinationRectangle: new Rectangle(_position, new Point(Game1.TileSize)),
                    sourceRectangle: new Rectangle(((new Vector2(8, 4) + Vector2.UnitX * _frame) * 16).ToPoint(), new Point(16, 16)),
                    color: Color.White
                );
            }

            _batch.End();
        }

        /// <summary>
        /// Reset enemy's position.
        /// </summary>
        public void ResetPosition()
        {
            _cooldown = _statCooldown;
            _runAwayTimer = 0.0f;
            _position = _origin;
            _targetPosition = (_position + new Point(0, -2)).Multiply(Game1.TileSize);
        }

        /// <summary>
        /// Handle enemy movement.
        /// </summary>
        private void Move()
        {
            float dist = Vector2.Distance(_position.ToVector2(), _targetPosition.ToVector2());

            if (dist <= 1)
            {
                List<Direction> availableDirections = new List<Direction>();

                foreach (var dir in _neighbors)
                    if (_game.Board[_targetPosition.Divide(Game1.TileSize) + dir.Value] == ' ')
                        availableDirections.Add(dir.Key);

                // This shouldn't happen, but better safe than sorry
                if (availableDirections.Count == 0) return;

                availableDirections = availableDirections.OrderBy(dir =>
                {
                    float distP1 = Vector2.Distance((_targetPosition + _neighbors[dir].Multiply(Game1.TileSize)).ToVector2(), _game.Player1.PositionVec);
                    float distP2 = Vector2.Distance((_targetPosition + _neighbors[dir].Multiply(Game1.TileSize)).ToVector2(), _game.Player2.PositionVec);

                    return distP1 <= distP2 ? distP1 : distP2;
                }).ToList();

                if (!_isRunningAway)
                {
                    // Where's the closest player?!?! I'M GOING FOR HIM!!!
                    _direction = availableDirections[0];
                }
                else
                {
                    // Ight imma head out
                    _direction = availableDirections[availableDirections.Count - 1];
                }
                
                _targetPosition = _targetPosition + _neighbors[_direction].Multiply(Game1.TileSize);
            }
            else
            {
                // Move enemy
                Vector2 vec = _targetPosition.ToVector2() - _position.ToVector2();
                vec.Normalize();

                float speed = _isRunningAway ? RunAwaySpeed : NormalSpeed;

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
            _cooldown = CooldownAfterDie;
            _runAwayTimer = 0.0f;
            _position = _origin;
            _targetPosition = _position + new Point(0, -2).Multiply(Game1.TileSize);
        }
    }
}
