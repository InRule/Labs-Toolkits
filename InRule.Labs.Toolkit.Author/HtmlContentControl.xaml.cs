using System.Windows.Controls;

namespace InRule.Addins.ScenarioManager
{
    /// <summary>
    /// Interaction logic for HtmlContentControl.xaml
    /// </summary>
    public partial class HtmlContentControl : UserControl
    {
        public HtmlContentControl(string content)
        {
            
            InitializeComponent();
            
            webBrowser.NavigateToString(content);
        }
    }
}
