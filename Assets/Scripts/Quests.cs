using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestsManager : MonoBehaviour
{
    public Image _questPanel;
    public int currentLevel = 1;
    private List<Quest> allQuests = new List<Quest>();
    public GameObject questTextPrefab;


    public void Awake()
    {
        Level1Quests();
        _questPanel = GetComponent<Image>();
    }

    public void Start()
    {
        FillQuestPanel();
    }

    public void Level1Quests()
    {
        Quest quest1 = new Quest();
        quest1.title = "Steal doritos";
        quest1.level = 1;
        quest1.state = Quest.QuestState.Inactive;

        Quest quest2 = new Quest();
        quest2.title = "Fly";
        quest2.level = 1;
        quest2.state = Quest.QuestState.Inactive;

        Quest quest3 = new Quest();
        quest3.title = "Shout at people";
        quest3.level = 1;
        quest3.state = Quest.QuestState.Inactive;

        allQuests.Add(quest1);
        allQuests.Add(quest2);
        allQuests.Add(quest3);
    }

    public void FillQuestPanel()
    {
        foreach (Quest quest in allQuests)
        {
            if (quest.level == currentLevel)
            {
                GameObject questUI = Instantiate(questTextPrefab, _questPanel.transform);
                if (quest.state == Quest.QuestState.Completed)
                {
                    questUI.GetComponent<TMP_Text>().text = "• " + "<s>" + quest.title + "</s>";
                    quest.text = questUI.GetComponent<TMP_Text>();
                } else {
                    questUI.GetComponent<TMP_Text>().text = "• " + quest.title;
                    quest.text = questUI.GetComponent<TMP_Text>();
                }
            }
        }
    }

    private void UpdateQuestPanel()
    {
        if (allQuests.Count == 0)
        {
            return;
        }
        bool allQuestCompleted = true;
        // Test if ALL quests have been completed
        for (int i = 0; i < allQuests.Count; i++)
        {
            if (allQuests[i].state != Quest.QuestState.Completed)
            {
                allQuestCompleted = true;
                break;
            }
        }
        if (allQuestCompleted) {
            ClearQuestPanel();
            currentLevel++;
            if (allQuests.Count > 0)
            {
                FillQuestPanel();
            }
        }
    }

    private void ClearQuestPanel()
    {
        foreach (Transform child in _questPanel.transform.parent)
        {
            if (child != _questPanel.transform && child.name.Contains("quest"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void CompleteQuest(string questTitle) {
        foreach (Quest quest in allQuests) {
            if (quest.title == questTitle) {
                quest.CompleteQuest();
                UpdateQuestPanel();
            }
        }
    }
}