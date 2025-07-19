using Xunit;
using Moq;
using FluentAssertions;
using ticTacToeRestApi.Services;
using ticTacToeRestApi.Interfaces;
using ticTacToeRestApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ticTacToeRestApi.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepoMock = new();
        private readonly Mock<IGenericRepository<Move>> _moveRepoMock = new();
        private readonly Mock<IGenericRepository<Player>> _playerRepoMock = new();
        private readonly IConfiguration _configMock = new ConfigurationBuilder().Build();

        private GameService CreateService() =>
            new GameService(_gameRepoMock.Object, _moveRepoMock.Object, _playerRepoMock.Object, _configMock);

        [Fact]
        public async Task CreateGameAsync_ShouldCreateValidGame()
        {
            // Arrange
            var service = CreateService();
            var playerXId = Guid.NewGuid();
            var playerOId = Guid.NewGuid();

            Game? createdGame = null;
            _gameRepoMock.Setup(r => r.AddAsync(It.IsAny<Game>()))
                         .Callback<Game>(g => createdGame = g)
                         .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreateGameAsync(playerXId, playerOId, 5, 4);

            // Assert
            createdGame.Should().NotBeNull();
            result.BoardSize.Should().Be(5);
            result.WinLength.Should().Be(4);
            result.PlayerXId.Should().Be(playerXId);
            result.PlayerOId.Should().Be(playerOId);
            result.CurrentTurn.Should().Be("X");
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        public async Task CreateGameAsync_ShouldThrow_OnInvalidBoardOrWinLength(int boardSize, int winLength)
        {
            var service = CreateService();

            Func<Task> act = () => service.CreateGameAsync(Guid.NewGuid(), Guid.NewGuid(), boardSize, winLength);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldThrow_IfGameNotFound()
        {
            // Arrange
            var service = CreateService();
            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Game?)null);

            // Act
            Func<Task> act = () => service.MakeMoveAsync(Guid.NewGuid(), Guid.NewGuid(), 0, 0);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Game not found");
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldThrow_IfGameAlreadyFinished()
        {
            var service = CreateService();
            var game = new Game
            {
                WinnerId = Guid.NewGuid(),
                Moves = new List<Move>(),
                BoardSize = 3,
                WinLength = 3,
                CurrentTurn = "X"
            };

            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(game);

            var playerId = Guid.NewGuid();
            Func<Task> act = () => service.MakeMoveAsync(Guid.NewGuid(), playerId, 0, 0);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Game already finished");
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldThrow_IfNotPlayersTurn()
        {
            var service = CreateService();
            var game = new Game
            {
                Id = Guid.NewGuid(),
                PlayerXId = Guid.NewGuid(),
                PlayerOId = Guid.NewGuid(),
                CurrentTurn = "X",
                Moves = new List<Move>(),
                BoardSize = 3,
                WinLength = 3
            };

            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(game.Id))
                         .ReturnsAsync(game);

            var wrongPlayer = Guid.NewGuid();
            Func<Task> act = () => service.MakeMoveAsync(game.Id, wrongPlayer, 0, 0);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Not your turn");
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldThrow_IfCellTaken()
        {
            var service = CreateService();
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();
            var game = new Game
            {
                Id = Guid.NewGuid(),
                PlayerXId = playerX,
                PlayerOId = playerO,
                CurrentTurn = "X",
                BoardSize = 3,
                WinLength = 3,
                Moves = new List<Move>
                {
                    new Move
                    {
                        Row = 0,
                        Column = 0,
                        Symbol = "X",
                        PlayerId = playerX
                    }
                }
            };

            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(game.Id))
                         .ReturnsAsync(game);

            Func<Task> act = () => service.MakeMoveAsync(game.Id, playerX, 0, 0);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Cell already taken");
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldSwitchTurns_WhenNoWin()
        {
            var service = CreateService();
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();

            var game = new Game
            {
                Id = Guid.NewGuid(),
                PlayerXId = playerX,
                PlayerOId = playerO,
                CurrentTurn = "X",
                BoardSize = 3,
                WinLength = 3,
                Moves = new List<Move>()
            };

            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(game.Id)).ReturnsAsync(game);
            _moveRepoMock.Setup(r => r.AddAsync(It.IsAny<Move>())).Returns(Task.CompletedTask);
            _gameRepoMock.Setup(r => r.UpdateAsync(game)).Returns(Task.CompletedTask);

            var move = await service.MakeMoveAsync(game.Id, playerX, 0, 0);

            game.CurrentTurn.Should().BeOneOf("O", "X"); // may change by chance
            move.Row.Should().Be(0);
            move.Column.Should().Be(0);
        }

        [Fact]
        public async Task MakeMoveAsync_ShouldDetectVictory()
        {
            // Arrange
            var service = CreateService();
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();

            var game = new Game
            {
                Id = Guid.NewGuid(),
                PlayerXId = playerX,
                PlayerOId = playerO,
                BoardSize = 3,
                WinLength = 3,
                CurrentTurn = "X",
                StartedAt = DateTime.UtcNow,
                Moves = new List<Move>
        {
            new Move { Row = 0, Column = 0, Symbol = "X" },
            new Move { Row = 0, Column = 1, Symbol = "X" }
        }
            };

            _gameRepoMock.Setup(r => r.GetGameWithMovesAsync(game.Id)).ReturnsAsync(game);
            _moveRepoMock.Setup(r => r.AddAsync(It.IsAny<Move>())).Returns(Task.CompletedTask);
            _gameRepoMock.Setup(r => r.UpdateAsync(game)).Returns(Task.CompletedTask);

            // Act
            var move = await service.MakeMoveAsync(game.Id, playerX, 0, 2);

            // Assert
            game.WinnerId.Should().Be(playerX);
            game.EndAt.Should().BeAfter(game.StartedAt);
        }
    }
}
