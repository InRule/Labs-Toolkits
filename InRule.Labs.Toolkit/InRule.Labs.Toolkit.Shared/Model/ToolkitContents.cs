using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Shared.Model
{
    public class ToolkitContents
    {
        private string _name;
        private string revision;
        private string _guid;
        private ObservableCollection<RuleRepositoryDefBase> _contents;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Revision
        {
            get { return revision; }
            set { revision = value; }
        }

        public string GUID
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public ObservableCollection<RuleRepositoryDefBase> Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }
    }
}
