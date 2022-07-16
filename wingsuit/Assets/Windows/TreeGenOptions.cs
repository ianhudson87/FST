using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeGenOptions : EditorWindow
{
    public Vector3 basePosition;
    public float baseRadius;
    public float radiusMultiplier; // everytime we go up a block, how much to reduce the radius
    public int numSides;
    public float levelHeight;
    public float firstBranchHeight;
    public float branchFrequency; // average number of branches
    // string myString = "Hello World";
    // bool groupEnabled;
    // bool myBool = true;
    // float myFloat = 1.23f;
    // string treeButton = "generate tree here!";
    public TreeGenOptions(Vector3 basePosition, float baseRadius, float radiusMultiplier, int numSides, float levelHeight) {
        this.basePosition = basePosition;
        this.baseRadius = baseRadius;
        this.radiusMultiplier = radiusMultiplier;
        this.numSides = numSides;
        this.levelHeight = levelHeight;
    }
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Tree Gen Options")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TreeGenOptions));
    }
    
    void OnGUI()
    {
        // GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        // myString = EditorGUILayout.TextField ("Text Field", myString);
        
        // groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        // myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        // myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup ();

        // if (GUILayout.Button(treeButton))
        // {
        //     Debug.Log("Generate Tree!");
        // }
    }
}
