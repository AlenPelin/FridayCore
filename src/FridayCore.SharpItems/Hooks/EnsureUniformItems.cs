using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Events.Hooks;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using FridayCore.Data;

namespace FridayCore.Hooks
{
    public partial class EnsureFridayCoreItems : IHook
    {
        public void Initialize()
        {
            // this is a replacement for Unicorn Serialization to simplify CI 

            var master = Factory.GetDatabase("master", false);
            if (master == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                Initialize(Factory.GetDatabase("core"), CoreDbData.CoreItems);
                Initialize(master, MasterDbData.MasterItems);
            }
        }

        private void Initialize(Database db, List<ItemDef> items)
        {
            items = items.OrderBy(x => x.ItemPath.ToLower()).ToList();

            // todo: template in a folder

            var allTemplates = items.Where(x => x.TemplateID == TemplateIDs.Template).ToArray();
            foreach (var template in allTemplates)
            {
                InitializeSingleTemplate(db, items, template);
            }

            var allStandardValues = items.Where(x => string.Equals(x.Name, "__Standard values", StringComparison.OrdinalIgnoreCase) && allTemplates.Any(z => z.ID == x.TemplateID));
            foreach (var standardValues in allStandardValues)
            {
                InitializeSingleItemAndChildren(db, standardValues, items);
            }

            // remaining items
            foreach (var item in items)
            {
                InitializeSingleItemAndChildren(db, item, items);
            }
        }

        private void InitializeSingleTemplate(Database db, List<ItemDef> items, ItemDef template)
        {
            items.Remove(template);
            InitializeSingleItemOnly(db, template);

            var templateSections = items.Where(x => x.TemplateID == TemplateIDs.TemplateSection && string.Equals(x.ParentItemPath, template.ItemPath, StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var templateSection in templateSections)
            {
                InitializeSingleTemplateSection(db, items, templateSection);
            }
        }

        private void InitializeSingleTemplateSection(Database db, List<ItemDef> items, ItemDef templateSection)
        {
            items.Remove(templateSection);
            InitializeSingleItemOnly(db, templateSection);

            var templateFields = items.Where(x => x.TemplateID == TemplateIDs.TemplateField && string.Equals(x.ParentItemPath, templateSection.ItemPath, StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var templateField in templateFields)
            {
                InitializeSingleItemAndChildren(db, templateField, items);
            }
        }

        private void InitializeSingleItemAndChildren(Database db, ItemDef data, List<ItemDef> items)
        {
            items.Remove(data);
            InitializeSingleItemOnly(db, data);

            var children = items.Where(x => string.Equals(x.ParentItemPath, data.ItemPath, StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var child in children)
            {
                InitializeSingleItemAndChildren(db, child, items);
            }
        }

        private void InitializeSingleItemOnly(Database db, ItemDef data)
        {
            var item = db.GetItem(data.ID);
            if (item == null)
            {
                var parent = db.GetItem(data.ParentItemPath);
                if (parent == null)
                {
                    Logging.Error($"Cannot find the parent item to install the {data.GetDataUri(db)} item, definition:\r\n{JsonConvert.SerializeObject(data, Formatting.Indented)}");
                }

                Logging.Info($"Installing the {data.GetDataUri(db)}");
                item = parent.Add(data.Name, new TemplateID(data.TemplateID), data.ID);
                foreach (var key in data.Keys)
                {
                    var value = data[key];
                    Logging.Info($"Writing the {data.GetDataUri(db)}[\"{key}\"] = \"{value}\"");

                    item[key] = value;
                }

                return;
            }

            if (item.Paths.FullPath != data.ItemPath)
            {
                Logging.Warn($"FridayCore item {data.ID} exists, but the path is different\r\n" +
                    $"  Act: {item.Paths.FullPath}\r\n" +
                    $"  Exp: {data.ItemPath}");
            }

            item.Editing.BeginEdit();
            try
            {
                var keys = new List<string>();
                foreach (var key in data.Keys)
                {
                    var value = data[key];
                    var field = item.Fields[key];
                    if (field == null || item[key] == null)
                    {
                        item[key] = value;
                    }
                    else
                    {
                        if (field.HasValue && field.Value != value && !field.InheritsValueFromOtherItem)
                        {
                            keys.Add(key);
                        }
                        else
                        {
                            item[key] = value;
                        }
                    }

                    if (keys.Any())
                    {
                        Logging.Warn($"The {data.GetDataUri(db)} item field value(s) look different than expected:\r\n{string.Join("\r\n\r\n", keys.Select(x => $"{data.GetDataUri(db)}[\"{key}\"]\r\n  Act: {item[x]}\r\n  Exp: {data[x]}"))}");
                    }
                }
            }
            catch
            {
                item.Editing.CancelEdit();

                throw;
            }

            item.Editing.EndEdit();
        }
    }
}
