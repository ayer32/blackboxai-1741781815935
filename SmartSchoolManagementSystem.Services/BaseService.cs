using AutoMapper;
using SmartSchoolManagementSystem.Core.Entities;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;

    protected BaseService(IRepository<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public virtual async Task<TDto> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with ID {id} was not found.");

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<TDto>>(entities);
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
    {
        var entity = _mapper.Map<TEntity>(createDto);
        
        await _repository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> UpdateAsync(Guid id, TUpdateDto updateDto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with ID {id} was not found.");

        _mapper.Map(updateDto, entity);
        
        await _repository.UpdateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with ID {id} was not found.");

        await _repository.DeleteAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }
}
