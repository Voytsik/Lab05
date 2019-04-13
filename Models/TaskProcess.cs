using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.VisualBasic.Devices;

namespace Lab05TaskManager.Models
{
    public class TaskProcess : INotifyPropertyChanged
    {
        #region Fields

        private readonly Process _process;
        private readonly PerformanceCounter _cpuCounter;

        private long _workingSet64;
        private float _cpu;

        private readonly DateTime _startDateTime;

        private double _memoryPercent = -1;
        private long _turningOn = -1;

        #endregion

        #region Commands

        public void Kill()
        {
            try
            {
                _process.Kill();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Update()
        {
            _workingSet64 = _process.WorkingSet64;
            try
            {
                if (_turningOn == -1)
                {
                    _turningOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _cpu = 0;
                    _cpuCounter.NextValue();
                }
                else if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _turningOn > 1000)
                {
                    _turningOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _cpu = _cpuCounter.NextValue() / Environment.ProcessorCount;
                }

            }
            catch (Exception)
            {
                // ignored
            }

            OnPropertyChanged($"MemoryWorkingSet64");
            OnPropertyChanged($"Ram");
            OnPropertyChanged($"Cpu");

        }

        #endregion

        #region Constructors

        public TaskProcess(Process process)
        {

            _process = process;
            Id = process.Id;
            Name = process.ProcessName;

            try
            {
                
                Path = process.MainModule.FileName;
            }
            catch (Exception)
            {
                Path = "Acces denied";
            }
           

            _cpuCounter =
                new PerformanceCounter("Process", "% Processor Time", process.ProcessName, process.MachineName);
            _memoryPercent = 100.0 / (new ComputerInfo()).TotalPhysicalMemory;

            try
            {
                _startDateTime = _process.StartTime;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }

        public string Cpu => _cpu.ToString("0.00")+"%";

        public long MemoryWorkingSet64 => _workingSet64;

        public string Ram => (_workingSet64 * _memoryPercent).ToString("0.00")+"%";

        public int CountThreads => _process?.Threads.Count ?? 0;


        public bool IsActive => _process.Responding;
        
        public DateTime StartDateTime => _startDateTime;

        public ProcessModuleCollection GetModules
        {
            get
            {
                try
                {
                    ProcessModuleCollection arr = _process?.Modules;
                    return arr;
                }

                catch (Exception)
                {
                    return null;
                }

                
            }
            
        }

        public ProcessThreadCollection GetThreads
        {
            get
            {
                try
                {
                    ProcessThreadCollection arr = _process?.Threads;

                    return arr;
                }
                catch (Exception)
                {
                    return null;
                }
                
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}