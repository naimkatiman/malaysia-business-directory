using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/businesses")]
    public class BusinessesController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessesController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessDto>>> GetBusinesses([FromQuery] BusinessSearchDto searchDto)
        {
            if (searchDto.Query == null && searchDto.CategoryId == null && 
                searchDto.Latitude == null && searchDto.Longitude == null)
            {
                // If no search parameters, return all businesses
                var businesses = await _businessService.GetAllBusinessesAsync();
                return Ok(businesses);
            }
            else
            {
                // Search businesses with filters
                var businesses = await _businessService.SearchBusinessesAsync(searchDto);
                return Ok(businesses);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessDto>> GetBusiness(Guid id)
        {
            var business = await _businessService.GetBusinessByIdAsync(id);
            if (business == null)
                return NotFound();

            return Ok(business);
        }

        [HttpPost]
        public async Task<ActionResult<BusinessDto>> CreateBusiness(BusinessCreateDto businessDto)
        {
            var business = await _businessService.CreateBusinessAsync(businessDto);
            return CreatedAtAction(nameof(GetBusiness), new { id = business.Id }, business);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BusinessDto>> UpdateBusiness(Guid id, BusinessUpdateDto businessDto)
        {
            var business = await _businessService.UpdateBusinessAsync(id, businessDto);
            if (business == null)
                return NotFound();

            return Ok(business);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBusiness(Guid id)
        {
            var result = await _businessService.DeleteBusinessAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
