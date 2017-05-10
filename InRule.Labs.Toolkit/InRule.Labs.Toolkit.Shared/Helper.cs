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
        public void ImportArtifacts(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            ImportArtifacts(source,dest);
            _dest.SaveToFile(savePath);
        }
        public void ImportArtifacts(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                ImportArtifacts(RuleApplicationDef.Load(sourceRuleappPath),
                    RuleApplicationDef.Load(destinationRuleappPath), destinationRuleappPath);    
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
        }
        public void RemoveArtifacts(RuleApplicationDef source, RuleApplicationDef dest)
        {
            _source = source;
            _dest = dest;
            MakeStamp();
            Remove();
        }
        public void RemoveArtifacts(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            RemoveArtifacts(source, dest);
            _dest.SaveToFile(savePath);
        }
        public void RemoveArtifacts(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                RemoveArtifacts(RuleApplicationDef.Load(sourceRuleappPath),
                    RuleApplicationDef.Load(destinationRuleappPath), destinationRuleappPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
        }
        private bool IsToolkitMatch(RuleRepositoryDefBase def)
        {
            return IsToolkitMatch(def, _stamp);
        }
        public bool IsToolkitMatch(RuleRepositoryDefBase def, string stamp)
        {
            var isMatch = false;
            var attributes =
                from XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in def.Attributes.Default
                where att.Value == _stamp 
                select att;
            if (attributes.Any())
            {
                isMatch = true;
            }
            return isMatch;
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
                if (IsToolkitMatch(entityDef))
                {
                    _dest.Entities.Remove(entityDef);
                }
            }
        }
        private void CleanRulesets()
        {
            foreach (RuleRepositoryDefBase rulesetDef in _dest.RuleSets.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(rulesetDef))
                {
                    _dest.RuleSets.Remove(rulesetDef);
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
            //if for whatever reason it's already been stamped
            if (IsToolkitMatch(def) == false)
            {
                def.Attributes.Default.Add("Toolkit", _stamp);
            }
        }
    }
}
