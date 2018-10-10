using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ApiWithRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IDistributedCache cache;
        private readonly List<Country> countries;
        private readonly string countriesKey = "countries";
        public CountriesController(IDistributedCache cache)
        {
            this.cache = cache;
            countries = new List<Country>()
            {
                new Country() { Id = 1, Name = "Brasil", LastUpdate = DateTime.Now },
                new Country() { Id = 2, Name = "Estados Unidos", LastUpdate = DateTime.Now },
                new Country() { Id = 3, Name = "Irlanda", LastUpdate = DateTime.Now },
            };
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countriesJson = await cache.GetStringAsync(countriesKey);

            if (countriesJson is null)
            {
                countriesJson = JsonConvert.SerializeObject(countries);
                var cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                await cache.SetStringAsync(countriesKey, countriesJson, cacheOptions);
            }

            var countriesFromJson = JsonConvert.DeserializeObject<IEnumerable<Country>>(countriesJson);

            return Ok(countriesFromJson);
        }


    }

    class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
