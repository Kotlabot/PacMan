using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Map Map { get; set; }
        private Renderer renderer = null;
        private List<GameObject> objects = new List<GameObject>();
        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(MainScreen, "../../../images/background.png");
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);
            MainScreen.Source = renderer.Render(objects);

        }

        private void OverlayCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            double cellWidth = OverlayCanvas.ActualWidth / Map.TileSizeWidth;
            double cellHeight = OverlayCanvas.ActualHeight / Map.TileSizeHeight;

            int column = (int)(clickPoint.X / cellWidth);
            int row = (int)(clickPoint.Y / cellHeight);

            MessageBox.Show($"Clicked on cell: Row {row}, Column {column}");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var CookieElement = new ScreenElement(30, 30, "../../../images/cookie.png");
            CookieElement.LoadImage();
            var CookieCoordinates = new Coordinates() { X = 0, Y = 0 };
            var PacmanElement = new ScreenElement(30, 30, "../../../images/pacman.png");
            PacmanElement.LoadImage();
            var PacmanCoordinates = new Coordinates() { X = 50, Y = 50};
            var CookieObject = new GameObject(CookieElement, CookieCoordinates);
            var PacmanObject = new GameObject(PacmanElement, PacmanCoordinates);

            objects.Add(CookieObject);
            objects.Add(PacmanObject);

            MainScreen.Source = renderer.Render(objects);

        }

        private void MapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Visibility = Visibility.Collapsed;
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;

            OverlayCanvas.Visibility = Visibility.Visible;
        }
    }
}