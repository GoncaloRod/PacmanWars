using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PacmanWars
{
    public class Fruit : DrawableGameComponent
    {
        private const int _size = 12;

        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _position;
        private int _type;
        private int _score;
        private Dictionary<int, Vector2> _fruitsType;
        private SoundEffect _pickUpSound;

        private bool _destroyNextFrame = false;

        public Fruit(Game1 game,Point position, int type) : base(game)
        {
            DrawOrder = 0;

            _game = game;
            _batch = game.SpriteBatch;
            _spriteSheet = game.SpriteSheet;
            _position = position;
            _type = type;

            _fruitsType = new Dictionary<int, Vector2>
            {
                [0] = new Vector2(2,3), //Cherry
                [1] = new Vector2(3,3), //Strawberry
                [2] = new Vector2(4,3), //Orange
                [3] = new Vector2(5,3), //Apple
                [4] = new Vector2(6,3), //Melon
            };

            switch (_type)
            {
                case 0:
                    _score = 100;
                    break;
                case 1:
                    _score = 300;
                    break;
                case 2:
                    _score = 500;
                    break;
                case 3:
                    _score = 700;
                    break;
                case 4:
                    _score = 1000;
                    break;
                default:
                    break;
            }

            _pickUpSound = game.Content.Load<SoundEffect>("Pickup");
        }

        protected override void UnloadContent()
        {
            _pickUpSound.Dispose();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle fruitArea = new Rectangle(_position.Multiply(Game1.TileSize) + new Point((Game1.TileSize - _size) / 2), new Point(_size));

            if (_destroyNextFrame)
            {
                _game.Fruits.Remove(this);
                _game.Components.Remove(this);

                return;
            }

            if (fruitArea.Intersects(_game.Player1.Area))
            {
                _destroyNextFrame = true;

                _game.Player1.AddPoints(_score);
            }
            else if (fruitArea.Intersects(_game.Player1.Area))
            {
                _destroyNextFrame = true;

                _game.Player2.AddPoints(_score);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle(_position.Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle((_fruitsType[_type] * 16).ToPoint(), new Point(1,1).Multiply(16)),
                color: Color.White
                );

            if (_destroyNextFrame)
            {
                _pickUpSound.Play();
            }

            _batch.End();
        }
    }
}
