using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace playNirvanaZadatak.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly string apiKey = "AIzaSyBNqYQufmjPmLzLsaZ6sF1tWEoVidITUfg";

        [HttpPost]
        public async Task<IActionResult> GetLocationsInRadius([FromBody] LocationRequest request)
        {
            var db = new ZadatakDatabase();
            try
            {
                string lat = $"{ request.Latitude }";
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

                //locations.ForEach(p =>
                //{
                //    db.Responses.Add(p);
                //    db.SaveChanges();
                //});

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
        [Key]
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; }
        public string Type { get; set; }
    }

    public class Location
    {
        [Key]
        public int Id { get; set; }
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
