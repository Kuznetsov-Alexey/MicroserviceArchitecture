using PlatformService.Dtos;

namespace PlatformService.SyncDataServices;

public interface ICommandDataClient
{
    Task SendPlatformToCommandAsync(PlatformReadDto platform);
}