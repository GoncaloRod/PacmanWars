using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PacmanWars
{
    /// <summary>
    /// Can be used to represent the direction that a component, like Players or Enemies, is facing.
    /// </summary>
    public enum Direction
    {
        Up, Down, Right, Left
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// Game's tile size in pixels.
        /// </summary>
        public const int TileSize = 32;

        /// <summary>
        /// Random number generator to be used across all game.
        /// </summary>
        public static Random Rnd = new Random();

        private readonly ControlSchema _player1Controls = new ControlSchema
        {
            MoveUp = Keys.W,
            MoveDown = Keys.S,
            MoveRight = Keys.D,
            MoveLeft = Keys.A
        };

        private readonly ControlSchema _player2Controls = new ControlSchema
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
        private List<Fruit> _fruits = new List<Fruit>();
        private Player _winner;
        private int _highScore;
        private int _fruitsSpawned = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Get screen size in the form of a Vector2.
        /// </summary>
        public Vector2 ScreenSize => new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        
        /// <summary>
        /// Get game's sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch => _spriteBatch;

        /// <summary>
        /// Get game's sprite sheet.
        /// </summary>
        public Texture2D SpriteSheet => _spriteSheet;

        /// <summary>
        /// Get game's board.
        /// </summary>
        public Board Board => _board;

        /// <summary>
        /// Get game's player 1.
        /// </summary>
        public Player Player1 => _players[0];

        /// <summary>
        /// Get game's player 2.
        /// </summary>
        public Player Player2 => _players[1];

        /// <summary>
        /// Get a list with game's current active Pac-Dots.
        /// </summary>
        public List<PacDot> PacDots => _pacDots;

        /// <summary>
        /// Get a list with game's current active Power Pellets.
        /// </summary>
        public List<PowerPellet> PowerPellets => _powerPellets;

        /// <summary>
        /// Get a list with game's enemies.
        /// </summary>
        public List<Enemy> Enemies => _enemies;

        /// <summary>
        /// Get a list with game's fruits.
        /// </summary>
        public List<Fruit> Fruits => _fruits;

        /// <summary>
        /// Get the game's winner.
        /// This will be null if there is no winner yet.
        /// </summary>
        public Player Winner => _winner;

        /// <summary>
        /// Get current existing High Score.
        /// </summary>
        public int HighScore => _highScore;

        /// <summary>
        /// Tell if the fruit was spawned.
        /// </summary>
        public bool WasFruitSpawned { get; set; } = false;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            LoadHighScore();

            Player.OnPlayerLose += () =>
            {
                _winner = _players.First(player => player.Lives != 0);

                // Disable components
                foreach (Player player in _players)
                    player.Enabled = false;

                foreach (PacDot pacDot in _pacDots)
                    pacDot.Enabled = false;

                foreach (PowerPellet powerPellet in _powerPellets)
                    powerPellet.Enabled = false;

                foreach (Enemy enemy in _enemies)
                    enemy.Enabled = false;

                if (_winner.Score > HighScore)
                    SaveHighScore(_winner.Score);
            };

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

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            _spriteSheet.Dispose();

            base.UnloadContent();
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

            if (_pacDots.Count == 0 && _powerPellets.Count == 0)
            {
                ReloadLevel();
            }

            bool availableToSpawnP1 = Player1.Score > 1200 && Player1.Score > 1200 * _fruitsSpawned && !WasFruitSpawned && _fruitsSpawned < 6;
            bool availableToSpawnP2 = Player2.Score > 1200 && Player2.Score > 1200 * _fruitsSpawned && !WasFruitSpawned && _fruitsSpawned < 6;

            if (availableToSpawnP1 || availableToSpawnP2)
            {
                SpawnFruit();
                _fruitsSpawned++;
                WasFruitSpawned = true;
            }

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

        /// <summary>
        /// Load level when game stars creating board, players, enemies, Pac-Dots and Power Pellets.
        /// </summary>
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

                            _players[0] = new Player(this, new Point(x, y), _player1Controls, 1, Direction.Right);

                            Components.Add(_players[0]);
                            break;
                        case '2':   // Player 2
                            boardMatrix[x, y] = ' ';

                            _players[1] = new Player(this, new Point(x, y), _player2Controls, 2, Direction.Left);

                            Components.Add(_players[1]);
                            break;
                        case 'S':   // Enemy Spawn
                            boardMatrix[x, y] = ' ';

                            for (int i = 0; i < 4; i++)
                            {
                                Enemy enemy = new Enemy(this, new Point(x, y), i, i * 5.0f);

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

        /// <summary>
        /// Reloads the level by moving players and enemies to their initial positions and recreates every Pac-Dot and Power Pellet.
        /// </summary>
        private void ReloadLevel()
        {
            string[] file = File.ReadAllLines($@"{Content.RootDirectory}\board.txt");

            int width = file[0].Length;
            int height = file.Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (file[y][x])
                    {
                        case ' ':
                            PacDot dot = new PacDot(this, new Point(x, y));

                            _pacDots.Add(dot);
                            Components.Add(dot);
                            break;
                        case 'P':
                            PowerPellet pellet = new PowerPellet(this, new Point(x, y));

                            _powerPellets.Add(pellet);
                            Components.Add(pellet);
                            break;
                    }
                }
            }

            foreach (Player player in _players)
                player.ResetPosition();

            foreach (Enemy enemy in _enemies)
                enemy.ResetPosition();
        }

        /// <summary>
        /// Read High Score from file if available.
        /// If not High Score will be set to 0.
        /// </summary>
        private void LoadHighScore()
        {
            try
            {
                string[] file = File.ReadAllLines($@"{Content.RootDirectory}\highscore.txt");

                if (file.Length != 0 && !int.TryParse(file[0], out _highScore))
                {
                    _highScore = 0;
                }
            }
            catch (FileNotFoundException)
            {
                _highScore = 0;
            }
        }

        /// <summary>
        /// Save new High Score in file.
        /// </summary>
        /// <param name="newHighScore">New High Score to save.</param>
        private void SaveHighScore(int newHighScore)
        {
            File.WriteAllText($@"{Content.RootDirectory}\highscore.txt", newHighScore.ToString());
        }

        /// <summary>
        /// Handles The Random Spawns of fruits when the player score is added 1200 points
        /// </summary>
        private void SpawnFruit()
        {
            int type;
            List<Point> availablePositions = new List<Point>();

            string[] file = File.ReadAllLines($@"{Content.RootDirectory}\board.txt");

            int width = file[0].Length;
            int height = file.Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (file[y][x])
                    {
                        case ' ':
                            availablePositions.Add(new Point(x, y));
                            break;
                    }
                }
            }

            int chance = Rnd.Next(0, 100);
            if (chance <= 50)
            {
                type = 0;
            }
            else if (chance <= 75)
            {
                type = 1;
            }
            else if (chance <= 90)
            {
                type = 2;
            }
            else if (chance <= 98)
            {
                type = 3;
            }
            else 
            {
                type = 4;
            }

            int index = Rnd.Next(availablePositions.Count);
            Point position = availablePositions[index];

            PacDot dot = PacDots.FirstOrDefault(p => p.Position == position);

            if (dot != null)
            {
                PacDots.Remove(dot);
                Components.Remove(dot);
            }

            Fruit fruit = new Fruit(this, position, type);
            Fruits.Add(fruit);
            Components.Add(fruit);
        }
    }
}