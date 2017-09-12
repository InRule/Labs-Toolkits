using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Shared.Model
{
    public class Artifact 
    {
        private RuleRepositoryDefBase _defBase;

       

        public RuleRepositoryDefBase DefBase
        {
            get { return _defBase; }
            set { _defBase = value; }
        }
    }

}
