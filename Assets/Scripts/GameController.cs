using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text creditsText;
    public TMP_Text rationsText;
    public TMP_Text ammoText;
    public TMP_Text oresText;
    public TMP_Text playerNameText;
    public GameObject playerDialogue;
    public TMP_Text NPCNameText;
    public Image NPCPortrait;
    public TMP_Text NPCDialogueText;

    private TextAsset[] conversationFiles;
    private struct convoNode
    {
        public string state;
        public string NPCDialogue;
        public List<string> responses;
        public int[,] inventoryChanges;
    }
    private List<List<convoNode>> conversations;
    private int conversationNum, conversationNodeNum;
    private TMP_Text[] choices;

    private int credits, rations, ammo;
    private float ores;
    [HideInInspector]
    public GameObject homePlanet;
    private GameObject starshipObject;
    private GameObject playerUI, NPCUI;
    private List<GameObject> planets;
    private bool isOpen;

    private string playerName;
    private string[] NPCNamePrefixes = {
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
    private string[] NPCNameSuffixes = {
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
    private Sprite[] portraits;

    private void Start()
    {
        credits = 50;
        rations = 50;
        ammo = 50;
        ores = 50;
        updateInventory();

        starshipObject = Resources.Load<GameObject>("Starship");
        portraits = Resources.LoadAll<Sprite>("Portraits");

        playerUI = GameObject.FindGameObjectWithTag("Player");
        playerUI.SetActive(false);
        NPCUI = GameObject.FindGameObjectWithTag("NPC");
        NPCUI.SetActive(false);

        conversationFiles = Resources.LoadAll<TextAsset>("Conversations");
        conversations = new List<List<convoNode>>();

        for (int f = 0; f < conversationFiles.Length; f++)
        {
            conversations.Add(new List<convoNode>());
            string[] split = conversationFiles[f].text.Split('\n');

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Trim().Equals("---"))
                {
                    convoNode newNode;
                    newNode.state = split[i + 1];
                    newNode.NPCDialogue = split[i + 2];
                    newNode.responses = new List<string>();

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
            }
        }

        choices = new TMP_Text[3];

        for (int j = 0; j < playerDialogue.transform.childCount; j++)
        {
            choices[j] = playerDialogue.transform.GetChild(j).GetComponentInChildren<TMP_Text>();
        }

        isOpen = true;

        playerName = "Player Name";
    }

    private void FixedUpdate()
    {
        if (isOpen && Random.Range(0, 200) <= 1)
        {
            createStarship();
        }
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

        GameObject homeMarker = Instantiate(Resources.Load<GameObject>("Home"), homePlanet.transform);

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
        newShip.transform.LookAt(homePlanet.transform);
    }

    public void startCoversation()
    {
        isOpen = false;
        conversationNum = Random.Range(0, conversations.Count);
        conversationNodeNum = 0;

        playerUI.SetActive(true);
        playerNameText.text = playerName;
        NPCUI.SetActive(true);
        NPCNameText.text = NPCNamePrefixes[Random.Range(0, NPCNamePrefixes.Length)] + NPCNameSuffixes[Random.Range(0, NPCNameSuffixes.Length)];
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
