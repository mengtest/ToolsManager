/************************************************************
//     文件名      : ObjectPool.cs
//     功能描述    : 对象创建池，用于管理创建初始化的对象
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/02.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using System;

public class ObjectPool<T> where T : class
{
    private int m_Capacity;
    private int m_ExpansionLimit;
    private int m_UnitPos;
    private int m_WaitUnitPos;
    private int m_WaitCount;
    private T[] m_Units;
    private Func<T> m_UnitFactory;

    public ObjectPool(int m_Capacity, int m_ExpansionLimit, Func<T> m_UnitFactory)
    {
        this.m_ExpansionLimit = m_ExpansionLimit;
        this.m_UnitFactory = m_UnitFactory;

        Init(m_Capacity);
    }

    public T Fetch()
    {
        T unit;

        unit = (m_UnitPos != m_Capacity) ? unit = m_Units[m_UnitPos++] : (m_Capacity < m_ExpansionLimit ? Expand() : Wait());
        return unit;
    }

    public void Store(T unit)
    {
        if (m_WaitCount == 0)
        {
            m_Units[--m_UnitPos] = unit;
        }
        else
        {
            Pulse(unit);
        }
    }

    private T Expand()
    {
        T unit = null;

        if (m_UnitPos != this.m_Capacity)
        {
            unit = m_Units[m_UnitPos++];
        }
        else
        {
            if (this.m_Capacity == m_ExpansionLimit)
            {
                unit = Wait();
            }
            else
            {
                int m_Capacity = this.m_Capacity;
                int newCapacity = m_Capacity * 2;

                if (newCapacity > m_ExpansionLimit)
                {
                    newCapacity = m_ExpansionLimit;
                }

                T[] newUnits = new T[newCapacity];

                for (int i = m_Capacity; i < newCapacity; i++)
                {
                    newUnits[i] = m_UnitFactory.Invoke();
                }

                Array.Copy(m_Units, 0, newUnits, 0, m_Capacity);
                m_Units = newUnits;
                this.m_Capacity = newCapacity;
                unit = m_Units[m_UnitPos++];
            }
        }


        return unit;
    }

    private T Wait()
    {
        m_WaitCount++;

        return m_Units[--m_WaitUnitPos];
    }

    private void Pulse(T unit)
    {
        m_WaitCount--;
        m_Units[m_WaitUnitPos++] = unit;
    }

    private void Init(int m_Capacity)
    {
        T[] m_Units = new T[m_Capacity];

        for (int i = 0; i < m_Capacity; i++)
        {
            m_Units[i] = m_UnitFactory.Invoke();
        }

        this.m_Units = m_Units;
        this.m_Capacity = m_Capacity;
    }
}