using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
namespace LivApp.Foundation.Content.Services
{
    public interface IBaseService
    {
        T GetDataSourceItem<T>() where T : class;
        T GetDataSourceOrCurrentItem<T>() where T : class;
        T GetItem<T>(Guid itemId) where T : class;
        T GetItem<T>(Item item) where T : class;
        List<T> GetItems<T>(IEnumerable<Item> items) where T : class;
        List<T> GetItems<T>(IEnumerable<Guid> itemIds) where T : class;
        string GetRenderingParameter(string parameter);
        int GetIntRenderingParameter(string parameter, int defaultValue = 0);

        bool GetBoolRenderingParameter(string parameter);

        T GetItemFromRenderingParameter<T>(string parameter) where T : class;

        bool IsExperienceEditor { get; }
    }
}
