using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Repository;

namespace com.inrule.labs.toolkit.shared
{
    public class Helper
    {
        private string _sourceRuleappPath = @"C:\Users\Christopher Berg\Documents\SourceRuleApplication.ruleappx";
        private string _destRuleappPath = @"C:\Users\Christopher Berg\Documents\DestRuleApplication.ruleappx";

        private RuleApplicationDef _source = null;
        private RuleApplicationDef _dest = null;

        public void RunTest()
        {
            try
            {
                _source = RuleApplicationDef.Load(_sourceRuleappPath);
                _dest = RuleApplicationDef.Load(_destRuleappPath);
           
                PullEntities();
                PullRulesets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
           
        }


        public void PullEntities()
        {

            for (int i = 0; i < _source.Entities.Count; i++)
            {
                Debug.WriteLine(_source.Entities[i].Name);
                _dest.Entities.Add(_source.Entities[i].CopyWithSameGuids());
            }
           // _dest.SaveToFile(_destRuleappPath);
            
        }
        public void PullRulesets()
        {

            for (int i = 0; i < _source.RuleSets.Count; i++)
            {
                Debug.WriteLine(_source.RuleSets[i].Name);
                _dest.RuleSets.Add(_source.RuleSets[i].CopyWithSameGuids());
            }
             _dest.SaveToFile(_destRuleappPath);

        }

        public void PushArtifact()
        {
            
        }

        public void LockArtifact()
        {
            
        }
    }
}
