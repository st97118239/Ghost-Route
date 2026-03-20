// Make sure your class is serializable
// And add the Node attribute. Here you can define a special color for your node as well as organize it with subcategories.
using NewGraph;
using System;
using UnityEngine;

[Serializable, Node("#962d21", "Dialogue")]
public class AnswerNode : INode
{ // Make sure to implement the INode interface so the graph can serialize this easily.
    // with the Portattribute you can create visual connectable ports in the graph view to connect to other nodes.
    // it is important to also add the SerializeReference attribute, so that unity serializes this field as a reference.

    [GraphDisplay(DisplayType.BothViews)]
    public Answer answer;
    public string text;
    [Port, SerializeReference]
    public DialogueNode dialogue;
}