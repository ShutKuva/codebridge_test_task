using Adapters.Abstractions;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dogs.Controllers
{
    [ApiController]
    public class DogsController : ControllerBase
    {
        private readonly IDogsAdapter _dogsAdapter;

        public DogsController(IDogsAdapter dogsAdapter)
        {
            _dogsAdapter = dogsAdapter;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Dogs house service. Version 1.0.1");
        }

        [HttpGet("dogs")]
        public async Task<IActionResult> Dogs(string? attribute, string? order, int? pageNumber, int? pageSize)
        {
            return Ok(await _dogsAdapter.GetDogsAsync(attribute, order, pageNumber, pageSize));
        }

        [HttpPost("dog")]
        public async Task<IActionResult> Dog(DogInfo dog)
        {
            await _dogsAdapter.AddDogAsync(dog);

            return Ok("Dog was added.");
        }
    }
}
