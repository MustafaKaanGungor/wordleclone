using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject[] boardItems;

    private string connectionString;
    string query;
    private MySqlConnection MS_Connection;
    private MySqlCommand MS_Command;
    private MySqlDataReader MS_Reader;
    public void UpdateBoard() {
        query = "SELECT * FROM leaderboard";

        connectionString = "Server = localhost; Database = wordlepc; User = sysadmin; Password = wordle01; Charset = utf8;";

        MS_Connection = new MySqlConnection(connectionString);
        MS_Connection.Open();

        MS_Command = new MySqlCommand(query, MS_Connection);

        MS_Reader = MS_Command.ExecuteReader();

        for(int i = 0; MS_Reader.Read(); i++) {
            boardItems[i].GetComponentInChildren<TextMeshProUGUI>().text = " " + MS_Reader[0] + "\t " + MS_Reader[1];
        }

        MS_Connection.Close();
    }
}
