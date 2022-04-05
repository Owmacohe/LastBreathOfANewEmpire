using System.Collections;
using System.Collections.Generic;
using System.IO;
using Febucci.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] TMP_Text creditsText;
    
    [Header("Player")]
    [SerializeField] TMP_Text rationsText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text oresText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject playerDialogue;
    
    [Header("NPC")]
    [SerializeField] TMP_Text NPCNameText;
    [SerializeField] Image NPCPortrait;
    [SerializeField] TMP_Text NPCDialogueText;
    
    struct ConvoNode
    {
        public string State, NPCDialogue;
        public List<string> Responses;
        public int[,] InventoryChanges, OpinionChanges;
    }
    struct Convo
    {
        public string Name;
        public int[] QuestlineRequirements;
        public List<ConvoNode> Nodes;
    }
    TextAsset[] conversationFiles;
    List<Convo> conversations;
    int conversationNum, conversationNodeNum;
    TMP_Text[] choices;
    List<string> completedConversations;
    int[] opinions; // refugees, Empire, revolution

    int[] questLines; // refugees, Empire, revolution

    int credits, rations, ammo;
    float ores;
    
    [HideInInspector] public GameObject homePlanet;
    List<GameObject> planets;
    GameObject homeMarker;
    
    [HideInInspector] public string playerName;
    GameObject playerUI, NPCUI, starshipObject;
    
    TextAnimator dayCounter;
    int dayCount, dayPopupCount;
    int phaseCount;
    
    bool isOpen, hasStartedGame;

    string[] NPCNamePrefixes = {
        "Gor",
        "Blez",
        "Zyhaph",
        "Bob",
        "Eg",
        "Will",
        "Suz",
        "Lill",
        "Jos",
        "Dave",
        "Orm",
        "Zeggl",
        "Son",
        "Yog"
    };
    string[] NPCNameSuffixes = {
        "go",
        "omat",
        "omel",
        "bert",
        "iam",
        "anna",
        "ith",
        "iah",
        "onath",
        "etor",
        "ia",
        "othor"
    };
    Sprite[] portraits;
    
    private void Start()
    {
        playerUI = GameObject.FindGameObjectWithTag("Player");
        playerUI.SetActive(false);
        NPCUI = GameObject.FindGameObjectWithTag("NPC");
        NPCUI.SetActive(false);

        credits = 30;
        rations = 30;
        ammo = 30;
        ores = 30;

        opinions = new int[3];
        
        questLines = new int[3];

        dayPopupCount = 1;

        starshipObject = Resources.Load<GameObject>("Starship");
        portraits = Resources.LoadAll<Sprite>("Portraits");
        
        choices = new TMP_Text[3];

        for (int j = 0; j < playerDialogue.transform.childCount; j++)
        {
            choices[j] = playerDialogue.transform.GetChild(j).GetComponentInChildren<TMP_Text>();
        }

        LoadConversations();
    }

    bool LoadConversations()
    {
        phaseCount++;
        
        conversations = new List<Convo>();
        completedConversations = new List<string>();

        string folderName = "Phase " + phaseCount;

        if (!Directory.Exists(Application.dataPath + "/Resources/" + folderName))
        {
            return false;
        }
        
        conversationFiles = Resources.LoadAll<TextAsset>(folderName);

        for (int f = 0; f < conversationFiles.Length; f++)
        {
            string[] split = conversationFiles[f].text.Split('\n');
            
            Convo newConvo;
            newConvo.Name = conversationFiles[f].name;
            newConvo.Nodes = new List<ConvoNode>();
            newConvo.QuestlineRequirements = new int[3];
            
            conversations.Add(newConvo);

            for (int i = 0; i < split.Length; i++)
            {
                if (i == 0)
                {
                    string[] tempSplit = split[i].Split(' ');
                    
                    newConvo.QuestlineRequirements[0] = int.Parse(tempSplit[0].Trim());
                    newConvo.QuestlineRequirements[1] = int.Parse(tempSplit[1].Trim());
                    newConvo.QuestlineRequirements[2] = int.Parse(tempSplit[2].Trim());
                }
                else
                {
                    if (split[i].Trim().Equals("---"))
                    {
                        ConvoNode newNode;
                        newNode.State = split[i + 1];
                        newNode.NPCDialogue = split[i + 2];
                        newNode.Responses = new List<string>();
                        newNode.InventoryChanges = new int[3, 4];
                        newNode.OpinionChanges = new int[3, 3];

                        try
                        {
                            int j = i + 3;
                            while (!split[j].Trim().Equals("==="))
                            {
                                newNode.Responses.Add(split[j]);
                                j++;
                            }

                            j++;

                            int tempCount = 0;
                            
                            for (int k = 0; k < 3; k++)
                            {
                                if ((j + k) < split.Length && !split[j + k].Trim().Equals("~~~"))
                                {
                                    string[] tempSplit = split[j + k].Split(' ');

                                    for (int l = 0; l < 4; l++)
                                    {
                                        newNode.InventoryChanges[k, l] = int.Parse(tempSplit[l]);
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                tempCount++;
                            }

                            j += tempCount + 1;
                            
                            for (int m = 0; m < 3; m++)
                            {
                                if ((j + m) < split.Length && !split[j + m].Trim().Equals("---"))
                                {
                                    string[] tempSplit = split[j + m].Split(' ');

                                    for (int n = 0; n < 3; n++)
                                    {
                                        newNode.OpinionChanges[m, n] = int.Parse(tempSplit[n]);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            newConvo.Nodes.Add(newNode);
                        }
                        catch
                        {
                            print("ERROR: " + newConvo.Name + " on line " + (i+1));
                        }
                    }
                }
            }
        }

        return true;
    }

    private void FixedUpdate()
    {
        if (hasStartedGame)
        {
            if (isOpen && Random.Range(0, 200) <= 1)
            {
                CreateStarship();
            }
        }

        if (homeMarker != null)
        {
            homeMarker.transform.rotation = Quaternion.identity;
        }

        if (dayCounter == null)
        {
            try
            {
                dayCounter = GameObject.FindGameObjectWithTag("Days").GetComponent<TextAnimator>();
            }
            catch { }
        }
    }

    public void StartTutorial()
    {
        LoadHome();
        //StartCoversation();
    }

    public void StartGame()
    {
        UpdateInventory();
        
        dayCounter.SetText("<fade d=3><wave>Day " + dayPopupCount + "</wave></fade>", false);
        

        isOpen = true;
        hasStartedGame = true;
    }

    public void LoadHome()
    {
        planets = GetComponent<SpaceSpawner>().planets;

        float minDistance = Mathf.Infinity;
        GameObject minPlanet = null;

        foreach (GameObject i in planets)
        {
            float tempDistance = Vector3.Distance(i.transform.position, Vector3.zero);

            if (tempDistance < minDistance)
            {
                minPlanet = i;
                minDistance = tempDistance;
            }
        }

        planets.Remove(minPlanet);
        homePlanet = minPlanet;

        homePlanet.GetComponent<Orbit>().speed = 0;

        homeMarker = Instantiate(Resources.Load<GameObject>("Home"), homePlanet.transform);

        if (isOpen)
        {
            CreateStarship();
        }
    }

    private void CreateStarship()
    {
        isOpen = false;

        GameObject newShip = Instantiate(starshipObject);
        newShip.transform.position = planets[Random.Range(0, planets.Count)].transform.position + (Vector3.up * 0.2f);
    }

    public void StartCoversation()
    {
        isOpen = false;
        conversationNodeNum = 0;
        
        conversationNum = Random.Range(0, conversations.Count);
        int count = 0;
        
        while (!(
            questLines[0] >= conversations[conversationNum].QuestlineRequirements[0] &&
            questLines[1] >= conversations[conversationNum].QuestlineRequirements[1] &&
            questLines[2] >= conversations[conversationNum].QuestlineRequirements[2]))
        {
            conversationNum = Random.Range(0, conversations.Count);

            count++;

            if (count >= conversations.Count)
            {
                bool isValid = LoadConversations();
                
                print(conversations.Count);
                
                if (isValid && conversations.Count > 0)
                {
                    conversationNum = Random.Range(0, conversations.Count);
                    count = 0;
                }
                else
                {
                    hasStartedGame = false;
                    GetComponent<SceneController>().Load("End");
                    return;
                }
            }
        }

        playerUI.SetActive(true);
        playerNameText.text = playerName;
        NPCUI.SetActive(true);
        //NPCNameText.text = NPCNamePrefixes[Random.Range(0, NPCNamePrefixes.Length)] + NPCNameSuffixes[Random.Range(0, NPCNameSuffixes.Length)];
        NPCNameText.text = conversations[conversationNum].Name;
        NPCPortrait.sprite = portraits[Random.Range(0, portraits.Length)];

        for (int i = 0; i < 3; i++)
        {
            choices[i].transform.parent.gameObject.SetActive(true);
        }

        LoadChoices();
    }

    public void MakeChoice(int choiceNum)
    {
        Convo tempConvo = conversations[conversationNum];
        ConvoNode tempConvoNode = tempConvo.Nodes[conversationNodeNum];
        
        credits += tempConvoNode.InventoryChanges[choiceNum, 0];
        rations += tempConvoNode.InventoryChanges[choiceNum, 1];
        ammo += tempConvoNode.InventoryChanges[choiceNum, 2];
        ores += tempConvoNode.InventoryChanges[choiceNum, 3];
        UpdateInventory();

        opinions[0] += tempConvoNode.OpinionChanges[choiceNum, 0];
        opinions[1] += tempConvoNode.OpinionChanges[choiceNum, 1];
        opinions[2] += tempConvoNode.OpinionChanges[choiceNum, 2];
        
        CheckQuestlines(tempConvo, tempConvoNode, choiceNum);

        bool hasFound = false;

        for (int i = 0; i < tempConvo.Nodes.Count; i++)
        {
            if (tempConvo.Nodes[i].State.Trim().Equals(tempConvoNode.State.Trim() + "_" + choiceNum))
            {
                conversationNodeNum = i;
                hasFound = true;
                break;
            }
        }

        if (hasFound)
        {
            LoadChoices();
        }
        else
        {
            EndConversation();
        }
    }

    void CheckQuestlines(Convo c, ConvoNode cn, int choice)
    {
        if (c.Name.Equals("Raggedy protesters") && cn.State.Equals("0_0_0") && choice == 0)
        {
            questLines[2]++;
        }
    }

    private void EndConversation()
    {
        Convo tempConvo = conversations[conversationNum];
        ConvoNode tempConvoNode = tempConvo.Nodes[conversationNodeNum];

        if (tempConvoNode.Responses.Count == 0 && tempConvoNode.InventoryChanges.Length > 0)
        {
            credits += tempConvoNode.InventoryChanges[0, 0];
            rations += tempConvoNode.InventoryChanges[0, 1];
            ammo += tempConvoNode.InventoryChanges[0, 2];
            ores += tempConvoNode.InventoryChanges[0, 3];
            UpdateInventory();
        }

        isOpen = true;

        playerUI.SetActive(false);
        NPCUI.SetActive(false);

        NPCDialogueText.text = "NPC Dialogue";

        completedConversations.Add(tempConvo.Name);
        conversations.Remove(tempConvo);
        
        bool isValid = LoadConversations();
        
        if (isValid && conversations.Count > 0)
        {
            dayCount++;
            
            if (dayCount % 3 == 0)
            {
                dayPopupCount++;
                dayCounter.SetText("<fade d=3><wave>Day " + dayPopupCount + "</wave></fade>", false);
            }
        }
        else
        {
            hasStartedGame = false;
            GetComponent<SceneController>().Load("End");
        }
    }

    private void LoadChoices()
    {
        NPCDialogueText.text = conversations[conversationNum].Nodes[conversationNodeNum].NPCDialogue;
        List<string> tempChoices = conversations[conversationNum].Nodes[conversationNodeNum].Responses;

        for (int i = 0; i < 3; i++)
        {
            if (i <= tempChoices.Count - 1)
            {
                choices[i].transform.parent.gameObject.SetActive(true);
                string choiceText = tempChoices[i];
                choices[i].text = choiceText;
            }
            else
            {
                choices[i].transform.parent.gameObject.SetActive(false);
            }
        }

        if (tempChoices.Count == 0)
        {
            Invoke("EndConversation", NPCDialogueText.text.Length * 0.1f);
        }
    }

    private void UpdateInventory()
    {
        creditsText.text = ColourString("Credits ", StringColours.Yellow) + "$" + credits;
        rationsText.text = ColourString("Rations ", StringColours.Pink) + rations + " cases";
        ammoText.text = ColourString("Ammo ", StringColours.Orange) + ammo + " rounds";
        oresText.text = ColourString("Ores ", StringColours.Green) + ores + "kg";
    }

    private enum StringColours { Yellow, Pink, Orange, Green };
    private string ColourString(string message, StringColours colour)
    {
        string code = "";

        switch (colour)
        {
            case StringColours.Yellow:
                code = "FFEA80";
                break;
            case StringColours.Pink:
                code = "EA80FF";
                break;
            case StringColours.Orange:
                code = "FFAA80";
                break;
            case StringColours.Green:
                code = "80FFD5";
                break;
        }

        return "<b><color=#" + code + ">" + message + "</color></b>";
    }
}
