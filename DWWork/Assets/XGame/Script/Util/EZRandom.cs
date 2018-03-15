
using System;
using System.Collections.Generic;
public class EZRandom 
{
	readonly int[] prime = {30269, 30307, 30323};
	readonly int[] step   = {171,   172,   170};
	int[] m_seed = null;
	public EZRandom(List<int> seed)
	{
		m_seed = new int[prime.Length];
		for (int i=0; i<prime.Length; ++i) 
		{
			m_seed[i] = seed[i];
		}
	}
	public double random()
	{
		for (int i=0; i<prime.Length; ++i) 
		{
			m_seed[i] = (m_seed[i] * step[i]) % prime[i];
		}
		double r = 0;
		for (int i=0; i<prime.Length; ++i)
		{
			r += (double)m_seed[i] / prime[i];
		}
		
		return r - Math.Floor(r);
	}
	public int random(int range)
	{
		double r = random();
		return (int)Math.Floor(r * range);
	}
}

