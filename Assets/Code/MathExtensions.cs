using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once CheckNamespace
public static class MathExtensions {
	public static Vector2 ToXY(this Vector3 v) => new(v.x, v.y);

	public static Vector2 Min(this Vector2 v, Vector2 other) => new(Mathf.Min(v.x, other.x), Mathf.Min(v.y, other.y));

	public static Vector2 Max(this Vector2 v, Vector2 other) => new(Mathf.Max(v.x, other.x), Mathf.Max(v.y, other.y));

	public static Vector3 Min(this Vector3 v, Vector3 other) => new(Mathf.Min(v.x, other.x), Mathf.Min(v.y, other.y), Mathf.Min(v.z, other.z));

	public static Vector3 Max(this Vector3 v, Vector3 other) => new(Mathf.Max(v.x, other.x), Mathf.Max(v.y, other.y), Mathf.Max(v.z, other.z));

	public static Vector2 ToXZ(this Vector3 v) => new(v.x, v.z);

	public static Vector2 ToYZ(this Vector3 v) => new(v.y, v.z);

	public static Vector3 To0YZ(this Vector3 v, float x = 0.0f) => new(x, v.y, v.z);

	public static Vector3 ToX0Z(this Vector2 v, float y = 0.0f) => new(v.x, y, v.y);

	public static Vector3 ToX0Z(this Vector3 v, float y = 0.0f) => new(v.x, y, v.z);

	public static Vector3 ToXY0(this Vector3 v, float z = 0.0f) => new(v.x, v.y, z);

	public static Vector3 ToXY0(this Vector2 v, float z = 0.0f) => new(v.x, v.y, z);

	public static Vector2 To0Y(this Vector2 v, float x = 0.0f) => new(x, v.y);

	public static Vector2 ToX0(this Vector2 v, float y = 0.0f) => new(v.x, y);
	public static Vector3 FlipX(this Vector3 v) => new(-v.x, v.y, v.z);
	public static Vector3 FlipY(this Vector3 v) => new(v.x, -v.y, v.z);
	public static Vector3 FlipZ(this Vector3 v) => new(v.x, v.y, -v.z);

	public static bool SameAs(this Vector2 v, Vector2 other, float epsilon = 0.0001f) => Mathf.Abs(v.x - other.x) <= epsilon && Mathf.Abs(v.y - other.y) <= epsilon;

	public static bool SameAs(this Vector3 v, Vector3 other, float epsilon = 0.0001f) =>
		Mathf.Abs(v.x - other.x) <= epsilon && Mathf.Abs(v.y - other.y) <= epsilon && Mathf.Abs(v.z - other.z) <= epsilon;

	public static bool SameAs(this Vector4 v, Vector4 other, float epsilon = 0.0001f) =>
		Mathf.Abs(v.x - other.x) <= epsilon && Mathf.Abs(v.y - other.y) <= epsilon && Mathf.Abs(v.z - other.z) <= epsilon && Mathf.Abs(v.w - other.w) <= epsilon;

	public static Color SetAlpha(this Color _this, float a) {
		_this.a = a;
		return _this;
	}

	public static Color MulAlpha(this Color _this, float a) {
		_this.a *= a;
		return _this;
	}

	public static Color MulRGB(this Color _this, float k) {
		_this.r *= k;
		_this.g *= k;
		_this.b *= k;
		return _this;
	}

	public static Color Desaturate(this Color _this) {
		var lum = Vector3.Dot(_this.ToVector3(), new Vector3(0.3f, 0.59f, 0.11f));
		_this.r = lum;
		_this.g = lum;
		_this.b = lum;
		return _this;
	}

	public static Color MulRGB(this Color _this, Color multiplier) => new(_this.r * multiplier.r, _this.g * multiplier.g, _this.b * multiplier.b, _this.a);

	public static Vector4 ToVector4(this Color _this) => new(_this.r, _this.g, _this.b, _this.a);
	public static Vector3 ToVector3(this Color _this) => new(_this.r, _this.g, _this.b);
}