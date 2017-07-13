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
                        ((RuleRepositoryDefBase)SelectionManager.SelectedItem).CatalogState = CatalogState.CheckedIn;
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxFactory.Show("Error trying to set selected item: " + value.ToString() + Environment.NewLine + ex.ToString(), "Error in setting selected item", MessageBoxFactoryImage.Error);
                }
            }
        }
        /// <summary>
        /// Gets a specific definition from any toolkit 
        /// </summary>
        public RuleRepositoryDefBase GetDef(Guid guid)
        {
            RuleRepositoryDefBase resultdef = null;
            foreach (var toolkit in _toolkits)
            {
                foreach (Artifact def in toolkit.Contents)
                {
                    if (def.DefBase.Guid.Equals(guid))
                    {
                        resultdef = def.DefBase;
                        break;
                    }
                }
                if (resultdef != null)
                {
                    break; //got a hit
                }
            }
            return resultdef;
        }

        public bool IsToolkit(RuleRepositoryDefBase def)
        {
            bool result = false;
            Helper h = new Helper();
            foreach (ToolkitContents toolkit in _toolkits)
            {
                if (h.IsToolkitMatch(def, toolkit.GetKey()))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


    }

}
