using UnityEngine;

namespace Infrastructure.Factory.UIFactory
{
    public interface IUIInfo
    {
        public GameObject LoadingScreen { get; }
        public GameObject MainLocationScreen { get; }
    }
}

