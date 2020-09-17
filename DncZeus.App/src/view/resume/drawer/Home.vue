<template>
  <div class="demo-grid">
    <Row>
      <Col class="resume-left" span="8">
        <!-- 头像 -->
        <header-img></header-img>
        <!-- 个人基本信息 -->
        <div class="baseInfo">
          <mu-container>
            <mu-text-field
              :underlineShow="false"
              fullWidth
              icon="face"
              placeholder="输入年龄"
              v-model="fields.age"
            ></mu-text-field>
            <mu-text-field
              :underlineShow="false"
              fullWidth
              icon="place"
              placeholder="输入住址"
              type="text"
              v-model="fields.address"
            ></mu-text-field>
            <mu-text-field
              :underlineShow="false"
              fullWidth
              icon="phone"
              placeholder="输入电话号码"
              v-model="fields.phone"
            ></mu-text-field>
            <mu-text-field
              :underlineShow="false"
              fullWidth
              icon="mail"
              placeholder="输入邮箱"
              suffix=".com"
              v-model="fields.email"
            ></mu-text-field>
          </mu-container>
        </div>
        <!-- 技能 -->
        <skills></skills>
        <!-- 兴趣爱好 -->
        <interest></interest>
      </Col>

      <Col span="16">
        <mu-button @click="createImg" class="demo-float-button createPic" icon>
          <mu-icon value="camera"></mu-icon>
        </mu-button>
        <div>
          <mu-text-field
            :underlineShow="false"
            class="nameInput"
            placeholder="请输入姓名"
            v-model="fields.realName"
          ></mu-text-field>
          <div class="ambition">
            <mu-select label="请选择求职部门" v-model="fields.departmentCode" 
            style="padding-right:20px">
              <mu-option
                :key="department.code"
                :label="department.name"
                :value="department.code"
                avatar
                v-for="department,index in departments"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{department.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{department.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
            <mu-select label="请选择求职岗位" v-model="fields.positionCode">
              <mu-option
                :key="position.code"
                :label="position.name"
                :value="position.code"
                avatar
                v-for="position,index in positions"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{position.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{position.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
          </div>
        </div>
        <!-- 教育背景 -->
        <education></education>
        <!-- 工作经验 -->
        <work></work>
        <!-- 奖项荣誉 -->
        <award></award>
        <!-- 自我评价 -->
        <assessment></assessment>

        <!-- 简历预览 -->
        <div v-if="readResume">
          <div class="shadow"></div>
          <div class="resume">
            <mu-card>
              <mu-card-media subTitle title="恭喜你，简历成功生成">
                <img :src="resumeImg" />
              </mu-card-media>
              <mu-card-actions>
                <a :href="url" class="generatePic" download target="_blank">下载简历</a>
                <mu-button @click="closeResume" flat label="关闭预览">关闭预览</mu-button>
              </mu-card-actions>
            </mu-card>
          </div>
        </div>
      </Col>
    </Row>
  </div>
</template>
<script>
import HeaderImg from './HeaderImg'
import Skills from './Skills'
import Interest from './Interest'
import Education from './Education'
import Work from './Work'
import Award from './Award'
import Assessment from './Assessment'
import Html2canvas from '@/libs/html2canvas'

import MuIcon from 'muse-ui/src/Icon/Icon'

import { loadPositionSimpleList } from '@/api/user/position'
import { loadDepartmentSimpleList } from '@/api/user/department'

export default {
   props:['fields'],
  watch: {
        fields:{
            handler: function (val, oldVal) {
              //注意这里接收，不能直接更新prop值，需要定义变量，否则会提示有风险更新父组件
            console.log("========",val.age,oldVal.age)
            this.$emit('getChildFields',val)
            },
            deep:true
        },
       
        
    },
  data() {
    return {
      positions: [],
      departments:[],
      age: '',
      adress: '',
      phone: '',
      email: '',
      name: '',
      ambition: '',
      url: '',
      resumeImg: '../../static/img/headerImg.jpg',
      readResume: false,
      custom: {
        value1: '',
        value2: '',
      },
    }
  },
  mounted() {
    console.log(this.fields)
    this.fields.works="rer"
    this.InitData()
  },
  methods: {
    InitData() {
      loadPositionSimpleList().then((res) => {
        this.positions = res.data.data
      })
      loadDepartmentSimpleList().then((res) => {
        this.departments = res.data.data
      })
    },
    createImg() {
      console.log('生成图片中')
      const height = document.getElementById('app').offsetHeight
      const width = document.getElementById('app').offsetWidth
      const canvas = document.querySelector('canvas')
      debugger
      const ctx = canvas.getContext('2d')
      const _this = this
      // canvas.width = width;
      // canvas.height = height;
      // Html2canvas(document.querySelector("#app"), {canvas: canvas}).then(function(canvas) {
      //     // console.log('简历已经生成');
      //     var img = canvas.toDataURL();
      //     console.log(img);
      // });
      Html2canvas(document.querySelector('#app'), {
        onrendered: function (canvas) {
          // document.body.appendChild(canvas);
          const img = canvas.toDataURL()
          // console.log(img); //在console中会输出图片的路径，然后复制在浏览器一粘贴，就可以看到。
          _this.url = img
          _this.resumeImg = img
          _this.readResume = true
        },
      })
    },
    closeResume() {
      this.readResume = false
    },
  },

  components: {
    MuIcon,
    Skills,
    Interest,
    Education,
    Work,
    Award,
    Assessment,
    HeaderImg,
  },
}
</script>
<style scoped>
@import './css/index.css';
</style>
