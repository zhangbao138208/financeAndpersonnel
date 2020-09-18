<template>
  <div class="skills">
    <div class="title">
      <span class>技能特点</span>
      <a @click="openSkills" class="add addBtn" href="javascript:void(0);">
        <mu-icon color="#fff" value="add"></mu-icon>
      </a>
    </div>

    <div>
      <mu-dialog :open="skillsDialog" @close="closeSkills" title="技能特点">
        <mu-text-field fullWidth label="掌握的技术" labelFloat v-model="skill.name"></mu-text-field>
        <mu-row gutter>
          <mu-col>
            <mu-slider :max="100" :min="0" :step="5" class="demo-slider" v-model="skill.value"></mu-slider>
          </mu-col>
          <mu-col>
            <span>{{skill.value}}</span>
            <span>/</span>
            <span>100</span>
          </mu-col>
        </mu-row>
        <mu-button @click="closeSkills" color="blue" flat>取消</mu-button>
        <mu-button @click="skillData" color="primary" flat>确定</mu-button>
      </mu-dialog>
    </div>

    <div class="skill-item">
      <div class="empty" v-if="skillEmpty">请先添加技能特点</div>
      <div class="list" v-else v-for="(item,index) in skills">
        <p>
          <span>{{item.name}}</span>
          <a @click="deleteSkill(index)" class="delete" href="javascript:void(0);">
            <mu-icon value="delete"></mu-icon>
          </a>
        </p>
        <mu-linear-progress :value="item.value" mode="determinate"></mu-linear-progress>
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
        if(val.skills&&val.skills.length){
          this.skills=JSON.parse(val.skills)
          this.skillEmpty=false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    skills:function(val, oldVal){
       this.fields.skills=JSON.stringify(val)
    }
  },
  data() {
    return {
      value: 20,
      skillsDialog: false,
      skill: {
        name: '',
        value: 0,
      },
      skills: [],
      skillEmpty: true,
    }
  },
  methods: {
    openSkills() {
      this.skillsDialog = true
    },
    closeSkills() {
      this.skillsDialog = false
    },
    skillData() {
      this.skills.push(this.skill)
      this.skill = {
        name: '',
        value: 0,
      }
      this.skillsDialog = false
      this.skillEmpty = false
      console.log(this.skills)
    },
    deleteSkill(index) {
      this.skills.splice(index, 1)
      if (this.interests === 0) {
        this.interestEmpty = true
      }
    },
  },
}
</script>
