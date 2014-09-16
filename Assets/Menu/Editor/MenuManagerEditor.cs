using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager)), CanEditMultipleObjects]
public class MenuManagerEditor : Editor {
    
    public SerializedProperty 
        type_Prop,
        action_Prop,
        currentPanelObject_Prop,
        nextPanelObject_Prop;
    
    void OnEnable () {
        // Setup the SerializedProperties
        type_Prop = serializedObject.FindProperty ("type");
        action_Prop = serializedObject.FindProperty ("action");  
        currentPanelObject_Prop = serializedObject.FindProperty("currentPanelObject");
        nextPanelObject_Prop = serializedObject.FindProperty ("nextPanelObject");  
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update ();
        
        EditorGUILayout.PropertyField( type_Prop );
        
        MenuManager.Type st = (MenuManager.Type)type_Prop.enumValueIndex;
        
        switch( st ) {
        case MenuManager.Type.Action:
            EditorGUILayout.PropertyField( action_Prop, new GUIContent("Action") );   
            break;            
        case MenuManager.Type.Switcher:           
            EditorGUILayout.PropertyField( currentPanelObject_Prop, new GUIContent("Current Panel Object") );    
            EditorGUILayout.PropertyField ( nextPanelObject_Prop, new GUIContent("Next Panel Object") );
            break;            
        }
        
        
        serializedObject.ApplyModifiedProperties ();
    }
}