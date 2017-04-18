using InRule.Labs.Toolkit.Shared;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Helper();
            runner.ImportArtifacts(@"C:\Users\Christopher Berg\Documents\SourceRuleApplication.ruleappx", @"C:\Users\Christopher Berg\Documents\DestRuleApplication.ruleappx");
        }
    }
}
