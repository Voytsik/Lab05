using System.ComponentModel;
using System.Windows.Controls;
using Lab05TaskManager.Tools.Managers;
using Lab05TaskManager.Tools.Navigation;
using Lab05TaskManager.ViewModels;


namespace Lab05TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IContentOwner
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            InitializeApp();
        }

        private void InitializeApp()
        {
            NavigationManager.Instance.Initialize(new InitializationNavigationModel(this));
            NavigationManager.Instance.Navigate(ViewType.ProcessList);
        }


        public ContentControl ContentControl => _contentControl;

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StationManager.CloseApp();
        }
    }
}