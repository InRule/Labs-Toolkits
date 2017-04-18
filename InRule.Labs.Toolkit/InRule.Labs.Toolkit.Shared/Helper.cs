using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Shared
{
    public class Helper
    {
        private string _sourceRuleappPath;  
        private string _destinationRuleappPath;   

        private RuleApplicationDef _source = null;
        private RuleApplicationDef _dest = null;

        public void ImportArtifacts(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                _sourceRuleappPath = sourceRuleappPath;
                _destinationRuleappPath = destinationRuleappPath;
                _source = RuleApplicationDef.Load(_sourceRuleappPath);
                _dest = RuleApplicationDef.Load(_destinationRuleappPath);
           
                PullEntities();
                PullRulesets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
           
        }


        private void PullEntities()
        {

            for (int i = 0; i < _source.Entities.Count; i++)
            {
                Debug.WriteLine(_source.Entities[i].Name);
                _dest.Entities.Add(_source.Entities[i].CopyWithSameGuids());
            }
           // _dest.SaveToFile(_destRuleappPath);
            
        }
        private void PullRulesets()
        {

            for (int i = 0; i < _source.RuleSets.Count; i++)
            {
                Debug.WriteLine(_source.RuleSets[i].Name);
                _dest.RuleSets.Add(_source.RuleSets[i].CopyWithSameGuids());
            }
             _dest.SaveToFile(_destinationRuleappPath);

        }

    }
}
