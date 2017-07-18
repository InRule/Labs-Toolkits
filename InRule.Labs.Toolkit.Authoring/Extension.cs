﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using InRule.Authoring.Commanding;
using InRule.Authoring.ComponentModel;
using InRule.Authoring.Media;
using InRule.Authoring.Services;
using InRule.Authoring.Windows;
using InRule.Authoring.Windows.Controls;
using InRule.Common.Utilities;
using InRule.Labs.Toolkit.Authoring;
using InRule.Repository;
using InRule.Repository.RuleElements;
using InRule.Authoring;
using InRule.Labs.Toolkit.Shared;
using InRule.Labs.Toolkit.Shared.Model;


namespace InRule.Labs.Toolkit.Authoring
{
    class Extension : ExtensionBase
    {
        
        public Helper _helper;
        public ToolkitsContainer _toolkitscontainer;
        private VisualDelegateCommand _showToolkitsCommand;
        //private RuleApplicationController _ruleAppController;
        private IToolWindow _toolWindow;
        private IRibbonToggleButton _button;
        private const string TOOLKITS = "Toolkits";


        public Extension()
            : base(
                "Toolkits", "Include and reuse rule applications by revision.",
                new Guid("{04757D3F-AD48-4E7D-8073-8B3F1D02FDE8}"))
        {}

        public override void Enable()
        {
            
            RuleApplicationService.Opened += WhenRuleAppLoaded;
            RuleApplicationService.Closed += WhenRuleAppClosed;
            SelectionManager.SelectedItemChanging += SelectionManagerOnSelectedItemChanging;
            SelectionManager.SelectedItemChanged += SelectionManager_SelectedItemChanged;
            
            
            var enableButton = RuleApplicationService.RuleApplicationDef != null;
            _showToolkitsCommand = new VisualDelegateCommand(ToggleDisplay, TOOLKITS, null, ImageFactory.GetImageAuthoringAssembly("/Images/SchemaSource32.png"), enableButton);


            IRibbonGroup group = IrAuthorShell.HomeTab.GetGroup("Schema");
	        _button = group.AddToggleButton(_showToolkitsCommand, "Toolkits");
            
        }
        private void SelectionManager_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((SelectionManager)sender).SelectedItem;
            if (item != null)
            {
                Type type = item.GetType();
                if (typeof(RuleRepositoryDefBase).IsAssignableFrom(type))
                {
                    if (_toolkitscontainer != null)
                    {
                        if (_toolkitscontainer.IsToolkit((RuleRepositoryDefBase)item))
                        {
                            Debug.WriteLine("This selected item is in a toolkit....");
                            ShowToolWindow();
                            Guid guid = ((RuleRepositoryDefBase) item).Guid;
                            Artifact a = _toolkitscontainer.GetArtifact(guid);
                            _tree.SelectArtifact(a);
                            //ToolkitsContainer.SetSelectedItem(this._toolWindow.);

                        }
                    }
                   
                }
            }

        }
        private void SelectionManagerOnSelectedItemChanging(object sender, SelectionChangingEventArgs selectionChangingEventArgs)
        {
           
        }

        public override void Disable()
        {
	        var tagsGroup = IrAuthorShell.HomeTab.GetGroup("Schema");
	        tagsGroup.RemoveItem(_button);
			if (_toolWindow != null)
			{
				_toolWindow.Destroy();
			}
        }

	    private void ToggleDisplay(object obj)
        {
            bool isChecked = !_showToolkitsCommand.IsChecked;

            _showToolkitsCommand.IsChecked = isChecked;

            if (isChecked)
            {
                ShowToolWindow();
            }
            else
            {
                DestroyWindow();
            }
        }

        private ToolkitTree _tree;
        private void ShowToolWindow()
        {
            if (_toolWindow != null)
                return;
          
            _tree = ServiceManager.Compose<ToolkitTree>(_toolkitscontainer);

            // to do as tool window
            _toolWindow = IrAuthorShell.AddToolWindow(_tree, "ToolkitTree", "Toolkits", false);
            _toolWindow.Dock(DockDirection.Right);
            _toolWindow.Closed += WhenToolwindowClosed;

            _showToolkitsCommand.IsChecked = true;
        }

        private void DestroyWindow()
        {
            if (_toolWindow != null)
            {
                _toolWindow.Destroy();
                _toolWindow = null;
            }
           
        }

        private void WhenRuleAppLoaded(object sender, EventArgs e)
        {
            _helper = new Helper();
            //Load the toolkits from the loaded ruleapp
            _toolkitscontainer = new ToolkitsContainer();
            _toolkitscontainer.Toolkits = _helper.GetToolkits(this.RuleApplicationService.RuleApplicationDef);
            _toolkitscontainer.SelectionManager = this.SelectionManager;  //required for context passing into the model

            Debug.WriteLine("Toolkits loaded...");

            _showToolkitsCommand.IsEnabled = true;
        }

        private void WhenToolwindowClosed(object sender, EventArgs e)
        {
            _showToolkitsCommand.IsChecked = false;
            DestroyWindow();
        }
       
        private void WhenRuleAppClosed(object sender, EventArgs<RuleApplicationDef> e)
        {
            DestroyWindow();
            _showToolkitsCommand.IsEnabled = false;
            //remove the toolkits from the last loaded ruleapp
            _toolkitscontainer = null;
            Debug.WriteLine("Toolkits unloaded...");
        }
    }
}
