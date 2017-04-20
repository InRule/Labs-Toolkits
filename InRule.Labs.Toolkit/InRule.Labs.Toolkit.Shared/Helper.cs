using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Authoring.Commands;
using InRule.Authoring.Extensions;
using InRule.Repository;
using InRule.Repository.Attributes;
using InRule.Repository.RuleElements;

namespace InRule.Labs.Toolkit.Shared
{
    public class Helper
    {
        private string _sourceRuleappPath;  
        private string _destinationRuleappPath;   
        private RuleApplicationDef _source = null;
        private RuleApplicationDef _dest = null;
        private string _stamp = "";

        public void ImportArtifacts(RuleApplicationDef source, RuleApplicationDef dest)
        {
            _source = source;
            _dest = dest;
            MakeStamp();
            Import();
        }
        public void RemoveArtifacts(RuleApplicationDef source, RuleApplicationDef dest)
        {
            _source = source;
            _dest = dest;
            MakeStamp();
            Remove();
        }
        public void ImportArtifacts(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                _sourceRuleappPath = sourceRuleappPath;
                _destinationRuleappPath = destinationRuleappPath;
                _source = RuleApplicationDef.Load(_sourceRuleappPath);
                _dest = RuleApplicationDef.Load(_destinationRuleappPath);
                MakeStamp();
                Import();
                _dest.SaveToFile(_destinationRuleappPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
        }
        private void MakeStamp()
        {
            _stamp = _source.Name + "," + _source.Revision + "," + _source.Guid;
        }
        private void Import()
        {
            PullEntities();
            PullRulesets();
        }
        private void Remove()
        {
            CleanEntities();
            CleanRulesets();
        }

        public void RemoveArtifacts(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                _sourceRuleappPath = sourceRuleappPath;
                _destinationRuleappPath = destinationRuleappPath;
                _source = RuleApplicationDef.Load(_sourceRuleappPath);
                _dest = RuleApplicationDef.Load(_destinationRuleappPath);
                _stamp = _source.Name + "," + _source.Revision + "," + _source.Guid;
                Remove();
                _dest.SaveToFile(_destinationRuleappPath);

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
        }

        private void CleanEntities()
        {
            foreach (EntityDef entityDef in _dest.Entities.ToList<EntityDef>())
            {
                    foreach (
                        XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in
                        entityDef.Attributes.Default)
                    {
                        if (att.Value == _stamp)
                        {
                            _dest.Entities.Remove(entityDef);
                        }
                    }
            }
        }
        private void CleanRulesets()
        {
            foreach (RuleRepositoryDefBase rulesetDef in _dest.RuleSets.ToList<RuleRepositoryDefBase>())
            {
                foreach (XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in rulesetDef.Attributes.Default )
                {
                    if (att.Value == _stamp)
                    {
                        _dest.RuleSets.Remove(rulesetDef);
                    }
                }
                
            }
        }

        private void ProcessRulesetChildren(RuleRepositoryDefBase child)
        {
            StampWithAttribute(child);
            var collquery = from childcollections in child.GetAllChildCollections()
                            select childcollections;
            foreach (RuleRepositoryDefCollection defcollection in collquery)
            {
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
