using AutoMapper;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public abstract class CrudServiceBase<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    protected CrudServiceBase(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    protected async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    protected async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<TDto>(entity);
    }

    protected async Task<TDto> CreateAsync<TCreateDto>(
        TCreateDto createDto,
        Func<TCreateDto, Task>? validateBeforeCreate = null)
    {
        if (validateBeforeCreate != null)
        {
            await validateBeforeCreate(createDto);
        }

        var entity = _mapper.Map<TEntity>(createDto);
        var createdEntity = await _repository.AddAsync(entity);

        return _mapper.Map<TDto>(createdEntity);
    }

    protected async Task<TDto?> UpdateAsync<TUpdateDto>(
        int id,
        TUpdateDto updateDto,
        Action<TEntity, TUpdateDto> applyUpdates,
        Func<int, TEntity, TUpdateDto, Task>? validateBeforeUpdate = null)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null)
        {
            return null;
        }

        if (validateBeforeUpdate != null)
        {
            await validateBeforeUpdate(id, existingEntity, updateDto);
        }

        applyUpdates(existingEntity, updateDto);

        await _repository.UpdateAsync(existingEntity);
        return _mapper.Map<TDto>(existingEntity);
    }

    protected async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        await _repository.DeleteAsync(entity);
        return true;
    }
}
