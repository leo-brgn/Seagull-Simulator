using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest
{
    public enum QuestState
    {
        Inactive,
        Active,
        Completed
    }

    public QuestState state;
    public int level;
    public string title;
    public TMP_Text text;

    public void CompleteQuest()
    {
        state = QuestState.Completed;
        text.text = "• " + "<s>" + title + "</s>";
    }
}
