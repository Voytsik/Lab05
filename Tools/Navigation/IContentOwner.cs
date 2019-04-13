using System.Windows.Controls;

namespace Lab05TaskManager.Tools.Navigation
{
    interface IContentOwner
    {
        ContentControl ContentControl { get; }
    }
}
