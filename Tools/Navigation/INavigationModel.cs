namespace Lab05TaskManager.Tools.Navigation
{
    internal enum ViewType
    {
        ProcessList
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType);

        void DeNavigate(ViewType viewType);
    }
}
