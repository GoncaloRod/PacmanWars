using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanWars
{
    internal struct Tile
    {
        public int Type;
        public Point Position;
    }

    public class Board : DrawableGameComponent
    {
        private Game1 _game;
        private SpriteBatch _batch;

        private Texture2D _spriteSheet;
        private int _width, _heigth;
        private char[,] _matrix;
        private List<Tile> _tiles;
        private Dictionary<int, Rectangle> _tileSprites;


        /// <summary>
        /// Creates an instance of Board.
        /// </summary>
        /// <param name="game">Reference to game</param>
        /// <param name="width">Width of the board</param>
        /// <param name="height">Height of the board</param>
        /// <param name="matrix">Char matrix of the board</param>
        public Board(Game1 game, int width, int height, char[,] matrix) : base(game)
        {
            _game = game;
            _batch = game.SpriteBatch;

            _spriteSheet = game.SpriteSheet;
            _width = width;
            _heigth = height;
            _matrix = matrix;

            LoadTileData();

            GenerateTileList();
        }

        /// <summary>
        /// Get board's tile char given an X and a Y position.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public char this[int x, int y] => _matrix[x, y];

        /// <summary>
        /// Get board's tile char given a Point for the position.
        /// </summary>
        /// <param name="pos">Point for the position.</param>
        public char this[Point pos] => _matrix[pos.X, pos.Y];

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(samplerState: SamplerState.PointClamp);

            foreach (Tile tile in _tiles)
            {
                Rectangle destRect = new Rectangle(tile.Position.Multiply(Game1.TileSize), new Point(Game1.TileSize));
                Rectangle srcRect = _tileSprites.ContainsKey(tile.Type) ? _tileSprites[tile.Type] : _tileSprites[-1];

                _batch.Draw(
                    texture: _spriteSheet,
                    destinationRectangle: destRect,
                    sourceRectangle: srcRect,
                    color: Color.White
                );
            }

            _batch.End();
        }

        /// <summary>
        /// Loads information referent to tile location on sprite sheet from file.
        /// </summary>
        private void LoadTileData()
        {
            string[] file = File.ReadAllLines($@"{Game.Content.RootDirectory}\tiles.txt");

            _tileSprites = new Dictionary<int, Rectangle>
            {
                [-1] = new Rectangle(8 * 16, 9 * 16, 16, 16)
            };

            foreach (string line in file)
            {
                if (line != "")
                {
                    string[] splitedLine = line.Trim().Split(' ');

                    int key = Convert.ToInt32(splitedLine[2], 2);
                    Rectangle value = new Rectangle(int.Parse(splitedLine[0]) * 16, int.Parse(splitedLine[1]) * 16, 16, 16);

                    _tileSprites[key] = value;
                }
            }
        }

        /// <summary>
        /// Reads board's char matrix and converts every wall to a list of tiles.
        /// </summary>
        private void GenerateTileList()
        {
            (int, Point)[] neighborPositions = {
                (0b00000001, new Point(-1, -1)),
                (0b00000010, new Point(0, -1)),
                (0b00000100, new Point(1, -1)),
                (0b00001000, new Point(1, 0)),
                (0b00010000, new Point(1, 1)),
                (0b00100000, new Point(0, 1)),
                (0b01000000, new Point(-1, 1)),
                (0b10000000, new Point(-1, 0))
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
                        Tile tile = new Tile(){ Position = new Point(x, y), Type = 0};

                        // Go tough the surroundings to find tile type
                        foreach (var (weight, pos) in neighborPositions)
                        {
                            bool isInsideBounds = (x + pos.X >= 0 && x + pos.X < _width) &&
                                                  (y + pos.Y >= 0 && y + pos.Y < _heigth);

                            bool isWall = isInsideBounds && _matrix[x + (int) pos.X, y + (int) pos.Y] == 'W';

                            if (!isInsideBounds || isWall)
                            {
                                // Neighbor is either outside of the board or a wall
                                tile.Type += weight;
                            }
                        }

                        _tiles.Add(tile);
                    }
                }
            }
        }
    }
}