using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private string saveFilePath;
    public Text avisoText;

    [System.Serializable]
    public class PlayerData
    {
        public float playerPosX;
        public float playerPosY;
        public float playerPosZ;
        public int playerScore;

        public PlayerData(float posX, float posY, float posZ, int score)
        {
            playerPosX = posX;
            playerPosY = posY;
            playerPosZ = posZ;
            playerScore = score;
        }
    }

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savegame.json";
    }

    // Called when we click the "save" button.
    public void SaveGame()
    {
        Vector3 playerPosition = transform.position;
        PlayerData playerData = new PlayerData(playerPosition.x, playerPosition.y, playerPosition.z, 100);
        
        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, jsonData);
        
        showStatus("Game successfully saved");
    }

    // Called when we click the "load" button.
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            transform.position = new Vector3(playerData.playerPosX, playerData.playerPosY, playerData.playerPosZ);

            showStatus("Game successfully loaded");
        }
        else
        {
            showStatus("No saved games");
        }
    }

    private void showStatus(string message)
    {
        avisoText.text = message;
        StartCoroutine(hideStatus());
    }

    private IEnumerator hideStatus()
    {
        yield return new WaitForSeconds(3f);
        avisoText.text = "";
    }
}