using System.Net.Sockets;
using System.Net;

namespace TongBuilder.Infrastructure.Implementions
{
    internal class UniqueIdGenerator : IUniqueIdGenerator
    {
        //基准时间
        private const long Twepoch = 1288834974657L;
        //机器标识位数
        private const int WorkerIdBits = 8;
        //序列号识位数
        private const int SequenceBits = 12;
        //时钟回拨位数
        private const int ClockBackBits = 2;
        //机器号最大值
        private const long MaxWorkerId = -1L ^ -1L << WorkerIdBits;
        //回拨位最大值
        private const long MaxClockBackId = -1L ^ -1L << ClockBackBits;
        //序列号ID最大值
        private const long SequenceMask = -1L ^ -1L << SequenceBits;
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits + ClockBackBits;
        //时间毫秒左移22位
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + ClockBackBits;

        private long _workerId = 1L;
        private long _sequence = 0L;
        private long _clockBackId = 0L;
        private long _maxTimestamp = -1L;
        private readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly object _lock = new object();

        public UniqueIdGenerator()
        {
            var address = GetIPAddress();
            if (address != null)
            {
                string[] segments = address.ToString().Split('.');
                if (segments != null && segments.Length == 4)
                {
                    _workerId = int.Parse(segments[3]);
                }
            }
        }

        public string Generate()
        {
            var uniqueId = Guid.NewGuid().ToString();
            return uniqueId.Replace("-", "");
        }

        public long GenerateLongId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _maxTimestamp)
                {
                    _clockBackId++;
                    if (_clockBackId > MaxClockBackId)
                    {
                        _clockBackId = 0L;
                    }
                }

                if (_maxTimestamp == timestamp)
                {
                    _sequence = _sequence + 1 & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_maxTimestamp);
                    }
                }
                else
                {
                    _sequence = Random.Shared.Next(10);
                }
                if (timestamp > _maxTimestamp)
                {
                    _maxTimestamp = timestamp;
                }
                // 0     41     8       2    12
                //正负位 时间戳 工作中心 回拨 序列号
                var id = timestamp - Twepoch << TimestampLeftShift | _workerId << WorkerIdShift | _clockBackId << SequenceBits | _sequence;
                return id;
            }
        }

        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private long TimeGen()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        private IPAddress? GetIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }
    }
}
