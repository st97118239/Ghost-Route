using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NewGraph;

using UnityEngine;
using UnityEditor;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

/// <summary>
/// Example to create a custom context menu.
/// Inherit from NewGraph.ContextMenu and add the [CustomContextMenu].
/// Please note: There can only be one custom context menu for the whole graph.
/// </summary>
[CustomContextMenu]
public class CustomMenu : NewGraph.ContextMenu
{
    static CustomMenu()
    {
        field = typeof(NewGraph.GraphWindow).GetField("window", BindingFlags.NonPublic | BindingFlags.Static);
    }

    private static GraphWindow window;
    private static readonly FieldInfo field;

    private static GraphWindow Window => window ??= (GraphWindow)field!.GetValue(null);

    protected override void AddNodeEntries()
    {
        base.AddNodeEntries();
        AddNodeEntry("Dialogues/Refresh SOs", (obj) => { RefreshSO(); });
        AddNodeEntry("Dialogues/Create SOs", (obj) => { CreateMissingSO(); });
        AddNodeEntry("Dialogues/Utility/Remove unused SOs", (obj) => { RemoveUnusedSO(); });
        AddNodeEntry("Dialogues/Utility/Update SOs", (obj) => { UpdateSO(); });
    }

    private static void CreateMissingSO()
    {
        DialogueHolderNode dialogueHolderNode = null;
        List<NodeModel> nodes = Window.graphController.graphData.Nodes;

        List<NodeModel> dialogueNodes = new();
        List<NodeModel> answerNodes = new();

        foreach (NodeModel node in nodes)
        {
            if (node.GetName() == "DialogueNode")
                dialogueNodes.Add(node);
            else if (node.GetName() == "AnswerNode")
                answerNodes.Add(node);
            else if (node.GetName() == "DialogueHolderNode")
            {
                DialogueHolderNode nodeData = (DialogueHolderNode)node.nodeData;
                dialogueHolderNode = nodeData;
            }
        }

        if (dialogueHolderNode == null)
        {
            Debug.LogError("No DialogueHolderNode found");
            return;
        }

        Directory.CreateDirectory("Assets/ScriptableObjects/Dialogues/" + dialogueHolderNode.graphName);
        Directory.CreateDirectory("Assets/ScriptableObjects/Answers/" + dialogueHolderNode.graphName);

        foreach (NodeModel node in dialogueNodes)
        {
            DialogueNode nodeData = (DialogueNode)node.nodeData;

            if (nodeData.dialogue != null) continue;
            Dialogue dialogue = CreateInstance<Dialogue>();
            nodeData.dialogue = dialogue;
            AssetDatabase.CreateAsset(dialogue, "Assets/ScriptableObjects/Dialogues/" + dialogueHolderNode.graphName + "/" + node.GetHashCode() + ".asset");
            EditorUtility.SetDirty(dialogue);
        }

        foreach (NodeModel node in answerNodes)
        {
            AnswerNode nodeData = (AnswerNode)node.nodeData;

            if (nodeData.answer != null) continue;
            Answer answer = CreateInstance<Answer>();
            nodeData.answer = answer;
            AssetDatabase.CreateAsset(answer, "Assets/ScriptableObjects/Answers/" + dialogueHolderNode.graphName + "/" + node.GetHashCode() + ".asset");
            EditorUtility.SetDirty(answer);
        }

        RefreshSO();
    }

    private static void RefreshSO()
    {
        DialogueHolderNode dialogueHolderNode = null;
        DialogueHolder dialogueHolder = null;

        List<NodeModel> nodes = Window.graphController.graphData.Nodes;

        List<NodeModel> dialogueNodes = new();
        List<NodeModel> answerNodes = new();

        foreach (NodeModel node in nodes)
        {
            if (node.GetName() == "DialogueNode")
                dialogueNodes.Add(node);
            else if (node.GetName() == "AnswerNode")
                answerNodes.Add(node);
            else if (node.GetName() == "DialogueHolderNode")
            {
                DialogueHolderNode nodeData = (DialogueHolderNode)node.nodeData;
                dialogueHolder = nodeData.dialogueHolder;

                dialogueHolderNode = nodeData;
            }
        }

        if (dialogueHolder == null)
        {
            Debug.LogError("No DialogueHolderNode found");
            return;
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
                    if (nodeData.answers[j] == null)
                    {
                        Debug.LogError("Empty answer found on " + dialogue.name);
                        continue;
                    }

                    if (nodeData.answers[j].answer == null)
                    {
                        AnswerNode answerNodeData = nodeData.answers[j];
                        Answer answer = answerNodeData.answer;

                        answer.text = answerNodeData.text;
                        answer.dialogueID = answerNodeData.dialogue != null ? answerNodeData.dialogue.dialogue.name : string.Empty;

                        dialogueHolder.answers[i] = answer;
                        EditorUtility.SetDirty(answer);
                    }

                    dialogue.answersID[j] = nodeData.answers[j].answer.name;
                }
            }
            else
                dialogue.answersID = null;
            dialogue.eventToPlay = nodeData.eventToPlay;
            dialogue.soundToPlay = nodeData.soundToPlay;
            dialogue.loopSound = nodeData.loopSound;
            dialogue.musicToPlay = nodeData.musicToPlay;
            dialogue.minigame = nodeData.minigame;
            dialogue.wonDialogueID = nodeData.wonDialogue != null ? nodeData.wonDialogue.dialogue.name : string.Empty;
            dialogue.loseDialogueID = nodeData.loseDialogue != null ? nodeData.loseDialogue.dialogue.name : string.Empty;
            dialogue.ending = nodeData.ending;
            dialogue.sprite = nodeData.sprite;
            dialogue.background = nodeData.background;

            dialogueHolder.dialogues[i] = dialogue;

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

        if (dialogueHolderNode.startingDialogue != null) 
            dialogueHolder.startingDialogueID = dialogueHolderNode.startingDialogue.dialogue.name;

        EditorUtility.SetDirty(dialogueHolder);

        Debug.Log("Saved dialogues");
    }

    private static void RemoveUnusedSO()
    {
        DialogueHolderNode dialogueHolderNode = null;

        List<NodeModel> nodes = Window.graphController.graphData.Nodes;
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
            else if (node.GetName() == "DialogueHolderNode")
            {
                DialogueHolderNode nodeData = (DialogueHolderNode)node.nodeData;
                dialogueHolderNode = nodeData;
            }

            nodeNames[i] = nodeName;
        }

        if (dialogueHolderNode == null)
        {
            Debug.LogError("No DialogueHolderNode found");
            return;
        }

        string[] dialogueFiles = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Dialogues/" + dialogueHolderNode.graphName);
        string[] answerFiles = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/Answers/" + dialogueHolderNode.graphName);
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
            assetPath = assetPath + dialogueHolderNode.graphName + "/";
            AssetDatabase.DeleteAsset("Assets/ScriptableObjects/" + assetPath + fileName + ".asset");
        }
    }

    private static void UpdateSO()
    {
        DialogueHolder dialogueHolder = null;

        List<NodeModel> nodes = Window.graphController.graphData.Nodes;

        List<NodeModel> dialogueNodes = new();
        List<NodeModel> answerNodes = new();

        foreach (NodeModel node in nodes)
        {
            if (node.GetName() == "DialogueNode")
                dialogueNodes.Add(node);
            else if (node.GetName() == "AnswerNode")
                answerNodes.Add(node);
            else if (node.GetName() == "DialogueHolderNode")
            {
                DialogueHolderNode nodeData = (DialogueHolderNode)node.nodeData;
                dialogueHolder = nodeData.dialogueHolder;
            }
        }

        if (dialogueHolder == null)
        {
            Debug.LogError("No DialogueHolderNode found");
            return;
        }

        dialogueHolder.dialogues = new Dialogue[dialogueNodes.Count];
        foreach (NodeModel node in dialogueNodes)
        {
            DialogueNode nodeData = (DialogueNode)node.nodeData;
            Dialogue dialogue = nodeData.dialogue;

            nodeData.charName = dialogue.charName;
            nodeData.text = dialogue.text;
            nodeData.voiceline = dialogue.voiceline;
            nodeData.delay = dialogue.delay;
            nodeData.eventToPlay = dialogue.eventToPlay;
            nodeData.soundToPlay = dialogue.soundToPlay;
            nodeData.loopSound = dialogue.loopSound;
            nodeData.musicToPlay = dialogue.soundToPlay;
            nodeData.minigame = dialogue.minigame;
            nodeData.ending = dialogue.ending;
            nodeData.sprite = dialogue.sprite;
            nodeData.background = dialogue.background;
        }

        dialogueHolder.answers = new Answer[answerNodes.Count];
        foreach (NodeModel node in answerNodes)
        {
            AnswerNode nodeData = (AnswerNode)node.nodeData;
            Answer answer = nodeData.answer;

            nodeData.text = answer.text;
        }

        Debug.Log("Saved dialogues");
    }
}