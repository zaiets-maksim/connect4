using System;

namespace Connect4.Scripts.Infrastructure
{
    public interface ILoadingCurtain
    {
        event Action OnComplete;
        void Show();
        void Hide();
    }
}