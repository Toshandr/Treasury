using Treasury.Api.Dtos;

namespace Treasury.Api.Services;

public interface IRestoreService
{
    RestoreResponse Restore(string reference, string damaged);
}
