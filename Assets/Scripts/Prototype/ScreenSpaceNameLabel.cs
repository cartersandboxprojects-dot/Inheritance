using UnityEngine;

namespace Inheritance.Prototype
{
    // Prototype-only identification: constant pixel size, drawn above scene geometry.
    public sealed class ScreenSpaceNameLabel : MonoBehaviour
    {
        [SerializeField] private string displayName;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
        private GUIStyle style;

        private void OnGUI()
        {
            if (viewCamera == null) return;
            var point = viewCamera.WorldToScreenPoint(transform.position + offset);
            if (point.z <= 0f) return;
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
                style.normal.textColor = Color.white;
            }
            GUI.Box(new Rect(point.x - 50f, Screen.height - point.y - 14f, 100f, 28f), displayName, style);
        }
    }
}
