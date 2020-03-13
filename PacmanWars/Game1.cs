using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacmanWars
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static int TileSize = 32;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _spriteSheet;
        private Board _board;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public SpriteBatch SpriteBatch => _spriteBatch;
        public Texture2D SpriteSheet => _spriteSheet;
        public Board Board => _board;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteSheet = Content.Load<Texture2D>("ss");

            LoadLevel();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            string[] file = File.ReadAllLines($@"{Content.RootDirectory}\board.txt");

            int width = file[0].Length;
            int height = file.Length;
            char[,] boardMatrix = new char[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Wall             'W'
                    // "Pac-Dot"        'D'
                    // "Power Pellet"   'P'
                    // Player 1         '1'
                    // Player 2         '2'
                    // Enemy Spawn      'S'

                    switch (file[y][x])
                    {
                        case 'W':
                            boardMatrix[x, y] = 'W';
                            break;
                        case 'D':
                            boardMatrix[x, y] = ' ';
                            break;
                        case 'P':
                            boardMatrix[x, y] = ' ';
                            break;
                        case '1':
                            boardMatrix[x, y] = ' ';
                            break;
                        case '2':
                            boardMatrix[x, y] = ' ';
                            break;
                        case 'S':
                            boardMatrix[x, y] = ' ';
                            break;
                        default:
                            boardMatrix[x, y] = ' ';
                            break;
                    }
                }
            }

            _board = new Board(this, width, height, boardMatrix);
            Components.Add(_board);

            _graphics.PreferredBackBufferWidth = width * TileSize;
            _graphics.PreferredBackBufferHeight = (height + 1) * TileSize;
            _graphics.ApplyChanges();
        }
    }
}
