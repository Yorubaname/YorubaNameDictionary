namespace Core.Repositories;

public interface ISuggestedNameRepository
{
    Task<Dictionary<string, int>> Count();

}
