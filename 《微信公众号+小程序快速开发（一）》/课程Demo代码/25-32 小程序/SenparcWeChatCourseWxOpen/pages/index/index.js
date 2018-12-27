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
  bindWebsocketTap: function () {
    wx.navigateTo({
      url: '../websocket/websocket'
    })
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
  //测试模板消息提交form
  formTemplateMessageSubmit: function (e) {
    var submitData = JSON.stringify({
      sessionId: wx.getStorageSync("sessionId"),
      formId: e.detail.formId
    });

    wx.request({
      url: wx.getStorageSync('domainName') + '/WxOpen/TemplateTest',
      data: submitData,
      method: 'POST',
      success: function (res) {
        // success
        var json = res.data;
        console.log(res.data);
        //模组对话框
        wx.showModal({
          title: '已尝试发送模板消息',
          content: json.msg,
          showCancel: false
        });
      }
    })
  },
  wxPay: function () {
    wx.request({
      url: wx.getStorageSync('domainName') + '/WxOpen/GetPrepayid',//注意：必须使用https
      data: {
        sessionId: wx.getStorageSync('sessionId')
      },
      method: 'POST',
      success: function (res) {
        // success
        var json = res.data;
        console.log(res.data);

        if (json.success) {
          wx.showModal({
            title: '得到预支付id',
            content: 'package' + json.package,
            showCancel: false
          });

          //开始发起微信支付
          wx.requestPayment(
            {
              'timeStamp': json.timeStamp,
              'nonceStr': json.nonceStr,
              'package': json.package,
              'signType': 'MD5',
              'paySign': json.paySign,
              'success': function (res) {
                wx.showModal({
                  title: '支付成功！',
                  content: '请在服务器后台的回调地址中进行支付成功确认，不能完全相信UI！',
                  showCancel: false
                });
              },
              'fail': function (res) {
                console.log(res);
                wx.showModal({
                  title: '支付失败！',
                  content: '请检查日志！',
                  showCancel: false
                });
              },
              'complete': function (res) {
                wx.showModal({
                  title: '支付流程结束！',
                  content: '执行 complete()，成功或失败都会执行！',
                  showCancel: false
                });
              }
            })

        } else {
          wx.showModal({
            title: '微信支付发生异常',
            content: json.msg,
            showCancel: false
          });
        }
      }
    });
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
