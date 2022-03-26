﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using LeaderboardCore.Interfaces;
using LeaderboardCore.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace LeaderboardCore.UI.ViewControllers
{
    [HotReload(RelativePathToLayout = @"..\Views\LeaderboardNavigationButtons.bsml")]
    [ViewDefinition("LeaderboardCore.UI.Views.LeaderboardNavigationButtons.bsml")]
    internal class LeaderboardNavigationButtonsController : BSMLAutomaticViewController, IInitializable, IDisposable, INotifyLeaderboardSet, INotifyLeaderboardActivate, INotifyLeaderboardLoad, INotifyCustomLeaderboardsChange
    {
        private PlatformLeaderboardViewController platformLeaderboardViewController;
        private FloatingScreen buttonsFloatingScreen;
        private FloatingScreen customPanelFloatingScreen;

        private bool leaderboardLoaded = false;
        private IPreviewBeatmapLevel selectedLevel;

        private List<CustomLeaderboard> customLeaderboards;
        private int currentIndex = 0;
        private CustomLeaderboard lastLeaderboard;

        private Transform containerTransform;
        private Vector3 containerPosition;

        private Transform ssLeaderboardElementsTransform;
        private Vector3 ssLeaderboardElementsPosition;

        private Transform ssPanelScreenTransform;
        private Vector3 ssPanelScreenPosition;

        [Inject]
        public void Construct(PlatformLeaderboardViewController platformLeaderboardViewController)
        {
            this.platformLeaderboardViewController = platformLeaderboardViewController;
        }

        public void Initialize()
        {
            buttonsFloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(120f, 25f), false, Vector3.zero, Quaternion.identity);
            
            var buttonsFloatingScreenTransform = buttonsFloatingScreen.transform;
            buttonsFloatingScreenTransform.SetParent(platformLeaderboardViewController.transform);
            buttonsFloatingScreenTransform.localPosition = new Vector3(3f, 50f);
            buttonsFloatingScreenTransform.localScale = Vector3.one;

            var buttonsFloatingScreenGO = buttonsFloatingScreen.gameObject;
            buttonsFloatingScreenGO.SetActive(false);
            buttonsFloatingScreenGO.SetActive(true);
            buttonsFloatingScreenGO.name = "LeaderboardNavigationButtonsPanel";
            

            customPanelFloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(100f, 25f), false, Vector3.zero, Quaternion.identity);

            var customFloatingScreenTransform = customPanelFloatingScreen.transform;
            customFloatingScreenTransform.SetParent(platformLeaderboardViewController.transform);
            customFloatingScreenTransform.localPosition = new Vector3(3f, 50f);
            customFloatingScreenTransform.localScale = Vector3.one;

            var customFloatingScreenGO = customPanelFloatingScreen.gameObject;
            customFloatingScreenGO.SetActive(false);
            customFloatingScreenGO.SetActive(true);
            customFloatingScreenGO.name = "CustomLeaderboardPanel";
        }

        public void Dispose()
        {
            if (buttonsFloatingScreen != null && buttonsFloatingScreen.gameObject != null)
            {
                Destroy(buttonsFloatingScreen.gameObject);
            }
        }

        public void OnEnable()
        {
            OnLeaderboardLoaded(leaderboardLoaded);
        }

        public void OnLeaderboardSet(IDifficultyBeatmap difficultyBeatmap)
        {
            selectedLevel = difficultyBeatmap.level;
            if (isActiveAndEnabled || selectedLevel is CustomPreviewBeatmapLevel)
            {
                OnLeaderboardLoaded(leaderboardLoaded);
            }
        }

        public void OnLeaderboardActivated()
        {
            if (containerTransform == null)
            {
                containerTransform = platformLeaderboardViewController.transform.Find("Container");
                containerPosition = containerTransform.localPosition;
            }

            if (ssLeaderboardElementsTransform == null)
            {
                ssLeaderboardElementsTransform = platformLeaderboardViewController.transform.Find("ScoreSaberLeaderboardElements");
                ssLeaderboardElementsPosition = ssLeaderboardElementsTransform.localPosition;
            }


            if (ssPanelScreenTransform == null)
            {
                ssPanelScreenTransform = platformLeaderboardViewController.transform.Find("ScoreSaberPanelScreen");
                ssPanelScreenPosition = ssPanelScreenTransform.localPosition;
            }

            buttonsFloatingScreen.SetRootViewController(this, AnimationType.None);
            OnLeaderboardLoaded(true);
        }

        public void OnLeaderboardLoaded(bool loaded)
        {
            leaderboardLoaded = loaded;
            int lastLeaderboardIndex = customLeaderboards == null ? -1 : customLeaderboards.IndexOf(lastLeaderboard);

            if (!loaded || selectedLevel == null || !(selectedLevel is CustomPreviewBeatmapLevel))
            {
                if (lastLeaderboard != null && lastLeaderboardIndex == -1 && currentIndex != 0)
                {
                    lastLeaderboard.Hide(customPanelFloatingScreen);
                    currentIndex = 0;
                    UnYeetSS();
                }
            }
            else if (lastLeaderboard != null && lastLeaderboardIndex != -1 && currentIndex == 0)
            {
                lastLeaderboard.Show(customPanelFloatingScreen, containerPosition, platformLeaderboardViewController);
                currentIndex = lastLeaderboardIndex + 1;
                YeetSS();
            }

            NotifyPropertyChanged(nameof(LeftButtonActive));
            NotifyPropertyChanged(nameof(RightButtonActive));
        }

        private void YeetSS()
        {
            if (containerTransform != null && ssLeaderboardElementsTransform != null && ssPanelScreenTransform != null)
            {
                containerTransform.localPosition = new Vector3(-999, -999);
                ssLeaderboardElementsTransform.localPosition = new Vector3(-999, -999);
                ssPanelScreenTransform.localPosition = new Vector3(-999, -999);
            }
        }

        private void UnYeetSS()
        {
            if (containerTransform != null && ssLeaderboardElementsTransform != null && ssPanelScreenTransform != null)
            {
                containerTransform.localPosition = containerPosition;
                ssLeaderboardElementsTransform.localPosition = ssLeaderboardElementsPosition;
                ssPanelScreenTransform.localPosition = ssPanelScreenPosition;
            }
        }

        [UIAction("left-button-click")]
        private void LeftButtonClick()
        {
            lastLeaderboard.Hide(customPanelFloatingScreen);
            currentIndex--;

            if (currentIndex == 0)
            {
                UnYeetSS();
                lastLeaderboard = null;
            }
            else
            {
                lastLeaderboard = customLeaderboards[currentIndex - 1];
                lastLeaderboard.Show(customPanelFloatingScreen, containerPosition, platformLeaderboardViewController);
            }

            NotifyPropertyChanged(nameof(LeftButtonActive));
            NotifyPropertyChanged(nameof(RightButtonActive));
        }

        [UIAction("right-button-click")]
        private void RightButtonClick()
        {
            if (currentIndex == 0)
            {
                YeetSS();
            }
            else
            {
                lastLeaderboard.Hide(customPanelFloatingScreen);
            }

            currentIndex++;
            lastLeaderboard = customLeaderboards[currentIndex - 1];
            lastLeaderboard.Show(customPanelFloatingScreen, containerPosition, platformLeaderboardViewController);

            NotifyPropertyChanged(nameof(LeftButtonActive));
            NotifyPropertyChanged(nameof(RightButtonActive));
        }

        public void OnLeaderboardsChanged(List<CustomLeaderboard> customLeaderboards)
        {
            int lastLeaderboardIndex = customLeaderboards.IndexOf(lastLeaderboard);

            if (lastLeaderboard != null && lastLeaderboardIndex == -1 && currentIndex != 0)
            {
                lastLeaderboard.Hide(customPanelFloatingScreen);
                currentIndex = 0;
                UnYeetSS();
            }
            else if (lastLeaderboard != null && lastLeaderboardIndex != -1 && currentIndex == 0)
            {
                lastLeaderboard.Show(customPanelFloatingScreen, containerPosition, platformLeaderboardViewController);
                currentIndex = lastLeaderboardIndex + 1;
                YeetSS();
            }

            this.customLeaderboards = customLeaderboards;
            NotifyPropertyChanged(nameof(LeftButtonActive));
            NotifyPropertyChanged(nameof(RightButtonActive));
        }

        [UIValue("left-button-active")]
        private bool LeftButtonActive => currentIndex > 0 && (leaderboardLoaded && selectedLevel != null && selectedLevel is CustomPreviewBeatmapLevel);

        [UIValue("right-button-active")]
        private bool RightButtonActive => currentIndex < customLeaderboards?.Count && (leaderboardLoaded && selectedLevel != null && selectedLevel is CustomPreviewBeatmapLevel);
    }
}
