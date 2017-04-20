using System;
using System.IO;
using InRule.Labs.Toolkit.Shared;


namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Helper();
            string destpath = @"C:\Users\Christopher Berg\Documents\DestRuleApplication" + "_" +
                              Guid.NewGuid().ToString() + ".ruleappx";
            File.Copy(@"C:\Users\Christopher Berg\Documents\DestRuleApplication.ruleappx", destpath);
            runner.ImportArtifacts(@"C:\Users\Christopher Berg\Documents\SourceRuleApplication.ruleappx", destpath);
            runner.RemoveArtifacts(@"C:\Users\Christopher Berg\Documents\SourceRuleApplication.ruleappx", destpath);
        }
    }
}
