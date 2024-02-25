using Connect4.Scripts.Services.StaticData;
using UnityEngine;
using Zenject;

namespace Connect4.Scripts.Services.Factories.UIFactory
{
    public class UIFactory : Factory, IUIFactory
    {
        private const string UiRootPath = "Prefabs/UI/UiRoot";
        private const string MenuPath = "Prefabs/UI/Menu";

        private readonly IInstantiator _instantiator;
        private readonly IStaticDataService _staticData;

        private Transform _uiRoot;

        public UIFactory(IInstantiator instantiator, IStaticDataService staticDataService) : base(instantiator)
        {
            _instantiator = instantiator;
            _staticData = staticDataService;
        }

        public void CreateUiRoot()
        {
            _uiRoot = InstantiateOnActiveScene(UiRootPath).transform;
        }

        public void CreateMenu()
        {
            InstantiateOnActiveScene(MenuPath);
        }
    }
}