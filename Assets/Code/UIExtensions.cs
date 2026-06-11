using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once CheckNamespace
public static class UIExtensions {
	public static RectTransform GetRectTransform(this Component obj) => (RectTransform)obj.transform;
}