<template>
  <div class="work">
    <div class="title">
      <mu-icon color="#1d3653" value="work"></mu-icon>
      <span>工作经验</span>
    </div>
    <mu-button @click="openWork" class="demo-float-button educationBtn" icon>
      <mu-icon color="#1d3653" value="add"></mu-icon>
    </mu-button>
    <Modal
      :styles="{top: '200px'}"
      @on-cancel="closeWork"
      @on-ok="workData"
      title="工作经验"
      v-model="workDialog"
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
              v-model="work.startTime"
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
            v-model="work.endTime"
          ></DatePicker>
        </mu-col>
        <mu-col span="6">
          <mu-text-field fullWidth help-text="工作单位" v-model="work.department"></mu-text-field>
        </mu-col>
        <mu-col span="6">
          <mu-text-field fullWidth help-text="职位" v-model="work.position"></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-row gutter>
        <mu-col>
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="输入工作经验实例"
            multiLine
            v-model="work.content"
          ></mu-text-field>
        </mu-col>
      </mu-row>
    </Modal>

    <div class="work-content">
      <div class="empty" v-if="workEmpty">请添加工作经验情况</div>
      <div class="list" v-else v-for="(item,index) in works">
        <mu-row gutter>
          <mu-col span="4">
            <span class="title-font">{{item.startTime| formatDate}}</span>
            <span class="title-font"> 至 </span>
            <span class="title-font">{{item.endTime| formatDate}}</span>
          </mu-col>
          <mu-col span="3">
            <span class="title-font">{{item.department}}</span>
          </mu-col>
          <mu-col span="3">
            <span class="title-font">{{item.position}}</span>
          </mu-col>
          <mu-col span="2">
            <a @click="deleteWork(index)" class="deleteBtn" href="javascript:;">
              <mu-icon color="#fff" value="delete"></mu-icon>
            </a>
          </mu-col>
        </mu-row>
        <mu-row gutter>
          <mu-col span="12">
            <span class="content-font">{{item.content}}</span>
          </mu-col>
        </mu-row>
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
        if (val.works && val.works.length) {
          this.works = JSON.parse(val.works)
          this.workEmpty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    works: {
      handler: function (val, oldVal) {
        this.fields.works = JSON.stringify(val)
      },
      deep: true,
    },
  },
  data() {
    return {
      workDialog: false,
      workEmpty: true,
      work: {
        startTime: '',
        endTime: '',
        department: '',
        position: '',
        content: '',
      },
      works: [],
    }
  },

  methods: {
    openWork() {
      this.workDialog = true
    },
    closeWork() {
      this.workDialog = false
    },
    workData() {
      this.works.push(this.work)
      this.work = {
        startTime: '',
        endTime: '',
        department: '',
        position: '',
        content: '',
      }
      this.workDialog = false
      this.workEmpty = false
    },
    deleteWork(index) {
      this.works.splice(index, 1)
      if (this.works.length == 0) {
        this.workEmpty = true
      }
    },
  },
}
</script>
