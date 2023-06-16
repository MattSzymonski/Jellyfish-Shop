using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Backend.Services
{
    public class UsersService
    {
        private readonly ApplicationSettings settings;
        private readonly ILogger<UsersService> logger;
        private readonly SecurityService securityService;

        private readonly IMongoDatabase mainDatabase;
        private readonly IMongoCollection<User> usersCollection;

        public UsersService(ApplicationSettings settings, ILogger<UsersService> logger, SecurityService securityService)
        {
            this.logger = logger;
            this.settings = settings;
            this.securityService = securityService;

            // Get databases and collections
            var client = new MongoClient(settings.DatabaseSettings.ConnectionString);
            mainDatabase = client.GetDatabase(settings.DatabaseSettings.MainDatabaseName);
            usersCollection = mainDatabase.GetCollection<User>(settings.DatabaseSettings.UsersCollectionName);
        }

        public async Task<Result<User>> GetUserByLogin(string login)
        {
            var filter = Builders<User>.Filter.Eq(o => o.Login, login);
            var user = await DatabaseUtils.FetchItemFromCollection(usersCollection, filter);

            if (user != null) 
            {
                return new Result<User>(Status.Success, "", user);
            }
            else
            {
                return new Result<User>(Status.Failure, $"User with login {login} was not found");    
            }
        }

        public async Task<Result<User>> GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq(o => o.Id, id);
            var user = await DatabaseUtils.FetchItemFromCollection(usersCollection, filter);

            if (user != null) 
            {
                return new Result<User>(Status.Success, "", user);
            }
            else
            {
                return new Result<User>(Status.Failure, $"User with id {id} was not found");    
            }
        }

        public async Task<Result<LoginResponseDTO>> LoginUser(LoginRequestDTO loginRequestDTO)
        {
            var filter = Builders<User>.Filter.Eq(o => o.Login, loginRequestDTO.Login);
            var user = await DatabaseUtils.FetchItemFromCollection(usersCollection, filter);

            if (user != null) 
            {
                if (user.Password == loginRequestDTO.Password) 
                {
                    // Generate token
                    var token = securityService.GenerateTokens(user.Id, user.Login);
                    var endpointResponse = new LoginResponseDTO
                    {
                        Id = user.Id,
                        AccessToken = token,
                    };

                    return new Result<LoginResponseDTO>(Status.Success, "", endpointResponse);
                }
            }

            return new Result<LoginResponseDTO>(Status.Failure, "Invalid credentials");
        }
    }
}





