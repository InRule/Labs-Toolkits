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
            foreach (RuleRepositoryDefBase entityDef in _source.Entities)
            {
                ProcessRulesetChildren(entityDef);
                _dest.Entities.Add(entityDef.CopyWithSameGuids());
            }
        }
        private void PullRulesets()
        {
            foreach (RuleRepositoryDefBase rulesetDef in _source.RuleSets)
            {
                ProcessRulesetChildren(rulesetDef);
                _dest.RuleSets.Add(rulesetDef.CopyWithSameGuids());
            }
            _dest.SaveToFile(_destinationRuleappPath);
        }

        private void ProcessRulesetChildren(RuleRepositoryDefBase child)
        {
            StampWithAttribute(child);
            var collquery = from childcollections in child.GetAllChildCollections()
                            select childcollections;
            foreach (RuleRepositoryDefCollection defcollection in collquery)
            {
               // Debug.WriteLine(defcollection.GetType().FullName);
                var defquery = from RuleRepositoryDefBase items in defcollection select items;
                foreach (var def in defquery)
                {
                    ProcessRulesetChildren(def);
                }
            } 
        }

        private void StampWithAttribute(RuleRepositoryDefBase def)
        {
            Debug.WriteLine(def.Name);
            if (def.Attributes.Default.Contains("Toolkit") == false)
            {
                def.Attributes.Default.Add("Toolkit", _source.Name + "," + _source.Revision + "," + _source.Guid);
            }
            
        }

    }
}
