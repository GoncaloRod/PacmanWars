using System;
using System.Collections.Generic;
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
        public event Action OnPowerPelletPickUp;

        public static Random Rnd = new Random();
        public static int TileSize = 32;

        private static ControlSchema _player1Controls = new ControlSchema
        {
            MoveUp = Keys.W,
            MoveDown = Keys.S,
            MoveRight = Keys.D,
            MoveLeft = Keys.A
        };

        private static ControlSchema _player2Controls = new ControlSchema
        {
            MoveUp = Keys.Up,
            MoveDown = Keys.Down,
            MoveRight = Keys.Right,
            MoveLeft = Keys.Left
        };

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _spriteSheet;
        private UI _ui;
        private Board _board;
        private Player[] _players = new Player[2];
        private List<PacDot> _pacDots;
        private List<PowerPellet> _powerPellets;
        private List<Enemy> _enemies;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public SpriteBatch SpriteBatch => _spriteBatch;
        public Texture2D SpriteSheet => _spriteSheet;
        public Board Board => _board;

        public Player Player1 => _players[0];
        public Player Player2 => _players[1];
        public List<PacDot> PacDots => _pacDots;
        public List<PowerPellet> PowerPellets => _powerPellets;
        public List<Enemy> Enemies => _enemies;


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            string[] file = File.ReadAllLines($@"{Content.RootDirectory}\board.txt");

            int width = file[0].Length;
            int height = file.Length;
            char[,] boardMatrix = new char[width, height];

            _ui = new UI(this);
            _pacDots = new List<PacDot>();
            _powerPellets = new List<PowerPellet>();
            _enemies = new List<Enemy>();

            Components.Add(_ui);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Wall             'W'
                    // Invisible Wall   'I'
                    // Empty Space      'E'
                    // "Pac-Dot"        ' '
                    // "Power Pellet"   'P'
                    // Player 1         '1'
                    // Player 2         '2'
                    // Enemy Spawn      'S'

                    switch (file[y][x])
                    {
                        case 'W':   // Wall
                            boardMatrix[x, y] = 'W';
                            break;
                        case 'I':   // Invisible Wall
                            boardMatrix[x, y] = 'I';
                            break;
                        case ' ':   // "Pac-Dot"
                            boardMatrix[x, y] = ' ';

                            PacDot dot = new PacDot(this, new Point(x, y));

                            _pacDots.Add(dot);
                            Components.Add(dot);
                            break;
                        case 'P':   // "Power Pellet"
                            boardMatrix[x, y] = ' ';

                            PowerPellet pellet = new PowerPellet(this, new Point(x, y));

                            _powerPellets.Add(pellet);
                            Components.Add(pellet);
                            break;
                        case '1':   // Player 1
                            boardMatrix[x, y] = ' ';

                            _players[0] = new Player(this, new Point(x, y), _player1Controls);

                            Components.Add(_players[0]);
                            break;
                        case '2':   // Player 2
                            boardMatrix[x, y] = ' ';

                            _players[1] = new Player(this, new Point(x, y), _player2Controls);

                            Components.Add(_players[1]);
                            break;
                        case 'S':   // Enemy Spawn
                            boardMatrix[x, y] = ' ';

                            for (int i = 0; i < 4; i++)
                            {
                                Enemy enemy = new Enemy(this, new Point(x, y), i, i + 1.0f);

                                _enemies.Add(enemy);
                                Components.Add(enemy);
                            }
                            break;
                        default:    // Others
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
