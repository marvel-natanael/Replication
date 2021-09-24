using System;

namespace MyLibrary
{
    [Serializable]
    public class PlayerState
    {
        public enum state
        {
            CLONE,
            DROID,
        }
        public state State;
    }

    [Serializable]
    public class CloneTrooper : PlayerState
    {
        public string legion;
        public string rank;
    }

    [Serializable]
    public class BattleDroid : PlayerState
    {
        public string type;
        public int serialNumber;
    }
}
