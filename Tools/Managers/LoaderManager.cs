using System.Windows;

namespace Lab05TaskManager.Tools.Managers
{
    internal class LoaderManeger
    {
        private static readonly object Locker = new object();
        private static LoaderManeger _instance;

        internal static LoaderManeger Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (Locker)
                {
                    return _instance ?? (_instance = new LoaderManeger());
                }
            }
        }

        private ILoaderOwner _loaderOwner;

        private LoaderManeger()
        {
        }

        internal void Initialize(ILoaderOwner loaderOwner)
        {
            _loaderOwner = loaderOwner;
        }

        internal void ShowLoader()
        {
            _loaderOwner.LoaderVisibility = Visibility.Visible;
            _loaderOwner.IsControlEnabled = false;
        }

        internal void HideLoader()
        {
            _loaderOwner.LoaderVisibility = Visibility.Hidden;
            _loaderOwner.IsControlEnabled = true;
        }
    }
}