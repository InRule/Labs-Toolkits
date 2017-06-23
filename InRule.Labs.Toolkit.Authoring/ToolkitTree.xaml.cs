using System.Windows;
using System.Windows.Controls;
using InRule.Labs.Toolkit.Shared;
using InRule.Labs.Toolkit.Shared.Model;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Authoring
{
    /// <summary>
    /// Interaction logic for RuleSetByCategoryTree.xaml
    /// </summary>
    public partial class ToolkitTree : UserControl
    {
        public ToolkitTree(ToolkitsContainer toolkitscontainer)
        {
            InitializeComponent();

            DataContext = toolkitscontainer;
        }

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // set selected item
            //((CategoryModel) DataContext).SelectedItem = e.NewValue;
        }
    }
}
