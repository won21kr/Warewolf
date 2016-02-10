using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Dev2.Common;
using Dev2.Common.Interfaces.Core.Graph;
using Dev2.Common.Interfaces.DB;
using Dev2.Common.Interfaces.ToolBase;
using Dev2.Communication;
using Dev2.Studio.Core.Activities.Utils;


namespace Dev2.Activities.Designers2.Core
{
    public class OutputsRegion : IOutputsToolRegion
    {
        private readonly ModelItem _modelItem;
        private double _minHeight;
        private double _currentHeight;
        private bool _isVisible;
        private double _maxHeight;
        private const double BaseHeight = 60;
        private ICollection<IServiceOutputMapping> _outputs;
        public OutputsRegion(ModelItem modelItem)
        {
            _modelItem = modelItem;
            if (_modelItem.GetProperty("Outputs") == null)
            {
                var current = _modelItem.GetProperty<ICollection<IServiceOutputMapping>>("Outputs");
                if(current == null)
                {
                    IsVisible = false;
                }
                var outputs = new ObservableCollection<IServiceOutputMapping>(current ?? new List<IServiceOutputMapping>());
                outputs.CollectionChanged += outputs_CollectionChanged;
                Outputs = outputs;
                SetInitialHeight();
            }
            else
            {
                IsVisible = true;
                var outputs = new ObservableCollection<IServiceOutputMapping>(_modelItem.GetProperty<ICollection<IServiceOutputMapping>>("Outputs"));
                outputs.CollectionChanged += outputs_CollectionChanged;
                Outputs = outputs;
                ReCalculateHeight();

            }
           
        }

        void SetInitialHeight()
        {
            MaxHeight = BaseHeight;
            MinHeight = BaseHeight;
            CurrentHeight = BaseHeight;
        }

        public OutputsRegion()
        {
            SetInitialHeight();
        }

        void outputs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReCalculateHeight();
            _modelItem.SetProperty("Outputs", _outputs.ToList());
            OnPropertyChanged("IsOutputsEmptyRows");
        }
        private bool _outputMappingEnabled;
        private string _recordsetName;
        private IOutputDescription _description;
        private bool _isOutputsEmptyRows;

        #region Implementation of IToolRegion

        public double MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                _minHeight = value;
                OnPropertyChanged();
            }
        }
        public double CurrentHeight
        {
            get
            {
                return _currentHeight;
            }
            set
            {
                _currentHeight = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnHeightChanged(this);
                OnPropertyChanged();
            }
        }
        public double MaxHeight
        {
            get
            {
                return _maxHeight;
            }
            set
            {
                _maxHeight = value;
                OnPropertyChanged();
            }
        }
        public event HeightChanged HeightChanged;
        public IList<IToolRegion> Dependants { get; set; }

        public IToolRegion CloneRegion()
        {
            var ser = new Dev2JsonSerializer();
            return ser.Deserialize<IToolRegion>(ser.SerializeToBuilder(this));
        }

        public void RestoreRegion(IToolRegion toRestore)
        {
            var region = toRestore as OutputsRegion;
            if (region != null)
            {
                MaxHeight = region.MaxHeight;
                MinHeight = region.MinHeight;
                IsVisible = region.IsVisible;
                CurrentHeight = region.CurrentHeight;
                Outputs = region.Outputs;
                RecordsetName = region.RecordsetName;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("IsOutputsEmptyRows");
            }
        }

        #endregion

        #region Implementation of IOutputsToolRegion

        public ICollection<IServiceOutputMapping> Outputs
        {
            get
            {
                return _outputs;
            }
            set
            {
                _outputs = value;
                _modelItem.SetProperty("Outputs", value.ToList());
                ReCalculateHeight();
                OnHeightChanged(this);
                OnPropertyChanged();
            }
        }

        private void ReCalculateHeight()
        {
            MaxHeight = BaseHeight + _outputs.Count * GlobalConstants.RowHeight;
            if(_outputs.Count == 0)
            {
                MaxHeight = 0;
            }
            if(_outputs.Count >= 3)
            {
                MinHeight = 110;
            }
            IsOutputsEmptyRows = _outputs == null || _outputs.Count == 0;
            CurrentHeight = MinHeight;
        }

        public bool OutputMappingEnabled
        {
            get
            {
                return _outputMappingEnabled;
            }
            set
            {
                _outputMappingEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsOutputsEmptyRows
        {
            get
            {
                return _isOutputsEmptyRows;
            }
            set
            {
                _isOutputsEmptyRows = value;
                OnPropertyChanged();
            }
        }
        public string RecordsetName
        {
            get
            {
                return _recordsetName;
            }
            set
            {
                _recordsetName = value;
                OnPropertyChanged();
            }
        }
        public IOutputDescription Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                _modelItem.SetProperty("OutputDescription",value);
                OnPropertyChanged();
            }
        }

        public IList<string> Errors
        {
            get
            {
                return Outputs.Where(a => WarewolfDataEvaluationCommon.ParseLanguageExpressionWithoutUpdate(a.MappedTo).IsComplexExpression || WarewolfDataEvaluationCommon.ParseLanguageExpressionWithoutUpdate(a.MappedTo).IsWarewolfAtomAtomExpression).Select(a => "Invalid Output Mapping" + a.ToString()).ToList();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnHeightChanged(IToolRegion args)
        {
            var handler = HeightChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}