﻿using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Helpers
{
    /// <summary>
    /// This helper used for saving information about included "plugins" with sites parsers thats saved by developers in SitesHelperCollection.cs
    /// </summary>
    internal class SitesHelper
    {
        //Saving states of fields from SitesHelperCollection in XML
        internal static void SaveSites()
        {
            List<string> values = new List<string>();
            List<Type> types = new List<Type>();
            types.Add(typeof(Guid));

            //find all static fields in SitesHelperCollection
            FieldInfo[] fields = typeof(SitesHelperCollection).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            //bring in collection
            foreach (FieldInfo field in fields)
            {
                values.Add(field.Name);
                //If the field is static, obj is ignored. For non-static fields, obj should be an instance of a class that inherits or declares the field.
                values.Add(field.GetValue(null).ToString());
            }

            //serialize collection in the file
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.NewLineHandling = NewLineHandling.Entitize;
            XmlSerializer ser = new XmlSerializer(typeof(List<object>), types.ToArray());

            using (var file = new FileStream("../Helpers/Sites.xml", FileMode.OpenOrCreate))
            {
                using (var xmlWriter = XmlWriter.Create(file, xmlSettings))
                {
                    ser.Serialize(file, values);
                }
            }
        }
    }
}
