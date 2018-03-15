using System;

public class TimeValue
{
    private int m_avoidTime = 0;
    private DateTime m_avoidLastOpTime;
    public int Value
    {
        get
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now.Subtract(m_avoidLastOpTime);
            int val = m_avoidTime - ((span.Seconds) + span.Minutes * 60 + span.Hours * 60 * 60);
            if (val < 0)
            {
                val = 0;
            }
            return val;
        }
        set
        {
            m_avoidLastOpTime = DateTime.Now;
            m_avoidTime = value;
        }
    }

    public override string ToString()
    {
        int hour = Value / 3600;
        int min = (Value % 3600) / 60;
        int sec = (Value % 60);
        return string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
    }

    public string ToShortString()
    {
        int hour = Value / 3600;
        int min = (Value % 3600) / 60;
        int sec = (Value % 60);
        return GetNormalStr(min) + ":" + GetNormalStr(sec);
    }
    string GetNormalStr(int time)
    {
        string str = "";

        if (time >= 10)
        {
            str = time.ToString();
        }
        else if (time > 0)
        {
            str = "0" + time.ToString();
        }
        else
        {
            str = "00";
        }

        return str;
    }
}