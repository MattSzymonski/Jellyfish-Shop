using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Backend.Models;
using Backend.Services;
using AutoMapper;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Controllers
{
    [ApiController]
    [EnableCors]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class JellyfishController : ControllerBase
    {
        private readonly ILogger<JellyfishController> logger;
        private readonly IMapper mapper;
        private readonly JellyfishService jellyfishService;

        public JellyfishController(ILogger<JellyfishController> logger, IMapper mapper, JellyfishService jellyfishService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.jellyfishService = jellyfishService;
        }

        public class JellyfishDataFilter : IDataFilter<Jellyfish>
        {
            public String Name { get; set; }

            [EnumDataType(typeof(JellyfishBehaviour))]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public JellyfishBehaviour? Behaviour { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime? MinAddDate { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime? MaxAddDate { get; set; }

            public double? MinPrice { get; set; }

            public double? MaxPrice { get; set; }

            public FilterDefinition<Jellyfish> GetFilter()
            {
                var filter = Builders<Jellyfish>.Filter.Empty;

                if (Name != null) filter &= Builders<Jellyfish>.Filter.Where(item => item.Name.ToLower().Contains(Name.ToLower()));
                if (Behaviour != null) filter &= Builders<Jellyfish>.Filter.Eq(item => item.Behaviour, Behaviour);
                if (MinAddDate != null) filter &= Builders<Jellyfish>.Filter.Where(item => item.AddDate >= MinAddDate);
                if (MaxAddDate != null) filter &= Builders<Jellyfish>.Filter.Where(item => item.AddDate <= MaxAddDate);
                if (MinPrice != null) filter &= Builders<Jellyfish>.Filter.Where(item => item.Price <= MinPrice);
                if (MaxPrice != null) filter &= Builders<Jellyfish>.Filter.Where(item => item.Price <= MaxPrice);

                return filter;
            }
        }

        // --- Endpoints ---

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<JellyfishDTO>>> GetSingleJellyfish(string id)
        {
            // Call service
            var jellyfishResult = await jellyfishService.GetSingleJellyfish(id);

            // Prepare result
            Func<JellyfishDTO> dataConversion = () => 
            {
                // Convert data to DTO
                return mapper.Map<JellyfishDTO>(jellyfishResult.Data);
            };

            var requestResult = jellyfishResult.ConvertToRequestResult<JellyfishDTO>(dataConversion);

            // Return
            return new OkObjectResult(requestResult);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PageDTO<JellyfishDTO>>>> GetMultipleJellyfish(
            [FromQuery] JellyfishDataFilter dataFilter, 
            [FromQuery] DataSorter dataSorter, 
            [FromQuery] PaginationFilter paginationFilter)
        {
            // Validate
            var validPaginationFilter = new PaginationFilter(paginationFilter.PageNumber, paginationFilter.PageSize);
            var validSorter = new DataSorter(dataSorter.SortProperty, dataSorter.Ascending, "Name");

            // Call service
            var jellyfishPageResult = await jellyfishService.GetMultipleJellyfish(dataFilter, validSorter, validPaginationFilter);

            // Prepare result
            Func<PageDTO<JellyfishDTO>> dataConversion = () => 
            {
                // Convert data to DTO
                var jellyfishPageResultDTO = new PageDTO<JellyfishDTO> 
                {
                    Items = jellyfishPageResult.Data.Items.Select(item => { return mapper.Map<JellyfishDTO>(item); }).ToList(),  // Convert Jellyfish to JellyfishDTO
                    CurrentPage = jellyfishPageResult.Data.CurrentPage,
                    TotalItemCount = jellyfishPageResult.Data.TotalItemCount,
                    TotalPageCount = jellyfishPageResult.Data.TotalPageCount,
                };

                return jellyfishPageResultDTO;
            };

            var requestResult = jellyfishPageResult.ConvertToRequestResult<PageDTO<JellyfishDTO>>(dataConversion);

            return new OkObjectResult(requestResult);
        }

        [HttpPost]
        public async Task<ActionResult<Result<JellyfishDTO>>> AddJellyfish([FromBody] AddJellyfishDTO addJellyfishDTO)
        {
            // Call service
            var jellyfishResult = await jellyfishService.AddSingleJellyfish(addJellyfishDTO);

            // Prepare result
            Func<JellyfishDTO> dataConversion = () => 
            {
                // Convert data to DTO
                return mapper.Map<JellyfishDTO>(jellyfishResult.Data);
            };

            var requestResult = jellyfishResult.ConvertToRequestResult<JellyfishDTO>(dataConversion);


            //JellyfishDTO jellyfishDTO = mapper.Map<JellyfishDTO>(jellyfish);
            
            // Return
            return new OkObjectResult(requestResult);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool?>>> DeleteJellyfish(string id)
        {
            // Call service
            var deleteResult = await jellyfishService.DeleteJellyfish(id);

            var requestResult = deleteResult.ConvertToRequestResult<bool?>(null);
            
            // Return
            return new OkObjectResult(requestResult);// result ? new OkResult() : new NotFoundResult();
        }
    }
}