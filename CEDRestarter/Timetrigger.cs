using System;
using System.Threading.Tasks;

namespace CEDRestarter
{
    public class Timetrigger
    {
        readonly TimeSpan triggerHour;

        public Timetrigger(int hour, int minute = 0, int second = 0)
        {
            triggerHour = new TimeSpan(hour, minute, second);
            InitiateAsync();
        }

        async void InitiateAsync()
        {
            while (true)
            {
                var triggerTime = DateTime.Today + triggerHour - DateTime.Now;
                if (triggerTime < TimeSpan.Zero)
                    triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                await Task.Delay(triggerTime);
                OnTimeTriggered?.Invoke();
            }
        }

        public event Action OnTimeTriggered;
    }
}
