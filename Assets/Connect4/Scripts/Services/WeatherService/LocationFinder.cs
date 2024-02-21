using System;
using System.Collections;
using Connect4.Scripts.Extensions;
using UnityEngine;

namespace Connect4.Scripts.Services.WeatherService
{
    public class LocationFinder : MonoBehaviour
    {
        [SerializeField] private WeatherDefiner weatherDefiner;

        private readonly Coordinates _coordinates = new Coordinates();
        private string _ipAddress;

        private void Start()
        {
            if (!IsInternetAvailable())
            {
                gameObject.SetActive(false);
                return;
            }

            StartCoroutine(TryGetLocation());
        }

        private IEnumerator TryGetLocation()
        {
            var ipApi = "https://api.myip.com";

            yield return this.GetData<MyIP>(ipApi, result => { _ipAddress = result.ip; });

            var locationApi = "https://ipapi.co/" + _ipAddress + "/json/";

            yield return this.GetData<Coordinates>(locationApi, result =>
            {
                weatherDefiner.Initialize(_coordinates.latitude, _coordinates.longitude);
            });
        }

        private bool IsInternetAvailable()
        {
            NetworkReachability reachability = Application.internetReachability;

            return reachability == NetworkReachability.ReachableViaLocalAreaNetwork ||
                   reachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        }
    }

    [Serializable]
    public class MyIP
    {
        public string ip;
    }

    [Serializable]
    public class Coordinates
    {
        public float latitude;
        public float longitude;
    }
}