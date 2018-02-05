using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using InRule.Labs.Toolkit.Shared;
using InRule.Repository;
using System.IO;
using InRule.Labs.Toolkit.Shared.Model;
using System.Collections.ObjectModel;
using InRule.Repository.EndPoints;

namespace InRule.Labs.Toolkit.Shared.Tests
{
    [TestFixture]
    class HelperTests
    {
        private string _sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
            AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\", "SourceRuleApplication.ruleappx");

        private string _destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
            AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\", "DestRuleApplication.ruleappx");
        
        /*private string _destPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
           AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\", "DestRuleApplication");
           */

        [Test]
        public void MakeTempPathTest()
        {
            //Not a great test but it helped get the framework up and running
            Helper h = new Helper();
            string path = h.GetTmpPath();
            Assert.IsNotNull(path);
            Assert.AreNotEqual(path, "");
        }

        [Test]
        public void MakeStampTest()
        {
            Helper h = new Helper();
            Console.WriteLine(_sourcePath);
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            Assert.NotNull(source);
            Console.WriteLine(source.Name);
            Console.WriteLine(source.Guid);
            Console.WriteLine(source.Revision);
            Console.WriteLine(h.MakeKey(source));
            Assert.AreEqual("SourceRuleApplication,1,9dd52e90-cdbb-4d1b-81f2-9956608c8793", h.MakeKey(source));
            //Test the method that sets the member variable fo the stamp
            //h._source = def;
            //h.MakeStamp();
            string key = h.MakeKey(source);
            Assert.AreEqual("SourceRuleApplication,1,9dd52e90-cdbb-4d1b-81f2-9956608c8793", key);

        }

        [Test]
        public void IsToolkitMatchTest()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            EntityDef ent = new EntityDef();
            string key = h.MakeKey(source);
            h.StampAttribute(ent,key);
            Assert.IsTrue(h.IsToolkitMatch(ent,key)); //the purpose is to test the helper method look for a stamped attribute
            Assert.IsFalse(h.IsToolkitMatch(new EntityDef(),key)); //no stamp should be false
        }

        [Test]
        public void ToolkitIntegrationTests()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportToolkit(source,dest);
            Assert.IsTrue(source.Entities.Count == dest.Entities.Count);
            Assert.IsTrue(source.EndPoints.Count == dest.EndPoints.Count);
            Assert.IsTrue(source.UdfLibraries.Count == dest.UdfLibraries.Count);
            Assert.IsTrue(source.Categories.Count == dest.Categories.Count);
            Assert.IsTrue(source.DataElements.Count == dest.DataElements.Count);
            Assert.IsTrue(source.RuleSets.Count == dest.RuleSets.Count);
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Ruleapps\\dest_" + Guid.NewGuid() + ".ruleappx";
            dest.SaveToFile(path);

            h.RemoveToolkit(source,dest);
            Assert.IsTrue(dest.Entities.Count == 0); //all imported entities are gone
            Assert.IsTrue(dest.RuleSets.Count == 0); //all imported RuleSets are gone
            Assert.IsTrue(dest.EndPoints.Count == 0); //all endpoints are gone
            Assert.IsTrue(dest.Categories.Count == 0); //all categories are gone
            Assert.IsTrue(dest.DataElements.Count == 0); //all data elements are gone
            Assert.IsTrue(dest.UdfLibraries.Count == 0);  //All udfs are gone
            Assert.IsTrue(dest.Attributes.Default.Count == 0);  //the base 64 encoded source ruleapp is removed
        }
        [Test]
        public void ImportRuleAppIntegrationTests()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportRuleApp(source, dest);
            Assert.IsTrue(source.Entities.Count == dest.Entities.Count);
            Assert.IsTrue(source.EndPoints.Count == dest.EndPoints.Count);
            Assert.IsTrue(source.UdfLibraries.Count == dest.UdfLibraries.Count);
            Assert.IsTrue(source.Categories.Count == dest.Categories.Count);
            Assert.IsTrue(source.DataElements.Count == dest.DataElements.Count);
            Assert.IsTrue(source.RuleSets.Count == dest.RuleSets.Count);
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Ruleapps\\dest_import_only_" + Guid.NewGuid() + ".ruleappx";
            dest.SaveToFile(path);
        }

        [Test]
        public void GetToolkitsTest()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportToolkit(source, dest);
            ObservableCollection<ToolkitContents> toolkits = h.GetToolkits(dest);
            Console.WriteLine("Toolkits Count: " + toolkits[0].Contents.Count);
            Assert.NotNull(toolkits);
            Assert.AreEqual(toolkits[0].Name,source.Name);
            Assert.AreEqual(toolkits[0].Revision, source.Revision.ToString());
            Assert.AreEqual(toolkits[0].GUID, source.Guid.ToString());
        }
        [Test]
        public void DuplicateToolkitTest()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportToolkit(source, dest);
            try
            {
                h.ImportToolkit(source, dest);
                Assert.Fail("Expected an exception from adding a duplicate");
            }
            catch (Exception ex)
            {
               
                Assert.IsTrue(ex.GetType().FullName.Contains("DuplicateToolkitException"));
                Assert.Pass("Threw an expected exception");
            }
        }
        [Test]
        public void DuplicateEntityNameTest()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            EntityDef entity = new EntityDef();
            EntityDef entity2 = new EntityDef();
            entity.Name = "HelloWorld";
            entity.Name = "HelloWorld";
            source.Entities.Add(entity);
            dest.Entities.Add(entity2);


            try
            {
                h.ImportToolkit(source, dest);
                Assert.Fail("Expected InvalidImportException.");
            }
            catch (Exception ex)
            {
                if (ex.GetType().ToString().Contains("InvalidImportException"))
                {
                    Assert.Pass("Got the expected exception");
                }
                else
                {
                    Assert.Fail("Did not get the expected exception.  Instead got: " + ex.GetType().ToString());
                }
               
            }
        }

        [Test]
        public void TestGetDef()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            EntityDef entity = new EntityDef();
            entity.Name = "HelloWorld";
            source.Entities.Add(entity);
            h.ImportToolkit(source, dest);
            ToolkitsContainer  tc = new ToolkitsContainer();
            tc.Toolkits = h.GetToolkits(dest);
            RuleRepositoryDefBase def = tc.GetDef(entity.Guid);
            Assert.IsNotNull(def);
            def = tc.GetDef(Guid.NewGuid());
            Assert.IsNull(def); //should return null becuase it's not there
        }

        [Test]
        public void TestDeepFindDef()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportToolkit(source, dest);
            RuleRepositoryDefBase found = h.FindDefDeep(dest, "ece7c19a-f8c1-4212-9cc2-90f6dcf837cf");
            Assert.NotNull(found);
        }
        [Test]
        public void TestCountSummary()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            ObservableCollection<ArtifactCount> result = new ObservableCollection<ArtifactCount>();
            h.CountArtifactsByType(source, result);
            foreach (ArtifactCount item in result)
            {
                Console.WriteLine(item.ArtifcatType + " - " + item.Count);
            }
        }
        [Test]
        public void TestCountInDirectory()
        {
            Helper h = new Helper();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
                AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\");
            //RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            ObservableCollection<ArtifactCount> result = new ObservableCollection<ArtifactCount>();
            h.CountArtifactsByTypeBatch(path, result);
            foreach (ArtifactCount item in result)
            {
                Console.WriteLine("\"" + item.ArtifcatType + "\", " + item.Count);
            }
        }
        [Test]
        public void TestImportDefByCategory()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
            h.ImportRuleApp(source, dest, "Test");
            
            //TESTS
            Assert.NotNull(dest.FindDef("Entity1"));

        }

    }
}
