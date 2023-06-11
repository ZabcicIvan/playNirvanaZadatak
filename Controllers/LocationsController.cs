using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace playNirvanaZadatak.Controllers
{
    [ApiController]
    [Route("api/locations/LocationsController")]
    public class LocationsController : ControllerBase
    {
        private readonly string apiKey = "AIzaSyBNqYQufmjPmLzLsaZ6sF1tWEoVidITUfg";

        [HttpGet]
        public async Task<IActionResult> GetLocationsInRadius([FromBody] LocationRequest request)
        {
            try
            {
                string lat = $"{request.Latitude}";
                string lng = $"{request.Longitude}";

                var location = lat.Replace(',','.') + "," + lng.Replace(',', '.');

                var places = await GetNearbyPlaces(location, request.Radius, request.Type);

                var locations = places.Select(p => new Location
                {
                    Naziv = p["name"].ToString(),
                    Latitude = double.Parse(p["geometry"]["location"]["lat"].ToString()),
                    Longitude = double.Parse(p["geometry"]["location"]["lng"].ToString()),
                    Adresa = p["vicinity"].ToString()
                }).ToList();

                var requests = new LocationRequest
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Radius = request.Radius,
                    Type = request.Type
                };

                using (var db = new ZadatakDatabase())
                {
                    db.Requests.Add(requests);
                    db.Responses.AddRange(locations);
                    db.SaveChanges();
                }

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<List<Dictionary<string, dynamic>>> GetNearbyPlaces(string location, int radius, string type)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&radius={radius}&type={type}&key={apiKey}";

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<GooglePlacesResponse>(content);

                    return result.Results;
                }

                throw new Exception("Error retrieving nearby places.");
            }
        }
    }

    public class LocationRequest
    {
        public int LocationRequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; }
        public string Type { get; set; }
    }

    public class Location
    {
        public int LocationId { get; set; }
        public string Naziv { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Adresa { get; set; }
    }

    public class GooglePlacesResponse
    {
        public List<Dictionary<string, dynamic>> Results { get; set; }
    }
}
