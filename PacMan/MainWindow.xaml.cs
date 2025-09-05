
using PacMan.Entities;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        // Current static map instance containing layout, entities, and background.
        public Map Map { get; set; }

        // Responsible for drawing map tiles and game objects onto the screen.
        private Renderer renderer = null;

        // Controller for game state with fixed update rate, containing dynamic map (two-dimensional array)
        // with game objects and other properties important for proper game flow.
        public GameManager gameManager = new GameManager(50);

        // Stopwatch to control the interval for updating game logic.
        private Stopwatch gameUpdateWatch = Stopwatch.StartNew();

        // Stopwatch to control the interval for updating animations.
        private Stopwatch animationWatch = Stopwatch.StartNew();

        // Queue for all JSON levels (maps) in one folder (same folder as selected map). 
        private Queue<string> pathToMaps = new Queue<string>();

        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(MainScreen);

            Map = new Map(MainScreen.Width, MainScreen.Height, Convert.ToDouble(TileSize.Text), Convert.ToDouble(TileSize.Text)); 

            gameUpdateWatch.Start();
            animationWatch.Start();
            CompositionTarget.Rendering += UpadeUI; // Attach game loop to rendering event
        }

        /// <summary>
        /// Handles main game loop. Updates the UI, checks win/loss conditions,
        /// processes animations, and renders the map.
        /// </summary>
        public void UpadeUI(object sender, EventArgs e)
        {
            // Check if all available levels are won, if not than dequeue next level (JSON map).
            if (gameManager.isGameWon)
            {
                if(pathToMaps.Count == 0) 
                {
                    MessageBox.Show("All levels won!");
                    gameManager.isGameWon = false;
                    return;
                }

                var newMap = Map.LoadUserMap(pathToMaps.Dequeue());
                if (newMap != null)
                {
                    LoadMap(newMap);
                }
                gameManager.isGameWon = false;
            }

            // Check if game is off and if so than reset objects based on map. This way the render is updated every frame,
            // so if you add an object to map, the change will be immediately visible.
            if (gameManager.isGameOff)
            {
                for (int row = 0; row < Map.map.GetLength(0); row++)
                {
                    for (int col = 0; col < Map.map.GetLength(1); col++)
                    {
                        if (Map.map[row, col] == null)
                            gameManager.objects[row, col] = null;
                        else
                            gameManager.objects[row, col] = Map.map[row, col].gameObject;
                    }
                }
            }

            // Render the map and entities.
            MainScreen.Source = renderer.Render(Map, gameManager.objects);

            //Update game logic at fixed intervals (best possible value was discovered when debugging)
            if (gameUpdateWatch.ElapsedTicks > 350000)
            {
                gameManager.Update(sender, e);
                gameUpdateWatch.Restart();
            }

            //Update animations at fixed intervals (best possible value was discovered when debugging)
            if (animationWatch.ElapsedTicks > 2000000)
            {
                gameManager.UpdateAnimations();
                animationWatch.Restart();
            }
        }

        /// <summary>
        /// Handles inserting a new game entity (Pacman, Cookie, Monster...) into the map
        /// when the user clicks on the overlay canvas in map editor mode.
        /// </summary>
        private void OverlayCanvas_InsertObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            // Get correct position in grid by dividing clickpoint by measurements of tiles in grid.
            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            string entity = AddObjectsBox.Text;
            Entity newEntity;

            // Decide which entity to create based on selected (by user) object in ComboBox.
            switch (entity)
            {
                case "Wall":
                    newEntity = new Wall();
                    break;

                case "Cookie":
                    newEntity = new Cookie();
                    break;

                case "Super Cookie":
                    newEntity = new SuperCookie();
                    break;

                case "Monster":
                    newEntity = new Monster();
                    break;

                case "Super Monster":
                    newEntity = new SuperMonster();
                    break;

                case "Pacman":
                    newEntity = new Pacman();
                    break;

                default:
                    MessageBox.Show("Please select an item!");
                    return;
            }

            // Place the entity in the selected tile by creating its coordinates according to tile selected. 
            newEntity.Coordinates = new Coordinates() { X = column, Y = row };
            // Setting its image path according to path in TextBox (default or new used by user).
            newEntity.ImagePath = PathToEntityImage.Text;
            // Creating and displaying screen element.
            var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, newEntity.ImagePath);
            newScreenElement.LoadImage();
            // And finally creating a game object and assigning it to correct place in the dynamic game object map.
            newEntity.CreateGameObject(newScreenElement);
            gameManager.objects[row, column] = newEntity.gameObject;

            // Add new entity also to static map.
            var actualPosition = Map.map[row, column];
            if (actualPosition == null)
                Map.map[row, column] = newEntity;
        }

        /// <summary>
        /// Handles removing an entity from the map editor when a user clicks on the overlay canvas.
        /// </summary>
        private void OverlayCanvas_DeleteObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            if (Map.map[row, column] != null)
                Map.map[row, column] = null;
        }

        /// <summary>
        /// Starts a new game. Resets state, hides menu buttons, and shows game buttons.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartButton_Click(sender, e);
            gameManager.isGameOff = false;

            StartButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;
            StartLabel.Visibility = Visibility.Collapsed;
            LoadLabel.Visibility = Visibility.Collapsed;
            MapEditorLabel.Visibility = Visibility.Collapsed;

            PauseButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            RestartButton.Visibility = Visibility.Visible;
            QuitLabel.Visibility = Visibility.Visible;
            RestartLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens the map editor mode, hides the main menu and displayes editing controls.
        /// </summary>
        private void MapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;
            StartLabel.Visibility = Visibility.Collapsed;
            LoadLabel.Visibility = Visibility.Collapsed;
            MapEditorLabel.Visibility = Visibility.Collapsed;
            

            OverlayCanvas.Visibility = Visibility.Visible;
            AddObjectsBox.Visibility = Visibility.Visible;
            QuitMapEditorButton.Visibility = Visibility.Visible;
            SaveMapButton.Visibility = Visibility.Visible;
            LoadBackgroundButton.Visibility = Visibility.Visible;
            Label1.Visibility = Visibility.Visible;
            PathToEntityImage.Visibility = Visibility.Visible;
            Label2.Visibility = Visibility.Visible;
            PathToBackgroundImage.Visibility = Visibility.Visible;
            DefaultBackgroundButton.Visibility = Visibility.Visible;
            ClearMapButton.Visibility = Visibility.Visible;
            TileSize.Visibility = Visibility.Visible;
            TileSizeLabel.Visibility = Visibility.Visible;
            ChangeTileSize.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Closes the map editor interface and returns to the main menu.
        /// </summary>
        private void QuitMapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Visibility = Visibility.Collapsed;
            AddObjectsBox.Visibility = Visibility.Collapsed;
            QuitMapEditorButton.Visibility = Visibility.Collapsed;
            SaveMapButton.Visibility = Visibility.Collapsed;
            Label1.Visibility = Visibility.Collapsed;
            PathToEntityImage.Visibility = Visibility.Collapsed;
            Label2.Visibility = Visibility.Collapsed;
            PathToBackgroundImage.Visibility = Visibility.Collapsed;
            LoadBackgroundButton.Visibility = Visibility.Collapsed;
            DefaultBackgroundButton.Visibility = Visibility.Collapsed;
            ClearMapButton.Visibility = Visibility.Collapsed;
            TileSize.Visibility = Visibility.Collapsed;
            TileSizeLabel.Visibility = Visibility.Collapsed;
            ChangeTileSize.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
            StartLabel.Visibility = Visibility.Visible;
            LoadLabel.Visibility = Visibility.Visible;
            MapEditorLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Updates entity image path preview in the editor when a new object type is selected from the dropdown menu.
        /// </summary>
        private void AddObjectsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (AddObjectsBox.SelectedIndex)
            {
                case 1:
                    PathToEntityImage.Text = "../../../images/wallOrange.png";
                    break;

                case 2:
                    PathToEntityImage.Text = "../../../images/cookie.png";
                    break;

                case 3:
                    PathToEntityImage.Text = "../../../images/superCookie.png";
                    break;

                case 4:
                    PathToEntityImage.Text = "../../../images/monster"; 
                    break;

                case 5:
                    PathToEntityImage.Text = "../../../images/superMonster";
                    break;

                case 6:
                    PathToEntityImage.Text = "../../../images/pacman";
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// Loads a custom background image from user input if it's valid.
        /// </summary>
        private void LoadBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            string userPath = PathToBackgroundImage.Text;
            if (File.Exists(userPath))
            {
                Map.ChangeBackgoundSource(userPath);
            }
            else
                MessageBox.Show("File does not exist.");
        }

        /// <summary>
        /// Resets the background to the default image.
        /// </summary>
        private void DefaultBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Map.ChangeBackgoundSource("../../../images/background.png");
        }

        /// <summary>
        /// Saves the current map created in the editor to a JSON file.
        /// </summary>
        private void SaveMapButton_Click(object sender, RoutedEventArgs e)
        {
            Map.SaveUserMap();
        }

        /// <summary>
        /// Loads a map from JSON file and initializes level progression queue.
        /// </summary>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var newMap = Map.LoadUserMap();
            if (newMap != null)
            {
                LoadMap(newMap);
                LoadJsonLevels();
            }
            else
                MessageBox.Show("Error in loading map.");
        }

        /// <summary>
        /// Loads all available JSON level files from the same directory as the current map
        /// and enqueues them for sequential play.
        /// </summary>
        public void LoadJsonLevels()
        {
            if (string.IsNullOrWhiteSpace(Map.PathToMap) || !File.Exists(Map.PathToMap))
                return;

            var folderPath = Path.GetDirectoryName(Map.PathToMap);
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return;

            string currentFileName = Path.GetFileName(Map.PathToMap);

            var maps = Directory.GetFiles(folderPath, "*.json")
                .Where(f => !string.Equals(Path.GetFileName(f), currentFileName, StringComparison.OrdinalIgnoreCase)) // Exclude current file.
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            pathToMaps = new Queue<string>(maps);
        }

        /// <summary>
        /// Loads a given map into the game, resets objects and applyes background.
        /// </summary>
        private void LoadMap(Map map)
        {
            var mapFolder = Map.PathToMap;
            Map = map;
            Map.PathToMap = mapFolder;
         
            gameManager.ClearMap();
            gameManager.Reset();

            foreach (var item in Map.map)
            {
                if (item != null)
                {
                    var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, item.ImagePath);
                    if (!newScreenElement.LoadImage())
                    {
                        MessageBox.Show("Error in loading image.");
                        return;
                    }

                    item.CreateGameObject(newScreenElement);
                    gameManager.objects[Convert.ToInt32(item.Coordinates.Y), Convert.ToInt32(item.Coordinates.X)] = item.gameObject;
                }
            }
            Map.ChangeBackgoundSource(Map.PathToBackground);
        }

        /// <summary>
        /// Quits the current game and returns to the main menu.
        /// </summary>
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            RestartButton_Click(sender, e);
            gameManager.isGameOff = true;

            QuitButton.Visibility = Visibility.Collapsed;
            RestartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            QuitLabel.Visibility = Visibility.Collapsed;
            RestartLabel.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
            StartLabel.Visibility = Visibility.Visible;
            LoadLabel.Visibility = Visibility.Visible;
            MapEditorLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles pause/resume state of the game when pause button is clicked.
        /// </summary>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameManager.isGamePaused)
            {
                gameManager.isGamePaused = false;
                PauseButton.Content = "PAUSE";
            }
            else
            {
                gameManager.isGamePaused = true;
                PauseButton.Content = "RESUME";
            }
        }

        /// <summary>
        /// Restarts the game. Resets state, reloads the map, counts cookies and resumes gameplay.
        /// </summary>
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.isGameOff = true;
            gameManager.Reset();
            ResetMap();
            UpadeUI(sender, e);
            CountCookies();
            gameManager.isGameOff = false;

        }

        /// <summary>
        /// Resets the map by reinitialiting the map entities by recreating their game objects.
        /// </summary>
        public void ResetMap()
        {
            for (int row = 0; row < Map.map.GetLength(0); row++)
            {
                for (int col = 0; col < Map.map.GetLength(1); col++)
                {
                    if (Map.map[row, col] != null)
                    {
                        Map.map[row, col].CreateGameObject(Map.map[row, col].gameObject.Sprite);
                    }
                }
            }
        }

        /// <summary>
        /// Counts the number of cookies and super cookies currently on the map which
        /// determines the completion of the current level.
        /// </summary>
        private void CountCookies()
        {
            gameManager.CookiesCount = 0;
            gameManager.EatenCookies = 0;
            foreach (var entity in gameManager.objects)
            {
                if (entity != null && entity.Entity is Cookie or SuperCookie)
                {
                    gameManager.CookiesCount++;
                }
            }
        }

        /// <summary>
        /// Clears the current map in map editor mode and initializes a new empty one.
        /// </summary>
        private void ClearMapButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.ClearMap();
            TileSize.Text = "16";
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);
            gameManager.MapHeight = (int)Map.HeightTileCount;
            gameManager.MapWidth = (int)Map.WidthTileCount;
            gameManager.Reset();
        }

        /// <summary>
        /// Changes sizes of a tile selected by user, by creating new static map and new dynamic map (objects).
        /// </summary>
        private void ChangeTileSize_Click(object sender, RoutedEventArgs e)
        {
            if(double.TryParse(TileSize.Text, out var size))
            {
                Map = new Map(MainScreen.Width, MainScreen.Height, size, size);
                gameManager.MapHeight = (int)Map.HeightTileCount;
                gameManager.MapWidth = (int)Map.WidthTileCount;
                gameManager.Reset();
            }
        }
    }
}