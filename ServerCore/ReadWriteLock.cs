namespace ServerCore;

//재귀적 락을 허용할 것인지?
//스핀락 정책 (5000번 -> Yield)
public class ReadWriteLock
{
    private const int EMPTY_FLAG = 0x00000000;
    private const int WRITE_MASK = 0x7FFF0000;
    private const int READ_MASK = 0x0000FFFF;
    private const int MAX_SPIN_COUNT = 5000;
    //[Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
    private int _flag = EMPTY_FLAG;
    private int _writeCount = 0;

    public void WriteLock()
    {
        int lockThreadId = (_flag & WRITE_MASK) >> 16;
        if (lockThreadId == Thread.CurrentThread.ManagedThreadId)
        {
            _writeCount++;
            return;
        }
        
        int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
        //아무도 WriteLock 또는 ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                {
                    _writeCount = 1;
                    return;
                }
            }
            Thread.Yield();
        }
    }
    public void WriteUnlock()
    {
        int lockCount = --_writeCount;
        if (lockCount == 0)
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
    }
    public void ReadLock()
    {
        int lockThreadId = (_flag & WRITE_MASK) >> 16;
        if (lockThreadId == Thread.CurrentThread.ManagedThreadId)
        {
            Interlocked.Increment(ref _flag);
            return;
        }
        
        //아무도 WriteLock을 획득하고 있지 않을 때, ReadCount를 1 늘린다.
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                int expected = (_flag & READ_MASK);
                if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                    return;
            }
            Thread.Yield();
        }
    }
    public void ReadUnlock()
    {
        Interlocked.Decrement(ref _flag);
    }
}