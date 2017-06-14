using System;
using InRule.Authoring.Commanding;
using InRule.Authoring.Media;
using InRule.Authoring.Windows;
using InRule.Authoring.Windows.Controls;
using InRule.Authoring.ComponentModel;
using InRule.Authoring.Services;
using InRule.Common.Utilities;
using InRule.Repository;
using InRule.Repository.RuleElements;

namespace InRule.Labs.Toolkit.Author
{
    class Extension : ExtensionBase
    {
        public CategoryModel _categoryModel;
        private VisualDelegateCommand _showRuleSetsByCategoryCommand;
        private RuleApplicationController _ruleAppController;
        private IToolWindow _toolWindow;
        private IRibbonToggleButton _button;
        private const string RULES_BY_CAT = "Rules by Category";

        private const string ExtensionGUID = "{12D17C1E-B381-45B9-9131-D1AA290D0312}";
      

        // To make system extension that cannot be disabled, change last parm to true
        public Extension()
            : base("Toolkits", "Safely reuse content from other rule applications.", new Guid(ExtensionGUID), false)
        {

        }

        public override void Enable()
        {
            RuleApplicationService.Opened += WhenRuleAppLoaded;
            RuleApplicationService.Closed += WhenRuleAppClosed;

            _ruleAppController = ServiceManager.GetService<RuleApplicationService>().Controller;
            _ruleAppController.CategoryRemovedFromDef += WhenCategoryRemovedFromDef;
            _ruleAppController.CategoryAddedToDef += WhenCategoryAddedToDef;
            _ruleAppController.RuleSetAdded += WhenRuleSetAdded;
            _ruleAppController.CategoryAdded += WhenCategoryAdded;
            _ruleAppController.RemovingDef += WhenDefRemoved;

            var enableButton = RuleApplicationService.RuleApplicationDef != null;
            _showRuleSetsByCategoryCommand = new VisualDelegateCommand(ToggleDisplay, RULES_BY_CAT, null, ImageFactory.GetImageAuthoringAssembly("/Images/SchemaSource32.png"), enableButton);

            var group = IrAuthorShell.HomeTab.GetGroup("Tags");
            _button = group.AddToggleButton(_showRuleSetsByCategoryCommand, "Rules by Category");
        }

        private void DoSomething(object obj)
        {
           
        }

        private void EnableButton(object sender, EventArgs e)
        {

        }

      

        public override void Disable()
        {
            var tagsGroup = IrAuthorShell.HomeTab.GetGroup("Tags");
            tagsGroup.RemoveItem(_button);

            _ruleAppController.CategoryRemovedFromDef -= WhenCategoryRemovedFromDef;
            _ruleAppController.CategoryAddedToDef -= WhenCategoryAddedToDef;
            _ruleAppController.RuleSetAdded -= WhenRuleSetAdded;
            _ruleAppController.CategoryAdded -= WhenCategoryAdded;
            _ruleAppController.RemovingDef -= WhenDefRemoved;

            if (_toolWindow != null)
            {
                _toolWindow.Destroy();
            }
        }
    }
}
