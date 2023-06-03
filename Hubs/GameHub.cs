using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class GameHub : Hub
{
    private static Game game = new Game();
    public async Task JoinGame()
    {
        if (game.Players.Count >= 2)
            return;

        var player = new Player { Score = 0 };

        switch (game.Players.Count)
        {
            case 0:
                player.Name = "Player 1";
                break;
            case 1:
                player.Name = "Player 2";
                break;
        }

        game.Players.Add(player);
        await Clients.All.SendAsync("UpdateGame", game);
    }


    public async Task StartGame()
    {
        if (game.Players.Count < 2)
            return;

        game.GameBoard = GenerateGameBoard();
        game.IsStarted = true;
        await Clients.All.SendAsync("StartGame", game);
    }

    public async Task NotifyPlayerCount(int playerCount)
    {
        await Clients.All.SendAsync("PlayerCountNotification", playerCount);
    }

    // public async Task CoordinateUpdated(Coordinate coordinate)
    // {
    //     await Clients.Others.SendAsync("CoordinateUpdated", coordinate);
    // }


    private int[][] GenerateGameBoard()
    {
        // Tu powinna być implementacja tworzenia planszy gry
        throw new NotImplementedException();
    }

    public async Task PlayerAction(int row, int col, string connectionId)
    {
        // Tu powinna być implementacja logiki gry, w tym aktualizacji wyniku gracza
        throw new NotImplementedException();
    }
}
