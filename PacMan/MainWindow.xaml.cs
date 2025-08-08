using PacMan.Entities;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private DispatcherTimer timer = new DispatcherTimer();
        public GameManager gameManager = new GameManager();
        //private Task GameLoop;
        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(MainScreen);
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);

            //GameLoop = new Task(() =>
            //{
            //    while (true)
            //    {
            //        gameManager.Update();
            //        Render();
            //        Thread.Sleep(100);
            //    }
            //});

            //GameLoop.Start();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Render;
            timer.Tick += gameManager.Update;
            timer.Start();
        }

        public void Render(object sender, EventArgs e) 
        {
            if(gameManager.isGameOff)
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

                case "SuperCookie":
                    newEntity = new SuperCookie();
                    break;

                case "Monster": 
                    newEntity = new Monster();
                    break;

                case "SuperMonster":
                    newEntity = new SuperMonster();
                    break;

                case "MonsterSpawn": 
                    newEntity = new MonsterSpawn();
                    break;

                case "Pacman":
                    newEntity = new Pacman();
                    break;

                default:
                    MessageBox.Show("Please select an item!");
                    return;
            }

            newEntity.Coordinates = new Coordinates() { X = column, Y = row };
            newEntity.ImagePath = PathToEntityImage.Text;
            var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, newEntity.ImagePath);
            newScreenElement.LoadImage();

            newEntity.gameObject = new GameObject(newScreenElement, newEntity);
            gameManager.objects[row, column] = newEntity.gameObject;

            var actualPosition = Map.map[row, column];

            if (actualPosition == null)
                Map.map[row, column] = newEntity;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.isGameOff = false;

            StartButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;

            PauseButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            RestartButton.Visibility = Visibility.Visible;
        }

        private void MapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;

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
        }

        private void QuitMapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Visibility = Visibility.Collapsed;
            AddObjectsBox.Visibility = Visibility.Collapsed;
            QuitMapEditorButton.Visibility = Visibility.Collapsed;
            SaveMapButton.Visibility = Visibility.Collapsed;
            Label1.Visibility= Visibility.Collapsed;
            PathToEntityImage.Visibility = Visibility.Collapsed;
            Label2.Visibility= Visibility.Collapsed;
            PathToBackgroundImage.Visibility = Visibility.Collapsed;
            LoadBackgroundButton.Visibility= Visibility.Collapsed;
            DefaultBackgroundButton.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
        }

        private void AddObjectsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(AddObjectsBox.SelectedIndex)
            {
                case 1: 
                    PathToEntityImage.Text = "../../../images/wall.png";
                    break;

                case 2: PathToEntityImage.Text = "../../../images/cookie.png";
                    break;

                case 3: PathToEntityImage.Text = "../../../images/flower.png"; //change to supercookie
                    break;

                case 4: PathToEntityImage.Text = "../../../images/monsters.png"; 
                    break;

                case 5: PathToEntityImage.Text = "../../../images/huntMode.png";
                    break;

                case 6: PathToEntityImage.Text = "../../../images/flower.png"; //change to spawn
                    break;

                case 7: PathToEntityImage.Text = "../../../images/pacman.png";
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
            Map = Map.LoadUserMap();
            if (Map != null)
            {
                ClearMap();
                foreach (var item in Map.map)
                {
                    if (item != null)
                    {
                        var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, item.ImagePath);
                        newScreenElement.LoadImage();

                        item.gameObject = new GameObject(newScreenElement, item);
                        gameManager.objects[Convert.ToInt32(item.Coordinates.Y), Convert.ToInt32(item.Coordinates.X)] = item.gameObject;
                    }
                }
                Map.ChangeBackgoundSource(Map.PathToBackground);
            }
            else
                MessageBox.Show("Error in loading map.");
            
        }

        private void ClearMap()
        {
            int rows = gameManager.objects.GetLength(0);
            int columns = gameManager.objects.GetLength(1);

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    gameManager.objects[i, j] = null;
                }
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.isGameOff = true;

            QuitButton.Visibility = Visibility.Collapsed;
            RestartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            gameManager.isGameOff = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameManager.isGameOff)
            {
                gameManager.RegisterEvent(e.Key);
                //MessageBox.Show(e.Key.ToString());
            }
        }
    }
}