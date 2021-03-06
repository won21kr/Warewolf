﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Dev2.Common.Interfaces;
using Infragistics.Controls.Menus;
using Microsoft.Practices.Prism.Mvvm;
using Warewolf.Studio.ViewModels;

namespace Warewolf.Studio.Views
{
    /// <summary>
    /// Interaction logic for ManagePluginSourceControl.xaml
    /// </summary>
    public partial class ManagePluginSourceControl : IView, ICheckControlEnabledView
    {
        public ManagePluginSourceControl()
        {
            InitializeComponent();
        }

        public string GetHeaderText()
        {
            BindingExpression be = HeaderTextBlock.GetBindingExpression(TextBlock.TextProperty);
            be?.UpdateTarget();
            return HeaderTextBlock.Text;
        }


        void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue.Equals(true) && e.NewValue.Equals(false))
            {
                SearchTextBox.IsEnabled = true;
                RefreshButton.IsEnabled = true;
            }
        }

        public IDllListingModel SelectItem(string itemName)
        {
            
            var xamDataTreeNode = GetItem(itemName);
            if (xamDataTreeNode != null)
            {
                xamDataTreeNode.IsSelected = true;
                ExplorerTree.ActiveNode = xamDataTreeNode;
            }
            return xamDataTreeNode?.Data as IDllListingModel;
        }

        public bool IsItemVisible(string itemName)
        {
            var xamDataTreeNode = GetItem(itemName);
            return xamDataTreeNode != null;
        }

        XamDataTreeNode GetItem(string itemName)
        {
            var xamDataTreeNodes = TreeUtils.Descendants(ExplorerTree.Nodes.ToArray());
            return xamDataTreeNodes.FirstOrDefault(node =>
            {
                var item = node.Data as IDllListingModel;
                if (item != null)
                {
                    if (item.Name.ToLowerInvariant().Contains(itemName.ToLowerInvariant()))
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        public string GetAssemblyName()
        {
            BindingExpression be = AssemblyNameTextBox.GetBindingExpression(TextBox.TextProperty);
            be?.UpdateTarget();
            return AssemblyNameTextBox.Text;
        }

        public void ClearFilter()
        {
            SearchTextBox.ClearSearchCommand.Execute(null);
        }

        public void PerformSave()
        {
            var viewModel = DataContext as ManagePluginSourceViewModel;
            viewModel?.OkCommand.Execute(null);
        }

        public void SetAssemblyName(string assemblyName)
        {
            AssemblyNameTextBox.Text = assemblyName;
            BindingExpression be = AssemblyNameTextBox.GetBindingExpression(TextBlock.TextProperty);
            be?.UpdateSource();
        }

        public IDllListingModel OpenItem(string itemNameToOpen)
        {
            var xamDataTreeNode = GetItem(itemNameToOpen);
            if (xamDataTreeNode != null)
            {
                xamDataTreeNode.IsExpanded = true;
            }
            return xamDataTreeNode?.Data as IDllListingModel;
        }

        public bool GetControlEnabled(string controlName)
        {
            switch(controlName)
            {
                case "Save":
                    var viewModel = DataContext as ManagePluginSourceViewModel;
                    return viewModel != null && viewModel.OkCommand.CanExecute(null);
                case "Filter":
                    return SearchTextBox.IsEnabled;
            }
            return false;
        }

        public void ExecuteRefresh()
        {
            RefreshButton.Command.Execute(null);
        }

        public void FilterItems()
        {
            var count = ExplorerTree.Nodes.Count;
        }

        void ExplorerTree_OnLoaded(object sender, RoutedEventArgs e)
        {
            var menuTree = sender as XamDataTree;
            if(menuTree != null)
            {
                var items = menuTree.ItemsSource;
            }
            SelectNode(ExplorerTree.Nodes, "KEYTOSELECT");
        }
        void SelectNode(XamDataTreeNodesCollection nodes, string nodeKey)
        {
            
        }

        private void ExplorerTree_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                AssemblyNameTextBox.Focus();
                e.Handled = true;
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                RefreshButton.Focus();
                e.Handled = true;
            }
        }
    }
}
