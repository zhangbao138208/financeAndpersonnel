<template>
  <div class="demo-grid" ref="imageResume">
    <Row>
      <Col class="resume-left" span="8">
        <!-- 头像 -->
        <header-img :fields="fields" @getSubChildFields="_getChildFields"></header-img>
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
              v-model="fields.mobile"
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
        <skills :fields="fields" @getSubChildFields="_getChildFields"></skills>
        <!-- 兴趣爱好 -->
        <interest :fields="fields" @getSubChildFields="_getChildFields"></interest>
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
            <mu-select label="请选择求职部门" style="padding-right:20px" v-model="fields.departmentCode">
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
          <div class="ambition">
            <mu-select label="性别" style="padding-right:20px" v-model="fields.sex">
              <mu-option
                :key="sexVal.code"
                :label="sexVal.name"
                :value="sexVal.code"
                avatar
                v-for="sexVal,index in sex"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{sexVal.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{sexVal.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
            <mu-select label="婚姻状况" v-model="fields.homeInfo">
              <mu-option
                :key="homeInfoVal.code"
                :label="homeInfoVal.name"
                :value="homeInfoVal.code"
                avatar
                v-for="homeInfoVal,index in homeInfo"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{homeInfoVal.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{homeInfoVal.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
          </div>
        </div>
        <!-- 教育背景 -->
        <education :fields="fields" @getSubChildFields="_getChildFields"></education>
        <!-- 工作经验 -->
        <work :fields="fields" @getSubChildFields="_getChildFields"></work>
        <!-- 奖项荣誉 -->
        <award :fields="fields" @getSubChildFields="_getChildFields"></award>
        <!-- 自我评价 -->
        <assessment :fields="fields" @getSubChildFields="_getChildFields"></assessment>

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
//import Html2canvas from '@/libs/html2canvas'
import html2canvas from 'html2canvas'

import MuIcon from 'muse-ui/src/Icon/Icon'

import { loadPositionSimpleList } from '@/api/user/position'
import { loadDepartmentSimpleList } from '@/api/user/department'

export default {
  props: ['fields'],
  watch: {
    fields: {
      handler: function (val, oldVal) {
        //注意这里接收，不能直接更新prop值，需要定义变量，否则会提示有风险更新父组件
        this.$emit('getChildFields', val)
      },
      deep: true,
    },
  },
  data() {
    return {
      positions: [],
      departments: [],
      sex: [
        { name: '男', code: 1 },
        { name: '女', code: 2 },
        { name: '其他', code: 3 },
      ],
      homeInfo: [
        { name: '已婚', code: 1 },
        { name: '未婚', code: 2 },
        { name: '离异', code: 3 },
        { name: '丧偶', code: 4 },
        { name: '其他', code: 5 },
      ],

      resumeImg: '../../static/img/headerImg.jpg',
      readResume: false,
    }
  },
  mounted() {
    this.InitData()
  },
  methods: {
    _getChildFields(fields) {
      this.fields = fields
    },
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
      html2canvas(this.$refs.imageResume).then((canvas) => {
        // 转成图片，生成图片地址
        this.url = canvas.toDataURL('image/png')
        this.resumeImg = canvas.toDataURL('image/png')
        this.readResume = true
      })
      // const height = document.getElementById('app').offsetHeight
      // const width = document.getElementById('app').offsetWidth
      // const canvas = document.querySelector('canvas')
      // debugger
      // const ctx = canvas.getContext('2d')
      // const _this = this
      // // canvas.width = width;
      // // canvas.height = height;
      // // Html2canvas(document.querySelector("#app"), {canvas: canvas}).then(function(canvas) {
      // //     // console.log('简历已经生成');
      // //     var img = canvas.toDataURL();
      // //     console.log(img);
      // // });
      // Html2canvas(document.querySelector('#app'), {
      //   onrendered: function (canvas) {
      //     // document.body.appendChild(canvas);
      //     const img = canvas.toDataURL()
      //     // console.log(img); //在console中会输出图片的路径，然后复制在浏览器一粘贴，就可以看到。
      //     _this.url = img
      //     _this.resumeImg = img
      //     _this.readResume = true
      //   },
      // })
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
