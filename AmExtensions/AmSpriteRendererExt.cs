using UnityEngine;

namespace am
{
    
public static class SpriteRendererExt
{
    /// <summary>
    ///   Alpha弄るためのヘルパーメソッド
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     実装参考：http://baba-s.hatenablog.com/entry/2018/02/26/085600
    ///   </para>
    /// </remarks>
    public static void SetAlpha( this SpriteRenderer self, float alpha )
    {
	var color = self.color;
	color.a = alpha;
	self.color = color;
    }
}
}
