using System;
using System.Collections.Generic;
using System.IO;
using NewGraph;

using UnityEngine;
using NewGraph;
using UnityEditor;
using System.Linq;

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
        AddNodeEntry("Dialogues/Utility/Remove unused SOs", (obj) => { RemoveUnusedSO(); });
    }

    private static void CreateMissingSO()
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
            EditorUtility.SetDirty(dialogue);
        }

        foreach (NodeModel node in answerNodes)
        {
            AnswerNode nodeData = (AnswerNode)node.nodeData;

            if (nodeData.answer != null) continue;
            Answer answer = CreateInstance<Answer>();
            nodeData.answer = answer;
            AssetDatabase.CreateAsset(answer, "Assets/ScriptableObjects/Answers/" + node.GetHashCode() + ".asset");
            EditorUtility.SetDirty(answer);
        }

        RefreshSO();
    }

    private static void RefreshSO()
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
                dialogueHolder.startingDialogueID = dialogue.name;

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

    private static void RemoveUnusedSO()
    {
        ScriptableGraphModel graph = AssetDatabase.LoadAssetAtPath<ScriptableGraphModel>("Assets/Graphs/DialogueGraph.asset");
        List<NodeModel> nodes = graph.Nodes;
        string[] nodeNames = new string[nodes.Count];

        for (int i = 0; i < nodeNames.Length; i++)
        {
            NodeModel node = nodes[i];
            string nodeName = string.Empty;

            if (node.GetName() == "DialogueNode")
            {
                DialogueNode nodeData = (DialogueNode)node.nodeData;

                nodeName = nodeData.dialogue == null ? string.Empty : nodeData.dialogue.name;
            }
            else if (node.GetName() == "AnswerNode")
            {
                AnswerNode nodeData = (AnswerNode)node.nodeData;

                nodeName = nodeData.answer == null ? string.Empty : nodeData.answer.name;
            }

            nodeNames[i] = nodeName;
        }

        string[] dialogueFiles = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Dialogues");
        string[] answerFiles = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Answers");
        string[] files = new string[dialogueFiles.Length + answerFiles.Length];

        for (int i = 0; i < dialogueFiles.Length; i++) 
            files[i] = dialogueFiles[i];

        for (int i = 0; i < answerFiles.Length; i++)
            files[i + dialogueFiles.Length] = answerFiles[i];

        bool isAnswer = false;
        for (int i = 0; i < files.Length; i++)
        {
            if (i >= dialogueFiles.Length) isAnswer = true;

            string fileName = files[i];

            if (fileName.EndsWith(".meta")) continue;

            string[] parsedName = fileName.Split("\\");
            fileName = parsedName[^1];
            parsedName = fileName.Split(".");
            fileName = parsedName[0];
            files[i] = fileName;

            bool foundName = false;
            foreach (string t in nodeNames)
            {
                if (fileName == t)
                {
                    foundName = true;
                    break;
                }
            }

            if (foundName) continue;
            string assetPath = isAnswer ? "Answers/" : "Dialogues/";
            AssetDatabase.DeleteAsset("Assets/ScriptableObjects/" + assetPath + fileName + ".asset");
        }
    }
}