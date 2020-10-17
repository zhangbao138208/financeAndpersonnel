<!-- 自我评价 -->
<template>
  <div class="assessment">
    <div class="title">
      <mu-icon color="#1d3653" value="mood"></mu-icon>
      <span>自我评价</span>
    </div>
    <mu-button @click="openAssessment" class="demo-float-button awardBtn" icon>
      <mu-icon value="add"></mu-icon>
    </mu-button>
    <mu-dialog :open="assessmentDialog" @close="closeAssessment" title="自我评价" width="500">
      <mu-row gutter>
        <mu-col>
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="进行自我评价"
            max-length="20"
            multiLine
            v-model="assessmentContent"
          ></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-button @click="closeAssessment" flat label="关闭" primary>关闭</mu-button>
      <mu-button @click="assessmentData" color="#2b85e4" flat label="确定">确定</mu-button>
    </mu-dialog>

    <div class="assessment-content">
      <div class="empty" v-if="assessmentEmpty">自我评价</div>
      <div class="list" v-else :key="index" v-for="(item,index) in assessments">
        <mu-row gutter>
          <mu-col span="10">
            <span class="content-font">• {{item}}</span>
          </mu-col>
          <mu-col span="2">
            <a @click="deleteAssessment(index)" class="deleteBtn" href="javascript:;">
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
        if (val.selfEvaluations && val.selfEvaluations.length) {
          this.assessments = JSON.parse(val.selfEvaluations)
          this.assessmentEmpty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    assessments: function (val, oldVal) {
      this.fields.selfEvaluations = JSON.stringify(val)
    },
  },
  data() {
    return {
      assessmentDialog: false,
      assessmentContent: '',
      assessments: [],
      assessmentEmpty: true,
    }
  },
  methods: {
    openAssessment() {
      this.assessmentDialog = true
    },
    closeAssessment() {
      this.assessmentDialog = false
    },
    assessmentData() {
      this.assessments.push(this.assessmentContent)
      this.assessmentContent = ''
      this.assessmentDialog = false
      this.assessmentEmpty = false
    },
    deleteAssessment(index) {
      this.assessments.splice(index, 1)
      if (this.assessments.length === 0) {
        this.assessmentEmpty = true
      }
    },
  },
}
</script>
