using System;
using System.Collections.Generic;
using System.Linq;
using DataStorage.AzureBlob;
using DataStorage.Data;
using DataStorage.Models;

namespace DataStorage
{
    internal class Test
    {
        public class DataStorage {
            public void Test<T>() where T : class
            {
                var instance = Activator.CreateInstance(typeof(T)) as T ?? throw new NullReferenceException();
                var props = instance.GetType().GetProperties();
                foreach(var prop in props)
                {
                    var blob = CloudBlockBlobFactory.Create("connectionString", "containerName", prop.Name);
                    var propInstance = Activator.CreateInstance(prop.PropertyType, "connectionString");
                    prop.SetValue(instance, propInstance, null);
                    if(prop.GetValue(instance) == null) throw new Exception($"Could not create a instancd for {prop.Name}");
                } 
            }
        }

        public class CustomDataStorage: DataStorage
        {
            public CustomDataStorage():base(){}
            
            public DataRepository<User> Users { get; set; }
        }    
        
        public class User:Entity{} 
    }
       
}