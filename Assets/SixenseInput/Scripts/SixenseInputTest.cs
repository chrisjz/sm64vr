
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class SixenseInputTest : MonoBehaviour
{
	void Start()
	{
	}
	
	void Update()
	{
		GameObject guiText = null;
		
		guiText = GameObject.Find( "gui_text_base_connected" );
		if ( guiText )
		{
			guiText.GetComponent<GUIText>().text = "Base Connected = " + SixenseInput.IsBaseConnected( 0 );
		}
		
		for ( uint i = 0; i < 2; i++ )
		{
			if ( SixenseInput.Controllers[i] != null )
			{
				uint controllerNumber = i + 1;
			
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_enabled" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Enabled = " + SixenseInput.Controllers[i].Enabled;
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_docked" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Docked = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].Docked;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_hand" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Hand = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].Hand;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_buttons" );
				GameObject guiText2 = GameObject.Find( "gui_text_controller_" + controllerNumber + "_buttons_2" );;
				if ( guiText && guiText2 )
				{
					uint buttonsCount = 0;
					string buttonsText = "";
					string buttonsText2 = "";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						foreach ( SixenseButtons button in System.Enum.GetValues( typeof( SixenseButtons ) ) )
						{
							if ( SixenseInput.Controllers[i].GetButton( button ) && ( buttonsCount < 4 ) )
							{
								if ( buttonsText != "" )
								{
									buttonsText += " | ";
								}
								buttonsText += button;
								buttonsCount++;
							}
							else if ( SixenseInput.Controllers[i].GetButton( button ) && ( buttonsCount >= 4 ) )
							{
								if ( buttonsText2 != "" )
								{
									buttonsText2 += " | ";
								}
								buttonsText2 += button;
								buttonsCount++;
							}
							
							//if ( SixenseInput.Controllers[i].GetButtonDown( button ) )
							//{
							//	Debug.Log( "Pressed = " + button );
							//}
							
							//if ( SixenseInput.Controllers[i].GetButtonUp( button ) )
							//{
							//	Debug.Log( "Released = " + button );
							//}
						}
					}
					guiText.GetComponent<GUIText>().text = "Buttons = " + buttonsText;
					guiText2.GetComponent<GUIText>().text = "" + buttonsText2;
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_trigger" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Trigger = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].Trigger;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_joystick_x" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Joystick X = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].JoystickX;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_joystick_y" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Joystick Y = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].JoystickY;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_position" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Position = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].Position;
					}
				}
				
				guiText = GameObject.Find( "gui_text_controller_" + controllerNumber + "_rotation" );
				if ( guiText )
				{
					guiText.GetComponent<GUIText>().text = "Rotation = ";
					if ( SixenseInput.Controllers[i].Enabled )
					{
						guiText.GetComponent<GUIText>().text += SixenseInput.Controllers[i].Rotation;
					}
				}
			}
		}
	}
}
