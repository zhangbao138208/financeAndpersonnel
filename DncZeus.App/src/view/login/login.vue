<style lang="less">
@import './login.less';
</style>

<template>
  <div>
    <div class="bg bg-blur"></div>
    <div class="content content-front">
      <div class="login">
        <div class="login-con">
          <Card icon="log-in" title="欢迎登录" :bordered="false">
            <div class="form-con">
              <login-form
                @on-success-valid="handleSubmit"
                @on-valid-true="handleValid"
                :processing="processing"
                :loading="loading"
                :sliderVlid="sliderVlid"
              >
              </login-form>
              <Modal
                v-model="modalShow"
                width="330"
                :footer-hide="true"
               
              >
                <div slot="header" style="color: #f60; text-align: center">
                 
                </div>
                <slider
                  v-if="modalShow"
                  :loadDataFunc="loadData"
                  :verifyFunc="verifyData"
                ></slider>
              </Modal>

              <!-- <SliderVerificationCode
                height="60px"
                sliderWidth="120px"
                inactiveValue="未解锁"
                activeValue="已解锁"
                v-model="value"
              /> -->
            </div>
          </Card>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import LoginForm from '_c/login-form'
import Slider from '_c/slider'
import { mapActions } from 'vuex'
import axios from '@/libs/api.request'
import store from '@/store'
import { initRouter } from '@/libs/router-util'
import { getVerification, verify } from '@/api/user'
export default {
  components: {
    LoginForm,
    Slider,
  },
  data() {
    return {
      key: '',
      slideCookie: '',
      modalShow: false,
      sliderVlid: false,
      processing: false,
      loading: false,
    }
  },
  methods: {
    //加载数据，这个函数要slider组件回调
    loadData(callback) {
      getVerification().then((response) => {
        this.key = response.data.key
        if (callback) {
          callback(response.data)
        }
      })
    },
    //校验结果，这个函数要slider组件回调
    verifyData(param, callback) {
      verify(param).then((response) => {
        if (callback) {
          callback(response.data)
        }
        if (response.data.code == true) {
          // window.setTimeout(function () {
          //   //添加后续处理逻辑
          // }, 2000)
          this.slideCookie = response.data.cookie
          this.sliderVlid = true
          this.$Message.success('验证成功')
          this.modalShow = false
        } else {
          this.sliderVlid = false
          this.$Message.error('验证失败')
        }
      })
    },
    ...mapActions(['handleLogin', 'getUserInfo']),
    handleValid() {
      this.modalShow = true
      this.loadData()
    },
    handleSubmit({ userName, password }) {
      if (!this.sliderVlid) {
        this.$Message.warning('请先拖动滑块验证')
        return
      }
      var target = this
      this.loading = true
      this.handleLogin({ userName, password, slideCookie: this.slideCookie })
        .then((res) => {
          if (res.data.code == 200) {
            this.processing = true
            this.$Message.loading({
              duration: 0,
              closable: false,
              content: '用户信息验证成功,正在登录系统...',
            })
            this.getUserInfo().then((res) => {
              setTimeout(() => {
                initRouter(target)
                this.$router.push({
                  name: 'home',
                })

                setTimeout(() => {
                  this.$Message.destroy()
                }, 1000)
              }, 1500)
            })
          } else {
            this.processing = false
            this.loading = false
            this.$Message.error(res.data.message)
          }
        })
        .catch((error) => {
          target.loading = false
          if (!error.status) {
            this.$Message.error({
              content: '网络出错,请检查你的网络或者服务是否可用',
              duration: 5,
            })
          }
        })
    },
  },
}
</script>

<style>
.demo-spin-icon-load {
  animation: ani-demo-spin 1s linear infinite;
}
.content {
  color: #ffffff;
  font-size: 40px;
}
.bg {
  /* background: url('../../assets/images/login-bg.jpg'); */
  background: url('../../assets/images/xinglian.jpg');
  height: 100%;
  text-align: center;
  line-height: 100%;
  position: absolute;
}
.bg-blur {
  float: left;
  width: 100%;
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover;
  -webkit-filter: blur(1px);
  -moz-filter: blur(1px);
  -o-filter: blur(1px);
  -ms-filter: blur(1px);
  filter: blur(1px);
}
.content-front {
  position: absolute;
  /* left: 10px; */
  right: 10px;

  height: 100%;
  line-height: 100%;
}
</style>
