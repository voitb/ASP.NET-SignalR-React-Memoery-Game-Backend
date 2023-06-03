public interface IGameService
{
    List<List<string>> GameBoard { get; }
    Player Player1 { get; }
    Player Player2 { get; }
    List<List<string>> GenerateGameBoard();
    Player AddPlayer();

    List<List<string>> gameBoard { get; set; }

    Coordinate pickedCords { get; set; }
    void ResetPlayers();
}

public class GameService : IGameService
{
    public List<List<string>> GameBoard { get; private set; }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    public List<List<string>> gameBoard { get; set; }
    public Coordinate pickedCords { get; set; }
    public List<List<string>> GenerateGameBoard()
    {
        List<string> colors = new List<string>
        {
            "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#00FFFF", "#FF00FF",
            "#C0C0C0", "#808080", "#800000", "#808000", "#008000", "#800080",
            "#008080", "#000080", "#FFA500", "#A52A2A", "#8B4513", "#D2691E"
        };

        List<string> doubleColors = new List<string>();
        doubleColors.AddRange(colors);
        doubleColors.AddRange(colors);

        var random = new Random();
        doubleColors = doubleColors.OrderBy(x => random.Next()).ToList();

        List<List<string>> gameBoard = new List<List<string>>();
        for (int i = 0; i < 6; i++)
        {
            List<string> row = new List<string>();
            for (int j = 0; j < 6; j++)
            {
                row.Add(doubleColors[i * 6 + j]);
            }
            gameBoard.Add(row);
        }

        return gameBoard;
    }
    public Player AddPlayer()
    {
        if (Player1 == null)
        {
            Player1 = new Player() { Name = "Player 1", Score = 0 };
            return Player1;
        }
        else if (Player2 == null)
        {
            Player2 = new Player() { Name = "Player 2", Score = 0 };
            return Player2;
        }
        else
        {
            throw new Exception("Game already has two players");
        }
    }

    public void ResetPlayers()
    {
        Player1 = null;
        Player2 = null;
    }
}
