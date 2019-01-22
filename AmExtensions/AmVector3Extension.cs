using UnityEngine;
using UnityEngine.Assertions;

namespace am
{
    
/// <summary>
///   Vector3拡張
/// </summary>
/// <remarks>
///   <para>
///     座標変換参考：http://tsubakit1.hateblo.jp/entry/2016/03/01/020510
///   </para>
/// </remarks>
public static class Vector3Extension
{
    /// <summary>
    /// CanvasのRender Mode が Scene Space - Overlay の場合に、ワールド座標をスクリーン座標に変換する
    /// </summary>
    /// <returns>変換されたスクリーン座標</returns>
    /// <param name="position">対象のワールド座標</param>
    public static Vector2 ToScreenPositionCaseScreenSpaceOverlay(this Vector3 position)
    {
        return position.ToScreenPositionCaseScreenSpaceOverlay(Camera.main);
    }

    /// <summary>
    /// CanvasのRender Mode が Scene Space - Overlay の場合に、ワールド座標をスクリーン座標に変換する
    /// </summary>
    /// <returns>変換されたスクリーン座標</returns>
    /// <param name="position">対象のワールド座標</param>
    /// <param name="camera">メインカメラ</param>
    public static Vector2 ToScreenPositionCaseScreenSpaceOverlay(this Vector3 position, Camera camera)
    {
        return RectTransformUtility.WorldToScreenPoint(camera, position);
    }

    /// <summary>
    /// CanvasのRender Mode が Scene Space - Camera の場合に、ワールド座標をスクリーン座標に変換する
    /// </summary>
    /// <returns>変換されたスクリーン座標</returns>
    /// <param name="position">対象のワールド座標</param>
    /// <param name="canvas">UIのCanvas</param>
    public static Vector2 ToScreenPositionCaseScreenSpaceCamera(this Vector3 position, Canvas canvas)
    {
        return position.ToScreenPositionCaseScreenSpaceCamera(canvas, Camera.main);
    }

    /// <summary>
    /// CanvasのRender Mode が Scene Space - Camera の場合に、ワールド座標をスクリーン座標に変換する
    /// </summary>
    /// <returns>変換されたスクリーン座標</returns>
    /// <param name="position">対象のワールド座標</param>
    /// <param name="canvas">UIのCanvas</param>
    /// <param name="uiCamera">UIを写すカメラ（CanvasのRenderCameraに設定されているカメラ）</param>
    public static Vector2 ToScreenPositionCaseScreenSpaceCamera(this Vector3 position, Canvas canvas, Camera uiCamera)
    {
        return position.ToScreenPositionCaseScreenSpaceCamera(canvas, uiCamera, Camera.main);
    }

    /// <summary>
    /// CanvasのRender Mode が Scene Space - Camera の場合に、ワールド座標をスクリーン座標に変換する
    /// </summary>
    /// <returns>変換されたスクリーン座標</returns>
    /// <param name="position">対象のワールド座標</param>
    /// <param name="canvas">UIのCanvas</param>
    /// <param name="uiCamera">UIを写すカメラ（CanvasのRenderCameraに設定されているカメラ）</param>
    /// <param name="worldCamera">メインカメラ</param>
    public static Vector2 ToScreenPositionCaseScreenSpaceCamera(this Vector3 position, Canvas canvas, Camera uiCamera, Camera worldCamera)
    {
        Assert.IsTrue(
            canvas.renderMode == RenderMode.ScreenSpaceCamera, 
            "Canvasのレンダーモードが「Scene Space - Camera」になっていません"
        );

        var p = RectTransformUtility.WorldToScreenPoint(worldCamera, position);
        var retPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            p,
            uiCamera,
            out retPosition
        );
        return retPosition;
    }
}
}
