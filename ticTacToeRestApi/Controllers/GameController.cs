using Microsoft.AspNetCore.Mvc;
using ticTacToeRestApi.Interfaces;
using ticTacToeRestApi.Models;

namespace ticTacToeRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame(Guid gameId)
        {
            var game = await _gameService.GetGameAsync(gameId);
            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameDto gameDto)
        {
            var game = await _gameService.CreateGameAsync(gameDto.PlayerXId, gameDto.PlayerOId, gameDto.boardSize, gameDto.winLength);
            return Ok(game);
        }

        [HttpPost("{gameId}/move")]
        public async Task<IActionResult> MakeMove(Guid gameId, [FromBody] MoveDto moveDto)
        {
            try
            {
                var move = await _gameService.MakeMoveAsync(gameId, moveDto.PlayerId, moveDto.Row, moveDto.Column);
                return Ok(move);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
