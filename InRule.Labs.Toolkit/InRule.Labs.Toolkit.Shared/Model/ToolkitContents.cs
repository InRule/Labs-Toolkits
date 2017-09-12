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
        private string _revision;
        private string _guid;
        private ObservableCollection<Artifact> _contents;

        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        public string GUID
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public ObservableCollection<Artifact> Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        public string GetKey()
        {
            string key = null;
            //TODO: Make static methods for helper
            Helper h = new Helper();  
            key = h.MakeKey(_name, _revision, _guid);
            h = null;
            return key;
        }
    }
}
