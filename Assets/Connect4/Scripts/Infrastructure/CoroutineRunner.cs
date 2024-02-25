using UnityEngine;

namespace Connect4.Scripts.Infrastructure
{
  public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
  {
    public void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}