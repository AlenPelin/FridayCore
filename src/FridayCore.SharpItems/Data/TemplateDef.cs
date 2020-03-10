using Sitecore;

namespace FridayCore.Data
{
    public class TemplateDef : ItemDef
    {
        public TemplateDef(string itemPath)
            : base(itemPath, TemplateIDs.Template)
        {
        }
    }
}
