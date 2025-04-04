using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MalaysiaBusinessDirectory.Api.DTOs;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public interface IBusinessService
    {
        Task<IEnumerable<BusinessDto>> GetAllBusinessesAsync();
        Task<BusinessDto?> GetBusinessByIdAsync(Guid id);
        Task<IEnumerable<BusinessDto>> SearchBusinessesAsync(BusinessSearchDto searchDto);
        Task<BusinessDto> CreateBusinessAsync(BusinessCreateDto businessDto);
        Task<BusinessDto?> UpdateBusinessAsync(Guid id, BusinessUpdateDto businessDto);
        Task<bool> DeleteBusinessAsync(Guid id);
    }
}
