using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    struct Tile
    {
        public int Type;
        public Vector2 Position;
    }

    public class Board : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private int _width, _heigth;
        private char[,] _matrix;
        private List<Tile> _tiles;

        public Board(Game1 game, int width, int height, char[,] matrix) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _width = width;
            _heigth = height;
            _matrix = matrix;

            GenerateTileList();
        }

        public char this[int x, int y] => _matrix[x, y];

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin();

            // TODO: Change this
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

        private void GenerateTileList()
        {
            var neighborPositions = new List<(int, Vector2)>
            {
                (0b00000001, -Vector2.One),
                (0b00000010, -Vector2.UnitY),
                (0b00000100, Vector2.UnitX - Vector2.UnitY),
                (0b00001000, Vector2.UnitX),
                (0b00010000, Vector2.One),
                (0b00100000, Vector2.UnitY),
                (0b01000000, -Vector2.UnitX + Vector2.UnitY),
                (0b10000000, -Vector2.UnitX)
            };

            _tiles = new List<Tile>();

            // Go tough every tile on the matrix
            for (int y = 0; y < _heigth; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_matrix[x, y] == 'W')
                    {
                        // Current position is a wall
                        Tile tile = new Tile(){ Position = new Vector2(x, y) };

                        // Go tough the surroundings to find tile type
                        foreach (var (weight, pos) in neighborPositions)
                        {
                            if ((x + pos.X < 0 || x + pos.X >= _width) || (y + pos.Y < 0 || y + pos.Y >= _heigth))
                            {
                                // Neighbor is outside of the board
                                // We consider it as a wall
                                tile.Type |= weight;
                            }
                            else if (_matrix[x + (int)pos.X, y + (int)pos.Y] == 'W')
                            {
                                // Neighbor is a wall
                                 tile.Type |= weight;
                            }
                        }

                        _tiles.Add(tile);
                    }
                }
            }
        }
    }
}