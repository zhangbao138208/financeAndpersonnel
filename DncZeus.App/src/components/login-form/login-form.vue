<template>
  <Form
    ref="loginForm"
    :model="form"
    :rules="rules"
    @keydown.enter.native="handleSubmit"
  >
    <FormItem prop="userName">
      <Input v-model="form.userName" placeholder="请输入用户名">
        <span slot="prepend">
          <Icon :size="16" type="ios-person" color="#2d8cf0"></Icon>
        </span>
      </Input>
    </FormItem>
    <FormItem prop="password">
      <Input type="password" v-model="form.password" placeholder="请输入密码">
        <span slot="prepend">
          <Icon :size="14" type="md-lock" color="#2d8cf0"></Icon>
        </span>
      </Input>
    </FormItem>
    <FormItem>
      <!-- <SliderVerificationCode
        v-model="value"
        inactiveValue="未解锁"
        activeValue="已解锁"
      >
      </SliderVerificationCode> -->
    </FormItem>
    <FormItem>
      <Button 
       v-if="!sliderVlid"
      long type="default" @click="handleValid"
        ><div class="loader-inner ball-scale-multiple">
          <div></div>
          <div></div>
          <div></div>
        </div>
        点击完成验证</Button
      >
      <Button 
       v-else
       ghost
      long type="success" 
      style="cursor:default"
        >
        <Icon type="md-checkmark" />
        验证通过</Button
      >
    </FormItem>
    <!-- <FormItem label="测试账户">
      <RadioGroup
        v-model="form.userType"
        type="button"
        @on-change="handleUserTypeChange"
      >
        <Radio label="超级管理员"></Radio>
        <Radio label="普通用户"></Radio>
      </RadioGroup>
    </FormItem> -->

    <FormItem>
      <Button
     
        :disabled="processing"
        @click="handleSubmit"
        type="primary"
        long
        :loading="loading"
        >{{ btnLoginText }}</Button
      >
    </FormItem>
    <p class="login-tip">欢迎使用天宫一号</p>
  </Form>
</template>
<script>
import Base64 from 'crypto-js/enc-base64'
import Utf8 from 'crypto-js/enc-utf8'
export default {
  name: 'LoginForm',
  props: {
    userNameRules: {
      type: Array,
      default: () => {
        return [{ required: true, message: '账号不能为空', trigger: 'blur' }]
      },
    },
    passwordRules: {
      type: Array,
      default: () => {
        return [{ required: true, message: '密码不能为空', trigger: 'blur' }]
      },
    },
    processing: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    sliderVlid:{
      type: Boolean,
      default: false,
    }
  },
  data() {
    return {
      value: false,
      form: {
        userName: '',
        password: '',
        userType: 1,
      },
    }
  },
  computed: {
    btnLoginText() {
      return this.processing ? '正在处理,请稍候...' : '登录'
    },
    rules() {
      return {
        userName: this.userNameRules,
        password: this.passwordRules,
      }
    },
  },
  methods: {
    handleValid() {
      this.$emit('on-valid-true')
    },
    handleSubmit() {
      // if (!this.value) {
      //    this.$Message.warning("请先拖动滑块解锁")
      //  return
      // }
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.$emit('on-success-valid', {
            userName: this.form.userName,
            password: this.$getRsaCode(this.form.password),
          })
        }
      })
    },
    handleUserTypeChange(val) {
      switch (val) {
        case '超级管理员':
          this.form.userName = 'administrator'
          break
        case '普通用户':
          this.form.userName = 'admin'
          break
      }
      this.form.password = '111111'
    },
  },
}
</script>
<style>
/*              ball-scale-multiple*/

@-webkit-keyframes ball-scale-multiple {
  0% {
    -webkit-transform: scale(0);
    transform: scale(0);
    opacity: 0;
  }
  5% {
    opacity: 1;
  }
  100% {
    -webkit-transform: scale(1);
    transform: scale(1);
    opacity: 0;
  }
}

@keyframes ball-scale-multiple {
  0% {
    -webkit-transform: scale(0);
    transform: scale(0);
    opacity: 0;
  }
  5% {
    opacity: 1;
  }
  100% {
    -webkit-transform: scale(1);
    transform: scale(1);
    opacity: 0;
  }
}

.ball-scale-multiple {
  position: relative;
  -webkit-transform: translateY(-30px);
  -ms-transform: translateY(-30px);
  transform: translateY(-30px);
}

.ball-scale-multiple > div:nth-child(2) {
  -webkit-animation-delay: 0.2s;
  animation-delay: 0.2s;
}

.ball-scale-multiple > div:nth-child(3) {
  -webkit-animation-delay: 0.4s;
  animation-delay: 0.4s;
}

.ball-scale-multiple > div {
  background-color: #03a9f4;
  width: 10px;
  height: 10px;
  border-radius: 100%;
  margin: 2px;
  -webkit-animation-fill-mode: both;
  animation-fill-mode: both;
  position: absolute;
  left: 28%;
top: 26px;
  opacity: 0;
  margin: 0;
  width: 30px;
  height: 30px;
  -webkit-animation: ball-scale-multiple 1s 0s linear infinite;
  animation: ball-scale-multiple 1s 0s linear infinite;
}
</style>
