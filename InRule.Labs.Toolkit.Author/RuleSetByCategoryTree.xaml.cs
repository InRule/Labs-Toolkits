using System.Windows;
using System.Windows.Controls;

namespace InRule.Labs.Toolkit.Author
{
    /// <summary>
    /// Interaction logic for RuleSetByCategoryTree.xaml
    /// </summary>
    public partial class RuleSetByCategoryTree : UserControl
    {
        public RuleSetByCategoryTree(CategoryModel categoryModel)
        {
            InitializeComponent();

            DataContext = categoryModel;
        }

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // set selected item
            ((CategoryModel) DataContext).SelectedItem = e.NewValue;
        }
    }
}
