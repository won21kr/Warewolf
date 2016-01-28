﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dev2.Common.Interfaces.Monitoring;

namespace Dev2.Diagnostics.PerformanceCounters
{
    public class WarewolfAverageExecutionTimePerformanceCounter:IPerformanceCounter
    {
       
        private PerformanceCounter _counter;
        private PerformanceCounter _baseCounter;
        private bool _started;
        private readonly WarewolfPerfCounterType _perfCounterType;

        public WarewolfAverageExecutionTimePerformanceCounter()
        {
            _started = false;
            IsActive = true;
            _perfCounterType = WarewolfPerfCounterType.AverageExecutionTime;
        }

        public WarewolfPerfCounterType PerfCounterType
        {
            get
            {
                return _perfCounterType;
            }
        }

        public IList<CounterCreationData> CreationData()
        {
          
            CounterCreationData totalOps = new CounterCreationData
            {
                CounterName = Name,
                CounterHelp = Name,
                CounterType = PerformanceCounterType.AverageTimer32,
                
            };
            CounterCreationData avgDurationBase = new CounterCreationData
            {
                CounterName = "average time per operation base",
                CounterHelp = "Average duration per operation execution base",
                CounterType = PerformanceCounterType.AverageBase
            };

            return new[] { totalOps ,avgDurationBase};
        
        }

        public bool IsActive { get; set; }

        #region Implementation of IPerformanceCounter

        public void Increment()
        {
            
            Setup();
            if (IsActive)
            {
                _counter.Increment();
                _baseCounter.Increment();
            }
        }

        public void IncrementBy(long ticks)
        {
           
            Setup();
            if (IsActive)
            {
                _counter.IncrementBy(ticks);
                _baseCounter.Increment();
            }
        }

        private void Setup()
        {
            if (!_started)
            {
                _counter = new PerformanceCounter("Warewolf", Name)
                {
                    MachineName = ".",
                    ReadOnly = false,
                    
                };
                _baseCounter = new PerformanceCounter("Warewolf", "average time per operation base")
                {
                    MachineName = ".",
                    ReadOnly = false,

                };
                _started = true;
            }
        }

        public void Decrement()
        {
            Setup();
            if (IsActive)
            {
                _counter.Decrement();
                _baseCounter.Decrement();
            }
        }

        public string Category
        {
            get
            {
                return "Warewolf";
            }
        }
        public string Name
        {
            get
            {
                return "Average workflow execution time";
            }
        }

        #endregion
    }
}