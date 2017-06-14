using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using InRule.Labs.Toolkit.Shared.Model;

namespace InRule.Labs.Toolkit.Shared
{
    public class Helper
    {
        public ObservableCollection<ToolkitContents> GetToolkits(RuleApplicationDef dest)
        {
            ObservableCollection <ToolkitContents> toolkits = new ObservableCollection<ToolkitContents>();
            foreach (XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in dest.Attributes.Default)
            {
                if (att.Key.Contains("Toolkit:"))
                {
                    string key = att.Key.Substring(8, att.Key.Length - 8);  //trim toolkit prefix
                    ToolkitContents tk = new ToolkitContents();
                    ParseKey(key,tk);
                    tk.Contents = GetToolkitContents(key, dest);
                    toolkits.Add(tk);
                }
            }
            return toolkits;
        }
        
        internal bool Exists(RuleApplicationDef source, RuleApplicationDef dest)
        {
            bool exists = false;
            foreach (XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in dest.Attributes.Default)
            {
                if (att.Key.Contains("Toolkit:"))
                {
                    string key = att.Key.Substring(8, att.Key.Length - 8); //trim toolkit prefix
                    if (MakeKey(source) == key)
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
        }
        
        internal void ParseKey(string key, ToolkitContents toolkit)
        {
            toolkit.Name = key.Split(',')[0];
            toolkit.Revision = key.Split(',')[1];
            toolkit.GUID = key.Split(',')[2];
        }
        internal ObservableCollection<RuleRepositoryDefBase> GetToolkitContents(string key, RuleApplicationDef dest)
        {
            ObservableCollection<RuleRepositoryDefBase> list = new ObservableCollection<RuleRepositoryDefBase>();
            //unpack the source ruleappdef
            RuleApplicationDef source = this.GetSourceRuleapp("Toolkit:" + key, dest);

            foreach (RuleRepositoryDefBase entityDef in source.Entities)
            {
                ProcessChildren(entityDef, list, key);
            }
            foreach (RuleRepositoryDefBase rulesetDef in source.RuleSets)
            {
                ProcessChildren(rulesetDef, list, key);
            }
            return list;
        }
        public void ImportArtifacts(RuleApplicationDef source, RuleApplicationDef dest)
        {
            if (Exists(source, dest))
            {
                throw new DuplicateToolkitException("Toolkit already exists in the destination rule application.");
            }
            Import(source, dest);
            StoreSourceRuleapp(source,dest);
        }
        public void ImportArtifacts(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            ImportArtifacts(source,dest);
            dest.SaveToFile(savePath);
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
            string key = MakeKey(source);
            Remove(dest, key);
            RemoveSourceRuleapp(source, dest);
        }
        public void RemoveArtifacts(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            RemoveArtifacts(source, dest);
            dest.SaveToFile(savePath);
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
        public bool IsToolkitMatch(RuleRepositoryDefBase def, string key)
        {
            var isMatch = false;
            var attributes =
                from XmlSerializableStringDictionary.XmlSerializableStringDictionaryItem att in def.Attributes.Default
                where att.Value == key 
                select att;
            if (attributes.Any())
            {
                isMatch = true;
            }
            return isMatch;
        }
        public string GetKey(RuleApplicationDef source)
        {
            return MakeKey(source);
        }
        internal string GetTmpPath()
        {
            return Path.GetTempPath() + Guid.NewGuid() + ".ruleappx";
        }
        internal string MakeKey(RuleApplicationDef source)
        {
            return source.Name + "," + source.Revision + "," + source.Guid;
        }
        public void StoreSourceRuleapp(RuleApplicationDef source, RuleApplicationDef dest)
        {
            //Save temporarily to the filesystem
            string tmp = GetTmpPath();
            source.SaveToFile(tmp);
            string file = EncodeFile(tmp);
            //Store in target attribute with stamp
            string key = MakeKey(source);
            StoreFileInAttribute(file, key, dest);
        }
        internal void StoreFileInAttribute(string file, string key, RuleApplicationDef dest)
        {
            dest.Attributes.Default.Add("Toolkit:" + key, file);
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
            string stamp = "Toolkit:" + MakeKey(source);
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
        internal void Import(RuleApplicationDef source, RuleApplicationDef dest)
        {
            ImportEntities(source, dest);
            ImportRulesets(source, dest);
        }
        internal void Remove(RuleApplicationDef dest, string key)
        {
            CleanEntities(dest, key);
            CleanRulesets(dest, key);
        }
        
        internal void ImportEntities(RuleApplicationDef source, RuleApplicationDef dest)
        {
            string key = MakeKey(source);
            foreach (RuleRepositoryDefBase entityDef in source.Entities)
            {
                ProcessChildren(entityDef, key);
                dest.Entities.Add(entityDef.CopyWithSameGuids());
            }
        }
        
        internal void ImportRulesets(RuleApplicationDef source, RuleApplicationDef dest)
        {
            string key = MakeKey(source);
            foreach (RuleRepositoryDefBase rulesetDef in source.RuleSets)
            {
                ProcessChildren(rulesetDef, key);
                dest.RuleSets.Add(rulesetDef.CopyWithSameGuids());
            }
        }
        internal void CleanEntities(RuleApplicationDef dest, string key)
        {
            foreach (EntityDef entityDef in dest.Entities.ToList<EntityDef>())
            {
                if (IsToolkitMatch(entityDef, key))
                {
                    dest.Entities.Remove(entityDef);
                }
            }
        }
        internal void CleanRulesets(RuleApplicationDef dest, string key)
        {
            foreach (RuleRepositoryDefBase rulesetDef in dest.RuleSets.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(rulesetDef,key))
                {
                    dest.RuleSets.Remove(rulesetDef);
                }
            }
        }

        internal void ProcessChildren(RuleRepositoryDefBase child, string key)
        {
            ProcessChildren(child, null,key);
        }
        internal void ProcessChildren(RuleRepositoryDefBase child, ObservableCollection<RuleRepositoryDefBase> list, string key)
        {
            StampAttribute(child,key);
            list?.Add(child);
            var collquery = from childcollections in child.GetAllChildCollections()
                            select childcollections;
            foreach (RuleRepositoryDefCollection defcollection in collquery)
            {
                var defquery = from RuleRepositoryDefBase items in defcollection select items;
                foreach (var def in defquery)
                {
                    ProcessChildren(def, list, key);
                }
            } 
        }

        internal void StampAttribute(RuleRepositoryDefBase def, string key)
        {
            Debug.WriteLine(def.Name);
            //if for whatever reason it's already been stamped
            if (IsToolkitMatch(def, key) == false)
            {
                def.Attributes.Default.Add("Toolkit", key);
            }
        }
    }
}
