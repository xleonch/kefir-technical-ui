using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once CheckNamespace
public static class MathExtensions {
	public static double ClampInfinity(this double v) {
		if (v >= double.PositiveInfinity) v = double.MaxValue;

		return v;
	}

	public static float ClampInfinity(this float v) {
		if (v >= float.PositiveInfinity) v = float.MaxValue;

		return v;
	}

	public static BigInteger ToBigInteger(this double v) => new(v.ClampInfinity());

	public static BigInteger ToBigInteger(this float v) => new(v.ClampInfinity());

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

	public static Color FromHtml(string htmlStringRGB, Color? defaultColor = null) {
		return ColorUtility.TryParseHtmlString(htmlStringRGB, out var result) ? result : (defaultColor ?? Color.black);
	}

	public static Vector4 ToVector4(this Color _this) => new(_this.r, _this.g, _this.b, _this.a);
	public static Vector3 ToVector3(this Color _this) => new(_this.r, _this.g, _this.b);

	public static bool Contains(this Rect self, Rect rect) => self.Contains(rect.min) && self.Contains(rect.max);

	public static float Area(this Rect rect) => rect.width * rect.height;

	public static bool ProjectToScreen(this Bounds bounds, Camera cam, out Rect rect) {
		rect = default;
		if (cam == null) return false;

		var minX = float.PositiveInfinity;
		var minY = float.PositiveInfinity;
		var maxX = float.NegativeInfinity;
		var maxY = float.NegativeInfinity;

		var center = bounds.center;
		var extents = bounds.extents;

		if (!EncapsulateProjectedPoint(cam, center + new Vector3(-extents.x, -extents.y, -extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(-extents.x, -extents.y, extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(-extents.x, extents.y, -extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(-extents.x, extents.y, extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(extents.x, -extents.y, -extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(extents.x, -extents.y, extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(extents.x, extents.y, -extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;
		if (!EncapsulateProjectedPoint(cam, center + new Vector3(extents.x, extents.y, extents.z), ref minX, ref minY, ref maxX, ref maxY)) return false;

		if (maxX <= minX || maxY <= minY) return false;

		rect = Rect.MinMaxRect(minX, minY, maxX, maxY);
		return true;
	}

	public static Rect Intersects(this Rect r1, Rect r2) {
		var result = new Rect();

		if (!r2.Overlaps(r1)) return result;

		var x1 = Mathf.Min(r1.xMax, r2.xMax);
		var x2 = Mathf.Max(r1.xMin, r2.xMin);
		var y1 = Mathf.Min(r1.yMax, r2.yMax);
		var y2 = Mathf.Max(r1.yMin, r2.yMin);
		result.x = Mathf.Min(x1, x2);
		result.y = Mathf.Min(y1, y2);
		result.width = Mathf.Max(0.0f, x1 - x2);
		result.height = Mathf.Max(0.0f, y1 - y2);

		return result;
	}

	public static Vector2 GetRotated(this Vector2 v, float degrees) {
		var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		var tx = v.x;
		var ty = v.y;
		v.x = cos * tx - sin * ty;
		v.y = sin * tx + cos * ty;
		return v;
	}

	public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) {
		return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
	}

	public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) {
		return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
	}

	public static bool Raycast(this Plane p, Ray ray, out Vector3 position) {
		if (!p.Raycast(ray, out var rayDist)) {
			position = Vector3.zero;
			return false;
		}

		position = ray.GetPoint(rayDist);

		return true;
	}

	public static bool PointInFrustum(this Camera c, Vector3 worldPos) {
		var p = c.WorldToViewportPoint(worldPos);
		return p.x >= 0 && p.x <= 1 && p.y >= 0 && p.y <= 1 && p.z > 0;
	}

	public static float GetRandom(this Vector2 vector) => Random.Range(vector.x, vector.y);

	public static int GetRandom(this Vector2Int vector) => Random.Range(vector.x, vector.y);

	private static bool EncapsulateProjectedPoint(Camera cam, Vector3 worldPoint, ref float minX, ref float minY, ref float maxX, ref float maxY) {
		var screenPoint = cam.WorldToScreenPoint(worldPoint);
		if (screenPoint.z <= 0f) return false;

		if (screenPoint.x < minX) minX = screenPoint.x;
		if (screenPoint.y < minY) minY = screenPoint.y;
		if (screenPoint.x > maxX) maxX = screenPoint.x;
		if (screenPoint.y > maxY) maxY = screenPoint.y;
		return true;
	}
}