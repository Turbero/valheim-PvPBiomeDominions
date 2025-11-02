using System.Collections.Generic;
using Groups;
using PvPBiomeDominions.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PvPBiomeDominions.PvPManagement.UI
{
    public class PlayersListPanel
    {
        public readonly GameObject panelRoot;
        public readonly GameObject content;

        private Sprite deathIconSprite;

        private Button showHidePanelButton;

        private readonly List<GameObject> playerEntries = new();
        
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
            panelRT.anchoredPosition = new Vector2(-652, 100);
            panelRT.sizeDelta = ConfigurationFile.mapPlayersListPosition.Value;

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
            
            deathIconSprite = minimap.m_largeRoot.transform.Find("IconPanel2/IconDeath").GetComponent<Image>().sprite;
        }
        
        public void UpdateList(List<ZNet.PlayerInfo> players)
        {
            // Clear previous entries
            foreach (var go in playerEntries)
                Object.Destroy(go);
            playerEntries.Clear();
            
            //Group info 
            List<PlayerReference> groupPlayers = GameManager.GetGroupPlayers();

            // Connected players list
            foreach (var info in players)
            {
                string playerName = info.m_name;
                bool isInCurrentGroup = groupPlayers.FindIndex(pRef => pRef.name.Equals(playerName)) >= 0;
                AddRowToScrollList(info, isInCurrentGroup);
            }
        }
        
        private void AddRowToScrollList(ZNet.PlayerInfo info, bool isInCurrentGroup)
        {
            var entry = new GameObject("Player_Row", typeof(RectTransform));
            entry.transform.SetParent(content.transform, false);
            RectTransform rt = entry.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -10);
            rt.sizeDelta = new Vector2(230f, 24f);
            
            var imageGO = new GameObject("Player_Icon", typeof(RectTransform), typeof(Image));
            imageGO.transform.SetParent(entry.transform, false);
            RectTransform imageRt = imageGO.GetComponent<RectTransform>();
            imageRt.sizeDelta = new Vector2(32, 32);
            imageRt.anchoredPosition = new Vector2(-105, 0);
            Image playerIcon = imageGO.GetComponent<Image>();
            if (isInCurrentGroup)
                playerIcon.sprite = ImageManager.spriteGroupIconImage;
            else
                playerIcon.sprite = GameManager.isInfoPVP(info) ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
            
            var textGO = new GameObject("Player_Name", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(entry.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchoredPosition = new Vector2(14, 0);
            var text = textGO.GetComponent<TextMeshProUGUI>();
            text.name = "name";
            text.text = info.m_name;
            text.font = GameManager.getFontAsset("Valheim-Norse");
            text.fontSize = 22;
            text.color = Color.white;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Left;

            /*if (GameManager.isWackyEpicMMOSystemInstalled())
            {
                Logger.Log("MMO is loaded!");
                var killsGO = new GameObject("Player_Level", typeof(RectTransform), typeof(TextMeshProUGUI));
                killsGO.transform.SetParent(entry.transform, false);
                RectTransform killRt = killsGO.GetComponent<RectTransform>();
                killRt.sizeDelta = new Vector2(32, 32);
                killRt.anchoredPosition = new Vector2(155, 0);
                var textKills = killsGO.GetComponent<TextMeshProUGUI>();
                textKills.name = "level";
                textKills.text = "LVL: ???";
                
                textKills.font = GameManager.getFontAsset("Valheim-Norse");
                textKills.fontSize = 22;
                textKills.color = Color.white;
                textKills.fontStyle = FontStyles.Bold;
                textKills.alignment = TextAlignmentOptions.Left;
            }*/
            
            //TODO Kills for m_knownTexts
            /*var deathIconGO = new GameObject("Player_DeathIcon", typeof(RectTransform), typeof(Image));
            deathIconGO.transform.SetParent(entry.transform, false);
            RectTransform deathIconRt = deathIconGO.GetComponent<RectTransform>();
            deathIconRt.sizeDelta = new Vector2(32, 32);
            deathIconRt.anchoredPosition = new Vector2(105, 0);
            Image deathIcon = deathIconGO.GetComponent<Image>();
            deathIcon.sprite = deathIconSprite;*/
            
            playerEntries.Add(entry);
        }
    }
}
