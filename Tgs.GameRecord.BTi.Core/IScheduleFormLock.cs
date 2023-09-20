namespace Tgs.GameRecord.BTi.Core;

public interface IScheduleFormLock
{
    public Task<bool> LockingAsync(long id);

    public Task<bool> UnlockAsync(long id);
}
