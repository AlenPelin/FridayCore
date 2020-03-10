using System.Collections.Generic;

namespace FridayCore.Data
{
    public interface ISharpItemsModule
    {
        DatabaseName DatabaseName { get; }

        IEnumerable<ItemDef> GetItems();
    }
}