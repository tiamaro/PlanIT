namespace PlanIT.API.Mappers.Interface;

// Generisk interface for å mappe data mellom en modell (TModel) og en DTO (TDto).
public interface IMapper<TModel, TDto>
{
    TDto MapToDTO(TModel model);
    TModel MapToModel(TDto dto);
}