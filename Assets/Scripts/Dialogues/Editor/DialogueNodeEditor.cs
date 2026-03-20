using NewGraph;
using UnityEngine;
using UnityEngine.UIElements;

// add the CustomNodeEditor attribute to your NodeEditor class
// make sure to specify the nodeType this NodeEditor should be attached to.
// optionally you can also set a bool flag (editorForChildClasses) to re-use the same NodeEditor in child classes.
[CustomNodeEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : NodeEditor
{ // don't forget to derive from NodeEditor!
    // Initialize is called after the base NodeView was created.
    // You'll be provided with the nodeController object from where you can modify nearly everything.
    public override void Initialize(NodeController nodeController)
    {
        nodeController.nodeView.TitleContainer.Add(new Label("AAAA"));
    }
}