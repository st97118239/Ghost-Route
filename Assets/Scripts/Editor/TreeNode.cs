using UnityEditor;
using UnityEngine;

public class TreeNode : ScriptableObject
{
    public TreeNode[] nextNodes;
    public SerializedObject obj;
    public Vector2 position;
}