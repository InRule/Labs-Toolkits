using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InRule.Labs.Toolkit.Shared.Model
{
    public class ToolkitsContainer
    {
        private ObservableCollection<ToolkitContents> _toolkits;

        public ObservableCollection<ToolkitContents> Toolkits
        {
            get { return _toolkits; }
            set { _toolkits = value; }
        }
    }

}
