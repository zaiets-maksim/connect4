using Connect4.Scripts.StaticData;
using UnityEngine;

namespace Connect4.Scripts.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string GameConfigPath = "StaticData/GameConfig";

        private GameStaticData _gameStaticData;

        public void LoadData()
        {
            _gameStaticData = Resources
                .Load<GameStaticData>(GameConfigPath);
        }
        
        public GameStaticData GameConfig() =>
            _gameStaticData;
    }
}