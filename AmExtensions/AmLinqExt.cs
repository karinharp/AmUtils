using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace am
{
public static class LinqExt
{
    static Random _Rand = new Random();
    
    /// <summary>
    ///   ランダムな要素を返す
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     実装参考：https://qiita.com/lacolaco/items/f2c83091c8b4014e3cda
    ///   </para>
    /// </remarks>
    public static T RandomElementAt<T>(IEnumerable<T> ie)
    {
	return ie.ElementAt(_Rand.Next(ie.Count()));
    }
}
}
