using System;

namespace KC
{
    public class TimeInfo : Singleton<TimeInfo>,ISingletonAwake
    {
        private int _timeZone;
        
        public int TimeZone
        {
            get => this._timeZone;
            set
            {
                this._timeZone = value;
                _dt = _dt1970.AddHours(TimeZone);
            }
        }
        
        private DateTime _dt1970;
        private DateTime _dt;
        
        public long FrameTime { get; private set; }
        
        
        public void Awake()
        {
            this._dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this._dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.FrameTime = this.ClientNow();
        }

        public void Update()
        {
            this.FrameTime = this.ClientNow();
        }
        
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return _dt.AddTicks(timeStamp * 10000);
        }
        
        // 线程安全
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this._dt1970.Ticks) / 10000;
        }
        
        
        public long ClientFrameTime()
        {
            return this.FrameTime;
        }
        
        public long Transition(DateTime d)
        {
            return (d.Ticks - _dt.Ticks) / 10000;
        }
    }
}