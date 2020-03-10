using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FridayCore.Data
{

    public class ItemDef : Dictionary<string, string>
    {
        private ID RealID;

        public ID ID
        {
            get { return RealID ?? (RealID = GetAutoID(ItemPath)); }
            set
            {
                RealID = value;
            }
        }

        public static ID GetAutoID(string itemPath)
        {
            using (var sha = MD5.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.Default.GetBytes(itemPath));
                return new ID(new Guid(hash));
            }
        }

        public string ItemPath { get; }

        public ID TemplateID { get; }

        public string ParentItemPath => ItemPath.Substring(0, ItemPath.LastIndexOf("/"));

        public string Name => ItemPath.Substring(ItemPath.LastIndexOf("/") + 1);

        public ItemDef(string itemPath, string templateId)
            : this(itemPath, Guid.Parse(templateId))
        {
        }

        public ItemDef(string itemPath, Guid templateId)
            : this(itemPath, new ID(templateId))
        {
        }

        public ItemDef(string itemPath, ID templateId)
        {
            this.ItemPath = itemPath.TrimEnd('/');
            this.TemplateID = templateId;
        }

        internal string GetDataUri(Database db)
        {
            return $"{db.Name}://{ID}{ItemPath}";
        }
    }
}
