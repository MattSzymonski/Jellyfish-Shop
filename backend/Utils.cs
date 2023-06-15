
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend
{
    public interface IDataFilter<T>
    {
        public abstract FilterDefinition<T> GetFilter();
    }

    public class DataSorter
    {
        // Data field by which to sort
        public string SortProperty { get; set; } 
        
        // Direction of filtering
        public bool Ascending { get; set; }

        public DataSorter()
        {
            this.SortProperty = String.Empty;
            this.Ascending = true;
        }

        public DataSorter(string sortProperty, bool ascending, string defaultSortProperty)
        {
            this.SortProperty = string.IsNullOrEmpty(sortProperty) ? defaultSortProperty : sortProperty;
            this.Ascending = ascending;
        }
    }

    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 5;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 5 ? 5 : pageSize;
        }
    }

    public class PageDTO<T> {
        
        public List<T> Items { get; set; }

        public int CurrentPage { get; set; }

        public int TotalItemCount { get; set; }

        public int TotalPageCount { get; set; }
    }

    public static class DatabaseUtils 
    {
        public static async Task<T> FetchItemFromCollection<T>(IMongoCollection<T> collection, FilterDefinition<T> filter)
        {
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<(List<T> items, int totalItemCount)> FetchItemsFromCollection<T>(IMongoCollection<T> collection, IDataFilter<T> dataFilter, DataSorter dataSorter, PaginationFilter paginationFilter)
        {
            var filter = dataFilter.GetFilter();
            var findOptions = new FindOptions<T>
            {
                Sort = dataSorter.Ascending ? Builders<T>.Sort.Ascending(dataSorter.SortProperty) : Builders<T>.Sort.Descending(dataSorter.SortProperty),
                Skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize,
                Limit = paginationFilter.PageSize,
            };

            var filteredItemsCursor = await collection.FindAsync(filter, findOptions);
            var filteredItems = await filteredItemsCursor.ToListAsync();
            
            int filteredItemCount = (int)collection.CountDocuments(filter);

            return (filteredItems, filteredItemCount);
        }

        public static async Task<T> AddItemToCollection<T>(IMongoCollection<T> collection, T item)
        {
            await collection.InsertOneAsync(item);
            return item;
        }

        public static async Task<bool> DeleteItemFromCollection<T>(IMongoCollection<T> collection, string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await collection.DeleteOneAsync(filter);

            return result.DeletedCount == 1;
        }
    }
}