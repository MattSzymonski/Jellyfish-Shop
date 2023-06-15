using System.Threading.Tasks;
using Backend.Models;
using Microsoft.Extensions.Logging;
using System;
using MongoDB.Driver;
using AutoMapper;

namespace Backend.Services
{
    public class JellyfishService
    {
        private readonly ApplicationSettings settings;
        private readonly ILogger<JellyfishService> logger;

        private readonly IMongoDatabase mainDatabase;
        private readonly IMongoCollection<Jellyfish> jellyfishCollection;
        private readonly IMapper mapper;

        public JellyfishService(ApplicationSettings settings, ILogger<JellyfishService> logger, IMapper mapper)
        {
            this.settings = settings;
            this.logger = logger;
            this.mapper = mapper;

            // Get databases and collections
            var client = new MongoClient(settings.DatabaseSettings.ConnectionString);
            mainDatabase = client.GetDatabase(settings.DatabaseSettings.MainDatabaseName);
            jellyfishCollection = mainDatabase.GetCollection<Jellyfish>(settings.DatabaseSettings.JellyfishCollectionName);
        }

        public async Task<Result<Jellyfish>> GetSingleJellyfish(string id)
        {
            var jellyfishFilter = Builders<Jellyfish>.Filter.Eq(o => o.Id, id);
            var jellyfish = await jellyfishCollection.Find(jellyfishFilter).FirstOrDefaultAsync();

            if (jellyfish != null) 
            {
                return new Result<Jellyfish>(Status.Success, "", jellyfish);
            }
            else
            {
                return new Result<Jellyfish>(Status.Failure, $"Jellyfish with id {id} was not found");    
            }
        }   

        public async Task<Result<PageDTO<Jellyfish>>> GetMultipleJellyfish(IDataFilter<Jellyfish> dataFilter, DataSorter dataSorter, PaginationFilter paginationFilter)
        {
            var fetchedItems = await DatabaseUtils.FetchItemsFromCollection<Jellyfish>(jellyfishCollection, dataFilter, dataSorter, paginationFilter);

            // Create page
            var result = new PageDTO<Jellyfish>() {
                Items = fetchedItems.items,
                CurrentPage = paginationFilter.PageNumber,
                TotalItemCount = fetchedItems.totalItemCount,
                TotalPageCount = (int)Math.Ceiling((float)fetchedItems.totalItemCount / (float)paginationFilter.PageSize)
            };

            return new Result<PageDTO<Jellyfish>>(Status.Success, "", result);
        }

        public async Task<Result<Jellyfish>> AddSingleJellyfish(AddJellyfishDTO jellyfishData)
        {
            var itemToAdd = mapper.Map<Jellyfish>(jellyfishData);
            itemToAdd.AddDate = DateTime.Now;
            var additionResult = await DatabaseUtils.AddItemToCollection<Jellyfish>(jellyfishCollection, itemToAdd);

            return new Result<Jellyfish>(Status.Success, "", additionResult);
        }

        public async Task<Result<bool?>> DeleteJellyfish(string id)
        {
            var deletionResult = await DatabaseUtils.DeleteItemFromCollection<Jellyfish>(jellyfishCollection, id);
            
            if (deletionResult) 
            {
                return new Result<bool?>(Status.Success, "");
            }
            else
            {
                return new Result<bool?>(Status.Failure, $"Jellyfish with id {id} was not found");  
            }
        }
    }
}