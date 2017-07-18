using System;
using System.Collections.ObjectModel;
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
            return GetArtifact(guid).DefBase;
        }
        /// <summary>
        /// Gets a specific Artifact from any toolkit 
        /// </summary>
        public Artifact GetArtifact(Guid guid)
        {
            Artifact artifact = null;
            foreach (var toolkit in _toolkits)
            {
                foreach (Artifact art in toolkit.Contents)
                {
                    if (art.DefBase.Guid.Equals(guid))
                    {
                        artifact = art;
                        break;
                    }
                }
                if (artifact != null)
                {
                    break; //got a hit
                }
            }
            return artifact;
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
