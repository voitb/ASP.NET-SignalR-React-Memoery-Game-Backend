public class Game
{
    public List<Player> Players { get; set; } = new List<Player>();
    public int[][] GameBoard { get; set; }
    public bool IsStarted { get; set; }
}