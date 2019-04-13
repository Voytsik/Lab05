using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Lab05TaskManager.Models;
using Lab05TaskManager.Tools;
using Lab05TaskManager.Tools.Managers;

namespace Lab05TaskManager.ViewModels
{
    class ProcessListViewModel : BaseViewModel
    {
        #region Fields

        // Dictionary of processes (can be made by HashSet by override hashCode)
        private static readonly ConcurrentDictionary<int, TaskProcess> _processes =
            new ConcurrentDictionary<int, TaskProcess>();

        private KeyValuePair<int, TaskProcess> _selectedProcess;

        private Thread _currentThread;
        private Thread _anotherThread;

        // ViewList
        private readonly CollectionViewSource _collection = new CollectionViewSource();

        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;

        #region Commands

        private RelayCommand<object> _killCommand;
        private RelayCommand<object> _openCommand;

        #endregion

        #endregion

        #region Constructors

        public ProcessListViewModel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Upload();
            StartWorkingThread();
            StationManager.StopThreads += StopThread;
            ViewListOfTasks.Source = _processes;
        }

        #endregion

        #region Properties

        public CollectionViewSource ViewListOfTasks
        {
            get
            {
                KeyValuePair<int, TaskProcess> pair = _selectedProcess;
                _collection?.View?.Refresh();
                SelectedProcess = pair;
                return _collection;
            }
        }

        public KeyValuePair<int, TaskProcess> SelectedProcess
        {
            set
            {
                _selectedProcess = value;

                OnPropertyChanged();
                OnPropertyChanged($"getThreads");
                OnPropertyChanged($"getModules");
            }
            get => _selectedProcess;
        }

        #region CommandsProperties

        public RelayCommand<object> OpenCommand =>
            _openCommand ?? (_openCommand = new RelayCommand<object>(
                o => OpenFilePath(), o => CanExecuteCommand()));

        public RelayCommand<object> KillCommand =>
            _killCommand ?? (_killCommand = new RelayCommand<object>(
                o => KillThread(), o => CanExecuteCommand()));

        #endregion

        #endregion

        #region PrivateMethods

        private void StartWorkingThread()
        {
            _currentThread = new Thread(WorkingThreadList);
            _anotherThread = new Thread(WorkingThreadInList);
            _currentThread.Start();
            _anotherThread.Start();
        }

        private void WorkingThreadList()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var processesO = new HashSet<int>(_processes.Keys);
                Process[] processesN = Process.GetProcesses();

                foreach (Process process in processesN)
                {
                    _processes.GetOrAdd(process.Id, new TaskProcess(process));
                    processesO.Remove(process.Id);
                    if (_cancellationToken.IsCancellationRequested)
                        return;
                }

                foreach (var oldOne in processesO)
                {
                    _processes.TryRemove(oldOne, out _);
                    if (_cancellationToken.IsCancellationRequested)
                        return;
                }
                OnPropertyChanged(nameof(ViewListOfTasks));

                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);

                    if (_cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }

        private void WorkingThreadInList()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                foreach (var process in _processes.Values)
                {
                    process.Update();
                    if (_cancellationToken.IsCancellationRequested)
                        return;
                }

                OnPropertyChanged(nameof(ViewListOfTasks));


                for (var j = 0; j < 4; j++)
                {
                    Thread.Sleep(500);

                    if (_cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }

        internal void StopThread()
        {
            _cancellationTokenSource.Cancel();
            _currentThread.Join(2000);
            _currentThread.Abort();
            _currentThread = null;
            _anotherThread.Join(2000);
            _anotherThread.Abort();
            _anotherThread = null;
        }

        private async void Upload()
        {
            LoaderManeger.Instance.ShowLoader();
            await Task.Run(() =>
            {
                Process[] processes = Process.GetProcesses();

                var processesO = new HashSet<int>(_processes.Keys);

                foreach (var process in processes)
                {
                    _processes.GetOrAdd(process.Id, new TaskProcess(process));
                    processesO.Remove(process.Id);
                    if (_cancellationToken.IsCancellationRequested)
                        return;
                }

                foreach (var o in processesO)
                {
                    _processes.TryRemove(o, out _);
                    if (_cancellationToken.IsCancellationRequested)
                        return;
                }

                OnPropertyChanged($"ViewListOfTasks");
            });

            LoaderManeger.Instance.HideLoader();
        }

        private bool CanExecuteCommand()
        {
            return (SelectedProcess.Value?.Path != null);
        }

        #endregion

        #region CommandsImpl

        private async void OpenFilePath()
        {
            LoaderManeger.Instance.ShowLoader();
            await Task.Run((() =>
            {
                if (String.IsNullOrWhiteSpace(SelectedProcess.Value.Path) || SelectedProcess.Value.Path.Equals("Acces denied"))
                {
                    MessageBox.Show("Access denied");
                    return;
                }

                string argument = "/select, \"" + SelectedProcess.Value.Path + "\"";
                Process.Start("explorer.exe", argument);
            }), _cancellationToken);
            LoaderManeger.Instance.HideLoader();
        }

        private async void KillThread()
        {
            LoaderManeger.Instance.ShowLoader();
            await Task.Run(() =>
            {
                try
                {
                    SelectedProcess.Value.Kill();
                    OnPropertyChanged($"ViewListOfTasks");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Access denied" + e.Message);
                }
            });
            LoaderManeger.Instance.HideLoader();
        }

        #endregion
    }
}