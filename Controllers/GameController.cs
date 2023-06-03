using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly IGameService _gameService;

    public GameController(IHubContext<GameHub> gameHubContext, IGameService gameService)
    {
        _gameHubContext = gameHubContext;
        _gameService = gameService;
        //_gameService.gameBoard = _gameService.GenerateGameBoard();
    }

    [HttpGet("start")]
    public async Task<IActionResult> StartGame()
    {
        _gameService.gameBoard = _gameService.GenerateGameBoard();
        Random random = new Random();
        int turn = random.Next(2);
        bool isPlayer1 = turn == 0;
        if (isPlayer1)
        {
            _gameService.Player1.IsTurn = true;
            _gameService.Player2.IsTurn = false;
        }
        else
        {
            _gameService.Player1.IsTurn = false;
            _gameService.Player2.IsTurn = true;
        }

        Player playerTurn = isPlayer1 ? _gameService.Player1 : _gameService.Player2;
        await _gameHubContext.Clients.All.SendAsync("SetTurn", playerTurn);
        await _gameHubContext.Clients.All.SendAsync("ReceiveGameBoard", _gameService.gameBoard);

        return Ok(_gameService.gameBoard);
    }
    [HttpGet("reinitialize")]
    public async Task<IActionResult> ReinitializeGame()
    {
        _gameService.Player1.Score = 0;
        _gameService.Player2.Score = 0;
        _gameService.Player1.Step = 0;
        _gameService.Player2.Step = 0;
        _gameService.gameBoard = _gameService.GenerateGameBoard();
        Random random = new Random();
        int turn = random.Next(2);
        bool isPlayer1 = turn == 0;
        if (isPlayer1)
        {
            _gameService.Player1.IsTurn = true;
            _gameService.Player2.IsTurn = false;
        }
        else
        {
            _gameService.Player1.IsTurn = false;
            _gameService.Player2.IsTurn = true;
        }

        Player playerTurn = isPlayer1 ? _gameService.Player1 : _gameService.Player2;
        await _gameHubContext.Clients.All.SendAsync("SetTurn", playerTurn);
        await _gameHubContext.Clients.All.SendAsync("ReceiveGameBoard", _gameService.gameBoard);
        await _gameHubContext.Clients.All.SendAsync("ResetWinner", true);

        return Ok(_gameService.gameBoard);
    }
    [HttpPost("add")]
    public async Task<IActionResult> AddPlayer()
    {
        try
        {
            var player = _gameService.AddPlayer();

            _gameHubContext.Clients.All.SendAsync("PlayerAdded", player);
            int length = _gameService.Player2 != null ? 2 : 1;
            _gameHubContext.Clients.All.SendAsync("PlayersLength", length);
            return Ok(player);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("update")]
    public IActionResult UpdateCoordinate(Coordinate coordinate)
    {
        try
        {
            _gameHubContext.Clients.All.SendAsync("CoordinateUpdated", coordinate);

            var playerPicked = _gameService.Player1.IsTurn ? _gameService.Player1 : _gameService.Player2;
            var nextPlayer = _gameService.Player1.IsTurn ? _gameService.Player2 : _gameService.Player1;
            ++playerPicked.Step;

            if (playerPicked.Step == 1)
            {
                _gameService.pickedCords = coordinate;
            }
            if (playerPicked.Step == 2)
            {
                Coordinate temp = coordinate;
                if (_gameService.gameBoard[_gameService.pickedCords.I][_gameService.pickedCords.J] == _gameService.gameBoard[temp.I][temp.J])
                {

                    playerPicked.Step = 0;
                    nextPlayer.Step = 0;
                    ++playerPicked.Score;
                    _gameHubContext.Clients.All.SendAsync("SetTurn", playerPicked);
                    if (playerPicked.Score + nextPlayer.Score == 18)
                    {
                        Player winner;
                        if (playerPicked.Score > nextPlayer.Score)
                        {
                            winner = playerPicked;
                        }
                        else if (playerPicked.Score < nextPlayer.Score)
                        {
                            winner = nextPlayer;
                        }
                        else
                        {
                            winner = null;
                        }
                        _gameHubContext.Clients.All.SendAsync("EndGame", winner);
                    }
                }
                else
                {
                    List<Coordinate> coordinates = new List<Coordinate>
                    {
                        _gameService.pickedCords,
                        coordinate
                    };
                    playerPicked.Step = 0;
                    nextPlayer.Step = 0;
                    playerPicked.IsTurn = false;
                    nextPlayer.IsTurn = true;

                    _gameHubContext.Clients.All.SendAsync("SetTurn", nextPlayer);
                    Thread.Sleep(1000);

                    _gameHubContext.Clients.All.SendAsync("CoordinatesToHide", coordinates);
                }

            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("reset-players")]
    public IActionResult ResetPlayers()
    {
        _gameService.ResetPlayers();
        return Ok();
    }


}
