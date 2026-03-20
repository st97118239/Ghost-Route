using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestGUI : EditorWindow
{
    [SerializeField] private SerializedObject rootDialogueOS;
    [SerializeField] private DialogueHolder dialogueHolder;
    private Vector2 scrollPos;
    private List<TreeNode> nodes;

    [MenuItem("Window/Test")]
    private static void OpenWindow()
    {
        TestGUI testGUI = GetWindow<TestGUI>();
        testGUI.Show();
    }

    private void Awake()
    {
        titleContent.text = "Test GUI";
        titleContent.tooltip = "AAAAAAAAAA";
        titleContent.image = EditorGUIUtility.IconContent("UnityLogo").image;
        minSize = new Vector2(400, 100);
        maxSize = new Vector2(1500, 900);
    }

    private void OnEnable()
    {
        Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>("Assets/ScriptableObjects/Dialogues/T0.asset");
        rootDialogueOS = new SerializedObject(dialogue);

        dialogueHolder = AssetDatabase.LoadAssetAtPath<DialogueHolder>("Assets/ScriptableObjects/DialogueHolder.asset");

        Debug.Log("Inspecting: " + rootDialogueOS.targetObject, rootDialogueOS.targetObject);

        LoadDialogues();
    }

    private void OnGUI()
    {
        Rect windowRect = position;
        Rect inspectorRect = new(10, 10, 250, windowRect.height - 20);
        float inspectorWidth = inspectorRect.x + inspectorRect.width;
        Rect boxRect = new(inspectorWidth + 10, 10, windowRect.width - 20 - inspectorWidth, windowRect.height - 20);
        float boxWidth = boxRect.x + boxRect.width;
        Rect boxContentRect = new(0, 0, 1500, 1500);
        Rect menuRect = new(boxWidth + 10, 10, windowRect.width - 20 - boxWidth, windowRect.height - 20);

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginArea(inspectorRect);

                rootDialogueOS.Update();

                SerializedObject currentDialogue = rootDialogueOS;
                SerializedProperty charNameProp = currentDialogue.FindProperty("charName");
                charNameProp.stringValue = EditorGUILayout.TextField("Character Name", charNameProp.stringValue);

                //SerializedProperty iterator = rootDialogueOS.GetIterator();
                //iterator.NextVisible(true);
                //while (iterator.NextVisible(false))
                //{
                //    EditorGUILayout.PropertyField(iterator);
                //}

                rootDialogueOS.ApplyModifiedProperties();
                GUILayout.EndArea();
            }
            GUILayout.EndVertical();

            scrollPos = GUI.BeginScrollView(boxRect, scrollPos, boxContentRect);
            {
                GUI.Box(boxContentRect, GUIContent.none);
                {
                    //float totalWidth = 10;
                    //float posBetweenNode = 15f;
                    //float nodeWidth = 50;

                    //SerializedObject dialogueObj = rootDialogueOS;

                    //GUI.Button(new Rect(totalWidth, 10, nodeWidth, 50), dialogueObj.targetObject.name);
                    //totalWidth += nodeWidth + posBetweenNode;

                    //Object next = rootDialogueOS.FindProperty("nextDialogue").objectReferenceValue;
                    //dialogueObj = new SerializedObject(next);

                    //while (true)
                    //{
                    //    GUI.Button(new Rect(totalWidth, 10, nodeWidth, 50), next.name);
                    //    totalWidth += nodeWidth + posBetweenNode;
                    //    next = dialogueObj.FindProperty("nextDialogue").objectReferenceValue;
                    //    if (next == null)
                    //    {
                    //        SerializedProperty answer = dialogueObj.FindProperty("answers");
                    //        for (int i = 0; i < answer.arraySize; i++)
                    //        {
                    //            GUI.Button(new Rect(totalWidth, 10, nodeWidth, 50), answer.GetArrayElementAtIndex(i).serializedObject.FindProperty("text").stringValue);
                    //            totalWidth += nodeWidth + posBetweenNode;
                    //        }
                    //        break;
                    //    }
                    //    dialogueObj = new SerializedObject(next);
                    //}
                }
            }
            GUI.EndScrollView();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Button :3"))
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("New Dialogue"), false, AAAA);

            menu.ShowAsContext();
        }
    }

    private void AAAA()
    {
        string[] files = Directory.GetFiles("Assets/ScriptableObjects/Dialogues/", "*.asset", SearchOption.TopDirectoryOnly);

        dialogueHolder.dialogues = new Dialogue[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(file);

            if (dialogue.answers.Length > 0)
            {
                dialogue.answersID = new string[dialogue.answers.Length];
                for (int j = 0; j < dialogue.answersID.Length; j++)
                    dialogue.answersID[j] = dialogue.answers[j].name;
            }

            if (dialogue.nextDialogue != null)
                dialogue.nextDialogueID = dialogue.nextDialogue.name;

            if (dialogue.wonDialogue != null)
                dialogue.wonDialogueID = dialogue.wonDialogue.name;

            if (dialogue.loseDialogue != null)
                dialogue.loseDialogueID = dialogue.loseDialogue.name;

            dialogueHolder.dialogues[i] = dialogue;
        }

        files = Directory.GetFiles("Assets/ScriptableObjects/Answers/", "*.asset", SearchOption.TopDirectoryOnly);

        dialogueHolder.answers = new Answer[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            Answer answer = AssetDatabase.LoadAssetAtPath<Answer>(file);

            if (answer.dialogue != null)
                answer.dialogueID = answer.dialogue.name;

            dialogueHolder.answers[i] = answer;
        }

        EditorUtility.SetDirty(dialogueHolder);
    }

    private void LoadDialogues()
    {
        float posBetweenNode = 15f;
        float nodeWidth = 50f;
        float nodeHeight = 50f;

        SerializedObject currentObj = rootDialogueOS;
        nodes = new List<TreeNode>();

        while (true)
        {
            // iterate through every dialogue and save it into a node
            // recalculate the positon for every node saved

            TreeNode newNode = CreateInstance<TreeNode>();
            newNode.obj = currentObj;

            System.Type objType = currentObj.targetObject.GetType();
            if (objType == typeof(Dialogue))
            {
                Object next = currentObj.FindProperty("nextDialogue").objectReferenceValue;
                Debug.Log(next);
                if (next == null) break;
                SerializedObject nextObj = new(next);
                if (nextObj.targetObject == null) break;
                //break;
                currentObj = nextObj;
            }
            else if (objType == typeof(Answer))
            {
                break;
            }
            else
            {
                break;
            }
        } 

        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i].obj.FindProperty("charName"));
        }
    }

    private void CreateNewNode()
    {

    }
}
