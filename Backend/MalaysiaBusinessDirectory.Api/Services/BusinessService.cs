using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using MalaysiaBusinessDirectory.Api.Data;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Models;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly ApplicationDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public BusinessService(ApplicationDbContext context)
        {
            _context = context;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326); // 4326 is the SRID for WGS84
        }

        public async Task<IEnumerable<BusinessDto>> GetAllBusinessesAsync()
        {
            var businesses = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                    .ThenInclude(bt => bt.Tag)
                .ToListAsync();

            return businesses.Select(MapToDto);
        }

        public async Task<BusinessDto?> GetBusinessByIdAsync(Guid id)
        {
            var business = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
                return null;

            return MapToDto(business);
        }

        public async Task<IEnumerable<BusinessDto>> SearchBusinessesAsync(BusinessSearchDto searchDto)
        {
            var query = _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                    .ThenInclude(bt => bt.Tag)
                .AsQueryable();

            // Apply name search filter
            if (!string.IsNullOrWhiteSpace(searchDto.Query))
            {
                query = query.Where(b => EF.Functions.ILike(b.Name, $"%{searchDto.Query}%"));
            }

            // Apply category filter
            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == searchDto.CategoryId);
            }

            // Apply geospatial filter
            if (searchDto.Latitude.HasValue && searchDto.Longitude.HasValue)
            {
                var userLocation = _geometryFactory.CreatePoint(new Coordinate(searchDto.Longitude.Value, searchDto.Latitude.Value));
                query = query.Where(b => b.Location != null)
                             .OrderBy(b => b.Location!.Distance(userLocation));
                
                // Filter by radius if specified (convert to degrees for spatial distance)
                if (searchDto.Radius > 0)
                {
                    // Approximate conversion from meters to degrees (this is a simplification, not exact)
                    // A more accurate calculation would use Earth's radius at the specific latitude
                    double radiusInDegrees = searchDto.Radius / 111000.0; // 1 degree ~ 111km at the equator
                    query = query.Where(b => b.Location!.Distance(userLocation) <= radiusInDegrees);
                }
            }

            // Apply pagination
            var skip = (searchDto.Page - 1) * searchDto.PageSize;
            var businesses = await query
                .Skip(skip)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return businesses.Select(MapToDto);
        }

        public async Task<BusinessDto> CreateBusinessAsync(BusinessCreateDto businessDto)
        {
            var business = new Business
            {
                Name = businessDto.Name,
                Description = businessDto.Description,
                Address = businessDto.Address,
                City = businessDto.City,
                PostalCode = businessDto.PostalCode,
                Phone = businessDto.Phone,
                Email = businessDto.Email,
                Website = businessDto.Website,
                Images = businessDto.Images != null ? string.Join(",", businessDto.Images) : null,
                OpeningHours = businessDto.OpeningHours,
                CategoryId = businessDto.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Set location if coordinates are provided
            if (businessDto.Latitude.HasValue && businessDto.Longitude.HasValue)
            {
                business.Location = _geometryFactory.CreatePoint(
                    new Coordinate(businessDto.Longitude.Value, businessDto.Latitude.Value));
            }

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            // Add tags if provided
            if (businessDto.TagIds != null && businessDto.TagIds.Any())
            {
                foreach (var tagId in businessDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        business.BusinessTags.Add(new BusinessTag
                        {
                            BusinessId = business.Id,
                            TagId = tagId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Reload the business with relationships
            var createdBusiness = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Id == business.Id);

            return MapToDto(createdBusiness!);
        }

        public async Task<BusinessDto?> UpdateBusinessAsync(Guid id, BusinessUpdateDto businessDto)
        {
            var business = await _context.Businesses
                .Include(b => b.BusinessTags)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(businessDto.Name))
                business.Name = businessDto.Name;
            
            if (businessDto.Description != null)
                business.Description = businessDto.Description;
            
            if (businessDto.Address != null)
                business.Address = businessDto.Address;
            
            if (businessDto.City != null)
                business.City = businessDto.City;
            
            if (businessDto.PostalCode != null)
                business.PostalCode = businessDto.PostalCode;
            
            if (businessDto.Phone != null)
                business.Phone = businessDto.Phone;
            
            if (businessDto.Email != null)
                business.Email = businessDto.Email;
            
            if (businessDto.Website != null)
                business.Website = businessDto.Website;
            
            if (businessDto.Images != null)
                business.Images = string.Join(",", businessDto.Images);
            
            if (businessDto.OpeningHours != null)
                business.OpeningHours = businessDto.OpeningHours;
            
            if (businessDto.CategoryId.HasValue)
                business.CategoryId = businessDto.CategoryId;

            // Update location if coordinates are provided
            if (businessDto.Latitude.HasValue && businessDto.Longitude.HasValue)
            {
                business.Location = _geometryFactory.CreatePoint(
                    new Coordinate(businessDto.Longitude.Value, businessDto.Latitude.Value));
            }

            business.UpdatedAt = DateTime.UtcNow;

            // Update tags if provided
            if (businessDto.TagIds != null)
            {
                // Remove existing tags
                _context.BusinessTags.RemoveRange(business.BusinessTags);

                // Add new tags
                foreach (var tagId in businessDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        business.BusinessTags.Add(new BusinessTag
                        {
                            BusinessId = business.Id,
                            TagId = tagId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Reload the business with relationships
            var updatedBusiness = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Id == business.Id);

            return MapToDto(updatedBusiness!);
        }

        public async Task<bool> DeleteBusinessAsync(Guid id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
                return false;

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to map Business entity to BusinessDto
        private BusinessDto MapToDto(Business business)
        {
            return new BusinessDto
            {
                Id = business.Id,
                Name = business.Name,
                Description = business.Description,
                Address = business.Address,
                City = business.City,
                PostalCode = business.PostalCode,
                Phone = business.Phone,
                Email = business.Email,
                Website = business.Website,
                Images = business.Images?.Split(',', StringSplitOptions.RemoveEmptyEntries),
                OpeningHours = business.OpeningHours,
                Latitude = business.Location?.Y,
                Longitude = business.Location?.X,
                CategoryId = business.CategoryId,
                Category = business.Category != null ? new CategoryDto
                {
                    Id = business.Category.Id,
                    Name = business.Category.Name,
                    Description = business.Category.Description,
                    Icon = business.Category.Icon,
                    Image = business.Category.Image,
                    CreatedAt = business.Category.CreatedAt,
                    UpdatedAt = business.Category.UpdatedAt
                } : null,
                CreatedAt = business.CreatedAt,
                UpdatedAt = business.UpdatedAt,
                Tags = business.BusinessTags
                    .Select(bt => new TagDto
                    {
                        Id = bt.Tag.Id,
                        Name = bt.Tag.Name,
                        Description = bt.Tag.Description,
                        CreatedAt = bt.Tag.CreatedAt,
                        UpdatedAt = bt.Tag.UpdatedAt
                    })
                    .ToList()
            };
        }
    }
}
