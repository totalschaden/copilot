using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;

namespace CoPilot
{
    public static class PartyElements
    {
        public static List<string> ListOfPlayersInParty(int child)
        {
            var playersInParty = new List<string>();

            try
            {
                var baseWindow = CoPilot.instance.GameController.IngameState.IngameUi.Children[child];
                if (baseWindow != null)
                {
                    var partyList = baseWindow.Children[0]?.Children[0]?.Children;
                    playersInParty.AddRange(from player in partyList where player != null && player.ChildCount >= 3 select player.Children[0].Text);
                }

            }
            catch (Exception)
            {
                // ignored
            }

            return playersInParty;
        }

        public static List<PartyElementWindow> GetPlayerInfoElementList()
        {
            var playersInParty = new List<PartyElementWindow>();

            try
            {
                var baseWindow = CoPilot.instance.GameController?.IngameState?.IngameUi?.Children?[19];
                var partElementList = baseWindow?.Children?[0]?.Children?[0]?.Children;
                if (partElementList != null)
                {
                    foreach (var partyElement in partElementList)
                    {
                        var playerName = partyElement?.Children?[0]?.Text;
                        if (partyElement?.Children != null)
                        {
                            var newElement = new PartyElementWindow
                            {
                                PlayerName = playerName,
                                //get party element
                                Element = partyElement,
                                //party element swirly tp thingo, if in another area it becomes child 4 as child 3 becomes the area string
                                TpButton = partyElement.Children[partyElement.ChildCount == 4 ? 3 : 2]
                            };

                            playersInParty.Add(newElement);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CoPilot.instance.LogError("Character: " + e, 5);
            }

            return playersInParty;
        }
    }

    public class PartyElementWindow
    {
        public string PlayerName { get; set; } = string.Empty;
        public PlayerData Data { get; set; } = new PlayerData();
        public Element Element { get; set; } = new Element();
        public Element TpButton { get; set; } = new Element();

        public override string ToString()
        {
            return $"PlayerName: {PlayerName}, Data.PlayerEntity.Distance: {Data.PlayerEntity.Distance(Entity.Player).ToString() ?? "Null"}";
        }
    }

    public class PlayerData
    {
        public Entity PlayerEntity { get; set; } = null;
    }
}