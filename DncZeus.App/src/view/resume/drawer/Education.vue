<template>
  <div class="education">
    <div class="title">
      <mu-icon color="#1d3653" value="school"></mu-icon>
      <span>教育背景</span>
    </div>
    <mu-button @click="openEducation" class="demo-float-button educationBtn" icon>
      <mu-icon value="add"></mu-icon>
    </mu-button>
    <Modal
      :styles="{top: '200px'}"
      @on-cancel="closeEducation"
      @on-ok="eduData"
      title="教育背景"
      v-model="educationDialog"
      width="647"
    >
      <mu-row gutter>
        <mu-col span="6">
          <div>
            <DatePicker
              format="yyyy-MM-dd"
              placeholder="开始时间"
              size="large"
              style="width: 220px;"
              type="date"
              v-model="edu.startTime"
            ></DatePicker>
          </div>
        </mu-col>

        <mu-col span="6">
          <DatePicker
            format="yyyy-MM-dd"
            placeholder="结束时间"
            size="large"
            style="width: 220px;"
            type="date"
            v-model="edu.endTime"
          ></DatePicker>
        </mu-col>
        <mu-col span="6">
          <mu-text-field fullWidth help-text="毕业学校" v-model="edu.school"></mu-text-field>
        </mu-col>
        <mu-col span="6">
          <mu-text-field fullWidth help-text="专业技能" v-model="edu.professional"></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-row gutter>
        <mu-col>
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="输入教育背景详情"
            multiLine
            v-model="edu.content"
          ></mu-text-field>
        </mu-col>
      </mu-row>
    </Modal>

    <div class="education-content lists">
      <div class="empty" v-if="empty">请添加教育背景</div>
      <div class="list" v-else v-for="(item,index) in edus">
        <mu-row gutter>
          <mu-col span="4">
            <span class="title-font">{{item.startTime| formatDate}}</span>
            <span class="title-font"> 至 </span>
            <span class="title-font">{{item.endTime| formatDate}}</span>
          </mu-col>
          <mu-col span="3">
            <span class="title-font">{{item.school}}</span>
          </mu-col>
          <mu-col span="3">
            <span class="title-font">{{item.professional}}</span>
          </mu-col>
          <mu-col span="2">
            <a @click="deleteData(index)" class="deleteBtn" href="javascript:;">
              <mu-icon color="#fff" value="delete"></mu-icon>
            </a>
          </mu-col>
        </mu-row>
        <mu-row gutter>
          <mu-col desktop="100" table="100" width="100">
            <span class="content-font">{{item.content}}</span>
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
        if (val.educationBackgrounds && val.educationBackgrounds.length) {
          this.edus = JSON.parse(val.educationBackgrounds)
          this.empty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    edus: function (val, oldVal) {
      this.fields.educationBackgrounds = JSON.stringify(val)
    },
  },
  data() {
    return {
      educationDialog: false,
      empty: true,
      edu: {
        startTime: '',
        endTime: '',
        school: '',
        professional: '',
        content: '',
      },
      edus: [],
    }
  },
  methods: {
    openEducation() {
      this.educationDialog = true
    },
    closeEducation() {
      this.educationDialog = false
    },
    eduData() {
      this.edus.push(this.edu)
      this.edu = {
        startTime: '',
        endTime: '',
        school: '',
        professional: '',
        content: '',
      }
      this.educationDialog = false
      this.empty = false
    },
    deleteData(index) {
      this.edus.splice(index, 1)
      if (this.edus.length === 0) {
        this.empty = true
      }
    },
  },
}
</script>








