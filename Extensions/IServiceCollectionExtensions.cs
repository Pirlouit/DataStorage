using System;
using DataStorage.AzureBlob;
using Microsoft.Extensions.DependencyInjection;

namespace DataStorage.Extensions
{
    public static class IServiceCollectionExtensions
    {
        // TO_DO: split this function
        public static IServiceCollection AddDataStorage<T>(this IServiceCollection service, string connectionString, string containerName = "default") where T : class, new()
        {
            Console.WriteLine("AddDataStorage");
            if(string.IsNullOrEmpty(connectionString)) 
                throw new InvalidOperationException("Connection can not be null or empty");

            var instance = Activator.CreateInstance(typeof(T)) as T ?? throw new NullReferenceException();
            var props = instance.GetType().GetProperties();
            foreach(var prop in props)
            {
                var blob = CloudBlockBlobFactory.Create(connectionString, containerName, prop.Name);
                var propInstance = Activator.CreateInstance(prop.PropertyType, blob);
                prop.SetValue(instance, propInstance, null);
                if(prop.GetValue(instance) == null) 
                    throw new Exception($"Could not create a instance for {prop.Name}");
            }             
            service.AddSingleton<T>(instance);
            return service;
        }
    }
}