// Make sure your class is serializable
// And add the Node attribute. Here you can define a special color for your node as well as organize it with subcategories.
using NewGraph;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable, Node("#277e9c", "Dialogue")]
public class DialogueHolderNode : INode
{ // Make sure to implement the INode interface so the graph can serialize this easily.
  // with the Portattribute you can create visual connectable ports in the graph view to connect to other nodes.
  // it is important to also add the SerializeReference attribute, so that unity serializes this field as a reference.

    [GraphDisplay(DisplayType.BothViews)]
    public string graphName;
    [GraphDisplay(DisplayType.BothViews)]
    public DialogueHolder dialogueHolder;
    [Port, SerializeReference]
    public DialogueNode startingDialogue;
}