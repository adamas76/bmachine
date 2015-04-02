// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections;


namespace ws.winx.csharp.extensions
{
	public static class IListExtensions
	{

		public static int FindIndex<T>(this IList<T> source, T value,
		                               Func<T,T,bool> match,int startIndex=0)
		{
			int count=source.Count;
			for (int i = startIndex; i < count; i++)
			{
				if (match(source[i],value))
				{
					return i;
				}
			}
			return -1;
		}
	}
}

