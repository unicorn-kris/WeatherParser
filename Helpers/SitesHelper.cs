using Newtonsoft.Json;
using System.Reflection;
using WeatherParser.Service.Entities;

namespace Helpers
{
    /// <summary>
    /// This helper used for saving information about included "plugins" with sites parsers thats saved by developers in SitesHelperCollection.cs
    /// </summary>
    public class SitesHelper
    {
        //Saving states of fields from SitesHelperCollection in Json
        public static Task SaveSites()
        {
            List<SiteService> values = new List<SiteService>();

            //find all static fields in SitesHelperCollection
            FieldInfo[] fields = typeof(SitesHelperCollection).GetFields(BindingFlags.Public | BindingFlags.Static);

            //bring in collection
            foreach (FieldInfo field in fields)
            {
                values.Add(new SiteService() { Name = field.Name, ID = new Guid(field.GetValue(null).ToString()) });
                //If the field is static, obj is ignored. For non-static fields, obj should be an instance of a class that inherits or declares the field.
            }

            //serialize collection in the file
            using (var file = new StreamWriter("../Helpers/Sites.json"))
            {
                foreach (var site in values)
                {
                    string jsonString = JsonConvert.SerializeObject(site);
                    file.WriteLine(jsonString);
                }
            }

            return Task.CompletedTask;
        }
    }
}
