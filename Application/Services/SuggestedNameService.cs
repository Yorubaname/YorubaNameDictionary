using Core.Repositories;

namespace Application.Services;

public class SuggestedNameService
{
    private readonly ISuggestedNameRepository _suggestedNameRepository;

    public SuggestedNameService(ISuggestedNameRepository suggestedNameRepository)
    {
        _suggestedNameRepository = suggestedNameRepository;
    }

    public async Task<Dictionary<string, int>> Count()
    {
        return await _suggestedNameRepository.Count();
    }
}
