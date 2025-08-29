using PacMan.Entities;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public Map Map { get; set; }
        private Renderer renderer = null;
        public GameManager gameManager = new GameManager(50);
        private Stopwatch gameUpdateWatch = Stopwatch.StartNew();
        private Stopwatch animationWatch = Stopwatch.StartNew();

        private Queue<string> pathToMaps = new Queue<string>();

        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(MainScreen);
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);

            gameUpdateWatch.Start();
            animationWatch.Start();
            CompositionTarget.Rendering += UpadeUI;
        }

        public void UpadeUI(object sender, EventArgs e)
        {
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

            MainScreen.Source = renderer.Render(Map, gameManager.objects);
            if (gameUpdateWatch.ElapsedTicks > 350000)
            {
                gameManager.Update(sender, e);
                gameUpdateWatch.Restart();
            }

            if (animationWatch.ElapsedTicks > 2000000)
            {
                gameManager.UpdateAnimations();
                animationWatch.Restart();
            }
        }

        private void OverlayCanvas_InsertObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            string entity = AddObjectsBox.Text;
            Entity newEntity;

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

            // TODO: Predelat tak aby se nevytvarel screenelement pro kazdou entitu ale nebyly duplicitni
            newEntity.Coordinates = new Coordinates() { X = column, Y = row };
            newEntity.ImagePath = PathToEntityImage.Text;
            var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, newEntity.ImagePath);
            newScreenElement.LoadImage();

            newEntity.CreateGameObject(newScreenElement);
            gameManager.objects[row, column] = newEntity.gameObject;

            var actualPosition = Map.map[row, column];

            if (actualPosition == null)
                Map.map[row, column] = newEntity;
        }

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
        }

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

            StartButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
            StartLabel.Visibility = Visibility.Visible;
            LoadLabel.Visibility = Visibility.Visible;
            MapEditorLabel.Visibility = Visibility.Visible;
        }

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

        private void OverlayCanvas_DeleteObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            if (Map.map[row, column] != null)
                Map.map[row, column] = null;
        }

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

        private void DefaultBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Map.ChangeBackgoundSource("../../../images/background.png");
        }

        private void SaveMapButton_Click(object sender, RoutedEventArgs e)
        {
            Map.SaveUserMap();
        }

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

        private void LoadMap(Map map)
        {
            var mapFolder = Map.PathToMap;
            Map = map;
            Map.PathToMap = mapFolder;
         
            ClearMap();
            GameManager.instance.Reset();

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

        //TODO: Opravit, odstranuje pouze dynamicke objekty
        private void ClearMap()
        {
            int rows = gameManager.objects.GetLength(0);
            int columns = gameManager.objects.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    gameManager.objects[i, j] = null;
                }
            }
        }

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameManager.isGameOff && !gameManager.isGamePaused)
            {
                gameManager.RegisterEvent(e.Key);
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.isGameOff = true;
            gameManager.Reset();
            ResetMap();
            UpadeUI(sender, e);
            CountCookies();
            gameManager.isGameOff = false;

        }

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

        private void ClearMapButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMap();
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);
        }

        public void LoadJsonLevels()
        {
            if (string.IsNullOrWhiteSpace(Map.PathToMap) || !File.Exists(Map.PathToMap))
                return; 

            var folderPath = Path.GetDirectoryName(Map.PathToMap);
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return;

            string currentFileName = Path.GetFileName(Map.PathToMap);

            var maps = Directory.GetFiles(folderPath, "*.json")
                .Where(f => !string.Equals(Path.GetFileName(f), currentFileName, StringComparison.OrdinalIgnoreCase)) // vyrazeni soucasne cesty
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            pathToMaps = new Queue<string>(maps);
        }
    }
}