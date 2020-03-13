using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    public class Board : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private int _width, _heigth;
        private char[,] _matrix;

        public Board(Game1 game, int width, int height, char[,] matrix) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _width = width;
            _heigth = height;
            _matrix = matrix;
        }

        public char this[int x, int y] => _matrix[x, y];

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _heigth; y++)
                {
                    Rectangle outRectangle = new Rectangle(new Point(x  * Game1.TileSize, y * Game1.TileSize), new Point(Game1.TileSize));

                    if (_matrix[x, y] == 'W')
                    {
                        _batch.Draw(
                            texture: _spriteSheet,
                            destinationRectangle: outRectangle,
                            sourceRectangle: new Rectangle(0, 13 * 16, 16, 16),
                            color: Color.White
                        );
                    }
                }
            }

            _batch.End();
        }
    }
}