using System.Windows.Controls;
using InRule.Authoring.Services;

namespace InRule.Addins.ScenarioManager
{
    class HtmlContentProvider : IContentProvider
    {
        public Control GetContentControl(object o)
        {
            if (o is HtmlDef)
            {
                var control = new HtmlContentControl(((HtmlDef)o).Content);
                return control;
            }

            return null;
        }
    }
}
