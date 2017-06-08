using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 
// A tool that will replace all instances of one game object (based on name or selection) in a scene with another
// Note: Save your scene before using this tool!!!!!!
public class GameObjectReplaceTool : EditorWindow
{
	// Vars
	private GameObject m_targetGameObject = null;
	private GameObject m_sourceGameObject = null;
 
	private bool useObjectSelection = false;
 
	private bool useOffsetPos = false;
	private float offsetPosX = 0;
	private float offsetPosY = 0;
	private float offsetPosZ = 0;
 
	private bool useOffsetRot = false;
	private float offsetRotX = 0;
	private float offsetRotY = 0;
	private float offsetRotZ = 0;
 
	private bool useOffsetScale = false;
	private float offsetScaleX = 0.0f;
	private float offsetScaleY = 0.0f;
	private float offsetScaleZ = 0.0f;
 
	private bool useSetScale = false;
	private float setScaleX = 1.0f;
	private float setScaleY = 1.0f;
	private float setScaleZ = 1.0f;
 
	private bool maintainChildren = false;
	private bool maintainParent = true;
 
	[MenuItem( "Edit/Game Object Replacer" )]
 
	static void Init()
	{
		// Create window
		GameObjectReplaceTool window = (GameObjectReplaceTool)EditorWindow.GetWindow(typeof(GameObjectReplaceTool));
		window.minSize = new Vector2( 400, 500 );
	}
 
	public void OnGUI()
	{
		// The target and source
		GUIContent sourcePop = new GUIContent("Source Object", "Select or drag an object onto this field that will replace all of the Target Objects");
		m_sourceGameObject = EditorGUILayout.ObjectField(sourcePop, m_sourceGameObject, typeof(GameObject), true) as GameObject;
		if(!useObjectSelection)
		{
			GUIContent targetPop = new GUIContent("Target Objects", "Select or drag an object onto this field that will designate all targets to be replaced with the Source Object (based on name)");
			m_targetGameObject = EditorGUILayout.ObjectField(targetPop, m_targetGameObject, typeof(GameObject), true) as GameObject;
		}
 
		GUIContent selectPop = new GUIContent("Use Selected Objects", "Check to use only selected objects within the scene as the targets for replacement");
		useObjectSelection = EditorGUILayout.Toggle(selectPop, useObjectSelection);
 
		// Set values
		useOffsetPos = EditorGUILayout.Toggle("Offset Position", useOffsetPos);
		if(useOffsetPos)
		{
			offsetPosX = EditorGUILayout.FloatField("X Pos Offset", offsetPosX);
			offsetPosY = EditorGUILayout.FloatField("Y Pos Offset", offsetPosY);
			offsetPosZ = EditorGUILayout.FloatField("Z Pos Offset", offsetPosZ);
		}
 
		useOffsetRot = EditorGUILayout.Toggle("Offset Rotation", useOffsetRot);
		if(useOffsetRot)
		{
			offsetRotX = EditorGUILayout.FloatField("X Rot Offset", offsetRotX);
			offsetRotY = EditorGUILayout.FloatField("Y Rot Offset", offsetRotY);
			offsetRotZ = EditorGUILayout.FloatField("Z Rot Offset", offsetRotZ);
		}
 
		if(!useSetScale)
		{
			useOffsetScale = EditorGUILayout.Toggle("Offset Scale", useOffsetScale);
			if(useOffsetScale)
			{
				offsetScaleX = EditorGUILayout.FloatField("X Scale Offset", offsetScaleX);
				offsetScaleY = EditorGUILayout.FloatField("Y Scale Offset", offsetScaleY);
				offsetScaleZ = EditorGUILayout.FloatField("Z Scale Offset", offsetScaleZ);
			}
		}
 
		useSetScale = EditorGUILayout.Toggle("Set Scale", useSetScale );
		if(useSetScale)
		{
			setScaleX = EditorGUILayout.FloatField("X Scale Value", setScaleX);
			setScaleY = EditorGUILayout.FloatField("Y Scale Value", setScaleY);
			setScaleZ = EditorGUILayout.FloatField("Z Scale Value", setScaleZ);
		}
 
		GUIContent childPop = new GUIContent("Maintain Children", "Check to replace only the Target's parent object and place all of the target's children into the Source Object");
		maintainChildren = EditorGUILayout.Toggle(childPop, maintainChildren);
 
		GUIContent parentPop = new GUIContent("Keep Target's Parent", "Check to place Source Objects into the same spot as their Targets within the hierarchy");
		maintainParent = EditorGUILayout.Toggle(parentPop, maintainParent);
 
		// Hit the button to do the swap
		if (GUILayout.Button("Replace Objects"))
		{
			CheckForReplace();
		}
	}
 
	private void CheckForReplace()
	{
		if( m_sourceGameObject != null && m_targetGameObject != null && !useObjectSelection)
		{
			string thisTargetName = m_targetGameObject.name;
 
			// Loop through whole scene
			foreach(GameObject thisObj in GameObject.FindObjectsOfType<GameObject>())
			{
				if(thisObj)
				{
					if (thisObj.name == thisTargetName)
					{
						ReplaceObject(thisObj);
					}
				}
			}
		}
		else if(useObjectSelection)
		{
			// Loop through selection
			foreach ( Transform transform in Selection.transforms )
			{
				if(transform.gameObject)
				{
					ReplaceObject(transform.gameObject);
				}
			}
		}
		else
		{
			EditorGUILayout.HelpBox("You must enter a source and a target!", MessageType.Error);
			Debug.LogWarning("GameObject Replace Tool: Both a source and target must be specified!");
		}
	}
 
	private void ReplaceObject(GameObject thisObj)
	{
		// Register for undo
		Undo.RecordObject(thisObj, "Replace Target");
 
		// Get transform data
		Vector3 p = thisObj.transform.position;
		Vector3 r = thisObj.transform.eulerAngles;
		Vector3 s = thisObj.transform.localScale;
 
		if(useOffsetPos)
		{				
			p.x += offsetPosX;
			p.y += offsetPosY;
			p.z += offsetPosZ;
		}
 
		if(useOffsetRot)
		{
			r.x += offsetRotX;
			r.y += offsetRotY;
			r.z += offsetRotZ;
		}
 
		if(useOffsetScale)
		{
			s.x += offsetScaleX;
			s.y += offsetScaleY;
			s.z += offsetScaleZ;
		}
 
		// Note: set scale will override an offset scale
		if(useSetScale)
		{
			s.x = setScaleX;
			s.y = setScaleY;
			s.z = setScaleZ;
		}
 
		// Create a clone based on prefab status
		GameObject source = null;
		PrefabType thisObjectType = PrefabUtility.GetPrefabType(m_sourceGameObject);
		if (thisObjectType != PrefabType.None)
		{
			Object thisPrefab = m_sourceGameObject;
			if(thisObjectType != PrefabType.Prefab)
			{
				// For some reason, IntantiatePrefab needs this...
				thisPrefab = PrefabUtility.GetPrefabParent(m_sourceGameObject);
			}
			source = PrefabUtility.InstantiatePrefab(thisPrefab) as GameObject;
		}
		else
		{
			source = Instantiate(m_sourceGameObject) as GameObject;
		 
			// Clear out the extra (clone) appended to name
			source.name = m_sourceGameObject.name;
		}
 
		// Set transform
		source.transform.position = p;
		source.transform.eulerAngles = r;
		source.transform.localScale = s;
 
		Undo.RegisterCreatedObjectUndo(source, "Replace Source");
 
		// Check children stuffs
		if(maintainChildren)
		{
			foreach(Transform thisChild in thisObj.transform)
			{
				thisChild.parent = source.transform;
			}
		}
 
		// Check for parents
		if (maintainParent)
		{
			source.transform.parent = thisObj.transform.parent;
		}
 
		// Undo stuffs
		Undo.DestroyObjectImmediate(thisObj);
		Undo.RecordObject(source, "Replace Source");		
	}
}