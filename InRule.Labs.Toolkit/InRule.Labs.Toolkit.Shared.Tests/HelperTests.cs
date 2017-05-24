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
        //private string _sourcePath = Path.Combine(Environment.CurrentDirectory, @"Ruleapps\", "SourceRuleApplication.ruleappx");
        //private string _destPath = Path.Combine(Environment.CurrentDirectory, @"Ruleapps\", "DestRuleApplication.ruleappx");
        //AppDomain.CurrentDomain.BaseDirectory,
        private string _sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ruleapps\", "SourceRuleApplication.ruleappx");
        private string _destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Ruleapps\", "DestRuleApplication.ruleappx");

        [Test]
        public void MakeTempPathTest()
        {
            //Not a great test but it helped get the framework up and running
            Helper h = new Helper();
            string path = h.GetTmpPath();
            Assert.IsNotNull(path);
            Assert.AreNotEqual(path,"");
        }
        [Test]
        public void MakeStampTest()
        {
            Helper h = new Helper();
            Console.WriteLine(_sourcePath);
            RuleApplicationDef def = RuleApplicationDef.Load(_sourcePath);
            Assert.NotNull(def);
            h.MakeStamp(def);
            Assert.AreEqual("SourceRuleApplication,1,9dd52e90-cdbb-4d1b-81f2-9956608c8793", h._stamp);
        }


    }
}
