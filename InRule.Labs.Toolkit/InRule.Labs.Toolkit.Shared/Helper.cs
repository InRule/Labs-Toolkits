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
        private string _importHash = ""; //prevents duplicate import
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
            GetAll(source, list);
            return list;
        }
        public void ImportToolkit(RuleApplicationDef source, RuleApplicationDef dest)
        {
            if (Exists(source, dest))
            {
                throw new DuplicateToolkitException("Toolkit already exists in the destination rule application.");
            }
            Import(source, dest);
            StoreSourceRuleapp(source,dest);
        }
        public void ImportToolkit(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            ImportToolkit(source,dest);
            dest.SaveToFile(savePath);
        }
        public void ImportToolkit(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                ImportToolkit(RuleApplicationDef.Load(sourceRuleappPath),
                    RuleApplicationDef.Load(destinationRuleappPath), destinationRuleappPath);    
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }
        }
        public void RemoveToolkit(RuleApplicationDef source, RuleApplicationDef dest)
        {
            string key = MakeKey(source);
            Remove(dest, key);
            RemoveSourceRuleapp(source, dest);
        }
        public void RemoveToolkit(RuleApplicationDef source, RuleApplicationDef dest, string savePath)
        {
            RemoveToolkit(source, dest);
            dest.SaveToFile(savePath);
        }
        public void RemoveToolkit(string sourceRuleappPath, string destinationRuleappPath)
        {
            try
            {
                RemoveToolkit(RuleApplicationDef.Load(sourceRuleappPath),
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
            string key = MakeKey(source);
            GetAll(source);  //stamps source artifacts with an attribute containing a toolkit key
            //import entities
            foreach (RuleRepositoryDefBase entityDef in source.Entities)
            {
                dest.Entities.Add(entityDef.CopyWithSameGuids());
            }
            //import rulesets
            foreach (RuleRepositoryDefBase rulesetDef in source.RuleSets)
            {
                dest.RuleSets.Add(rulesetDef.CopyWithSameGuids());
            }
            //import endpoints
            foreach (RuleRepositoryDefBase endpoint in source.EndPoints)
            {
                dest.EndPoints.Add(endpoint.CopyWithSameGuids());
            }
            //import udfs
            foreach (RuleRepositoryDefBase udf in source.UdfLibraries)
            {
                dest.UdfLibraries.Add(udf.CopyWithSameGuids());
            }
            //import categories
            foreach (RuleRepositoryDefBase category in source.Categories)
            {
                dest.Categories.Add(category.CopyWithSameGuids());
            }
            //data elements
            foreach (RuleRepositoryDefBase dataelement in source.DataElements)
            {
                dest.DataElements.Add(dataelement.CopyWithSameGuids());
            }
        }
        internal void Remove(RuleApplicationDef dest, string key)
        {
           
            //remove entities
            foreach (EntityDef entity in dest.Entities.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(entity, key))
                {
                    dest.Entities.Remove(entity);
                }
            }
            //remove rulesets
            foreach (RuleRepositoryDefBase ruleset in dest.RuleSets.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(ruleset, key))
                {
                    dest.RuleSets.Remove(ruleset);
                }
            }
            //remove endpoints
            foreach (RuleRepositoryDefBase endpoint in dest.EndPoints.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(endpoint, key))
                {
                    dest.EndPoints.Remove(endpoint);
                }
            }
            //remove categories
            foreach (RuleRepositoryDefBase category in dest.Categories.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(category, key))
                {
                    dest.Categories.Remove(category);
                }
            }
            //remove dataelements
            foreach (RuleRepositoryDefBase dataelement in dest.DataElements.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(dataelement, key))
                {
                    dest.DataElements.Remove(dataelement);
                }
            }
            //remove UDFs
            foreach (RuleRepositoryDefBase udf in dest.UdfLibraries.ToList<RuleRepositoryDefBase>())
            {
                if (IsToolkitMatch(udf, key))
                {
                    dest.UdfLibraries.Remove(udf);
                }
            }
        }
      
        internal void ProcessChildren(RuleRepositoryDefBase child, ObservableCollection<RuleRepositoryDefBase> list, string key)
        {
            if (_importHash.Contains(child.Name) == false)
            {
                _importHash = _importHash + child.Name;  //update the hash
                Console.WriteLine(child.Name);
                StampAttribute(child, key);
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

        internal void GetAll(RuleApplicationDef source)
        {
            GetAll(source, null);
        }
        internal void GetAll(RuleApplicationDef source, ObservableCollection<RuleRepositoryDefBase> list)
        {
            _importHash = "";  //reset
            string key = MakeKey(source);
            RuleRepositoryDefCollection[] colls = source.GetAllChildCollections();
            foreach (RuleRepositoryDefCollection coll in colls)
            {
                foreach (RuleRepositoryDefBase def in coll)
                {
                        ProcessChildren(def, list, key);
                }
            }
        }
    }
}
