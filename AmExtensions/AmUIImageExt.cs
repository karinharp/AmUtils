using System;   
using UnityEngine;   
using UnityEngine.UI;   

namespace am
{

public static class UIImageExt
{
    
    public static void SetPosition(this Image src, int x, int y){	
	var rect = src.gameObject.GetComponent<RectTransform>();
	rect.anchoredPosition = new Vector2(x, -y); // 座標系補正
    }
    public static void SetPosition(this Image src, float x, float y){	
	var rect = src.gameObject.GetComponent<RectTransform>();
	rect.anchoredPosition = new Vector2(x, -y); // 座標系補正
    }
    public static Vector2 GetPosition(this Image src){	
	var rect = src.gameObject.GetComponent<RectTransform>();
	return rect.anchoredPosition;
    }

    static Color s_color = new Color(1f, 1f, 1f, 1f);
    
    // 独自拡張, 0xAARRGGBB で受け取って、パースしてカラーをセット
    // 引数の順番が、R,G,B,A なので注意...
    public static void SetColor(this Image src, uint value){
	s_color.a = (float)((value & 0xFF000000) >> 24) / 256.00f;
	s_color.r = (float)((value & 0x00FF0000) >> 16) / 256.00f;
	s_color.g = (float)((value & 0x0000FF00) >> 8)  / 256.00f;
	s_color.b = (float)((value & 0x000000FF) >> 0)  / 256.00f;
	src.color = s_color;
    }

    public static int w(this Image src){
	return Convert.ToInt32(src.preferredWidth);
    }
    public static int h(this Image src){
	return Convert.ToInt32(src.preferredHeight);
    }

    public static float getWidth(this RectTransform src){
	return src.sizeDelta.x;
    }
    public static float getHeight(this RectTransform src){
	return src.sizeDelta.y;
    }
    
    public static void SetRect(this Image src, Vector2 size){
	var rect = src.gameObject.GetComponent<RectTransform>();
	rect.sizeDelta = size;
    }

    public static Vector2 GetRect(this Image src){
	var rect = src.gameObject.GetComponent<RectTransform>();
	return rect.sizeDelta;
    }

    public static void FixRect(this Image src){
	var rect = src.gameObject.GetComponent<RectTransform>();
	rect.sizeDelta = new Vector2(src.w(), src.h());	
    }

    // 0,0 をセットすると、左上が原点になるような調整になる。
    public static void SetAnchorLeftTop(this Image src){
	var rect = src.gameObject.GetComponent<RectTransform>();
	rect.anchorMax  = new Vector2(0f,1f);
	rect.anchorMin  = new Vector2(0f,1f);
	rect.pivot      = new Vector2(0f,1f); 	
    }
}
}

/*
 * Local variables:
 * compile-command: "make -C../"
 * End:
 */
