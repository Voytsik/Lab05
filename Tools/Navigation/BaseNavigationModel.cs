using System.Collections.Generic;

namespace Lab05TaskManager.Tools.Navigation
{
    internal abstract class BaseNavigationModel : INavigationModel
    {
        protected BaseNavigationModel(IContentOwner contentOwner)
        {
            ContentOwner = contentOwner;
            ViewsDictionary = new Dictionary<ViewType, INavigatable>();
        }

        protected IContentOwner ContentOwner { get; }

        protected Dictionary<ViewType, INavigatable> ViewsDictionary { get; }

        public void Navigate(ViewType viewType)
        {
            if (!ViewsDictionary.ContainsKey(viewType))
                InitializeView(viewType);
            ContentOwner.ContentControl.Content = ViewsDictionary[viewType];
        }

        public void DeNavigate(ViewType viewType)
        {
            if (!ViewsDictionary.ContainsKey(viewType))
                DeInitializeView(viewType);
            ContentOwner.ContentControl.Content = ViewsDictionary[viewType];

        }
        protected abstract void InitializeView(ViewType viewType);

        protected abstract void DeInitializeView(ViewType viewType);
    }
}
