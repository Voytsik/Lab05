using System.Windows.Controls;
using System.Windows.Input;
using Lab05TaskManager.Tools.Navigation;
using Lab05TaskManager.ViewModels;

namespace Lab05TaskManager.Views
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class TaskListView : UserControl,INavigatable
    {
        public TaskListView()
        {
            InitializeComponent();
            DataContext = new ProcessListViewModel();
        }
    }
}
