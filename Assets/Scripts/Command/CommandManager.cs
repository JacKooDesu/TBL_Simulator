using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Command
{
    public class CommandManager : MonoBehaviour
    {
        public static Command<int, int> DRAW_CARD;
        public static Command<int, int> ADD_CARD;
        public static List<object> commandList;

        void Awake()
        {
            DRAW_CARD = new Command<int, int>("drawCard", (v1, v2) =>
            {
                (NetworkRoomManager.singleton as NetworkRoomManager).players[v1].DrawCard(v2);
            });

            ADD_CARD = new Command<int, int>("addCard", (v1, v2) =>
            {
                (NetworkRoomManager.singleton as NetworkRoomManager).players[v1].AddCard(v2);
            });



            commandList = new List<object>{
                DRAW_CARD,
                ADD_CARD
            };
        }

        public static void CheckCommand(string s)
        {
            // print(s);
            string[] properties = s.Split(' ');

            for (int i = 0; i < commandList.Count; ++i)
            {
                CommandBase commandBase = commandList[i] as CommandBase;

                if (s.Contains(commandBase.CommandId))
                {

                    if (commandList[i] as Command != null)
                    {
                        (commandBase as Command).Invoke();
                    }
                    else if (commandList[i] as Command<int> != null)
                    {
                        (commandBase as Command<int>).Invoke(int.Parse(properties[1]));
                    }
                    else if (commandList[i] as Command<int, int> != null)
                    {
                        (commandBase as Command<int, int>).Invoke(int.Parse(properties[1]), int.Parse(properties[2]));
                    }
                }
            }
        }
    }

}
