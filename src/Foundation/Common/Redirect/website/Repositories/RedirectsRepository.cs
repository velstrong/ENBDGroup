using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace ENBDGroup.Foundation.Common.Redirect.Repositories
{
    public static class RedirectsRepository
    {
        public static void Reset()
        {
            List<string> strs = new List<string>();
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string str = enumerator.Key.ToString();
                if (!str.StartsWith("LivApp-Foundation-Redirects-"))
                {
                    continue;
                }
                strs.Add(str);
            }
            foreach (string str1 in strs)
            {
                HttpRuntime.Cache.Remove(str1);
            }
        }
    }
}