using System;
using System.Xml.Serialization;
using InRule.Authoring.Services;
using InRule.Authoring.Windows.Controls;
using InRule.Repository;

namespace InRule.Labs.Toolkit.Author
{
    class DockSettings
    {
        [XmlIgnore]
        public IToolWindow ToolWindow { get; set; }

        [XmlIgnore]
        public RuleApplicationService RuleApplicationService { get; set; }
        
        private readonly static AttributeGroupKey _attributeGroupKey = new AttributeGroupKey("RuleSetsByCategorySettings",new Guid("{04757D3F-AD48-4E7D-8073-8B3F1D02FDE5}"));

        public DockDirection DockDirection { get; set; }

        public void SaveCurrentSettings()
        {
            var settings = InRule.Common.Utilities.XmlSerializationUtility.ObjectToXML(this);
            RuleApplicationService.RuleApplicationDef.Attributes[_attributeGroupKey]["Settings"] = settings;
        }

        public void LoadSavedSettings()
        {
            var settings = RuleApplicationService.RuleApplicationDef.Attributes[_attributeGroupKey]["Settings"];
            if (! string.IsNullOrEmpty(settings))
            {
                var o = InRule.Common.Utilities.XmlSerializationUtility.GetObjectFromXmlString(settings, typeof(DockSettings));
                var dockSettings = (DockSettings)o;

                this.DockDirection = dockSettings.DockDirection;

                ToolWindow.Dock(this.DockDirection);

            }
        }
    }
}
