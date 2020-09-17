<template>
  <div class="headerImg">
    <img :src="pictureUrl" alt />
    <input
      @change="onFileChange"
      accept="image/gif, image/jpeg, image/jpg, image/png, image/svg"
      class="file-button"
      type="file"
    />
  </div>
</template>
<style></style>
<script>
import pictureUrl from '@/assets/images/headerImg.jpg'
import { upload } from '@/api/common'
export default {
  data() {
    return {
      images: [],
      pictureUrl,
    }
  },
  methods: {
    test() {
      var vm = this
      console.log(vm.message)
    },
    onFileChange(e) {
      let formData = new FormData()
      // 　　　　　let data = JSON.stringify({
      // 　　　　　　　　user: "username",
      // 　　　　　　　　env: "dev"
      // 　　　　　　})
      formData.append('file', e.target.files[0]) //formData.append('data', data);   // 上传文件的同时， 也可以上传其他数据
      this.createImage(formData)
    },
    createImage(file) {
      upload(file).then((res) => {
        if (res.data.code == 200) {
          this.pictureUrl = res.data.data.hostUrl
        } else {
          this.$Message.error(res.data.message)
        }
      })
    },
  },
}
</script>
