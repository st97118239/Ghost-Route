// Make sure your class is serializable
// And add the Node attribute. Here you can define a special color for your node as well as organize it with subcategories.
using NewGraph;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable, Node("#5d289c", "Dialogue")]
public class DialogueNode : INode
{ // Make sure to implement the INode interface so the graph can serialize this easily.
    // with the Portattribute you can create visual connectable ports in the graph view to connect to other nodes.
    // it is important to also add the SerializeReference attribute, so that unity serializes this field as a reference.

    [GraphDisplay(DisplayType.NodeView)]
    public Dialogue dialogue;
    [GraphDisplay(DisplayType.BothViews)]
    public string charName;
    [GraphDisplay(DisplayType.BothViews)]
    public string text;
    public AudioClip voiceline;
    [Port, SerializeReference]
    public DialogueNode nextDialogue;
    public float delay;
    [PortList, SerializeReference]
    public List<AnswerNode> answers;
    [GraphDisplay(DisplayType.BothViews)]
    public Events eventToPlay;
    [GraphDisplay(DisplayType.BothViews)] 
    public Sounds soundToPlay;
    public bool loopSound;
    [GraphDisplay(DisplayType.BothViews)]
    public Minigames minigame;
    public int scoreToWin;
    [Port, SerializeReference]
    public DialogueNode wonDialogue;
    [Port, SerializeReference]
    public DialogueNode loseDialogue;
    public Ending ending;
    public Sprite sprite;
    public Sprite background;
}