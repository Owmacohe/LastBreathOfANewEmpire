using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] TMP_Text creditsText;
    [SerializeField] TMP_Text rationsText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text oresText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject playerDialogue;
    [SerializeField] TMP_Text NPCNameText;
    [SerializeField] Image NPCPortrait;
    [SerializeField] TMP_Text NPCDialogueText;

    TextAsset[] conversationFiles;
    struct convoNode
    {
        public string name;
        public string state;
        public string NPCDialogue;
        public List<string> responses;
        public int[,] inventoryChanges;
    }
    List<List<convoNode>> conversations;
    int conversationNum, conversationNodeNum;
    TMP_Text[] choices;

    int credits, rations, ammo;
    float ores;
    [HideInInspector] public GameObject homePlanet;
    GameObject playerUI;
    GameObject NPCUI;
    GameObject homeMarker;
    GameObject starshipObject;
    List<GameObject> planets;
    bool isOpen, hasStartedGame;

    [HideInInspector] public string playerName;

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

        starshipObject = Resources.Load<GameObject>("Starship");
        portraits = Resources.LoadAll<Sprite>("Portraits");

        conversationFiles = Resources.LoadAll<TextAsset>("Conversations");
        conversations = new List<List<convoNode>>();

        for (int f = 0; f < conversationFiles.Length; f++)
        {
            conversations.Add(new List<convoNode>());
            string[] split = conversationFiles[f].text.Split('\n');

            int count = 0;

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Trim().Equals("---"))
                {
                    convoNode newNode;
                    newNode.name = conversationFiles[f].name;
                    newNode.state = split[i + 1];
                    newNode.NPCDialogue = split[i + 2];
                    newNode.responses = new List<string>();

                    try
                    {
                        int j = i + 3;
                        while (!split[j].Trim().Equals("==="))
                        {
                            newNode.responses.Add(split[j]);
                            j++;
                        }

                        j++;

                        newNode.inventoryChanges = new int[3, 4];

                        for (int k = 0; k < 3; k++)
                        {
                            if ((j + k) < split.Length && !split[j + k].Trim().Equals("---"))
                            {
                                string[] tempSplit = split[j + k].Split(' ');

                                for (int l = 0; l < 4; l++)
                                {
                                    newNode.inventoryChanges[k, l] = int.Parse(tempSplit[l]);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        conversations[f].Add(newNode);
                    }
                    catch
                    {
                        print("ERROR: " + newNode.name + " on line " + count);
                    }
                }

                count++;
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasStartedGame)
        {
            if (isOpen && Random.Range(0, 200) <= 1)
            {
                createStarship();
            }
        }

        if (homeMarker != null)
        {
            homeMarker.transform.rotation = Quaternion.Euler(Vector3.right * 90);
        }
    }

    public void startTutorial()
    {
        loadHome();
    }

    public void startGame()
    {
        updateInventory();

        choices = new TMP_Text[3];

        for (int j = 0; j < playerDialogue.transform.childCount; j++)
        {
            choices[j] = playerDialogue.transform.GetChild(j).GetComponentInChildren<TMP_Text>();
        }

        isOpen = true;
        hasStartedGame = true;
    }

    public void loadHome()
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
            createStarship();
        }
    }

    private void createStarship()
    {
        isOpen = false;

        GameObject newShip = Instantiate(starshipObject);
        newShip.transform.position = planets[Random.Range(0, planets.Count)].transform.position + (Vector3.up * 0.2f);
    }

    public void startCoversation()
    {
        isOpen = false;
        conversationNum = Random.Range(0, conversations.Count);
        conversationNodeNum = 0;

        playerUI.SetActive(true);
        playerNameText.text = playerName;
        NPCUI.SetActive(true);
        //NPCNameText.text = NPCNamePrefixes[Random.Range(0, NPCNamePrefixes.Length)] + NPCNameSuffixes[Random.Range(0, NPCNameSuffixes.Length)];
        NPCNameText.text = conversations[conversationNum][conversationNodeNum].name;
        NPCPortrait.sprite = portraits[Random.Range(0, portraits.Length)];

        for (int i = 0; i < 3; i++)
        {
            choices[i].transform.parent.gameObject.SetActive(true);
        }

        loadChoices();
    }

    public void makeChoice(int choiceNum)
    {
        credits += conversations[conversationNum][conversationNodeNum].inventoryChanges[choiceNum, 0];
        rations += conversations[conversationNum][conversationNodeNum].inventoryChanges[choiceNum, 1];
        ammo += conversations[conversationNum][conversationNodeNum].inventoryChanges[choiceNum, 2];
        ores += conversations[conversationNum][conversationNodeNum].inventoryChanges[choiceNum, 3];
        updateInventory();

        bool hasFound = false;

        for (int i = 0; i < conversations[conversationNum].Count; i++)
        {
            if (conversations[conversationNum][i].state.Trim().Equals(conversations[conversationNum][conversationNodeNum].state.Trim() + "_" + choiceNum))
            {
                conversationNodeNum = i;
                hasFound = true;
                break;
            }
        }

        if (hasFound)
        {
            loadChoices();
        }
        else
        {
            endConversation();
        }
    }

    private void endConversation()
    {
        convoNode convoTemp = conversations[conversationNum][conversationNodeNum];

        if (convoTemp.responses.Count == 0 && convoTemp.inventoryChanges.Length > 0)
        {
            credits += conversations[conversationNum][conversationNodeNum].inventoryChanges[0, 0];
            rations += conversations[conversationNum][conversationNodeNum].inventoryChanges[0, 1];
            ammo += conversations[conversationNum][conversationNodeNum].inventoryChanges[0, 2];
            ores += conversations[conversationNum][conversationNodeNum].inventoryChanges[0, 3];
            updateInventory();
        }

        isOpen = true;

        playerUI.SetActive(false);
        NPCUI.SetActive(false);

        NPCDialogueText.text = "NPC Dialogue";

        conversations.Remove(conversations[conversationNum]);

        if (conversations.Count == 0)
        {
            hasStartedGame = false;
            GetComponent<SceneController>().load("End");
        }
    }

    private void loadChoices()
    {
        NPCDialogueText.text = conversations[conversationNum][conversationNodeNum].NPCDialogue;
        List<string> tempChoices = conversations[conversationNum][conversationNodeNum].responses;

        for (int i = 0; i < 3; i++)
        {
            if (i <= tempChoices.Count - 1)
            {
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
            Invoke("endConversation", (NPCDialogueText.text.Length * 0.1f));
        }
    }

    private void updateInventory()
    {
        creditsText.text = colourString("Credits ", StringColours.Yellow) + "$" + credits;
        rationsText.text = colourString("Rations ", StringColours.Pink) + rations + " cases";
        ammoText.text = colourString("Ammo ", StringColours.Orange) + ammo + " rounds";
        oresText.text = colourString("Ores ", StringColours.Green) + ores + "kg";
    }

    private enum StringColours { Yellow, Pink, Orange, Green };
    private string colourString(string message, StringColours colour)
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
