using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Sitecore.Data;
using System.Collections.Specialized;

namespace ENBDGroup.Foundation.Common.Core.Extensions
{
    public static class LinqExtensions
    {
        public static List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();
            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            return randomList; //return the new random list
        }
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        public static NameValueCollection UpdateCustomAttributes(this object obj, Guid Id, string fieldName)
        {
            NameValueCollection nvc = new NameValueCollection();
            var jsonConvertedObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(obj));
            foreach (KeyValuePair<string, string> item in jsonConvertedObj)
            {
                nvc.Add(item.Key, item.Value);
            }
            try
            {
                var currentItem = Sitecore.Context.Database.GetItem(ID.Parse(Id));
                if (currentItem != null)
                {
                    var jsonData = currentItem.GetCustomAttributes(fieldName);
                    if (obj != null && !string.IsNullOrEmpty(jsonData))
                    {
                        var jsonConvertedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                        Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
                        foreach (var data in jsonConvertedData)
                        {
                            jsonDictionary.Add(data.Key, data.Value);
                        }

                        foreach (KeyValuePair<string, string> item in jsonDictionary)
                        {
                            nvc.Add(item.Key, item.Value);
                        }
                        return nvc;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return nvc;
        }

        public static IDictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            return collection.Cast<string>().ToDictionary(k => k, v => collection[v]);
        }
    }
}
