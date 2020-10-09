// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'
import iView from 'iview'
import i18n from '@/locale'
import config from '@/config'
import importDirective from '@/directive'
import installPlugin from '@/plugin'
import 'iview/dist/styles/iview.css'

import './index.less'
import '@/assets/icons/iconfont.css'
import TreeTable from 'tree-table-vue'
import Moment from 'moment'

import SliderVerificationCode from 'slider-verification-code';
import 'slider-verification-code/lib/slider-verification-code.css';

Vue.use(SliderVerificationCode);

// 引入 vue-kikindeditor 需要的文件
import VueKindEditor from 'vue-kindeditor'
import 'kindeditor/kindeditor-all.js'
import 'kindeditor/themes/default/default.css'

Vue.config.productionTip = false

// 注册 vue-kikindeditor plugin
Vue.use(VueKindEditor)


import Blob from './excel/Blob'
import Export2Excel from './excel/Export2Excel.js'

// 定义全局时间戳过滤器
Vue.filter('formatDate', function(value) {
    return Moment(value).format('YYYY-MM-DD')
})
import { initRouter } from '@/libs/router-util'
import MuseUI from 'muse-ui';
import 'muse-ui/dist/muse-ui.css';
//import '../static/css/theme-carbon.css' // 使用 carbon 主题(可更改)

Vue.use(MuseUI);

import '@/directive/echartResizeHelper.js';

import JSEncrypt from 'jsencrypt';


Vue.prototype.$getRsaCode = function(str) { // 注册方法
        let pubKey = config.rsaPublicKey // ES6 模板字符串 引用 rsa 公钥
        let encryptStr = new JSEncrypt();
        encryptStr.setPublicKey(pubKey); // 设置 加密公钥
        let data = encryptStr.encrypt(str.toString()); // 进行加密
        return data;
    }
    //const Base64 = require('js-base64').Base64


// 实际打包时应该不引入mock
/* eslint-disable */
// if (process.env.NODE_ENV !== 'production') require('@/mock')

import hasPermission from '@/directive/hasPermission.js';
Vue.use(hasPermission);

Vue.use(iView, {
    i18n: (key, value) => i18n.t(key, value)
})
Vue.use(TreeTable)
    /**
     * @description 注册admin内置插件
     */
installPlugin(Vue)
    /**
     * @description 生产环境关掉提示
     */
Vue.config.productionTip = false
    /**
     * @description 全局注册应用配置
     */
Vue.prototype.$config = config
    /**
     * 注册指令
     */
importDirective(Vue)

/* eslint-disable no-new */
new Vue({
    el: '#app',
    router,
    i18n,
    store,
    created() {

    },
    mounted() {
        var target = this;
        //initRouter(target);
        // 调用方法，动态生成路由
        setTimeout(function() {
            //initRouter(target);
        }, 1500);
    },
    render: h => h(App)
})