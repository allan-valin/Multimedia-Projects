using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    // save current scene name
    public HashSet<string> sceneNames;

    // save point
    public string savepointSceneName;
    public Vector2 savepointPos;

    // player data
    public int playerHealth;
    public int playerHeartPieces;
    public float playerMana;
    public Vector2 playerPosition;
    public string lastScene;

    // unlocked abilities
    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump;
    public bool playerUnlockedSideCast, playerUnlockedUpCast, playerUnlockedDownCast;


    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }

        if (sceneNames == null) sceneNames = new HashSet<string>();
    }

    public void StoreSavepoint()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.data")))
        {
            writer.Write(savepointSceneName);
            //writer.Write((int)savepointPos.x);
            writer.Write(savepointPos.x);
            writer.Write(savepointPos.y);

        }
    }

    public void LoadSavepoint()
    {
        //if (File.Exists(Application.persistentDataPath + "/save.data"))
        string savePath = Application.persistentDataPath + "/save.data";
        if (File.Exists(savePath) && new FileInfo(savePath).Length > 0)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.data")))
            {
                savepointSceneName = reader.ReadString();
                savepointPos.x = reader.ReadSingle();
                savepointPos.y = reader.ReadSingle();
            }
        }

    }

    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerHeartPieces = PlayerController.Instance.heartPieces;
            writer.Write(playerHeartPieces);
            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);

            playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);
            playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);
            playerUnlockedVarJump = PlayerController.Instance.unlockedVarJump;
            writer.Write(playerUnlockedVarJump);

            playerUnlockedSideCast = PlayerController.Instance.unlockedSideCast;
            writer.Write(playerUnlockedSideCast);
            playerUnlockedUpCast = PlayerController.Instance.unlockedUpCast;
            writer.Write(playerUnlockedUpCast);
            playerUnlockedDownCast = PlayerController.Instance.unlockedDownCast;
            writer.Write(playerUnlockedDownCast);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }

    public void LoadPlayerData()
    {
        //if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        string savePath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(savePath) && new FileInfo(savePath).Length > 0)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerHeartPieces = reader.ReadInt32();
                playerMana = reader.ReadSingle();

                playerUnlockedWallJump = reader.ReadBoolean();
                playerUnlockedDash = reader.ReadBoolean();
                playerUnlockedVarJump = reader.ReadBoolean();

                playerUnlockedSideCast = reader.ReadBoolean();
                playerUnlockedUpCast = reader.ReadBoolean();
                playerUnlockedDownCast = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;
                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.heartPieces = playerHeartPieces;
                PlayerController.Instance.Mana = playerMana;
                
                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedVarJump = playerUnlockedVarJump;

                PlayerController.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerController.Instance.unlockedUpCast = playerUnlockedUpCast;
                PlayerController.Instance.unlockedDownCast = playerUnlockedDownCast;

            }
        }
        else
        {
            Debug.Log("File doesn't exist");
            PlayerController.Instance.Health = PlayerController.Instance.playerHealth.MaxHealth;
            PlayerController.Instance.heartPieces = 0;
            PlayerController.Instance.Mana = 0.5f;

            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
            PlayerController.Instance.unlockedVarJump = false;

            PlayerController.Instance.unlockedSideCast = false;
            PlayerController.Instance.unlockedUpCast = false;
            PlayerController.Instance.unlockedDownCast = false;
        }
    }
}
