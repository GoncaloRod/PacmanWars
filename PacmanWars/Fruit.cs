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
    /// <summary>
    /// This represents a fruit and it's responsible for its intersections with players.
    /// </summary>
    public class Fruit : DrawableGameComponent
    {
        private const int Size = 12;

        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private Point _position;
        private int _type;
        private int _score;
        private Dictionary<int, Vector2> _fruitsType;
        private SoundEffect _pickUpSound;

        private bool _destroyNextFrame = false;

        /// <summary>
        /// Instance of a fruit
        /// </summary>
        /// <param name="game">Reference to game.</param>
        /// <param name="position">Position to spawn a fruit.</param>
        /// <param name="type">Type of fruit to be spawned.</param>
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
            if (_destroyNextFrame)
            {
                _game.Fruits.Remove(this);
                _game.Components.Remove(this);

                return;
            }

            Rectangle fruitArea = new Rectangle(_position.Multiply(Game1.TileSize) + new Point((Game1.TileSize - Size) / 2), new Point(Size));

            //Player 1 pick
            if (fruitArea.Intersects(_game.Player1.Area))
            {
                _game.Player1.AddPoints(_score);

                _game.WasFruitSpawned = false;

                _destroyNextFrame = true;
            }

            //Inter
            else if (fruitArea.Intersects(_game.Player2.Area))
            {
                _game.Player2.AddPoints(_score);

                _game.WasFruitSpawned = false;

                _destroyNextFrame = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            _batch.Draw(
                texture: _spriteSheet,
                destinationRectangle: new Rectangle(_position.Multiply(Game1.TileSize), new Point(Game1.TileSize)),
                sourceRectangle: new Rectangle((_fruitsType[_type] * 16).ToPoint(), (Vector2.One * 16).ToPoint()),
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
