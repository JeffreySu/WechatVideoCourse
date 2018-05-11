//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    motto: 'Hello Senparc',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    currentTime:''
  },
  //事件处理函数
  bindViewTap1: function() {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  openUserInfo:function(){
    wx.navigateTo({
      url: '../userinfo/userinfo',
    })
  },
  //处理wx.request请求
  doRequest: function () {
    var that = this;
    wx.request({
      url: wx.getStorageSync('domainName') + '/WxOpen/RequestData',
      data: { nickName: that.data.userInfo.nickName },
      method: 'POST', // OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, CONNECT
      // header: {}, // 设置请求的 header
      success: function (res) {
        console.log(res);
        // success
        var json = res.data;
        //模组对话框
        wx.showModal({
          title: '收到消息',
          content: json.msg,
          showCancel: false,
          success: function (modalRes) {
            if (modalRes.confirm) {
              console.log('用户点击确定')
            }
          }
        });
      },
      fail: function () {
        // fail
      },
      complete: function () {
        // complete
      }
    })
  },
  oauth:function(){
    wx.navigateTo({
      url: '../oauth/oauth',
    })
  },
  onLoad: function () {
    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse){
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }

    // //启动计时器
    // var that = this;
    // var interval = setInterval(function(){
    //   that.setData({currentTime : new Date().toLocaleTimeString()},1000);
    // });
  },
  getUserInfo: function(e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  }
})
