﻿using System;

namespace Lab05TaskManager.Tools.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;

        internal static void CloseApp()
        {
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}