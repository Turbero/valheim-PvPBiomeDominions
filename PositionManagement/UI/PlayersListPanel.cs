using System.Collections.Generic;
using System.Linq;
using Groups;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.RPC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PvPBiomeDominions.PositionManagement.UI
{
    public class PlayerEntry
    {
        public string name;
        public int level;
        public TextMeshProUGUI levelUI;
        public TextMeshProUGUI killedTimesUI;
        public Image iconPlayer;
        public bool isPvP;

        public string GetLevelText()
        {
            return level > 0 ? level.ToString() : "???";
        }
    }
    
    public class PlayersListPanel
    {
        private static readonly Vector2 ROW_SIZE_DELTA = new(230f, 24f);
        private static readonly string PREFIX_KILLS = GameManager.PREFIX_KILLS;
        
        public readonly GameObject panelRoot;
        public readonly GameObject content;

        private readonly Sprite killsIconSprite;

        private Button showHidePanelButton;

        private readonly List<GameObject> playerEntriesObjects = new(); //TODO To be removed
        public readonly List<PlayerEntry> cachedPlayerEntries = new();
        
        
        public PlayersListPanel(Minimap minimap)
        {
            TMP_FontAsset font = TMP_Settings.defaultFontAsset;
            if (font == null)
                TMP_Settings.defaultFontAsset = GameManager.getFontAsset("Valheim-AveriaSansLibre");

            // === PANEL PRINCIPAL ===
            panelRoot = new GameObject("PlayersListPanel", typeof(RectTransform), typeof(Image));
            panelRoot.transform.SetParent(minimap.transform.Find("large"), false);

            RectTransform panelRT = panelRoot.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = ConfigurationFile.mapPlayersListPosition.Value;
            panelRT.sizeDelta = ConfigurationFile.mapPlayersListSize.Value;

            Image bgImage = panelRoot.GetComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);

            // --- SCROLLRECT ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(panelRoot.transform, false);
            Object.Destroy(scrollObj.GetComponent<Image>()); // transparent
            RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0, 0);
            scrollRT.anchorMax = new Vector2(1, 1);
            scrollRT.offsetMin = Vector2.zero;
            scrollRT.offsetMax = Vector2.zero;

            ScrollRect scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 2000f;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // --- VIEWPORT ---
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(scrollObj.transform, false);
            RectTransform vpRT = viewport.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;

            viewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;
            scrollRect.viewport = vpRT;

            // --- CONTENT ---
            content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = Vector2.zero;
            contentRT.sizeDelta = Vector2.zero;

            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = 35f;
            layout.padding = new RectOffset(0, 0, 20, 20);

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRT;

            // --- SCROLLBAR ---
            GameObject recipeScroll = InventoryGui.instance.m_recipeListScroll.gameObject;
            GameObject scrollbarObj = Object.Instantiate(recipeScroll, panelRoot.transform);
            scrollbarObj.name = "Scrollbar";

            RectTransform sbRT = scrollbarObj.GetComponent<RectTransform>();
            sbRT.anchorMin = new Vector2(1, 0);
            sbRT.anchorMax = new Vector2(1, 1);
            sbRT.pivot = new Vector2(1, 1);
            sbRT.sizeDelta = new Vector2(11, 0);
            sbRT.anchoredPosition = Vector2.zero;

            Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            // --- SHOW/HIDE BUTTON ---
            GameObject showHidePanelButtonGO =
                GameObject.Instantiate(InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton").gameObject,
                    minimap.transform.Find("large"));
            showHidePanelButtonGO.name = "PlayersListPanelButton";
            RectTransform showHidePanelButtonRt = showHidePanelButtonGO.GetComponent<RectTransform>();
            showHidePanelButtonRt.anchoredPosition = new Vector2(-650, 45);
            showHidePanelButton = showHidePanelButtonGO.GetComponent<Button>();
            showHidePanelButton.onClick = new Button.ButtonClickedEvent();
            showHidePanelButton.onClick.AddListener(() =>
            {
                panelRoot.SetActive(!panelRoot.activeSelf);
            });
            TextMeshProUGUI buttonText = showHidePanelButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.fontStyle = FontStyles.Normal;
            buttonText.color = new Color(1f, 0.7176f, 0.3603f);
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.text = ConfigurationFile.playersListPanelButtonText.Value;
            
            killsIconSprite = minimap.m_largeRoot.transform.Find("IconPanel2/IconDeath").GetComponent<Image>().sprite;
        }
        
        public void RefreshContent(List<ZNet.PlayerInfo> players, bool createNewCache)
        {
            // Refresh button name
            showHidePanelButton.GetComponentInChildren<TextMeshProUGUI>().text = ConfigurationFile.playersListPanelButtonText.Value;
            
            // Clear previous entries
            foreach (var go in playerEntriesObjects)
                Object.Destroy(go);
            playerEntriesObjects.Clear();

            if (createNewCache)
                cachedPlayerEntries.Clear();
            else
            {
                //Remove already disconnected players
                List<PlayerEntry> cachedPlayerEntriesToRemove = new List<PlayerEntry>();
                foreach (var cachedPlayerEntry in cachedPlayerEntries)
                    if (!players.Exists(p => p.m_name.Equals(cachedPlayerEntry.name)))
                        cachedPlayerEntriesToRemove.Add(cachedPlayerEntry);

                foreach (var cachedPlayerEntryToRemove in cachedPlayerEntriesToRemove)
                    cachedPlayerEntries.Remove(cachedPlayerEntryToRemove);
            }

            //Group info 
            List<PlayerReference> groupPlayers = GameManager.GetGroupPlayers();

            // Connected players list
            AddTitleHeaderToScrollList(players.Count);
            
            // Add first people in the group
            foreach (var info in players)
            {
                string playerName = info.m_name;
                bool isInCurrentGroup = groupPlayers.FindIndex(pRef => pRef.name.Equals(playerName)) >= 0;
                if (isInCurrentGroup)
                    AddRowToScrollList(info, true, createNewCache);
            }
            // Then people not in the group
            foreach (var info in players)
            {
                string playerName = info.m_name;
                bool isInCurrentGroup = groupPlayers.FindIndex(pRef => pRef.name.Equals(playerName)) >= 0;
                if (!isInCurrentGroup)
                    AddRowToScrollList(info, false, createNewCache);
            }

            // ----- SEND RPC MESSAGE TO EVERYONE TO REQUEST INFO AND FILL THE FIELDS WITH UPDATED VALUES ----- //
            Logger.Log("Sending request to everyone");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "RPC_RequestPlayerRelevantInfo");
        }

        private void AddTitleHeaderToScrollList(int playersCount)
        {
            var entry = new GameObject("Title_Row", typeof(RectTransform));
            entry.transform.SetParent(content.transform, false);
            RectTransform rt = entry.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -10);
            rt.sizeDelta = ROW_SIZE_DELTA;
            
            var textGO = new GameObject("Title_Name", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(entry.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchoredPosition = Vector2.zero;
            var text = textGO.GetComponent<TextMeshProUGUI>();
            text.name = "name";
            text.text = $"{ConfigurationFile.playersMapListTitle.Value}: <color=yellow>{playersCount}</color>";
            text.font = GameManager.getFontAsset("Valheim-Norse");
            text.fontSize = 22;
            text.color = Color.white;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            
            playerEntriesObjects.Add(entry);
        }
        
        private void AddRowToScrollList(ZNet.PlayerInfo info, bool isInCurrentGroup, bool newCache)
        {
            // -------- CREATE ROW COMPONENTS ONLY WITH ICON/NAME -------- //
            var entry = new GameObject("Player_Row", typeof(RectTransform));
            entry.transform.SetParent(content.transform, false);
            RectTransform rt = entry.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -10);
            rt.sizeDelta = ROW_SIZE_DELTA;
            
            //Player Name
            var iconGO = new GameObject("Player_Icon", typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(entry.transform, false);
            RectTransform imageRt = iconGO.GetComponent<RectTransform>();
            imageRt.sizeDelta = new Vector2(32, 32);
            imageRt.anchoredPosition = new Vector2(-150, 0);
            Image playerIcon = iconGO.GetComponent<Image>();
            playerIcon.sprite = ImageManager.spriteIconVanillaImage; //by default
            
            var nameGO = new GameObject("Player_Name", typeof(RectTransform), typeof(TextMeshProUGUI));
            nameGO.transform.SetParent(entry.transform, false);
            RectTransform textRt = nameGO.GetComponent<RectTransform>();
            textRt.anchoredPosition = new Vector2(-30, 0);
            var nameText = GetTextEntryComponent(nameGO, "Name");
            nameText.text = info.m_name;
            
            //Killed value in m_knownTexts
            var killsIconGO = new GameObject("Player_KillsIcon", typeof(RectTransform), typeof(Image));
            killsIconGO.transform.SetParent(entry.transform, false);
            //killsIconGO.SetActive(info.m_name != Player.m_localPlayer.GetPlayerName());
            killsIconGO.SetActive(false); //TODO Fix patch for lethal damage to player
            RectTransform killsIconRt = killsIconGO.GetComponent<RectTransform>();
            killsIconRt.sizeDelta = new Vector2(32, 32);
            killsIconRt.anchoredPosition = new Vector2(EpicMMOSystem_API.IsLoaded() ? 100 : 20, 0);
            Image killsIcon = killsIconGO.GetComponent<Image>();
            killsIcon.sprite = killsIconSprite;

            var killsValueGO = new GameObject("Player_KilledByHostValue", typeof(RectTransform), typeof(TextMeshProUGUI));
            killsValueGO.transform.SetParent(entry.transform, false);
            killsValueGO.SetActive(false); //TODO Fix patch for lethal damage to player
            RectTransform killedValueGORt = killsValueGO.GetComponent<RectTransform>();
            killedValueGORt.sizeDelta = new Vector2(32, 32);
            killedValueGORt.anchoredPosition = new Vector2(EpicMMOSystem_API.IsLoaded() ? 140 : 60, 0);
            TextMeshProUGUI killedValue = GetTextEntryComponent(killsValueGO, "KilledByHost");

            //MMO Level
            var levelGO = new GameObject("Player_Level", typeof(RectTransform), typeof(TextMeshProUGUI));
            levelGO.transform.SetParent(entry.transform, false);
            levelGO.SetActive(EpicMMOSystem_API.IsLoaded());
            RectTransform killRt = levelGO.GetComponent<RectTransform>();
            killRt.anchoredPosition = new Vector2(105, 0);
            var levelText = GetTextEntryComponent(levelGO, "Level");
            levelText.text = "LVL: ???"; //init value
            
            playerEntriesObjects.Add(entry);

            //Local player
            if (info.m_name == Player.m_localPlayer.GetPlayerName())
            {
                //1) Icon
                if (isInCurrentGroup)
                    playerIcon.sprite = ImageManager.spriteGroupIconImage;
                else
                    playerIcon.sprite = Player.m_localPlayer.IsPVPEnabled() ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
                
                // 2) Level
                levelText.text = $"LVL: {EpicMMOSystem_API.GetLevel()}";
                return;
            }

            //Other player
            if (newCache)
                AddPlayerEntryToCachedPlayers(info, levelText, killedValue, playerIcon);
            else 
            {
                // -------- FILL FIELDS WITH CACHE -------- //
                PlayerEntry playerEntry = cachedPlayerEntries.Find(pe => pe.name == info.m_name);
                if (playerEntry != null)
                {
                    //1) Icon
                    if (isInCurrentGroup)
                        playerIcon.sprite = ImageManager.spriteGroupIconImage;
                    else
                        playerIcon.sprite = playerEntry.isPvP ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;

                    // 2) Level
                    levelText.text = "LVL: " + playerEntry.GetLevelText();

                    // 3) Killed number
                    List<KeyValuePair<string, string>> knownTexts = Player.m_localPlayer.GetKnownTexts();
                    string killsValueToLookUp = knownTexts.FirstOrDefault(kvp => kvp.Key.Equals(PREFIX_KILLS + info.m_name)).Value;
                    killedValue.text = string.IsNullOrEmpty(killsValueToLookUp) ? "0" : killsValueToLookUp;
                }
                else
                {
                    //New player connected after table creation
                    AddPlayerEntryToCachedPlayers(info, levelText, killedValue, playerIcon);
                }
            }
        }

        private void AddPlayerEntryToCachedPlayers(ZNet.PlayerInfo info, TextMeshProUGUI levelText, TextMeshProUGUI killedValue, Image playerIcon)
        {
            cachedPlayerEntries.Add(new PlayerEntry
            {
                name = info.m_name,
                level = 0, //default
                levelUI = levelText,
                killedTimesUI = killedValue,
                iconPlayer = playerIcon,
                isPvP = false //default
            });
            //Request the info with a message and update in response
            ZRoutedRpc.instance.InvokeRoutedRPC(info.m_characterID.UserID, "RPC_RequestPlayerRelevantInfo");
        }

        private TextMeshProUGUI GetTextEntryComponent(GameObject parent, string name)
        {
            var text = parent.GetComponent<TextMeshProUGUI>();
            text.name = name;
            text.font = GameManager.getFontAsset("Valheim-Norse");
            text.fontSize = 22;
            text.color = Color.white;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Left;
            return text;
        }

        public void UpdatePlayerRelevantInfo(RPC_PlayerRelevantInfo playerRelevantInfo)
        {
            Logger.Log($"[UpdatePlayerRelevantInfo] playerEntry found: name {playerRelevantInfo.playerName}, level {playerRelevantInfo.level}, isPvP {playerRelevantInfo.isPvP}");
            
            //It has to exist at this point
            PlayerEntry playerEntry = cachedPlayerEntries.Find(p => p.name.Equals(playerRelevantInfo.playerName));

            //Icon (UI)
            if (playerEntry.iconPlayer != null && Groups.API.GroupPlayers().FindIndex(p => p.name.Equals(playerRelevantInfo.playerName)) >= 0)
            {
                Logger.Log("[UpdatePlayerRelevantInfo] isInGroup");
                playerEntry.iconPlayer.sprite = ImageManager.spriteGroupIconImage;
            }
            else
                playerEntry.iconPlayer.sprite = playerRelevantInfo.isPvP 
                    ? ImageManager.spriteIconVanillaImage
                    : ImageManager.spriteBlueIconImage;

            //isPvP
            playerEntry.isPvP = playerRelevantInfo.isPvP;
            
            //Level
            if (playerEntry.levelUI != null)
                playerEntry.levelUI.text = "LVL: " + playerRelevantInfo.GetLevelText();
            //Level (UI)
            playerEntry.level = playerRelevantInfo.level;
            
            //Killed number (UI)
            if (playerEntry.killedTimesUI != null)
            {
                var knownTexts = Player.m_localPlayer.GetKnownTexts();
                bool existKilledTimes =
                    knownTexts.Exists(kp => kp.Key.Equals(PREFIX_KILLS + playerRelevantInfo.playerName));
                Logger.Log("UpdatePlayerRelevantInfo - existKilledTimes: " + existKilledTimes);
                if (existKilledTimes)
                    playerEntry.killedTimesUI.text = knownTexts
                        .Find(kp => kp.Key.Equals(PREFIX_KILLS + playerRelevantInfo.playerName)).Value;
                else
                {
                    playerEntry.killedTimesUI.text = "0";
                    var newKnownText = PREFIX_KILLS + playerRelevantInfo.playerName;
                    Logger.Log("UpdatePlayerRelevantInfo - Add new knownText: " + newKnownText + " with value=0");
                    var dicKnownTexts =
                        (Dictionary<string, string>)GameManager.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
                    dicKnownTexts.Add(newKnownText, "0");
                }
            }
        }

        public void UpdatePlayerKilledCount(string playerNameToFind, int newCount)
        {
            PlayerEntry cachedPlayer = cachedPlayerEntries.Find(cpe => cpe.name.Equals(playerNameToFind));
            if (cachedPlayer != null)
            {
                Logger.Log($"cachedPlayer {cachedPlayer.name} found. Updating killed count...");
                cachedPlayer.killedTimesUI.text = newCount.ToString();
            }
        }
    }
}
