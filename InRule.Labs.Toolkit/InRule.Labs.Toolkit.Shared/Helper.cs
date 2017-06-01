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
using System.IO;

namespace InRule.Labs.Toolkit.Shared
{
    public class Helper
    {
        internal RuleApplicationDef _source = null;
        internal RuleApplicationDef _dest = null;
        internal string _stamp = "";

        public void ImportArtifacts(RuleApplicationDef source, RuleApplicationDef dest)
        {
            _source = source;
            _dest = dest;
            MakeStamp();
            Import();
            StoreSourceRuleapp(_source,_dest);
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
            RemoveSourceRuleapp(source, dest);
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
        internal bool IsToolkitMatch(RuleRepositoryDefBase def)
        {
            return IsToolkitMatch(def, _stamp);
        }
        public bool IsToolkitMatch(RuleRepositoryDefBase def, string stamp)
        {
            var isMatch = false;
            var attributes =
                from XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in def.Attributes.Default
                where att.Value == stamp 
                select att;
            if (attributes.Any())
            {
                isMatch = true;
            }
            return isMatch;
        }
        public string GetStamp(RuleApplicationDef source)
        {
            return MakeStamp(source);
        }
        internal string GetTmpPath()
        {
            return Path.GetTempPath() + Guid.NewGuid() + ".ruleappx";
        }
        internal string MakeStamp(RuleApplicationDef source)
        {
            return source.Name + "," + source.Revision + "," + source.Guid;
        }
        internal void MakeStamp()
        {
            _stamp = MakeStamp(_source);
        }
        public void StoreSourceRuleapp(RuleApplicationDef source, RuleApplicationDef dest)
        {
            //Save temporarily to the filesystem
            string tmp = GetTmpPath();
            source.SaveToFile(tmp);
            string file = EncodeFile(tmp);
            //Store in target attribute with stamp
            string stamp = MakeStamp(source);
            StoreFileInAttribute(file, stamp, dest);
        }
        internal void StoreFileInAttribute(string file, string key, RuleApplicationDef dest)
        {
            dest.Attributes.Default.Add(key, file);
        }
        internal string EncodeFile(string path)
        {
            //Base64Encode file
            byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
        internal void DecodeFile(string file, string path)
        {
            byte[] bytes = Convert.FromBase64String(file);
            File.WriteAllBytes(path, bytes);
        }
        public RuleApplicationDef  GetSourceRuleapp(string key, RuleApplicationDef dest)
        {
            RuleApplicationDef def = null;
            //Get from attribute
            XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att = FindSourceAttribute(key, dest);
            if (att != null)
            {
                string file = att.Value;     
                string tmpPath = GetTmpPath();
                DecodeFile(file, tmpPath);
                def = RuleApplicationDef.Load(tmpPath);
            }
            return def;
        }
        public void RemoveSourceRuleapp(RuleApplicationDef source, RuleApplicationDef dest)
        {
            string stamp = MakeStamp(source);
            dest.Attributes.Default.Remove(stamp);
        }
        internal XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem FindSourceAttribute(string key, RuleApplicationDef dest)
        {
            XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem resultAtt = null;
            foreach (XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in dest.Attributes.Default)
            {
                if (att.Key == key)
                {
                    resultAtt = att;
                    break;
                }
            }
            return resultAtt;
        }
        internal void Import()
        {
            PullEntities();
            PullRulesets();
        }
        internal void Remove()
        {
            CleanEntities();
            CleanRulesets();
        }
        internal void PullEntities()
        {
            foreach (RuleRepositoryDefBase entityDef in _source.Entities)
            {
                ProcessRulesetChildren(entityDef);
                _dest.Entities.Add(entityDef.CopyWithSameGuids());
            }
        }
        internal void PullRulesets()
        {
            foreach (RuleRepositoryDefBase rulesetDef in _source.RuleSets)
            {
                ProcessRulesetChildren(rulesetDef);
                _dest.RuleSets.Add(rulesetDef.CopyWithSameGuids());
            }
        }
        internal void CleanEntities()
        {
            foreach (EntityDef entityDef in _dest.Entities.ToList<EntityDef>())
            {
                if (IsToolkitMatch(entityDef))
                {
                    _dest.Entities.Remove(entityDef);
                }
            }
        }
        internal void CleanRulesets()
        {
            foreach (RuleRepositoryDefBase rulesetDef in _dest.RuleSets.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(rulesetDef))
                {
                    _dest.RuleSets.Remove(rulesetDef);
                }
            }
        }
        internal void ProcessRulesetChildren(RuleRepositoryDefBase child)
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
        internal void StampWithAttribute(RuleRepositoryDefBase def)
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
