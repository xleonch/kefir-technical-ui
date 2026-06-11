using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel.
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    [ExecuteInEditMode]
    public class UISafeArea : MonoBehaviour
    {
        private RectTransform[] _panels;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        [SerializeField, HideIf("@_usePanelTransform"), Required]
        private RectTransform[] _safeZonePanels;

        [SerializeField, Required] private bool _usePanelTransform;

        [FormerlySerializedAs("ConformX"), SerializeField, OnValueChanged(nameof(Refresh))]
        private bool _conformX = true; // Conform to screen safe area on X-axis (default true, disable to ignore)

        [FormerlySerializedAs("ConformY"), SerializeField, OnValueChanged(nameof(Refresh))]
        private bool _conformY = true; // Conform to screen safe area on Y-axis (default true, disable to ignore)

        [FormerlySerializedAs("Logging"), SerializeField]
        private bool _logging = false; // Conform to screen safe area on Y-axis (default true, disable to ignore)

        [Header("iOS")] [SerializeField, OnValueChanged(nameof(Refresh))]
        private Vector2Int _offsetAreaIOS = new(0, 0);

        [SerializeField, OnValueChanged(nameof(Refresh))]
        private Vector2Int _offsetNoAreaIOS = new(0, 0);

        [Header("Android")] [SerializeField, OnValueChanged(nameof(Refresh))]
        private Vector2Int _offsetAreaAndroid = new(0, 0);

        [SerializeField, OnValueChanged(nameof(Refresh))]
        private Vector2Int _offsetNoAreaAndroid = new(0, 0);

        private CanvasScaler _scaler;

        private CanvasScaler CanvasScaler
        {
            get
            {
                if (_scaler == null) _scaler = GetComponentInParent<CanvasScaler>();
                return _scaler;
            }
        }

#if UNITY_EDITOR
        public const string UISafeAreaIsEnabledKey = "UISafeAreaIsEnabled";

        private bool _testInEditor;

        private string GetName() => _testInEditor ? "Enabled" : "Turn Testing ON";
        private Color TestColor() => _testInEditor ? Color.green : new Color(0.25f, 0.5f, 0.25f);

        [PropertySpace(20)]
        [Button(ButtonSizes.Medium, Name = "@GetName()"), GUIColor(nameof(TestColor))]
        private void ToggleTesting()
        {
            _testInEditor = !_testInEditor;
            if (_testInEditor) Refresh();
        }
#endif

        private void PreparePanels()
        {
            _panels = _usePanelTransform
                ? new[] { this.GetRectTransform() }
                : _safeZonePanels?.Where(panel => panel != null).ToArray() ?? Array.Empty<RectTransform>();
        }

        private void OnEnable() => Refresh();

        private void Update() => Refresh();

        private void Refresh()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !_testInEditor) return;

            if (!UnityEditor.EditorPrefs.GetBool(UISafeAreaIsEnabledKey, true))
            {
                PreparePanels();
                _panels.ForEach(x =>
                {
                    x.anchorMin = Vector2.zero;
                    x.anchorMax = Vector2.one;
                });
                return;
            }
#endif

            if (Screen.width <= 0 || Screen.width <= 0) return;

            if (Application.isPlaying && Screen.width == _lastScreenSize.x && Screen.height == _lastScreenSize.y &&
                Screen.orientation == _lastOrientation)
                return;

            PreparePanels();
            var safeArea = GetSafeArea();
            if (Application.isPlaying && safeArea == _lastSafeArea) return;

            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;
            _lastOrientation = Screen.orientation;

            ApplySafeArea(safeArea);
        }

        private Rect GetSafeArea()
        {
            var safeArea = Screen.safeArea;

#if UNITY_IOS
            var offsetArea = _offsetAreaIOS;
            var offsetNoArea = _offsetNoAreaIOS;
#else
			var offsetArea = _offsetAreaAndroid;
			var offsetNoArea = _offsetNoAreaAndroid;
#endif

#if UNITY_EDITOR
            var isSimulator = UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop;

            if (isSimulator)
            {
                var isIOS = UnityEngine.Device.SystemInfo.operatingSystem.ToLower().Contains("ios");
                offsetArea = isIOS ? _offsetAreaIOS : _offsetAreaAndroid;
                offsetNoArea = isIOS ? _offsetNoAreaIOS : _offsetNoAreaAndroid;
            }
#endif

            if (Screen.width > 0 && Screen.height > 0 &&
                (offsetArea != Vector2Int.zero || offsetNoArea != Vector2Int.zero))
            {
                float scale;
                var scaler = CanvasScaler;
                if (scaler != null && scaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
                {
                    scale = scaler.scaleFactor > 0.5f
                        ? Screen.height / scaler.referenceResolution.y
                        : Screen.width / scaler.referenceResolution.x;
                }
                else
                {
                    scale = 1.0f;
                }

                offsetArea = new Vector2Int((int)(offsetArea.x * scale), (int)(offsetArea.y * scale));
                offsetNoArea = new Vector2Int((int)(offsetNoArea.x * scale), (int)(offsetNoArea.y * scale));

                if (_conformX)
                {
                    safeArea.xMin += (safeArea.xMin == 0) ? offsetNoArea.x : offsetArea.x;
                    safeArea.xMax -= (safeArea.xMax == Screen.width) ? offsetNoArea.x : offsetArea.x;
                }

                if (_conformY)
                {
                    safeArea.yMin += (safeArea.yMin == 0) ? offsetNoArea.y : offsetArea.y;
                    safeArea.yMax -= (safeArea.yMax == Screen.height) ? offsetNoArea.y : offsetArea.y;
                }
            }

            return safeArea;
        }

        private void ApplySafeArea(Rect r)
        {
            _lastSafeArea = r;

            // Ignore x-axis?
            if (!_conformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!_conformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                var anchorMin = r.position;
                var anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    _panels.ForEach(x =>
                    {
                        x.anchorMin = anchorMin;
                        x.anchorMax = anchorMax;
                    });
                }
            }

            if (_logging)
            {
                Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                    name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
            }
        }
    }
}