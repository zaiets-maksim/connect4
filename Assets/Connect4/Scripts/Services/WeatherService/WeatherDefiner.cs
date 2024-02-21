using System;
using System.Collections;
using System.Collections.Generic;
using Connect4.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Connect4.Scripts.Services.WeatherService
{
    public class WeatherDefiner : MonoBehaviour
    {
        private const float KELVIN = -273.15f;
    
        [SerializeField] private Text _currentWeatherText;
        [SerializeField] private string _apiKey;
    
        private readonly Coordinates _coordinates = new Coordinates();
    
        public void Initialize(float latitude, float longitude)
        {
            _coordinates.latitude = latitude;
            _coordinates.longitude = longitude;

            StartCoroutine(GetWeatherInfo());
        }

        private IEnumerator GetWeatherInfo()
        {
            var weatherApi = $"https://api.openweathermap.org/data/2.5/weather?lat={_coordinates.latitude}&lon={_coordinates.longitude}&appid={_apiKey}";

            yield return this.GetData<Root>(weatherApi, Output);
        }

        private void Output(Root root) => 
            _currentWeatherText.text = $"{root.name} \n {root.weather[0].description} {ToCelsius(root.main.temp):0.0}°C";

        private double ToCelsius(double inKelvins) => 
            inKelvins + KELVIN;
    }

    [Serializable]
    public class Root
    {
        public List<Weather> weather;
        public Main main;
        public string name;
    }

    [Serializable]
    public class Weather
    {
        public string main;
        public string description;
    }

    [Serializable]
    public class Main
    {
        public float temp;
    }
}