using ReportPortal.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;

namespace ReportPortal.Client.Converters
{
    public class ModelSerializer
    {
        public static T Deserialize<T>(string json)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
       
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (T)serializer.ReadObject(stream);
        }

        public static string Serialize<T>(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            var bytes = stream.ToArray();
            return Encoding.UTF8.GetString(bytes, 0 , bytes.Length);
        }
    }
}
