using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to control game UI
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Text scoreTxt;        //ref to score text
    [SerializeField] private Image powerBar;        //ref to powerBar image
    [SerializeField] private Text shotText;         //ref to shot info text
    [SerializeField] private GameObject mainMenu, gameMenu, gameOverPanel, retryBtn, nextBtn;   //important gameobjects
    [SerializeField]
    private GameObject AllLevelCompletionScoreMenu;
    [SerializeField]
    private Text AllLevelScoreTxt;
    [SerializeField]
    private Text GameEndTxt;
    [SerializeField]
    public Text currentLevelTxt;
    [SerializeField] private GameObject container, lvlBtnPrefab;    //important gameobjects

    [SerializeField] private Text[] bestScoreTitlesTxt;
    [SerializeField] private Text[] bestScoreTxt;

    public Text ShotText { get { return shotText; } }   //getter for shotText
    public Image PowerBar { get => powerBar; }          //getter for powerBar

    float playTime;
  [HideInInspector] public  bool isGameStart;
   // public int[] levelbaseScores;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       
        powerBar.fillAmount = 0;                        //set the fill amount to zero

        //if(!PlayerPrefs.HasKey("CurrentLevel"))
        //{
        //    PlayerPrefs.SetInt("CurrentLevel", 0);
        //}
      
    }

    void Start()
    {
        scoreTxt.text = "Score: " + PlayerPrefs.GetInt("TotalScore").ToString();
        if (GameManager.singleton.gameStatus == GameStatus.None)    //if gamestatus is none
        {   
          //  CreateLevelButtons();                       //create level buttons
        }     //we check for game status, failed or complete
        else if (GameManager.singleton.gameStatus == GameStatus.Failed ||
            GameManager.singleton.gameStatus == GameStatus.Complete)
        {
         
            mainMenu.SetActive(false);                                      //deavtivate main menu
            gameMenu.SetActive(true);                                       //activate game menu
            LevelManager.instance.SpawnLevel(GameManager.singleton.currentLevelIndex);  //spawn level
            

        }
    }

    private void Update()
    {
        if(isGameStart)
        {
            playTime += Time.deltaTime;
        }
        else
        {
            playTime = 0;
        }
    }
    /// <summary>
    /// Method which spawn levels button
    /// </summary>
    void CreateLevelButtons()
    {
        //total count is number of level datas
        for (int i = 0; i < LevelManager.instance.levelDatas.Length; i++)
        {
            GameObject buttonObj = Instantiate(lvlBtnPrefab, container.transform);   //spawn the button prefab
            buttonObj.transform.GetChild(0).GetComponent<Text>().text = "" + (i + 1);   //set the text child
            Button button = buttonObj.GetComponent<Button>();                           //get the button componenet
           // button.onClick.AddListener(() => OnClick(button));                          //add listner to button
        }
    }

    /// <summary>
    /// Method called when we click on button
    /// </summary>
   public void OnClick()
    {
        if(PlayerPrefs.HasKey("TotalScore"))
        {
            PlayerPrefs.DeleteKey("TotalScore");
        }
        scoreTxt.text = "Score: " + PlayerPrefs.GetInt("TotalScore").ToString();
        isGameStart = true;
        mainMenu.SetActive(false);                                                      //deactivate main menu
        gameMenu.SetActive(true);                                                       //activate game manu
        GameManager.singleton.currentLevelIndex = 0;    //set current level equal to sibling index on button
        LevelManager.instance.SpawnLevel(GameManager.singleton.currentLevelIndex);      //spawn level
        currentLevelTxt.text = "Level: " + (GameManager.singleton.currentLevelIndex + 1).ToString();
    }

    /// <summary>
    /// Method call after level fail or win
    /// </summary>
    public void GameResult()
    {
        switch (GameManager.singleton.gameStatus)
        {
            case GameStatus.Complete:                       //if completed
                GameEndTxt.text = "Level Complete";
                GiveScoreBasedOnTime(playTime);

                if (GameManager.singleton.currentLevelIndex == LevelManager.instance.levelDatas.Length)
                {
                    //5 Levels Completed
                    SetHighScore(PlayerPrefs.GetInt("TotalScore"));
                    gameOverPanel.SetActive(false);              //activate game finish panel
                    nextBtn.SetActive(false);                    //activate next button
                    ShowAllLevelScoreMenu();
                }
                else
                {
                    gameOverPanel.SetActive(true);              //activate game finish panel
                    nextBtn.SetActive(true);                    //activate next button
                }

                SoundManager.instance.PlayFx(FxTypes.GAMECOMPLETEFX);
               
                isGameStart = false;
                playTime = 0;

                //Update Next Level
                //int playedLevelIndx = PlayerPrefs.GetInt("CurrentLevel");
                //if (playedLevelIndx < 4)
                //{
                //    playedLevelIndx += 1;
                //    PlayerPrefs.SetInt("CurrentLevel", playedLevelIndx);

                //}
                //else
                //{
                //    PlayerPrefs.SetInt("CurrentLevel", 0);
                //    GameManager.singleton.currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel");
                //}
                break;
            case GameStatus.Failed:                         //if failed
                GameEndTxt.text = "Level Failed";
                isGameStart = false;
                gameOverPanel.SetActive(true);              //activate game finish panel
                retryBtn.SetActive(true);                   //activate retry button
                SoundManager.instance.PlayFx(FxTypes.GAMEOVERFX);
                break;
        }
    }


    public void GiveScoreBasedOnTime(float currentTime)
    {
        int totalScore = PlayerPrefs.GetInt("TotalScore");
        if (currentTime > 0 && currentTime <= 10)
        {

            totalScore += 10;
            Debug.Log("Give Timer Score: " + totalScore);
        }
        else if(currentTime > 10 && currentTime <= 20)
        {
            totalScore += 7;
            Debug.Log("Give Timer Score: " + totalScore);
        }
        else if(currentTime > 20 && currentTime <= 30)
        {
            totalScore += 5;
            Debug.Log("Give Timer Score: " + totalScore);
        }
        else
        {
            Debug.Log("No Score & time is: "+ currentTime);
        }
        PlayerPrefs.SetInt("TotalScore", totalScore);
        GivePointsNoOfRemainingStrikes(LevelManager.instance.shotCount);
        scoreTxt.text = "Score: " + PlayerPrefs.GetInt("TotalScore").ToString();
       
    }
    //method to go to main menu
    public void HomeBtn()
    {
        GameManager.singleton.gameStatus = GameStatus.None;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //method to reload scene
    public void NextRetryBtn()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        playTime = 0;
    }

    public void ShowAllLevelScoreMenu()
    {
        AllLevelCompletionScoreMenu.SetActive(true);
        AllLevelScoreTxt.text = PlayerPrefs.GetInt("TotalScore").ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void UpdateScoreboard()
    {
        for (int i = 0; i < LevelManager.instance.levelDatas.Length; i++)
        {
            string bestScoreKey = "BestScore" + i;
            if(PlayerPrefs.HasKey(bestScoreKey))
            {
                bestScoreTitlesTxt[i].text = (i + 1) + ". Best Score";
                bestScoreTxt[i].text = PlayerPrefs.GetInt(bestScoreKey).ToString();
            }
            else
            {
                bestScoreTitlesTxt[i].text = "None.";
                bestScoreTxt[i].text = "0";
            }
        }
    }


    private void SetHighScore(int score)
    {
        // Get the current high scores
        int[] highScores = GetHighScores();

        // Check if the current score is higher than any of the high scores
        for (int i = 0; i < highScores.Length; i++)
        {
            if (score > highScores[i])
            {
                // Insert the new score at the appropriate position
                for (int j = highScores.Length - 1; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                }
                highScores[i] = score;

                // Save the updated high scores to PlayerPrefs
                SaveHighScores(highScores);
                break;
            }
        }
    }

    private int[] GetHighScores()
    {
        // Load the high scores from PlayerPrefs
        int[] highScores = new int[5];

        for (int i = 0; i < highScores.Length; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("BestScore" + i , 0);
        }

        return highScores;
    }

    private void SaveHighScores(int[] highScores)
    {
        // Save the high scores to PlayerPrefs
        for (int i = 0; i < highScores.Length; i++)
        {
            PlayerPrefs.SetInt("BestScore" + i , highScores[i]);
            
        }
    }

    void GivePointsNoOfRemainingStrikes(int remainingPoints)
    {
       // Debug.Log("Remaining Shots:" + remainingPoints);
        int totalScore = PlayerPrefs.GetInt("TotalScore");
        totalScore += (remainingPoints - 1);
        PlayerPrefs.SetInt("TotalScore", totalScore);
    }
}
