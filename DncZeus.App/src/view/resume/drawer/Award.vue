<template>
  <div class="award">
    <div class="title">
      <mu-icon color="#1d3653" value="star"></mu-icon>
      <span>荣誉奖项</span>
    </div>
    <mu-button @click="openAward" class="demo-float-button awardBtn" icon>
      <mu-icon color="#1d3653" value="add"></mu-icon>
    </mu-button>
    <mu-dialog :open="awardDialog" @close="closeAward" title="荣誉奖项" width="500">
      <mu-row gutter>
        <mu-col span="6">
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="所获得的奖项"
            multiLine
            v-model="awardContent"
          ></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-button @click="closeAward" flat label="关闭" primary>关闭</mu-button>
      <mu-button @click="awardData" flat label="确定" primary>确定</mu-button>
    </mu-dialog>

    <div class="award-content">
      <div class="empty" v-if="awardEmpty">请添加荣誉奖项</div>
      <div class="list" v-else v-for="(item,index) in awards">
        <mu-row gutter>
          <mu-col>
            <span class="content-font">• {{item}}</span>
          </mu-col>
          <mu-col>
            <a @click="deleteAward(index)" class="deleteBtn" href="javascript:;">
              <mu-icon color="#fff" value="delete"></mu-icon>
            </a>
          </mu-col>
        </mu-row>
      </div>
    </div>
  </div>
</template>
<style></style>
<script>
import MuIcon from 'muse-ui/src/Icon/Icon'
export default {
  components: { MuIcon },
  props: ['fields'],
  watch: {
    fields: {
      handler: function (val, oldVal) {
        if (val.awards && val.awards.length) {
          this.awards = JSON.parse(val.awards)
          this.awardEmpty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    awards: function (val, oldVal) {
      this.fields.awards = JSON.stringify(val)
    },
  },
  data() {
    return {
      awardDialog: false,
      awardContent: '',
      awards: [],
      awardEmpty: true,
    }
  },
  methods: {
    openAward() {
      this.awardDialog = true
    },
    closeAward() {
      this.awardDialog = false
    },
    awardData() {
      this.awards.push(this.awardContent)
      this.awardContent = ''
      this.awardDialog = false
      this.awardEmpty = false
    },
    deleteAward(index) {
      this.awards.splice(index, 1)
      if (this.awards.length === 0) {
        this.awardEmpty = true
      }
    },
  },
}
</script>
