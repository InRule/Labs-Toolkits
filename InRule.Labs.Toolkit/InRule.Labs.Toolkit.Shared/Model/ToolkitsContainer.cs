using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Authoring.Services;
using InRule.Authoring.Windows;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Shared.Model
{
    public class ToolkitsContainer
    {
        private ObservableCollection<ToolkitContents> _toolkits;
        public SelectionManager SelectionManager { get; set; }
        public ObservableCollection<ToolkitContents> Toolkits
        {
            get { return _toolkits; }
            set { _toolkits = value; }
        }
        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                try
                {
                    var item = value as Artifact;
                    if (item != null)
                    {
                        _selectedItem = item.DefBase;
                        SelectionManager.SelectedItem = item.DefBase;
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxFactory.Show("Error trying to set selected item: " + value.ToString() + Environment.NewLine + ex.ToString(), "Error in setting selected item", MessageBoxFactoryImage.Error);
                }
            }
        }


    }

}
