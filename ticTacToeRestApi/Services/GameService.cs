using ticTacToeRestApi.Data.Entities;
using ticTacToeRestApi.Interfaces;

namespace ticTacToeRestApi.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGenericRepository<Move> _moveRepository;
        private readonly IGenericRepository<Player> _playrRepository;
        private readonly IConfiguration _config;
        public GameService(
            IGameRepository gameRepository,
            IGenericRepository<Move> moveRepository,
            IGenericRepository<Player> playrRepository,
            IConfiguration config
            )
        {
            _gameRepository = gameRepository;
            _moveRepository = moveRepository;
            _playrRepository = playrRepository;
            _config = config;
        }

        public async Task<Game> CreateGameAsync(Guid playerXId, Guid playerOId, int boardSize = 3, int winLength = 3)
        {
            if (boardSize < 3)
                throw new ArgumentException("The field size must be at least 3", nameof(boardSize));

            if (winLength < 3 || winLength > boardSize)
                throw new ArgumentException("The number of symbols to win must be at least 3 and no more than the field size.", nameof(winLength));

            var game = new Game
            {
                Id = Guid.NewGuid(),
                PlayerXId = playerXId,
                PlayerOId = playerOId,
                BoardSize = boardSize,
                CurrentTurn = "X",
                WinLength = winLength

            };

            await _gameRepository.AddAsync(game);
            return game;

        }

        public Task<Game?> GetGameAsync(Guid gameId)
        {
            return _gameRepository.GetGameWithMovesAsync(gameId);
        }

        public async Task<Move> MakeMoveAsync(Guid gameId, Guid playerId, int row, int column)
        {
            var game = await _gameRepository.GetGameWithMovesAsync(gameId)
                ?? throw new Exception("Game not found");

            if (game.WinnerId != null)
                throw new Exception("Game already finished");

            var symbol = game.CurrentTurn;

            var currentPlayer = symbol == "X" ? game.PlayerXId : game.PlayerOId;
            if (playerId != currentPlayer)
                throw new Exception("Not your turn");

            if (game.Moves.Any(m => m.Row == row && m.Column == column))
                throw new Exception("Cell already taken");

            var isThirdTurn = (game.Moves.Count + 1) % 3 == 0;
            var rnd = new Random();
            if (isThirdTurn && rnd.NextDouble() < 0.1)
            {
                symbol = symbol == "X" ? "O" : "X";
            }

            var move = new Move
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                PlayerId = playerId,
                Row = row,
                Column = column,
                Symbol = symbol
            };

            game.Moves.Add(move);

            if (CheckVictory(game, symbol, row, column))
            {
                game.WinnerId = playerId;
                game.EndAt = DateTime.UtcNow;
            }
            else if (game.Moves.Count == game.BoardSize * game.BoardSize)
            {
                game.EndAt = DateTime.UtcNow;
            }
            else
            {
                game.CurrentTurn = game.CurrentTurn == "X" ? "O" : "X";
            }

            await _moveRepository.AddAsync(move);
            await _gameRepository.UpdateAsync(game);

            return move;
        }

        private bool CheckVictory(Game game, string symbol, int lastRow, int lastCol)
        {
            var size = game.BoardSize;
            var winLength = game.WinLength;
            var board = new string[size, size];

            foreach (var move in game.Moves)
            {
                board[move.Row, move.Column] = move.Symbol;
            }

            return CheckDirection(board, symbol, lastRow, lastCol, 1, 0, winLength) ||  
                   CheckDirection(board, symbol, lastRow, lastCol, 0, 1, winLength) ||  
                   CheckDirection(board, symbol, lastRow, lastCol, 1, 1, winLength) ||  
                   CheckDirection(board, symbol, lastRow, lastCol, 1, -1, winLength);  
        }

        private bool CheckDirection(string[,] board, string symbol, int row, int col, int dRow, int dCol, int winLength)
        {
            int size = board.GetLength(0);
            int count = 1;

            for (int i = 1; i < winLength; i++)
            {
                int newRow = row + i * dRow;
                int newCol = col + i * dCol;
                if (newRow < 0 || newRow >= size || newCol < 0 || newCol >= size)
                    break;
                if (board[newRow, newCol] != symbol)
                    break;
                count++;
            }

            for (int i = 1; i < winLength; i++)
            {
                int newRow = row - i * dRow;
                int newCol = col - i * dCol;
                if (newRow < 0 || newRow >= size || newCol < 0 || newCol >= size)
                    break;
                if (board[newRow, newCol] != symbol)
                    break;
                count++;
            }

            return count >= winLength;
        }
    }
}
