using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Mask))]
public class MaskPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property wrapper
        EditorGUI.BeginProperty(position, label, property);

        // Retrieve the serialized fields within the "Mask" class
        SerializedProperty maskField = property.FindPropertyRelative("_mask");

        // Validate the `_mask` field
        if (!ValidateMask(maskField))
        {
            // Set `_mask` to default value if validation fails
            SetDefaultMask(maskField);
        }

        // Define rectangles for each part of the drawer
        Rect maskStringRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
        Rect maskArrayRect = new Rect(
            position.x,
            position.y
                + EditorGUIUtility.singleLineHeight
                + EditorGUIUtility.standardVerticalSpacing,
            position.width,
            EditorGUI.GetPropertyHeight(maskField)
        );

        // Draw the non-editable Mask string
        string maskString = GetString(maskField);
        EditorGUI.LabelField(maskStringRect, "Mask Address", maskString);

        // Draw the editable byte array fields
        EditorGUI.PropertyField(maskArrayRect, maskField, new GUIContent("Bytes"));

        // Apply changes
        property.serializedObject.ApplyModifiedProperties();

        // End property wrapper
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Retrieve the serialized fields within the "Mask" class
        SerializedProperty maskField = property.FindPropertyRelative("_mask");

        // Calculate the dynamic height for the byte[] (handles expanded array)
        float arrayHeight = EditorGUI.GetPropertyHeight(maskField);

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

    // Helper method to validate the `_mask` byte array
    private bool ValidateMask(SerializedProperty property)
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

    // Helper method to set `_mask` to the default value
    private void SetDefaultMask(SerializedProperty property)
    {
        property.arraySize = 4;
        for (int i = 0; i < 4; i++)
        {
            if (i < 3)
                property.GetArrayElementAtIndex(i).intValue = 255;
            else
                property.GetArrayElementAtIndex(i).intValue = 0;
        }
    }

    private string GetString(SerializedProperty property)
    {
        byte[] maskBytes = GetByteArray(property);
        int bits = GetMaskBitsCount(maskBytes);
        return $"{string.Join(".", maskBytes)} (/{bits})";
    }

    public int GetMaskBitsCount(byte[] _mask)
    {
        int bits = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 7; j >= 0; j--)
            {
                if ((_mask[i] & (1 << j)) != 0)
                    bits++;
            }
        }
        return bits;
    }
}
