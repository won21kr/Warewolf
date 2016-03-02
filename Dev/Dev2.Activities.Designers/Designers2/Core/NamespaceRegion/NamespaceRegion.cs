﻿using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Dev2.Common.Interfaces;
using Dev2.Common.Interfaces.ToolBase;
using Dev2.Common.Interfaces.ToolBase.DotNet;
using Dev2.Studio.Core.Activities.Utils;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ExplicitCallerInfoArgument

namespace Dev2.Activities.Designers2.Core.NamespaceRegion
{
    public class DotNetNamespaceRegion : INamespaceToolRegion<INamespaceItem>
    {
        private readonly ModelItem _modelItem;
        private readonly ISourceToolRegion<IPluginSource> _source;
        private double _minHeight;
        private double _currentHeight;
        private double _maxHeight;
        private bool _isVisible;
        private const double BaseHeight = 25;

        readonly Dictionary<int, IList<IToolRegion>> _previousRegions = new Dictionary<int, IList<IToolRegion>>();
        private Action _sourceChangedNamespace;
        private INamespaceItem _selectedNamespace;
        private IPluginServiceModel _model;
        private ICollection<INamespaceItem> _namespaces;
        private bool _isRefreshing;
        private double _labelWidth;
        private int _namespaceId;
        private bool _isNamespaceEnabled;

        public DotNetNamespaceRegion()
        {
            ToolRegionName = "DotNetNamespaceRegion";
            SetInitialValues();
        }

        public DotNetNamespaceRegion(IPluginServiceModel model, ModelItem modelItem, ISourceToolRegion<IPluginSource> source)
        {
            LabelWidth = 70;
            ToolRegionName = "DotNetNamespaceRegion";
            _modelItem = modelItem;
            _model = model;
            _source = source;
            _source.SomethingChanged += SourceOnSomethingChanged;
            SetInitialValues();
            Dependants = new List<IToolRegion>();
            IsRefreshing = false;
            if (_source.SelectedSource != null)
            {
                Namespaces = model.GetNameSpaces(source.SelectedSource);
            }
            NamespaceId = modelItem.GetProperty<int>("NamespaceId");
            if (NamespaceId != 0)
            {
                SelectedNamespace = Namespaces.FirstOrDefault(action => source.GetHashCode() == NamespaceId);
            }

            RefreshNamespaceCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(() =>
            {
                IsRefreshing = true;
                if(_source.SelectedSource != null)
                {
                    Namespaces = model.GetNameSpaces(_source.SelectedSource);
                }
                IsRefreshing = false;
            }, CanRefresh);

            IsVisible = true;
            _modelItem = modelItem;
        }

        public double LabelWidth
        {
            get
            {
                return _labelWidth;
            }
            set
            {
                _labelWidth = value;
                OnPropertyChanged();
            }
        }

        private void SourceOnSomethingChanged(object sender, IToolRegion args)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            if (_source != null && _source.SelectedSource != null)
            {
                Namespaces = _model.GetNameSpaces(_source.SelectedSource);
                SelectedNamespace = null;
                IsNamespaceEnabled = true;
                IsVisible = true;
            }
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(@"IsVisible");
            OnHeightChanged(this);
        }

        private void SetInitialValues()
        {
            MinHeight = BaseHeight;
            MaxHeight = BaseHeight;
            CurrentHeight = BaseHeight;
            IsVisible = true;
        }

        public bool CanRefresh()
        {
            return SelectedNamespace != null;
        }

        public INamespaceItem SelectedNamespace
        {
            get
            {
                return _selectedNamespace;
            }
            set
            {
                if (!Equals(value, _selectedNamespace) && _selectedNamespace != null)
                {
                    if (!String.IsNullOrEmpty(_selectedNamespace.FullName))
                        StorePreviousValues(_selectedNamespace.FullName.GetHashCode());
                }

                if (IsAPreviousValue(value) && _selectedNamespace != null)
                {
                    RestorePreviousValues(value);
                    SetSelectedNamespace(value);
                }
                else
                {
                    SetSelectedNamespace(value);
                    SourceChangedNamespace();
                    OnSomethingChanged(this);
                }
                var delegateCommand = RefreshNamespaceCommand as Microsoft.Practices.Prism.Commands.DelegateCommand;
                if (delegateCommand != null)
                {
                    delegateCommand.RaiseCanExecuteChanged();
                }

                _selectedNamespace = value;
                OnPropertyChanged();
            }
        }
        public ICollection<INamespaceItem> Namespaces
        {
            get
            {
                return _namespaces;
            }
            set
            {
                _namespaces = value;
                OnPropertyChanged();
            }
        }
        public ICommand RefreshNamespaceCommand { get; set; }
        public bool IsNamespaceEnabled
        {
            get
            {
                return _isNamespaceEnabled;
            }
            set
            {
                _isNamespaceEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public Action SourceChangedNamespace
        {
            get
            {
                return _sourceChangedNamespace ?? (() => { });
            }
            set
            {
                _sourceChangedNamespace = value;
            }
        }
        public event SomethingChanged SomethingChanged;

        #region Implementation of IToolRegion

        public string ToolRegionName { get; set; }
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
            return new DotNetNamespaceRegion
            {
                MaxHeight = MaxHeight,
                MinHeight = MinHeight,
                IsVisible = IsVisible,
                SelectedNamespace = SelectedNamespace,
                CurrentHeight = CurrentHeight
            };
        }

        public void RestoreRegion(IToolRegion toRestore)
        {
            var region = toRestore as DotNetNamespaceRegion;
            if (region != null)
            {
                MaxHeight = region.MaxHeight;
                SelectedNamespace = region.SelectedNamespace;
                MinHeight = region.MinHeight;
                CurrentHeight = region.CurrentHeight;
                IsVisible = region.IsVisible;
            }
        }

        public int GetId()
        {
            return SelectedNamespace.FullName.GetHashCode();
        }

        #endregion

        #region Implementation of INamespaceToolRegion<INamespaceItem>

        private void SetSelectedNamespace(INamespaceItem value)
        {
            if (value != null)
            {
                _selectedNamespace = value;
                SavedNamespace = value;
                NamespaceId = value.GetHashCode();
            }

            OnHeightChanged(this);
            OnPropertyChanged("SelectedNamespace");
        }

        int NamespaceId
        {
            get
            {
                return _namespaceId;
            }
            set
            {
                _namespaceId = value;
                if (_modelItem != null)
                {
                    _modelItem.SetProperty("NamespaceId", value);
                }
            }
        }

        private void StorePreviousValues(int id)
        {
            _previousRegions.Remove(id);
            _previousRegions[id] = new List<IToolRegion>(Dependants.Select(a => a.CloneRegion()));
        }

        private void RestorePreviousValues(INamespaceItem value)
        {
            var toRestore = _previousRegions[value.FullName.GetHashCode()];
            foreach (var toolRegion in Dependants.Zip(toRestore, (a, b) => new Tuple<IToolRegion, IToolRegion>(a, b)))
            {
                toolRegion.Item1.RestoreRegion(toolRegion.Item2);
            }
        }

        private bool IsAPreviousValue(INamespaceItem value)
        {
            return _previousRegions.Keys.Any(a => a == value.FullName.GetHashCode());
        }

        public IList<string> Errors
        {
            get
            {
                return SelectedNamespace == null ? new List<string> { "Invalid Namespace Selected" } : new List<string>();
            }
        }

        public INamespaceItem SavedNamespace
        {
            get
            {
                return _modelItem.GetProperty<INamespaceItem>("SavedNamespace");
            }
            set
            {
                _modelItem.SetProperty("SavedNamespace", value);
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

        protected virtual void OnSomethingChanged(IToolRegion args)
        {
            var handler = SomethingChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}