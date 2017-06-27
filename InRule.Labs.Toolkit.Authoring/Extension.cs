using System;
using System.Collections.ObjectModel;
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
        //public CategoryModel _categoryModel;
        public Helper _helper;
        public ToolkitsContainer _toolkitscontainer;
        //public ObservableCollection<ToolkitContents> _toolkits;
        private VisualDelegateCommand _showToolkitsCommand;
        private RuleApplicationController _ruleAppController;
        private IToolWindow _toolWindow;
	    private IRibbonToggleButton _button;
	    //private const string RULES_BY_CAT = "Rules by Category";
        private const string TOOLKITS = "Toolkits";
        

        public Extension()
            : base("Toolkits", "Include and reuse rule applications by revision.", new Guid("{04757D3F-AD48-4E7D-8073-8B3F1D02FDE8}"))
        {}

        public override void Enable()
        {
            _helper = new Helper();
            RuleApplicationService.Opened += WhenRuleAppLoaded;
            RuleApplicationService.Closed += WhenRuleAppClosed;
            
            _ruleAppController = ServiceManager.GetService<RuleApplicationService>().Controller;
            //_ruleAppController.CategoryRemovedFromDef += WhenCategoryRemovedFromDef;
            //_ruleAppController.CategoryAddedToDef += WhenCategoryAddedToDef;
            //_ruleAppController.RuleSetAdded += WhenRuleSetAdded;
            //_ruleAppController.CategoryAdded += WhenCategoryAdded;
            //_ruleAppController.RemovingDef += WhenDefRemoved;
            

            var enableButton = RuleApplicationService.RuleApplicationDef != null;
            _showToolkitsCommand = new VisualDelegateCommand(ToggleDisplay, TOOLKITS, null, ImageFactory.GetImageAuthoringAssembly("/Images/SchemaSource32.png"), enableButton);


            IRibbonGroup group = IrAuthorShell.HomeTab.GetGroup("Schema");
	        _button = group.AddToggleButton(_showToolkitsCommand, "Toolkits");
            
        }

        public override void Disable()
        {
	        var tagsGroup = IrAuthorShell.HomeTab.GetGroup("Schema");
	        tagsGroup.RemoveItem(_button);

	       // _ruleAppController.CategoryRemovedFromDef -= WhenCategoryRemovedFromDef;
	      //  _ruleAppController.CategoryAddedToDef -= WhenCategoryAddedToDef;
	      //  _ruleAppController.RuleSetAdded -= WhenRuleSetAdded;
	      //  _ruleAppController.CategoryAdded -= WhenCategoryAdded;
	      //  _ruleAppController.RemovingDef -= WhenDefRemoved;

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

        private void ShowToolWindow()
        {
            if (_toolWindow != null)
                return;

            //Load the toolkits from the loaded ruleapp
            _toolkitscontainer = new ToolkitsContainer();
            _toolkitscontainer.Toolkits = _helper.GetToolkits(this.RuleApplicationService.RuleApplicationDef);
            _toolkitscontainer.SelectionManager = this.SelectionManager;  //required for context passing into the model

            // creating an instance of the class in this manner will automatically set 
            // any properties in the class that are services that are contained in the service manager
            //_categoryModel = ServiceManager.Compose<CategoryModel>(RuleApplicationService.RuleApplicationDef);

            //var tree = ServiceManager.Compose<RuleSetByCategoryTree>(_categoryModel);
            var tree = ServiceManager.Compose<ToolkitTree>(_toolkitscontainer);

            // to do as tool window
            _toolWindow = IrAuthorShell.AddToolWindow(tree, "ToolkitTree", "Toolkits", false);
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
            _helper = null;
            _toolkitscontainer = null;
            //_categoryModel = null;
        }

        private void WhenRuleAppLoaded(object sender, EventArgs e)
        {
            _showToolkitsCommand.IsEnabled = true;
        }

        private void WhenToolwindowClosed(object sender, EventArgs e)
        {
            _showToolkitsCommand.IsChecked = false;
            DestroyWindow();
        }
        /*
        private void WhenCategoryAdded(object sender, EventArgs<CategoryDef> e)
        {

            if (_categoryModel != null)
            {
                _categoryModel.AddCategory(e.Item);
            }
        }
        */
        /*
        private void WhenDefRemoved(object sender, CancelEventArgs<RuleRepositoryDefBase> e)
        {
            if (_categoryModel != null)
            {

                if (e.Item is CategoryDef)
                {
                    _categoryModel.RemoveCategory((CategoryDef) e.Item);
                }
                else if (e.Item is RuleSetDef)
                {
                    _categoryModel.RemoveRuleSet((RuleSetDef) e.Item);
                }
            }
        }
        */
        /*
        private void WhenRuleSetAdded(object sender, EventArgs<RuleSetDefBase> e)
        {
            if (_categoryModel != null)
            {
                _categoryModel.AddRuleSet(e.Item);
            }
        }
        */
        /*
        private void WhenCategoryAddedToDef(object sender, ParentChildEventArgs<RuleRepositoryDefBase, string> e)
        {
            if (_categoryModel != null)
            {
                _categoryModel.AddCategoryToDef(e.Parent, e.Child);
            }
        }
        */
        /*
        private void WhenCategoryRemovedFromDef(object sender, ParentChildEventArgs<RuleRepositoryDefBase, string> e)
        {
            if (_categoryModel != null)
            {
                _categoryModel.RemoveCategoryFromDef(e.Parent, e.Child);
            }
        }
        */

        private void WhenRuleAppClosed(object sender, EventArgs<RuleApplicationDef> e)
        {
            DestroyWindow();
            _showToolkitsCommand.IsEnabled = false;
        }
    }
}
