using System;
using Lab05TaskManager.Views;

namespace Lab05TaskManager.Tools.Navigation
{
    internal class InitializationNavigationModel : BaseNavigationModel
    {
        public InitializationNavigationModel(IContentOwner contentOwner) : base(contentOwner)
        {

        }

        protected override void InitializeView(ViewType viewType)
        {
            switch (viewType)
            {
                  case ViewType.ProcessList:
                    ViewsDictionary.Add(viewType, new TaskListView());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
        }

        protected override void DeInitializeView(ViewType viewType)
        {
            ViewsDictionary.Remove(viewType);
        }
    }
}
