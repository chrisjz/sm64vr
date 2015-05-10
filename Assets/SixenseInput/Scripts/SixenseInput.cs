
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//
// Sixense Driver Unity Plugin
// Version 1.0
//

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Hand controller is bound to.
/// </summary>
public enum SixenseHands
{
	UNKNOWN = 0,
	LEFT = 1,
	RIGHT = 2,
}

/// <summary>
/// Controller button mask.
/// </summary>
/// <remarks>
/// The TRIGGER button is set when the Trigger value is greater than the TriggerButtonThreshold.
/// </remarks>
public enum SixenseButtons
{
	START = 1,
	ONE = 32,
	TWO = 64,
	THREE = 8,
	FOUR = 16,
	BUMPER = 128,
	JOYSTICK = 256,
	TRIGGER = 512,
}

/// <summary>
/// SixenseInput provides an interface for accessing Sixense controllers.
/// </summary>
/// <remarks>
/// This script should be bound to a GameObject in the scene so that its Start(), Update() and OnApplicationQuit() methods are called.  This can be done by adding the SixenseInput prefab to a scene.  The public static interface to the Controller objects provides a user friendly way to integrate Sixense controllers into your application.
/// </remarks>
public class SixenseInput : MonoBehaviour
{
	/// <summary>
	/// Controller objects provide access to Sixense controllers data.
	/// </summary>
	public class Controller
	{
		/// <summary>
		/// The controller enabled state.
		/// </summary>
		public bool Enabled { get { return m_Enabled; } }
		
		/// <summary>
		/// The controller docked state.
		/// </summary>
		public bool Docked { get { return m_Docked; } }
		
		/// <summary>
		/// Hand the controller bound to, which could be UNKNOWN.
		/// </summary>
		public SixenseHands Hand { get { return ( ( m_Hand == SixenseHands.UNKNOWN ) ? m_HandBind : m_Hand ); } }
		
		/// <summary>
		/// Value of trigger from released (0.0) to pressed (1.0).
		/// </summary>
		public float Trigger { get { return m_Trigger; } }
		
		/// <summary>
		/// Value of joystick X axis from left (-1.0) to right (1.0).
		/// </summary>
		public float JoystickX { get { return m_JoystickX; } }
		
		/// <summary>
		/// Value of joystick Y axis from bottom (-1.0) to top (1.0).
		/// </summary>
		public float JoystickY { get { return m_JoystickY; } }
		
		/// <summary>
		/// The controller position in Unity coordinates.
		/// </summary>
		public Vector3 Position { get { return new Vector3( m_Position.x, m_Position.y, -m_Position.z ); } }
		
		/// <summary>
		/// The raw controller position value.
		/// </summary>
		public Vector3 PositionRaw { get { return m_Position; } }
		
		/// <summary>
		/// The controller rotation in Unity coordinates.
		/// </summary>
		public Quaternion Rotation { get { return new Quaternion( -m_Rotation.x, -m_Rotation.y, m_Rotation.z, m_Rotation.w ); } }
		
		/// <summary>
		/// The raw controller rotation value.
		/// </summary>
		public Quaternion RotationRaw { get { return m_Rotation; } }
		
		/// <summary>
		/// The value which the Trigger value must pass to register a TRIGGER button press.  This value can be set.
		/// </summary>
		public float TriggerButtonThreshold { get { return m_TriggerButtonThreshold; } set { m_TriggerButtonThreshold = value; } }
		
		/// <summary>
		/// Returns true if the button parameter is being pressed.
		/// </summary>
		public bool GetButton( SixenseButtons button )
		{
			return ( ( button & m_Buttons ) != 0 );
		}
		
		/// <summary>
		/// Returns true if the button parameter was pressed this frame.
		/// </summary>
		public bool GetButtonDown( SixenseButtons button )
		{
			return ( ( button & m_Buttons ) != 0 ) && ( ( button & m_ButtonsPrevious ) == 0 );
		}
		
		/// <summary>
		/// Returns true if the button parameter was released this frame.
		/// </summary>
		public bool GetButtonUp( SixenseButtons button )
		{
			return ( ( button & m_Buttons ) == 0 ) && ( ( button & m_ButtonsPrevious ) != 0 );
		}
		
		/// <summary>
		/// The default trigger button threshold constant.
		/// </summary>
		public const float DefaultTriggerButtonThreshold = 0.9f;
		
		internal Controller()
		{
			m_Enabled = false;
			m_Docked = false;
			m_Hand = SixenseHands.UNKNOWN;
			m_HandBind = SixenseHands.UNKNOWN;
			m_Buttons = 0;
			m_ButtonsPrevious = 0;
			m_Trigger = 0.0f;
			m_JoystickX = 0.0f;
			m_JoystickY = 0.0f;
			m_Position.Set( 0.0f, 0.0f, 0.0f );
			m_Rotation.Set( 0.0f, 0.0f, 0.0f, 1.0f );
		}
		
		internal void SetEnabled( bool enabled )
		{
			m_Enabled = enabled;
		}
		
		internal void Update( ref SixensePlugin.sixenseControllerData cd )
		{
			m_Docked = ( cd.is_docked != 0 );
			m_Hand = ( SixenseHands )cd.which_hand;
			m_ButtonsPrevious = m_Buttons;
			m_Buttons = ( SixenseButtons )cd.buttons;
			m_Trigger = cd.trigger;
			m_JoystickX = cd.joystick_x;
			m_JoystickY = cd.joystick_y;
			m_Position.Set( cd.pos[0], cd.pos[1], cd.pos[2] );
			m_Rotation.Set( cd.rot_quat[0], cd.rot_quat[1], cd.rot_quat[2], cd.rot_quat[3] );
			if ( m_Trigger > TriggerButtonThreshold )
			{
				m_Buttons |= SixenseButtons.TRIGGER;
			}
		}
		
		internal SixenseHands HandBind { get { return m_HandBind; } set { m_HandBind = value; } }
		
		private bool m_Enabled;
		private bool m_Docked;
		private SixenseHands m_Hand;
		private SixenseHands m_HandBind;
		private SixenseButtons m_Buttons;
		private SixenseButtons m_ButtonsPrevious;
		private float m_Trigger;
		private float m_TriggerButtonThreshold = DefaultTriggerButtonThreshold;
		private float m_JoystickX;
		private float m_JoystickY;
		private Vector3 m_Position;
		private Quaternion m_Rotation;
	}
	
	/// <summary>
	/// Max number of controllers allowed by driver.
	/// </summary>
	public const uint MAX_CONTROLLERS = 4;
	
	/// <summary>
	/// Access to Controller objects.
	/// </summary>
	public static Controller[] Controllers { get { return m_Controllers; } }
	
	/// <summary>
	/// Gets the Controller object bound to the specified hand.
	/// </summary>
	public static Controller GetController( SixenseHands hand )
	{
		for ( int i = 0; i < MAX_CONTROLLERS; i++ )
		{
			if ( ( m_Controllers[i] != null ) && ( m_Controllers[i].Hand == hand ) )
			{
				return m_Controllers[i];
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Returns true if the base for zero-based index i is connected.
	/// </summary>
	public static bool IsBaseConnected( int i )
	{
		return ( SixensePlugin.sixenseIsBaseConnected( i ) != 0 );
	}
	
	/// <summary>
	/// Enable or disable the controller manager.
	/// </summary>
	public static bool ControllerManagerEnabled = true;
	
	private enum ControllerManagerState
	{
		NONE,
		BIND_CONTROLLER_ONE,
		BIND_CONTROLLER_TWO,
	}
	
	private static Controller[] m_Controllers = new Controller[MAX_CONTROLLERS];
	private ControllerManagerState m_ControllerManagerState = ControllerManagerState.NONE;
	
	/// <summary>
	/// Initialize the sixense driver and allocate the controllers.
	/// </summary>
	void Start()
	{
		SixensePlugin.sixenseInit();
		
		for ( int i = 0; i < MAX_CONTROLLERS; i++ )
		{
			m_Controllers[i] = new Controller();
		}

		// don't let the mobile device sleep
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	/// <summary>
	/// Update the static controller data once per frame.
	/// </summary>
	void Update()
	{
		// update controller data
		uint numControllersBound = 0;
		uint numControllersEnabled = 0;
		SixensePlugin.sixenseControllerData cd = new SixensePlugin.sixenseControllerData();
		for ( int i = 0; i < MAX_CONTROLLERS; i++ )
		{
			if ( m_Controllers[i] != null )
			{
				if ( SixensePlugin.sixenseIsControllerEnabled( i ) == 1 )
				{
					SixensePlugin.sixenseGetNewestData( i, ref cd );
					m_Controllers[i].Update( ref cd );
					m_Controllers[i].SetEnabled( true );
					numControllersEnabled++;
					if ( ControllerManagerEnabled && ( SixenseInput.Controllers[i].Hand != SixenseHands.UNKNOWN ) )
					{
						numControllersBound++;
					}
				}
				else
				{
					m_Controllers[i].SetEnabled( false );
				}
			}
		}
		
		// update controller manager
		if ( ControllerManagerEnabled )
		{
			if ( numControllersEnabled < 2 )
			{
				m_ControllerManagerState = ControllerManagerState.NONE;
			}
				
			switch( m_ControllerManagerState )
			{
			case ControllerManagerState.NONE:
				{
					if ( IsBaseConnected( 0 ) && ( numControllersEnabled > 1 ) )
					{
						if ( numControllersBound == 0 )
						{
							m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_ONE;
						}
						else if ( numControllersBound == 1 )
						{
							m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
						}
					}
				}
				break;
			case ControllerManagerState.BIND_CONTROLLER_ONE:
				{
					if ( numControllersBound > 0 )
					{
						m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
					}
					else
					{
						for ( int i = 0; i < MAX_CONTROLLERS; i++ )
						{
							if ( ( m_Controllers[i] != null ) && Controllers[i].GetButtonDown( SixenseButtons.TRIGGER ) && ( Controllers[i].Hand == SixenseHands.UNKNOWN ) )
							{
								Controllers[i].HandBind = SixenseHands.LEFT;
								SixensePlugin.sixenseAutoEnableHemisphereTracking( i );
								m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
								break;
							}
						}
					}
				}
				break;
			case ControllerManagerState.BIND_CONTROLLER_TWO:
				{
					if ( numControllersBound > 1 )
					{
						m_ControllerManagerState = ControllerManagerState.NONE;
					}
					else
					{
						for ( int i = 0; i < MAX_CONTROLLERS; i++ )
						{
							if ( ( m_Controllers[i] != null ) && Controllers[i].GetButtonDown( SixenseButtons.TRIGGER ) && ( Controllers[i].Hand == SixenseHands.UNKNOWN ) )
							{
								Controllers[i].HandBind = SixenseHands.RIGHT;
								SixensePlugin.sixenseAutoEnableHemisphereTracking( i );
								m_ControllerManagerState = ControllerManagerState.NONE;
								break;
							}
						}
					}
				}
				break;
			default:
				break;
			}
		}
	}
	
	/// <summary>
	/// Updates the controller manager GUI.
	/// </summary>
	void OnGUI()
	{
		if ( ControllerManagerEnabled && ( m_ControllerManagerState != ControllerManagerState.NONE ) )
		{
			uint boxWidth = 300;
			uint boxHeight = 24;
			string boxText = ( m_ControllerManagerState == ControllerManagerState.BIND_CONTROLLER_ONE ) ?
							 "Point left controller at base and pull trigger." :
							 "Point right controller at base and pull trigger.";
			GUI.Box( new Rect( ( ( Screen.width / 2 ) - ( boxWidth / 2 ) ), ( ( Screen.height / 2 ) - ( boxHeight / 2 ) ), boxWidth, boxHeight ), boxText );
		}
	}
	
	/// <summary>
	/// Exit sixense when the application quits.
	/// </summary>
	void OnApplicationQuit()
	{
		SixensePlugin.sixenseExit();
	}
}
