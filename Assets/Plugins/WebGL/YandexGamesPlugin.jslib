mergeInto(LibraryManager.library, {

  YandexGames_Init: function() {
    if (typeof ysdk === 'undefined') {
      window.ysdk = null;
      window.ysdkPlayer = null;
      YaGames.init().then(ysdk_instance => {
        window.ysdk = ysdk_instance;
        window.ysdk.features.GameplayAPI?.ready();

        // Init player
        window.ysdk.getPlayer().then(_player => {
            window.ysdkPlayer = _player;
            SendMessage('YandexGamesManager', 'OnYandexSdkInitialized');
        }).catch(err => {
            console.error("Player initialization failed: ", err);
            SendMessage('YandexGamesManager', 'OnYandexSdkInitialized');
        });
      }).catch(err => {
        console.error("Yandex SDK initialization failed: ", err);
      });
    }
  },

  YandexGames_GameplayStart: function() {
    if (window.ysdk && window.ysdk.features.GameplayAPI) {
      window.ysdk.features.GameplayAPI.start();
    }
  },

  YandexGames_GameplayStop: function() {
    if (window.ysdk && window.ysdk.features.GameplayAPI) {
      window.ysdk.features.GameplayAPI.stop();
    }
  },

  YandexGames_ShowFullscreenAdv: function() {
    if (window.ysdk) {
      window.ysdk.adv.showFullscreenAdv({
        callbacks: {
          onClose: function(wasShown) {
            SendMessage('YandexGamesManager', 'OnInterstitialClosed');
          },
          onError: function(error) {
            SendMessage('YandexGamesManager', 'OnInterstitialClosed');
          }
        }
      });
    } else {
      SendMessage('YandexGamesManager', 'OnInterstitialClosed');
    }
  },

  YandexGames_ShowRewardedVideo: function() {
    if (window.ysdk) {
      window.ysdk.adv.showRewardedVideo({
        callbacks: {
          onOpen: () => {
            // Video opened
          },
          onRewarded: () => {
            SendMessage('YandexGamesManager', 'OnRewarded');
          },
          onClose: () => {
            SendMessage('YandexGamesManager', 'OnRewardedClosed');
          },
          onError: (e) => {
            SendMessage('YandexGamesManager', 'OnRewardedClosed');
          }
        }
      });
    } else {
      SendMessage('YandexGamesManager', 'OnRewardedClosed');
    }
  },

  YandexGames_SaveData: function(jsonData) {
    if (window.ysdkPlayer) {
      var dataString = UTF8ToString(jsonData);
      var dataObj = JSON.parse(dataString);
      window.ysdkPlayer.setData(dataObj).then(() => {
        console.log('Data saved successfully');
      }).catch(err => {
        console.error('Data save failed: ', err);
      });
    }
  },

  YandexGames_LoadData: function() {
    if (window.ysdkPlayer) {
      window.ysdkPlayer.getData().then(dataObj => {
        var dataString = JSON.stringify(dataObj);
        SendMessage('SaveSystem', 'OnDataLoaded', dataString);
      }).catch(err => {
        console.error('Data load failed: ', err);
        SendMessage('SaveSystem', 'OnDataLoaded', "{}");
      });
    } else {
      SendMessage('SaveSystem', 'OnDataLoaded', "{}");
    }
  },

  YandexGames_GetDeviceType: function() {
    if (window.ysdk) {
      var isMobile = window.ysdk.deviceInfo.isMobile() || window.ysdk.deviceInfo.isTablet();
      return isMobile ? 1 : 0;
    }
    // Fallback if SDK not initialized yet, try to guess from userAgent
    var ua = navigator.userAgent;
    if(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(ua)) {
      return 1;
    }
    return 0;
  }

});
