﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnshObjectBehaviour))]
[CanEditMultipleObjects]
public class CustomUnshadedEditor : Editor
{
    //References the TabExample Srcipt and gives us access to it. 
    private UnshObjectBehaviour myTarget;

    //Gives access to the serialized objects
    private SerializedObject soTarget;

    //Exact same variables we had on UnshObjectBehaviour but as Serialized Properties

    //Audio
    private SerializedProperty bandFrequency;
    private SerializedProperty threshold;
    private SerializedProperty audioClip;

    //Scale
    private SerializedProperty X;
    private SerializedProperty Y;
    private SerializedProperty Z;
    private SerializedProperty scaleMultiplier;

    //Rotation
    private SerializedProperty rotate;
    private SerializedProperty speedMultiplier;
    private SerializedProperty RotateX;
    private SerializedProperty RotateY;
    private SerializedProperty RotateZ;

    //Color
    private SerializedProperty changeColor;
    private SerializedProperty colorStart;
    private SerializedProperty colorEnd;

    //Emission
    private SerializedProperty changeEmission;
    private SerializedProperty emissionStart;
    private SerializedProperty emissionEnd;

    //Light
    private SerializedProperty useLight;
    private SerializedProperty lightColor;
    private SerializedProperty lightIntensity;



    //Default color
    Color TempColor = Color.white;

    private void OnEnable()
    {
        //Populates 'myTarget' with whatever script the inspector is attached to and all its variables
        myTarget = (UnshObjectBehaviour)target;

        //Populate 'soTarget' with the target object
        soTarget = new SerializedObject(targets);

        //On the serialized object, go and find the property with the name given. (Seraches for it on UnshObjectBehaviour)

        bandFrequency = soTarget.FindProperty("bandFrequency");
        audioClip  = soTarget.FindProperty("audioController");
        threshold  = soTarget.FindProperty("threshold");


        X               = soTarget.FindProperty("X");
        Y               = soTarget.FindProperty("Y");
        Z               = soTarget.FindProperty("Z");
        scaleMultiplier = soTarget.FindProperty("scaleMultiplier");

        rotate          = soTarget.FindProperty("rotate");
        speedMultiplier = soTarget.FindProperty("speedMultiplier");
        RotateX         = soTarget.FindProperty("RotateX");
        RotateY         = soTarget.FindProperty("RotateY");
        RotateZ         = soTarget.FindProperty("RotateZ");

        changeColor = soTarget.FindProperty("changeColor");
        colorStart = soTarget.FindProperty("colorStart");
        colorEnd = soTarget.FindProperty("colorEnd");

        changeEmission = soTarget.FindProperty("changeEmission");
        emissionStart = soTarget.FindProperty("emissionStart");
        emissionEnd = soTarget.FindProperty("emissionEnd");

        useLight        = soTarget.FindProperty("useLight");
        lightColor      = soTarget.FindProperty("lightColor");
        lightIntensity  = soTarget.FindProperty("lightIntensity");



        if (myTarget.block == null)
        {
            myTarget.block = new MaterialPropertyBlock();
        }
        if (myTarget.myObject == null)
        {
            myTarget.myObject = myTarget.GetComponent<Renderer>();
        }

    }

    public override void OnInspectorGUI()
    {
        //Updates the representation of the serialized object
        soTarget.Update();

        //Checks for any changes on the selected tab
        EditorGUI.BeginChangeCheck();

        //Alows you to move between tabs, you send the current index tab and the text each button has
        myTarget.toolbarTab = GUILayout.Toolbar(myTarget.toolbarTab, new string[] { "Audio", "Scale", "Rotation", "Material", "Light" });

        Renderer renderColor = myTarget.GetComponent<Renderer>();

        //Checks which tab has been clicked on
        switch (myTarget.toolbarTab)
        {
            //First tab selected
            case 0:
                //Set the current tab to the tab named B1
                myTarget.currentTab = "Audio";
                break;

            //Second tab selected
            case 1:
                myTarget.currentTab = "Scale";
                break;

            //Third tab selected
            case 2:
                myTarget.currentTab = "Rotation";
                break;

            //Fourth tab selected
            case 3:
                myTarget.currentTab = "Material";
                break;
            //Fifth tab selected
            case 4:
                myTarget.currentTab = "Light";
                break;
        }

        ////Returns true if there has been a change of selection between tabs
        //if (EditorGUI.EndChangeCheck())
        //{
        //    //Apply changes and set the focus control of the variables to null
        //    soTarget.ApplyModifiedProperties();
        //    GUI.FocusControl(null);
        //}

        //Checks for any changes on the properties of the current tab
        EditorGUI.BeginChangeCheck();

        myTarget.myObject.GetPropertyBlock(myTarget.block);
        myTarget.block.SetColor("_Color", myTarget.colorStart);
        myTarget.myObject.SetPropertyBlock(myTarget.block);

        //Drawing the properties depending on the selected tab
        switch (myTarget.currentTab)
        {
            case "Audio":
                //Draw properties on Inspector
                EditorGUILayout.PropertyField(bandFrequency);
                EditorGUILayout.PropertyField(audioClip);
                EditorGUILayout.PropertyField(threshold);
                break;
            case "Scale":
                //Draw properties on Inspector
                EditorGUILayout.PropertyField(X);
                EditorGUILayout.PropertyField(Y);
                EditorGUILayout.PropertyField(Z);
                EditorGUILayout.PropertyField(scaleMultiplier);
                break;
            case "Rotation":
                //Draw properties on Inspector
                EditorGUILayout.PropertyField(rotate);
                EditorGUILayout.PropertyField(speedMultiplier);
                EditorGUILayout.PropertyField(RotateX);
                EditorGUILayout.PropertyField(RotateY);
                EditorGUILayout.PropertyField(RotateZ);
                break;
            case "Material":
                //Draw properties on Inspector
                EditorGUILayout.PropertyField(changeColor);
                EditorGUILayout.PropertyField(colorStart);
                EditorGUILayout.PropertyField(colorEnd); 

                EditorGUILayout.PropertyField(changeEmission);
                EditorGUILayout.PropertyField(emissionStart);
                EditorGUILayout.PropertyField(emissionEnd);

                myTarget.myObject.GetPropertyBlock(myTarget.block);
                myTarget.block.SetColor("_Color", myTarget.colorStart);
                myTarget.myObject.SetPropertyBlock(myTarget.block);
                break;

            case "Light":
                EditorGUILayout.PropertyField(useLight);
                EditorGUILayout.PropertyField(lightIntensity);
                EditorGUILayout.PropertyField(lightColor);
                break;
        }

        //Returns true if there has been a change on the properties of the selected tab
        if (EditorGUI.EndChangeCheck())
        {
            //Apply changes to the properties
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

    }
}
