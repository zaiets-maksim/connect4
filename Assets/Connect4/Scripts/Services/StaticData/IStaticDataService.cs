using Connect4.Scripts.StaticData;

namespace Connect4.Scripts.Services.StaticData
{
    public interface IStaticDataService
    {
        void LoadData();
        GameStaticData GameConfig();
    }
}