using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IP))]
public class IPPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property wrapper
        EditorGUI.BeginProperty(position, label, property);

        // Retrieve the serialized fields within the "IP" class
        SerializedProperty ipField = property.FindPropertyRelative("_ip");

        // Validate the `_ip` field
        if (!ValidateIP(ipField))
        {
            // Set `_ip` to default value if validation fails
            SetDefaultIP(ipField);
        }

        // Define rectangles for each part of the drawer
        Rect ipStringRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
        Rect ipArrayRect = new Rect(
            position.x,
            position.y
                + EditorGUIUtility.singleLineHeight
                + EditorGUIUtility.standardVerticalSpacing,
            position.width,
            EditorGUI.GetPropertyHeight(ipField)
        );

        // Draw the non-editable IP string
        string ipString = GetString(ipField);
        EditorGUI.LabelField(ipStringRect, "IP Address", ipString);

        // Draw the editable byte array fields
        EditorGUI.PropertyField(ipArrayRect, ipField, new GUIContent("Bytes"));

        // Apply changes
        property.serializedObject.ApplyModifiedProperties();

        // End property wrapper
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Retrieve the serialized fields within the "IP" class
        SerializedProperty ipField = property.FindPropertyRelative("_ip");

        // Calculate the dynamic height for the byte[] (handles expanded array)
        float arrayHeight = EditorGUI.GetPropertyHeight(ipField);

        // Total height: Label height + Spacing + Dynamic array height
        return EditorGUIUtility.singleLineHeight
            + EditorGUIUtility.standardVerticalSpacing
            + arrayHeight;
    }

    // Helper method to get the byte array from the serialized property
    private byte[] GetByteArray(SerializedProperty property)
    {
        byte[] array = new byte[property.arraySize];
        for (int i = 0; i < property.arraySize; i++)
        {
            array[i] = (byte)property.GetArrayElementAtIndex(i).intValue;
        }
        return array;
    }

    // Helper method to validate the `_ip` byte array
    private bool ValidateIP(SerializedProperty property)
    {
        if (property == null)
            return false;
        if (property.arraySize != 4)
            return false;

        for (int i = 0; i < property.arraySize; i++)
        {
            int value = property.GetArrayElementAtIndex(i).intValue;
            if (value < 0 || value > 255)
            {
                return false;
            }
        }
        return true;
    }

    // Helper method to set `_ip` to the default value
    private void SetDefaultIP(SerializedProperty property)
    {
        property.arraySize = 4;
        for (int i = 0; i < 4; i++)
        {
            property.GetArrayElementAtIndex(i).intValue = 0;
        }
    }

    private string GetString(SerializedProperty property)
    {
        byte[] ipBytes = GetByteArray(property);
        return string.Join(".", ipBytes);
    }
}
