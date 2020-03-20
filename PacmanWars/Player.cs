using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacmanWars
{
    public struct ControlSchema
    {
        public Keys MoveUp;
        public Keys MoveDown;
        public Keys MoveRight;
        public Keys MoveLeft;
    }

    public class Player : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Vector2 _position;
        private Vector2 _targetPosition;
        private ControlSchema _controls;
        private Vector2 _direction;
        private int _score = 0;

        public Player(Game1 game, Vector2 position, ControlSchema controls) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _position = _targetPosition = position;
            _controls = controls;
        }

        public Vector2 Position => _position;
        public int Score => _score;

        public override void Update(GameTime gameTime)
        {
            /*
            if ()
            {
                HandleInput();

                _targetPosition += _direction;
            }
            else
            {
                // Move player
                Vector2 vec = _targetPosition - _position;
                vec.Normalize();

                _position = _position + vec * gameTime.DeltaTime();
            }
            */
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle((_position * Game1.TileSize).ToPoint(), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle(new Point(0), new Point(16)),
                color: Color.White
            );

            _batch.End();
        }

        public void AddPoints(int points)
        {
            _score += points;
        }

        private void HandleInput()
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 dir = Vector2.Zero;

            if (state.IsKeyDown(_controls.MoveUp)) dir -= Vector2.UnitY;
            if (state.IsKeyDown(_controls.MoveDown)) dir += Vector2.UnitY;
            
            if (state.IsKeyDown(_controls.MoveRight)) dir += Vector2.UnitX;
            if (state.IsKeyDown(_controls.MoveLeft)) dir -= Vector2.UnitX;

            if ((dir.X == 0 || dir.Y == 0) && dir != Vector2.Zero) _direction = dir;
        }
    }
}