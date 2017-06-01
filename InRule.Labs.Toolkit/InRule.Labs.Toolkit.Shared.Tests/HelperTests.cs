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

namespace InRule.Labs.Toolkit.Shared.Tests
{
    [TestFixture]
    class HelperTests
    {
        private string _sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
            AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\", "SourceRuleApplication.ruleappx");

        private string _destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Substring(0,
            AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"Ruleapps\", "DestRuleApplication.ruleappx");

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
            RuleApplicationDef def = RuleApplicationDef.Load(_sourcePath);
            Assert.NotNull(def);
            Console.WriteLine(def.Name);
            Console.WriteLine(def.Guid);
            Console.WriteLine(def.Revision);
            Console.WriteLine(h.MakeStamp(def));
            Assert.AreEqual("SourceRuleApplication,1,9dd52e90-cdbb-4d1b-81f2-9956608c8793", h.MakeStamp(def));
            //Test the method that sets the member variable fo the stamp
            h._source = def;
            h.MakeStamp();
            Assert.AreEqual("SourceRuleApplication,1,9dd52e90-cdbb-4d1b-81f2-9956608c8793", h._stamp);

        }

        [Test]
        public void IsToolkitMatchTest()
        {
            Helper h = new Helper();
            RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);
            RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);
           

        }
    }
}
