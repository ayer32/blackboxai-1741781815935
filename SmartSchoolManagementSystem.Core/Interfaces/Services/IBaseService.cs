namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    Task<TDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TDto>> GetAllAsync();
    Task<TDto> CreateAsync(TCreateDto createDto);
    Task<TDto> UpdateAsync(Guid id, TUpdateDto updateDto);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
