<template>
  <!-- <div class="headerImg"> -->
  <Upload
    :before-upload="handleUpload"
    :format="['jpg','jpeg','png','gif','svg']"
    accept="image/gif, image/jpeg, image/jpg, image/png, image/svg"
    action
    style="padding-left:66px;"
  >
    <div class="headerImg">
      <img :src="fields.imagePath" alt />
    </div>
  </Upload>

  <!-- <input
      @change="onFileChange"
      accept="image/gif, image/jpeg, image/jpg, image/png, image/svg"
      class="file-button"
      type="file"
  />-->
  <!-- </div> -->
</template>
<style></style>
<script>
import pictureUrl from '@/assets/images/headerImg.jpg'
import { upload } from '@/api/common'
export default {
  props: ['fields'],
  watch: {
    fields: {
      handler: function (val, oldVal) {
        // if (!val.imagePath) {
        //   val.imagePath = pictureUrl
        // }
        this.fields = val
        //注意这里接收，不能直接更新prop值，需要定义变量，否则会提示有风险更新父组件
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
  },
  data() {
    return {
      images: [],
      pictureUrl,
    }
  },
  created() {
    if (!this.fields.imagePath) {
      //val.imagePath = pictureUrl
      this.fields.imagePath = pictureUrl
    }
  },
  methods: {
    handleUpload(file) {
      let formData = new FormData()
      // 　　　　　let data = JSON.stringify({
      // 　　　　　　　　user: "username",
      // 　　　　　　　　env: "dev"
      // 　　　　　　})
      formData.append('file', file) //formData.append('data', data);   // 上传文件的同时， 也可以上传其他数据
      this.createImage(formData)
      return false
    },

    test() {
      var vm = this
      console.log(vm.message)
    },
    onFileChange(e) {
      // let formData = new FormData()
      // // 　　　　　let data = JSON.stringify({
      // // 　　　　　　　　user: "username",
      // // 　　　　　　　　env: "dev"
      // // 　　　　　　})
      // formData.append('file', e.target.files[0]) //formData.append('data', data);   // 上传文件的同时， 也可以上传其他数据
      // this.createImage(formData)
    },
    createImage(file) {
      upload(file).then((res) => {
        if (res.data.code == 200) {
          this.fields.imagePath = res.data.data.hostUrl
        } else {
          this.$Message.error(res.data.message)
        }
      })
    },
  },
}
</script>
