using System;
using System.Collections.Generic;
using NewGraph;

using UnityEngine;
using NewGraph;
using UnityEditor;

/// <summary>
/// Example to create a custom context menu.
/// Inherit from NewGraph.ContextMenu and add the [CustomContextMenu].
/// Please note: There can only be one custom context menu for the whole graph.
/// </summary>
[CustomContextMenu]
public class CustomMenu : NewGraph.ContextMenu
{
    protected override void AddNodeEntries()
    {
        base.AddNodeEntries();
        AddNodeEntry("Dialogues/Refresh SOs", (obj) => { RefreshSO(); });
        AddNodeEntry("Dialogues/Create SOs", (obj) => { CreateMissingSO(); });
    }

    private void CreateMissingSO()
    {
        ScriptableGraphModel graph = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>("Assets/Graphs/DialogueGraph.asset");
        List<NodeModel> nodes = graph.Nodes;

        List<NodeModel> dialogueNodes = new();
        List<NodeModel> answerNodes = new();

        foreach (NodeModel node in nodes)
        {
            if (node.GetName() == "DialogueNode")
                dialogueNodes.Add(node);
            else if (node.GetName() == "AnswerNode")
                answerNodes.Add(node);
        }

        foreach (NodeModel node in dialogueNodes)
        {
            DialogueNode nodeData = (DialogueNode)node.nodeData;

            if (nodeData.dialogue != null) continue;
            Dialogue dialogue = CreateInstance<Dialogue>();
            nodeData.dialogue = dialogue;
            AssetDatabase.CreateAsset(dialogue, "Assets/ScriptableObjects/Dialogues/" + node.GetHashCode() + ".asset");
        }

        foreach (NodeModel node in answerNodes)
        {
            AnswerNode nodeData = (AnswerNode)node.nodeData;

            if (nodeData.answer != null) continue;
            Answer answer = CreateInstance<Answer>();
            nodeData.answer = answer;
            AssetDatabase.CreateAsset(answer, "Assets/ScriptableObjects/Answers/" + node.GetHashCode() + ".asset");
        }

        RefreshSO();
    }

    private void RefreshSO()
    {
        ScriptableGraphModel graph = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>("Assets/Graphs/DialogueGraph.asset");
        DialogueHolder dialogueHolder = AssetDatabase.LoadAssetAtPath<DialogueHolder>("Assets/ScriptableObjects/DialogueHolder.asset");

        List<NodeModel> nodes = graph.Nodes;

        List<NodeModel> dialogueNodes = new();
        List<NodeModel> answerNodes = new();

        foreach (NodeModel node in nodes)
        {
            if (node.GetName() == "DialogueNode")
                dialogueNodes.Add(node);
            else if (node.GetName() == "AnswerNode") 
                answerNodes.Add(node);
        }

        dialogueHolder.dialogues = new Dialogue[dialogueNodes.Count];
        for (int i = 0; i < dialogueNodes.Count; i++)
        {
            NodeModel node = dialogueNodes[i];

            DialogueNode nodeData = (DialogueNode)node.nodeData;
            Dialogue dialogue = nodeData.dialogue;

            dialogue.charName = nodeData.charName;
            dialogue.text = nodeData.text;
            dialogue.voiceline = nodeData.voiceline;
            dialogue.nextDialogueID = nodeData.nextDialogue != null ? nodeData.nextDialogue.dialogue.name : string.Empty;
            dialogue.delay = nodeData.delay;
            if (nodeData.answers != null && nodeData.answers.Count > 0)
            {
                dialogue.answersID = new string[nodeData.answers.Count];
                for (int j = 0; j < nodeData.answers.Count; j++)
                {
                    dialogue.answersID[j] = nodeData.answers[j].answer.name;
                }
            }
            else
                dialogue.answersID = null;
            dialogue.eventToPlay = nodeData.eventToPlay;
            dialogue.minigame = nodeData.minigame;
            dialogue.scoreToWin = nodeData.scoreToWin;
            dialogue.wonDialogueID = nodeData.wonDialogue != null ? nodeData.wonDialogue.dialogue.name : string.Empty;
            dialogue.loseDialogueID = nodeData.loseDialogue != null ? nodeData.loseDialogue.dialogue.name : string.Empty;
            dialogue.ending = nodeData.ending;
            dialogue.sprite = nodeData.sprite;
            dialogue.goreSprite = nodeData.goreSprite;
            dialogue.goreBackground = nodeData.goreBackground;

            dialogueHolder.dialogues[i] = dialogue;

            if (nodeData.isDefault)
                dialogueHolder.startingDialogue = dialogue;

            EditorUtility.SetDirty(dialogue);
        }

        dialogueHolder.answers = new Answer[answerNodes.Count];
        for (int i = 0; i < answerNodes.Count; i++)
        {
            NodeModel node = answerNodes[i];

            AnswerNode nodeData = (AnswerNode)node.nodeData;
            Answer answer = nodeData.answer;

            answer.text = nodeData.text;
            answer.dialogueID = nodeData.dialogue != null ? nodeData.dialogue.dialogue.name : string.Empty;

            dialogueHolder.answers[i] = answer;
            EditorUtility.SetDirty(answer);
        }

        EditorUtility.SetDirty(dialogueHolder);
    }
}