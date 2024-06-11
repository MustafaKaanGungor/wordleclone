using TMPro;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
        KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
        KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
        KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
        KeyCode.Z
    };

    private int rowIndex;
    private int columnIndex;

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;

    private string word;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI InvalidText;
    [SerializeField] private TextMeshProUGUI CorrectAnswerText;
    [SerializeField] private GameObject RetryButton;
    [SerializeField] private TextMeshProUGUI TimerText;
    private float time;

    [SerializeField] private GameObject newScorePanel;
    [SerializeField] private TextMeshProUGUI inputField;


    [Header("SQL")]
    private string connectionString;
    private MySqlConnection MS_Connection;
    private MySqlCommand MS_Command;
    string query;


    void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    void Start()
    {    
        LoadData();
        SetRandomWord();
    }

    public void Newgame() {
        CorrectAnswerText.gameObject.SetActive(false);
        InvalidText.gameObject.SetActive(false);
        SetRandomWord();
        ClearBoard();
        enabled = true;
    }

    private void LoadData() {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n');
    }

    private void SetRandomWord() {
        word = solutions[Random.Range(0,solutions.Length)];
        word = word.ToLower().Trim();
    }

    void Update()
    {
        time += Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if(Input.GetKeyDown(KeyCode.Backspace) && columnIndex > 0) {
            columnIndex--;
            rows[rowIndex].tiles[columnIndex].SetLetter('\0');
            InvalidText.gameObject.SetActive(false);
        } else if(columnIndex >= rows[rowIndex].tiles.Length) {
            if(Input.GetKeyDown(KeyCode.Return)) {
                if(SubmitRow(rows[rowIndex])) {
                    if(rows[rowIndex].word == word) {
                        enabled = false;
                    } else {
                        if(rowIndex + 1 < rows.Length) {
                            rowIndex++;
                        } else {
                            CorrectAnswerText.text = "Answer was " + word;
                            CorrectAnswerText.gameObject.SetActive(true);
                            enabled = false;
                        }
                        columnIndex = 0;
                    }
                }
            }
        } else {
            for(int i = 0; i < SUPPORTED_KEYS.Length; i++) {
                if(Input.GetKeyDown(SUPPORTED_KEYS[i])) {
                    rows[rowIndex].tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    columnIndex++;
                    break;
                }
            }
        }
    }

    private bool SubmitRow(Row row) {

        if(!IsValidWord(row.word)) {
            InvalidText.gameObject.SetActive(true);
            return false;
        }

        string remain = word;
        Debug.Log(remain);

        for(int i = 0; i < row.tiles.Length; i++) {
            if(row.tiles[i].letter == word[i]) {
                row.tiles[i].SetColour(Colours.GREEN);

                remain = remain.Remove(i, 1);
                remain = remain.Insert(i, " "); 
                Debug.Log(remain);
            } else if(!word.Contains(row.tiles[i].letter)) {
                row.tiles[i].SetColour(Colours.BLACK);
            }
        }

        for(int i = 0; i < row.tiles.Length; i++) {
            if(row.tiles[i].currentColour != Colours.GREEN && row.tiles[i].currentColour != Colours.BLACK) {
                if(remain.Contains(row.tiles[i].letter)) {
                    row.tiles[i].SetColour(Colours.YELLOW);
                    remain = remain.Remove(i, 1);
                    remain = remain.Insert(i, " "); 
                    Debug.Log(remain);
                } else {
                    row.tiles[i].SetColour(Colours.BLACK);
                }
            }
        }

        return true;
    }

    private bool IsValidWord(string word) {
        for(int i = 0; i < validWords.Length; i++) {
            if(validWords[i] == word) {
                Debug.Log("heyo?");
                return true;
            }
        }
        return false;
    }

    private void ClearBoard() {
        for(int i = 0; i < rows.Length; i++) {
            for(int ii = 0; ii < rows[i].tiles.Length; ii++) {
                rows[i].tiles[ii].SetLetter('\0');
                rows[i].tiles[ii].SetColour(Colours.EMPTY);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    void OnEnable()
    {
        time = 0;
        RetryButton.SetActive(false);
        newScorePanel.SetActive(false);
    }

    void OnDisable()
    {
        RetryButton.SetActive(true);
        Debug.Log(PlayerPrefs.HasKey("HIGH_SCORE"));
        Debug.Log(PlayerPrefs.GetInt("HIGH_SCORE"));
        if(!PlayerPrefs.HasKey("HIGH_SCORE") || (int)time < PlayerPrefs.GetInt("HIGH_SCORE")) {
            PlayerPrefs.SetInt("HIGH_SCORE", (int)time);
            newScorePanel.SetActive(true);
        }
    }

    public void SendInfo() {
        Connection(); 

        string playerName = inputField.text;
        query = "insert into leaderboard(PlayerName, Score) values(' " + playerName  + " ' , '" + (int)time + " ' )";
        MS_Command = new MySqlCommand(query, MS_Connection);

        MS_Command.ExecuteNonQuery();

        MS_Connection.Close();
    }

    public void Connection() {
        connectionString = "Server = localhost; Database = wordlepc; User = sysadmin; Password = wordle01; Charset = utf8;";
        MS_Connection = new MySqlConnection(connectionString);

        MS_Connection.Open();
    }
}
