using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestValueChange : MonoBehaviour
{
    public int myInt;
    public float myFloat;
    public string myString;

    [ContextMenu("Change Value")]
    private void ChangeValue()
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "change test value");
#endif
        myInt = 9;
        myFloat = 88f;
        myString = "Hello!";
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
#endif
    }

    [ContextMenu("Create Dummy GameObject")]
    private void CreateDummyGameObject()
    {
        GameObject go = new("Dummy");
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, "create dummy gameObject");
#endif
    }
}
