/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Ovr;

/// <summary>
/// Configuration data for Oculus virtual reality.
/// </summary>
public class OVRManager : MonoBehaviour
{
	/// <summary>
	/// Contains information about the user's preferences and body dimensions.
	/// </summary>
	public struct Profile
	{
		public float ipd;
		public float eyeHeight;
		public float eyeDepth;
		public float neckHeight;
	}

	/// <summary>
	/// Contains the valid range of antialiasing levels usable with Unity render textures.
	/// </summary>
	public enum RenderTextureAntiAliasing
	{
		_1 = 1,
		_2 = 2,
		_4 = 4,
		_8 = 8,
	}

	/// <summary>
	/// Contains the valid range of texture depth values usable with Unity render textures.
	/// </summary>
	public enum RenderTextureDepth
	{
		_0  =  0,
		_16 = 16,
		_24 = 24,
	}

	/// <summary>
	/// Gets the singleton instance.
	/// </summary>
	public static OVRManager instance { get; private set; }
		
	private static Hmd _capiHmd;
	/// <summary>
	/// Gets a reference to the low-level C API Hmd Wrapper
	/// </summary>
	public static Hmd capiHmd
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (_capiHmd == null)
			{
				IntPtr hmdPtr = IntPtr.Zero;
				OVR_GetHMD(ref hmdPtr);
				_capiHmd = (hmdPtr != IntPtr.Zero) ? new Hmd(hmdPtr) : null;
			}
#else
			_capiHmd = null;
#endif
			return _capiHmd;
		}
	}
		
	/// <summary>
	/// Gets a reference to the active OVRDisplay
	/// </summary>
	public static OVRDisplay display { get; private set; }

	/// <summary>
	/// Gets a reference to the active OVRTracker
	/// </summary>
	public static OVRTracker tracker { get; private set; }

	private static bool _profileIsCached = false;
	private static Profile _profile;
	/// <summary>
	/// Gets the current profile, which contains information about the user's settings and body dimensions.
	/// </summary>
	public static Profile profile
	{
		get {
			if (!_profileIsCached)
			{
#if !UNITY_ANDROID || UNITY_EDITOR
				float ipd = capiHmd.GetFloat(Hmd.OVR_KEY_IPD, Hmd.OVR_DEFAULT_IPD);
				float eyeHeight = capiHmd.GetFloat(Hmd.OVR_KEY_EYE_HEIGHT, Hmd.OVR_DEFAULT_EYE_HEIGHT);
				float[] defaultOffset = new float[] { Hmd.OVR_DEFAULT_NECK_TO_EYE_HORIZONTAL, Hmd.OVR_DEFAULT_NECK_TO_EYE_VERTICAL };
				float[] neckToEyeOffset = capiHmd.GetFloatArray(Hmd.OVR_KEY_NECK_TO_EYE_DISTANCE, defaultOffset);
				float neckHeight = eyeHeight - neckToEyeOffset[1];
				
				_profile = new Profile
				{
					ipd = ipd,
					eyeHeight = eyeHeight,
					eyeDepth = neckToEyeOffset[0],
					neckHeight = neckHeight,
				};
#else
				float ipd = 0.0f;
				OVR_GetInterpupillaryDistance(ref ipd);
				
				float eyeHeight = 0.0f;
				OVR_GetPlayerEyeHeight(ref eyeHeight);
				
				_profile = new Profile
				{
					ipd = ipd,
					eyeHeight = eyeHeight,
					eyeDepth = 0f, //TODO
					neckHeight = 0.0f, // TODO
				};
#endif
				_profileIsCached = true;
			}

			return _profile;
		}
	}

	/// <summary>
	/// Occurs when an HMD attached.
	/// </summary>
	public static event Action HMDAcquired;

	/// <summary>
	/// Occurs when an HMD detached.
	/// </summary>
	public static event Action HMDLost;

	/// <summary>
	/// Occurs when the tracker gained tracking.
	/// </summary>
	public static event Action TrackingAcquired;

	/// <summary>
	/// Occurs when the tracker lost tracking.
	/// </summary>
	public static event Action TrackingLost;
	
	/// <summary>
	/// Occurs when HSW dismissed.
	/// </summary>
	public static event Action HSWDismissed;

	/// <summary>
	/// Occurs when the Native Texture Scale is modified.
	/// </summary>
	internal static event Action<float, float> NativeTextureScaleModified;

	/// <summary>
	/// Occurs when the Virtual Texture Scale is modified.
	/// </summary>
	internal static event Action<float, float> VirtualTextureScaleModified;

	/// <summary>
	/// Occurs when the Eye Texture AntiAliasing level is modified.
	/// </summary>
	internal static event Action<RenderTextureAntiAliasing, RenderTextureAntiAliasing> EyeTextureAntiAliasingModified;

	/// <summary>
	/// Occurs when the Eye Texture Depth is modified.
	/// </summary>
	internal static event Action<RenderTextureDepth, RenderTextureDepth> EyeTextureDepthModified;

	/// <summary>
	/// Occurs when the Eye Texture Format is modified.
	/// </summary>
	internal static event Action<RenderTextureFormat, RenderTextureFormat> EyeTextureFormatModified;

	/// <summary>
	/// Occurs when Monoscopic mode is modified.
	/// </summary>
	internal static event Action<bool, bool> MonoscopicModified;

	/// <summary>
	/// Occurs when HDR mode is modified.
	/// </summary>
	internal static event Action<bool, bool> HdrModified;
	
	/// <summary>
	/// If true, then the Oculus health and safety warning (HSW) is currently visible.
	/// </summary>
	public static bool isHSWDisplayed
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			return (capiHmd.GetHSWDisplayState().Displayed != 0);
#else
			return false;
#endif
		}
	}
	
	/// <summary>
	/// If the HSW has been visible for the necessary amount of time, this will make it disappear.
	/// </summary>
	public static void DismissHSWDisplay()
	{
#if !UNITY_ANDROID || UNITY_EDITOR
		capiHmd.DismissHSWDisplay();
#endif
	}
	
	/// <summary>
	/// Gets the current battery level.
	/// </summary>
	/// <returns><c>battery level in the range [0.0,1.0]</c>
	/// <param name="batteryLevel">Battery level.</param>
	public static float batteryLevel
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			return 1.0f;
#else
			return OVR_GetBatteryLevel();
#endif
		}
	}
	
	/// <summary>
	/// Gets the current battery temperature.
	/// </summary>
	/// <returns><c>battery temperature in Celsius</c>
	/// <param name="batteryTemperature">Battery temperature.</param>
	public static float batteryTemperature
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			return 0.0f;
#else
			return OVR_GetBatteryTemperature();
#endif
		}
	}
	
	/// <summary>
	/// Gets the current battery status.
	/// </summary>
	/// <returns><c>battery status</c>
	/// <param name="batteryStatus">Battery status.</param>
	public static int batteryStatus
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			return 0;
#else
			return OVR_GetBatteryStatus();
#endif
		}
	}

	/// <summary>
	/// Gets the current volume level.
	/// </summary>
	/// <returns><c>volume level in the range [0,MaxVolume], or -1 for not initialized.</c>
	public static int volumeLevel
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			Debug.LogError( "GetVolume() is only supported on Android" );
			return -1;
#else
			return OVR_GetVolume();
#endif
		}
	}
	
	/// <summary>
	/// Gets the time since last volume change
	/// </summary>
	/// <returns><c>time since last volume change or -1 for not initialized.</c>
	public static double timeSinceLastVolumeChange
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			Debug.LogError( "GetTimeSinceLastVolumeChange() is only supported on Android" );
			return -1;
#else
			return OVR_GetTimeSinceLastVolumeChange();
#endif
		}
	}

	[Range(0.1f, 4.0f)]
	/// <summary>
	/// Controls the size of the eye textures.
	/// Values must be above 0.
	/// Values below 1 permit sub-sampling for improved performance.
	/// Values above 1 permit super-sampling for improved sharpness.
	/// </summary>
	public float nativeTextureScale = 1.0f;
	
	[Range(0.1f, 1.0f)]
	/// <summary>
	/// Controls the size of the rendering viewport.
	/// Values must be above 0 and less than or equal to 1.
	/// Values below 1 permit dynamic sub-sampling for improved performance.
	/// </summary>
	public float virtualTextureScale = 1.0f;

	/// <summary>
	/// The format of each eye texture.
	/// </summary>
	public RenderTextureFormat eyeTextureFormat = RenderTextureFormat.Default;

	/// <summary>
	/// The antialiasing level of each eye texture.
	/// </summary>
	public RenderTextureAntiAliasing eyeTextureAntiAliasing = RenderTextureAntiAliasing._2;

	/// <summary>
	/// The depth of each eye texture in bits. Valid Unity render texture depths are 0, 16, and 24.
	/// </summary>
	public RenderTextureDepth eyeTextureDepth = RenderTextureDepth._24;

	/// <summary>
	/// If true, head tracking will affect the orientation of each OVRCameraRig's cameras.
	/// </summary>
	public bool usePositionTracking = true;

	/// <summary>
	/// If true, TimeWarp will be used to correct the output of each OVRCameraRig for rotational latency.
	/// </summary>
	public bool timeWarp = true;

	/// <summary>
	/// If this is true and TimeWarp is true, each OVRCameraRig will stop tracking and only TimeWarp will respond to head motion.
	/// </summary>
	public bool freezeTimeWarp = false;

	/// <summary>
	/// If true, each scene load will cause the head pose to reset.
	/// </summary>
	public bool resetTrackerOnLoad = true;

	/// <summary>
	/// If true, the eyes see the same image, which is rendered only by the left camera.
	/// </summary>
	public bool monoscopic = false;

	/// <summary>
	/// If true, enable high dynamic range support.
	/// </summary>
	public bool hdr = false;

	/// <summary>
	/// True if the current platform supports virtual reality.
	/// </summary>
    public bool isSupportedPlatform { get; private set; }
	
	private static bool usingPositionTrackingCached = false;
	private static bool usingPositionTracking = false;
	private static bool wasHmdPresent = false;
	private static bool wasPositionTracked = false;
	private static float prevNativeTextureScale;
	private static float prevVirtualTextureScale;
	private static RenderTextureAntiAliasing prevEyeTextureAntiAliasing;
	private static RenderTextureDepth prevEyeTextureDepth;
	private static bool prevMonoscopic;
	private static bool prevHdr;
	private static RenderTextureFormat prevEyeTextureFormat;
	private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

#if UNITY_ANDROID && !UNITY_EDITOR
	// Get this from Unity on startup so we can call Activity java functions
	private static bool androidJavaInit = false;
	private static AndroidJavaObject activity;
	private static AndroidJavaClass javaVrActivityClass;
	internal static int timeWarpViewNumber = 0;

	[NonSerialized]
	private static OVRVolumeControl VolumeController = null;

	public static void EnterVRMode()
	{
		OVRPluginEvent.Issue(RenderEventType.Resume);
	}

	public static void LeaveVRMode()
	{
		OVRPluginEvent.Issue(RenderEventType.Pause);
	}
#else
	private static bool ovrIsInitialized;
	private static bool isQuitting;
#endif

#region Unity Messages

	private void Awake()
	{
		// Only allow one instance at runtime.
		if (instance != null)
		{
			enabled = false;
			DestroyImmediate(this);
			return;
		}

		instance = this;

#if !UNITY_ANDROID || UNITY_EDITOR
		if (!ovrIsInitialized)
		{
			OVR_Initialize();
			OVRPluginEvent.Issue(RenderEventType.Initialize);

			ovrIsInitialized = true;
		}

		var netVersion = new System.Version(Ovr.Hmd.OVR_VERSION_STRING);
		System.Version ovrVersion = new System.Version("0.0.0");
		var versionString = Ovr.Hmd.GetVersionString();
		var success = false;
		try {
			ovrVersion = new System.Version(versionString);
			success = true;
		} catch (Exception e) {
			Debug.Log("Failed to parse Oculus version string \"" + versionString + "\" with message \"" + e.Message + "\".");
		}
		if (!success || netVersion > ovrVersion)
			Debug.LogWarning("Version check failed. Please make sure you are using Oculus runtime " +
			                 Ovr.Hmd.OVR_VERSION_STRING + " or newer.");
#endif

        // Detect whether this platform is a supported platform
        RuntimePlatform currPlatform = Application.platform;
        isSupportedPlatform |= currPlatform == RuntimePlatform.Android;
        isSupportedPlatform |= currPlatform == RuntimePlatform.LinuxPlayer;
        isSupportedPlatform |= currPlatform == RuntimePlatform.OSXEditor;
        isSupportedPlatform |= currPlatform == RuntimePlatform.OSXPlayer;
        isSupportedPlatform |= currPlatform == RuntimePlatform.WindowsEditor;
        isSupportedPlatform |= currPlatform == RuntimePlatform.WindowsPlayer;
        if (!isSupportedPlatform)
        {
            Debug.LogWarning("This platform is unsupported");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		// don't allow the application to run if orientation is not landscape left.
		if (Screen.orientation != ScreenOrientation.LandscapeLeft)
		{
			Debug.LogError("********************************************************************************\n");
			Debug.LogError("***** Default screen orientation must be set to landscape left for VR.\n" +
			               "***** Stopping application.\n");
			Debug.LogError("********************************************************************************\n");

			Debug.Break();
			Application.Quit();
		}

		// don't enable gyro, it is not used and triggers expensive display calls
		if (Input.gyro.enabled)
		{
			Debug.LogError("*** Auto-disabling Gyroscope ***");
			Input.gyro.enabled = false;
		}
		
		// don't enable antiAliasing on the main window display, it may cause
		// bad behavior with various tiling controls.
		if (QualitySettings.antiAliasing > 1)
		{
			Debug.LogError("*** Main Display should have 0 samples ***");
		}

		// we sync in the TimeWarp, so we don't want unity
		// syncing elsewhere
		QualitySettings.vSyncCount = 0;

		// try to render at 60fps
		Application.targetFrameRate = 60;
		// don't allow the app to run in the background
		Application.runInBackground = false;
		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		if (!androidJavaInit)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			javaVrActivityClass = new AndroidJavaClass("com.oculusvr.vrlib.VrActivity");
			// Prepare for the RenderThreadInit()
			SetInitVariables(activity.GetRawObject(), javaVrActivityClass.GetRawClass());
			
			androidJavaInit = true;
		}

		// We want to set up our touchpad messaging system
		OVRTouchpad.Create();

		InitVolumeController();
#else
		SetEditorPlay(Application.isEditor);
#endif

		prevEyeTextureAntiAliasing = OVRManager.instance.eyeTextureAntiAliasing;
		prevEyeTextureDepth = OVRManager.instance.eyeTextureDepth;
		prevEyeTextureFormat = OVRManager.instance.eyeTextureFormat;
        prevNativeTextureScale = OVRManager.instance.nativeTextureScale;
        prevVirtualTextureScale = OVRManager.instance.virtualTextureScale;
        prevMonoscopic = OVRManager.instance.monoscopic;
        prevHdr = OVRManager.instance.hdr;

		if (display == null)
			display = new OVRDisplay();
		if (tracker == null)
			tracker = new OVRTracker();

		if (resetTrackerOnLoad)
			display.RecenterPose();

#if !UNITY_ANDROID || UNITY_EDITOR
		// Except for D3D9, SDK rendering forces vsync unless you pass ovrHmdCap_NoVSync to Hmd.SetEnabledCaps().
		if (timeWarp)
		{
			bool useUnityVSync = SystemInfo.graphicsDeviceVersion.Contains("Direct3D 9");
			QualitySettings.vSyncCount = useUnityVSync ? 1 : 0;
		}
#endif

#if UNITY_STANDALONE_WIN
		if (!OVRUnityVersionChecker.hasD3D9ExclusiveModeSupport && !display.isDirectMode && SystemInfo.graphicsDeviceVersion.Contains("Direct3D 9"))
		{
			MessageBox(0, "Direct3D 9 extended mode is not supported in this configuration. "
				+ "Please use direct display mode, a different graphics API, or rebuild the application with a newer Unity version."
				, "VR Configuration Warning", 0);
		}

		if (!OVRUnityVersionChecker.hasD3D11ExclusiveModeSupport && !display.isDirectMode && SystemInfo.graphicsDeviceVersion.Contains("Direct3D 11"))
		{
			MessageBox(0, "Direct3D 11 extended mode is not supported in this configuration. "
				+ "Please use direct display mode, a different graphics API, or rebuild the application with a newer Unity version."
				, "VR Configuration Warning", 0);
		}
#endif
	}

	private void OnDestroy()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		RenderTexture.active = null;
#endif
	}

	private void OnApplicationQuit()
	{
#if !UNITY_ANDROID || UNITY_EDITOR
		isQuitting = true;
#else
		Debug.Log( "OnApplicationQuit" );
		
		// This will trigger the shutdown on the render thread
		OVRPluginEvent.Issue( RenderEventType.ShutdownRenderThread );
#endif
	}

	private void OnEnable()
	{
#if !UNITY_ANDROID || UNITY_EDITOR
		Camera cam = GetComponent<Camera>();
		if (cam == null)
		{
			// Ensure there is a non-RT camera in the scene to force rendering of the left and right eyes.
			cam = gameObject.AddComponent<Camera>();
			cam.cullingMask = 0;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.0f, 0.0f, 0.0f);
			cam.renderingPath = RenderingPath.Forward;
			cam.orthographic = true;
			cam.useOcclusionCulling = false;
		}
#endif

		bool isD3d = SystemInfo.graphicsDeviceVersion.Contains("Direct3D") ||
			Application.platform == RuntimePlatform.WindowsEditor &&
				SystemInfo.graphicsDeviceVersion.Contains("emulated");
		display.flipInput = isD3d;

		StartCoroutine(CallbackCoroutine());

#if UNITY_ANDROID && !UNITY_EDITOR
		if (VolumeController != null)
		{
			OVRPose pose = OVRManager.display.GetHeadPose();
			VolumeController.UpdatePosition(pose.orientation, pose.position);
		}
#endif
	}

	private void OnDisable()
	{
#if !UNITY_ANDROID || UNITY_EDITOR
		if (!isQuitting)
			return;

		if (ovrIsInitialized)
		{
			OVR_Destroy();
			OVRPluginEvent.Issue(RenderEventType.Destroy);
			_capiHmd = null;

			ovrIsInitialized = false;
		}
#else
		StopAllCoroutines();
#endif
	}

	private void Start()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		// NOTE: For Android, the resolution should be the same for both left and right eye
		OVRDisplay.EyeRenderDesc leftEyeDesc = OVRManager.display.GetEyeRenderDesc(OVREye.Left);
		Vector2 resolution = leftEyeDesc.resolution;
		OVR_SetEyeParms((int)resolution.x,(int)resolution.y);

		// This will trigger the init on the render thread
		InitRenderThread();
#endif
	}

	private void Update()
	{
		if (!usingPositionTrackingCached || usingPositionTracking != usePositionTracking)
		{
			tracker.isEnabled = usePositionTracking;
			usingPositionTracking = usePositionTracking;
			usingPositionTrackingCached = true;
		}

		// Dispatch any events.
		if (HMDLost != null && wasHmdPresent && !display.isPresent)
			HMDLost();

		if (HMDAcquired != null && !wasHmdPresent && display.isPresent)
			HMDAcquired();

		wasHmdPresent = display.isPresent;

		if (TrackingLost != null && wasPositionTracked && !tracker.isPositionTracked)
			TrackingLost();

		if (TrackingAcquired != null && !wasPositionTracked && tracker.isPositionTracked)
			TrackingAcquired();

		wasPositionTracked = tracker.isPositionTracked;

		if (NativeTextureScaleModified != null && nativeTextureScale != prevNativeTextureScale)
			NativeTextureScaleModified(prevNativeTextureScale, nativeTextureScale);

		prevNativeTextureScale = nativeTextureScale;

		if (VirtualTextureScaleModified != null && virtualTextureScale != prevVirtualTextureScale)
			VirtualTextureScaleModified(prevVirtualTextureScale, virtualTextureScale);

		prevVirtualTextureScale = virtualTextureScale;

		if (EyeTextureAntiAliasingModified != null && eyeTextureAntiAliasing != prevEyeTextureAntiAliasing)
			EyeTextureAntiAliasingModified(prevEyeTextureAntiAliasing, eyeTextureAntiAliasing);

		prevEyeTextureAntiAliasing = eyeTextureAntiAliasing;

		if (EyeTextureDepthModified != null && eyeTextureDepth != prevEyeTextureDepth)
			EyeTextureDepthModified(prevEyeTextureDepth, eyeTextureDepth);

		prevEyeTextureDepth = eyeTextureDepth;

		if (EyeTextureFormatModified != null && eyeTextureFormat != prevEyeTextureFormat)
			EyeTextureFormatModified(prevEyeTextureFormat, eyeTextureFormat);

		prevEyeTextureFormat = eyeTextureFormat;

		if (MonoscopicModified != null && monoscopic != prevMonoscopic)
			MonoscopicModified(prevMonoscopic, monoscopic);

		prevMonoscopic = monoscopic;

		if (HdrModified != null && hdr != prevHdr)
			HdrModified(prevHdr, hdr);

		prevHdr = hdr;

		if (isHSWDisplayed && Input.anyKeyDown)
		{
			DismissHSWDisplay();
			
			if (HSWDismissed != null)
				HSWDismissed();
		}
		
		display.timeWarp = timeWarp;

		display.Update();

#if UNITY_ANDROID && !UNITY_EDITOR
		if (VolumeController != null)
		{
			OVRPose pose = OVRManager.display.GetHeadPose();
			VolumeController.UpdatePosition(pose.orientation, pose.position);
		}
#endif
	}

#if (UNITY_EDITOR_OSX)
	private void OnPreCull() // TODO: Fix Mac Unity Editor memory corruption issue requiring OnPreCull workaround.
#else
	private void LateUpdate()
#endif
	{
#if (!UNITY_ANDROID || UNITY_EDITOR)
		display.BeginFrame();
#endif
	}

	private IEnumerator CallbackCoroutine()
	{
        while (true)
        {
			yield return waitForEndOfFrame;

#if UNITY_ANDROID && !UNITY_EDITOR
			OVRManager.DoTimeWarp(timeWarpViewNumber);
#else
			display.EndFrame();
#endif
        }
	}
	
#if UNITY_ANDROID && !UNITY_EDITOR
	private void OnPause()
	{
		LeaveVRMode();
	}

	private IEnumerator OnResume()
	{
		yield return null; // delay 1 frame to allow Unity enough time to create the windowSurface

		EnterVRMode();
	}

	private void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause() " + pause);
		if (pause)
		{
			OnPause();
		}
		else
		{
			StartCoroutine(OnResume());
		}
	}

	void OnApplicationFocus( bool focus )
	{
		// OnApplicationFocus() does not appear to be called 
		// consistently while OnApplicationPause is. Moved
		// functionality to OnApplicationPause().
		
		Debug.Log( "OnApplicationFocus() " + focus );
	}

	/// <summary>
	/// Creates a popup dialog that shows when volume changes.
	/// </summary>
	private static void InitVolumeController()
	{
		if (VolumeController == null)
		{
			Debug.Log("Creating volume controller...");
			// Create the volume control popup
			GameObject go = GameObject.Instantiate(Resources.Load("OVRVolumeController")) as GameObject;
			if (go != null)
			{
				VolumeController = go.GetComponent<OVRVolumeControl>();
			}
			else
			{
				Debug.LogError("Unable to instantiate volume controller");
			}
		}
	}

	/// <summary>
	/// Leaves the application/game and returns to the launcher/dashboard
	/// </summary>
	public void ReturnToLauncher()
	{
		// show the platform UI quit prompt
		OVRManager.PlatformUIConfirmQuit();
	}
#endif
#endregion

    public static void SetEditorPlay(bool isEditor)
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        OVR_SetEditorPlay(isEditor);
#endif
    }

    public static void SetInitVariables(IntPtr activity, IntPtr vrActivityClass)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		OVR_SetInitVariables(activity, vrActivityClass);
#endif
    }

    public static void PlatformUIConfirmQuit()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		OVRPluginEvent.Issue(RenderEventType.PlatformUIConfirmQuit);
#endif
    }

    public static void PlatformUIGlobalMenu()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		OVRPluginEvent.Issue(RenderEventType.PlatformUI);
#endif
    }

    public static void DoTimeWarp(int timeWarpViewNumber)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		OVRPluginEvent.IssueWithData(RenderEventType.TimeWarp, timeWarpViewNumber);
#endif
    }

    public static void EndEye(OVREye eye)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		RenderEventType eventType = (eye == OVREye.Left) ?
			RenderEventType.LeftEyeEndFrame :
			RenderEventType.RightEyeEndFrame;
		int eyeTextureId = display.GetEyeTextureId(eye);

		OVRPluginEvent.IssueWithData(eventType, eyeTextureId);
#endif
    }

    public static void InitRenderThread()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		OVRPluginEvent.Issue(RenderEventType.InitRenderThread);
#endif
    }

    private const string LibOVR = "OculusPlugin";

#if !UNITY_ANDROID || UNITY_EDITOR
	[DllImport(LibOVR, CallingConvention = CallingConvention.Cdecl)]
	private static extern void OVR_GetHMD(ref IntPtr hmdPtr);
    [DllImport(LibOVR, CallingConvention = CallingConvention.Cdecl)]
    private static extern void OVR_SetEditorPlay(bool isEditorPlay);
	[DllImport(LibOVR, CallingConvention = CallingConvention.Cdecl)]
	private static extern void OVR_Initialize();
	[DllImport(LibOVR, CallingConvention = CallingConvention.Cdecl)]
	private static extern void OVR_Destroy();

#if UNITY_STANDALONE_WIN
	[DllImport("user32", EntryPoint = "MessageBoxA", CharSet = CharSet.Ansi)]
	public static extern bool MessageBox(int hWnd,
	                                     [MarshalAs(UnmanagedType.LPStr)]string text,
	                                     [MarshalAs(UnmanagedType.LPStr)]string caption, uint type);
#endif

#else
	[DllImport(LibOVR)]
	private static extern void OVR_SetInitVariables(IntPtr activity, IntPtr vrActivityClass);
	[DllImport(LibOVR)]
	private static extern void OVR_SetEyeParms( int fbWidth, int fbHeight );
	[DllImport(LibOVR)]
	private static extern float OVR_GetBatteryLevel();
	[DllImport(LibOVR)]
	private static extern int OVR_GetBatteryStatus();
	[DllImport(LibOVR)]
	private static extern float OVR_GetBatteryTemperature();
	[DllImport(LibOVR)]
	private static extern int OVR_GetVolume();
	[DllImport(LibOVR)]
	private static extern double OVR_GetTimeSinceLastVolumeChange();

	[DllImport(LibOVR)]
	private static extern bool OVR_GetPlayerEyeHeight(ref float eyeHeight);
	[DllImport(LibOVR)]
	private static extern bool OVR_GetInterpupillaryDistance(ref float interpupillaryDistance);
#endif
}
