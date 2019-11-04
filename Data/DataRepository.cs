using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DataStorage.Models;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace DataStorage.Data
{
    public class DataRepository<T> where T : Entity, new()
    {
        private int? idCount = null;
        private CloudBlockBlob blob;
        private List<T> blobContent => JsonConvert.DeserializeObject<List<T>>(blob?.DownloadText());

        public DataRepository(){}

        public DataRepository(CloudBlockBlob blob)
        {
            // If file does not exists or is empty
            if(!blob.Exists() || string.IsNullOrEmpty(blob.DownloadText()))
            {
                // Add a new T instance serialized to the file
                blob.UploadText(JsonConvert.SerializeObject(new T()));
            }

            this.blob = blob;
        }

        public void SetBlob(CloudBlockBlob blob)
        {
             // If file does not exists or is empty
            if(!blob.Exists() || string.IsNullOrEmpty(blob.DownloadText()))
            {
                // Add a new T instance serialized to the file
                blob.UploadText(JsonConvert.SerializeObject(new T()));
            }

            this.blob = blob;
        }
    
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual List<T> GetAll()
            => blobContent?.OrderBy(i => i.Id).ToList();


        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual List<T> Search(Func<T, bool> predicate)
            => blobContent?.Where(predicate).ToList();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual T GetById(int id)
            => blobContent?.FirstOrDefault(item => item.Id == id); 

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Add(T item)
        {
            try{
                item.Id = ++idCount ?? -int.MaxValue;
                var newBlobContent = new List<T>(blobContent);
                newBlobContent.Add(item);
                UpdateBlobContent(newBlobContent);
                return true;
            }catch{
                return false;
            }            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Update(T item)
        {
            try{
                var newBlobContent = new List<T>(blobContent);
                int v = newBlobContent.RemoveAll(i => i.Id == item.Id);
                newBlobContent.Add(item);
                UpdateBlobContent(newBlobContent);
                return v > 0;
            }catch{
                return false;
            }
            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Remove(Predicate<T> match)
        {
            try{
                var newBlobContent = new List<T>(blobContent);
                int v = newBlobContent.RemoveAll(match);
                UpdateBlobContent(newBlobContent);
                return v > 0;
            }catch{
                return false;
            }            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool RemoveById(int id)
        {
            var newBlobContent = new List<T>(blobContent);
            int v = newBlobContent.RemoveAll(item => item.Id == id);
            UpdateBlobContent(newBlobContent);
            return  v > 0;     
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void UpdateBlobContent(List<T> newBlobContent)
        {
            string json = JsonConvert.SerializeObject(newBlobContent);
            blob?.UploadText(json);
        }
    }

}