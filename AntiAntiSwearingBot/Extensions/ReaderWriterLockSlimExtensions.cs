using System.Threading;

namespace AntiAntiSwearingBot;

public readonly ref struct ReadLockToken
{
    ReaderWriterLockSlim Lock { get; }
    public ReadLockToken(ReaderWriterLockSlim l) => (Lock = l).EnterReadLock();
    public void Dispose() => Lock.ExitReadLock();
}

public readonly ref struct WriteLockToken
{
    ReaderWriterLockSlim Lock { get; }
    public WriteLockToken(ReaderWriterLockSlim l) => (Lock = l).EnterWriteLock();
    public void Dispose() => Lock.ExitWriteLock();
}

public static class ReaderWriterLockSlimExtensions
{
    public static ReadLockToken GetReadLockToken(this ReaderWriterLockSlim l) => new ReadLockToken(l);
    public static WriteLockToken GetWriteLockToken(this ReaderWriterLockSlim l) => new WriteLockToken(l);
}
