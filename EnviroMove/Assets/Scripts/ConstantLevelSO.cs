using System;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class ConstantLevelSO : ScriptableObject
    {
        [TextArea] public string level1;
        [TextArea] public string level2;
        [TextArea] public string level3;
        [TextArea] public string level4;
        [TextArea] public string level5;

        public string GetLevel(int i)
        {
            switch (i)
            {
                case 1 : return level1;
                case 2 : return level2;
                case 3 : return level3;
                case 4 : return level4;
                case 5 : return level5;
                default: throw new IndexOutOfRangeException();
            }
        }
    }
}