using System;
using InRule.Authoring;
using InRule.Authoring.Settings;

namespace InRule.Labs.Toolkit.Author
{
    public class MyExtensionSettings : ISettings
    {
        public static Guid Guid = new Guid(@"{45FEFEBA-34D1-415A-A533-A7E2C6EE9D65}");
        public bool MyProperty { get; set; }

        public MyExtensionSettings() {
            MyProperty = false;
        }

        public string Description {
            get { return @"Rule Set by Category Options"; }
        }

        public Guid ID {
            get { return Guid; }
        }
    }
}
