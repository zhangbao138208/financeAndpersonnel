
<template>
  <div class="interest">
    <div class="title">
      <span>兴趣爱好</span>
      <a @click="openInterest" class="add addBtn" href="javascript:void(0);">
        <mu-icon color="#fff" value="add"></mu-icon>
      </a>
    </div>
    <div>
      <mu-dialog :open="interestDialog" @close="closeInterest" title="兴趣爱好">
        <mu-text-field fullWidth label="兴趣爱好" labelFloat v-model="interest"></mu-text-field>
        <mu-button @click="closeInterest" color="primary" flat label="取消">取消</mu-button>
        <mu-button @click="interestData" color="success" flat label="确定">确定</mu-button>
      </mu-dialog>
    </div>

    <div class="interest-item">
      <div class="empty" v-if="interestEmpty">请先添加兴趣爱好</div>
      <div class="list" v-else v-for="(item,index) in interests">
        <p>
          <span>• {{item}}</span>
          <a @click="deleteInterest(index)" class="delete" href="javascript:void (0);">
            <mu-icon value="delete"></mu-icon>
          </a>
        </p>
      </div>
    </div>
  </div>
</template>
<style></style>
<script>
export default {
  props: ['fields'],
  watch: {
    fields: {
      handler: function (val, oldVal) {
        if (val.interests && val.interests.length) {
          this.interests = JSON.parse(val.interests)
          this.interestEmpty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    interests: function (val, oldVal) {
      this.fields.interests = JSON.stringify(val)
    },
  },
  data() {
    return {
      interestDialog: false,
      interest: '',
      interests: [],
      interestEmpty: true,
    }
  },
  methods: {
    openInterest() {
      this.interestDialog = true
    },
    closeInterest() {
      this.interestDialog = false
    },
    interestData() {
      this.interests.push(this.interest)
      this.interest = ''
      this.interestDialog = false
      this.interestEmpty = false
    },
    deleteInterest(index) {
      this.interests.splice(index, 1)
      if (this.interests === 0) {
        this.interestEmpty = true
      }
    },
  },
}
</script>
