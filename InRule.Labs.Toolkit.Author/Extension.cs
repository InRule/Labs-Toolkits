using System;
using InRule.Authoring.Commanding;
using InRule.Authoring.Media;
using InRule.Authoring.Windows;
using InRule.Authoring.Windows.Controls;

namespace InRule.Addins.ScenarioManager
{
    class Extension : ExtensionBase
    {
        private const string ExtensionGUID = "{12D17C1E-B381-45B9-9131-D1AA290D0301}";
        private VisualDelegateCommand _loadHtmlCommand;
        private VisualDelegateCommand _otherCommand;
        private IRibbonGroup _group;
        private IRibbonTab _tab;
        

        // To make system extension that cannot be disabled, change last parm to true
        public Extension()
            : base("MyExtensionName", "MyExtensionDescription", new Guid(ExtensionGUID), false)
        {

        }

        public override void Enable()
        {
            _tab = IrAuthorShell.Ribbon.AddTab("Simulation");
            
            _group = _tab.AddGroup("My Group", null, "");

            _loadHtmlCommand = new VisualDelegateCommand(LoadHtmlWindow, 
                "Load HTML", 
                ImageFactory.GetImageAuthoringAssembly(@"/Images/Find16.png"), 
                ImageFactory.GetImageAuthoringAssembly(@"/Images/Find32.png"), 
                false);

            _group.AddButton(_loadHtmlCommand);

            _otherCommand = new VisualDelegateCommand(DoSomething, 
                "Do something", 
                ImageFactory.GetImageAuthoringAssembly(@"/Images/Find16.png"), 
                ImageFactory.GetImageAuthoringAssembly(@"/Images/Find32.png"), 
                false);

            _group.AddButton(_otherCommand);
            
            RuleApplicationService.Opened += EnableButton;
            RuleApplicationService.Closed += EnableButton;

            SelectionManager.SelectedItemChanged += EnableButton;

            var htmlContentService = ServiceManager.Compose<HtmlContentProvider>();
            ServiceManager.SetService(htmlContentService);
            
            ContentManager.SetProvider(typeof(HtmlDef), htmlContentService);
        }

        private void DoSomething(object obj)
        {
            var config = new Configuration();
            config.Height = 39;
            config.Width = 20;

            // create an instance of the user control
            var control = new ConfigurationControl(config);


            // use the window factory to create the window passing the description, control and required buttons
            var window = WindowFactory.CreateWindow("Config Settings", control, "OK", "Cancel");

            // subscribe to the click event to run the desired code
            window.ButtonClicked += delegate (object sender, WindowButtonClickedEventArgs<ConfigurationControl> e)
            {
                var closeWindow = true; // if you want to prevent them from closing the window (e.g. invalid item), set closeWindow to true

                if (e.ClickedButtonText == "OK")
                {
                    // validate
                    if (false) // not valid scenario
                    {
                        // report issue however you want and leave window open
                        MessageBoxFactory.Show("Invalid stuff", "Invalid", MessageBoxFactoryImage.Error);

                        closeWindow = false;
                    }
                }

                if (closeWindow)
                {
                    window.Close();
                }
            };

            window.Show();
        }

        private void EnableButton(object sender, EventArgs e)
        {

            // if rule app is open, buttons are enabled, otherwise they aren't
            if (RuleApplicationService.RuleApplicationDef == null)
            {
                _loadHtmlCommand.IsEnabled = false;
                _otherCommand.IsEnabled = false;
            }
            else
            {
                _loadHtmlCommand.IsEnabled = true;
                _otherCommand.IsEnabled = true;
            }
        }

        private void LoadHtmlWindow(object obj)
        {
            var html = new HtmlDef();
            html.Content = "hello there";

            SelectionManager.SelectedItem = html;
        }

        public override void Disable()
        {
            IrAuthorShell.HomeTab.RemoveGroup(_group);
        }
    }
}
