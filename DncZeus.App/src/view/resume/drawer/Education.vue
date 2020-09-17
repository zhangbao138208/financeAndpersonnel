<template>
	<div class="education">
	    <div class="title">
	        <mu-icon value="school" color="#1d3653" ></mu-icon>
	    <span>教育背景</span>
	    </div>
	    <mu-button icon class="demo-float-button educationBtn" @click="openEducation">
        <mu-icon value="add"></mu-icon>
      </mu-button>
	   <Modal
        v-model="educationDialog"
        title="教育背景"
       :styles="{top: '200px'}"
        width="647"
        @on-ok="eduData"
        @on-cancel="closeEducation"
        >
          <mu-row gutter>
      <mu-col span="6">
        <div >
         <DatePicker 
         type="date" 
         placeholder="开始时间" 
         format="yyyy-MM-dd" 
         style="width: 220px;" 
         size="large" 
         v-model="edu.startTime">
         </DatePicker>
          
        </div>
      </mu-col>

      <mu-col span="6">
         <DatePicker 
         type="date" 
         placeholder="结束时间" 
         format="yyyy-MM-dd" 
         style="width: 220px;" 
         size="large" 
         v-model="edu.endTime">
         </DatePicker>

       
      </mu-col>
      <mu-col span="6">
	              <mu-text-field help-text="毕业学校" fullWidth v-model="edu.school" ></mu-text-field>
	            </mu-col>
	            <mu-col span="6">
	              <mu-text-field help-text="专业技能" fullWidth v-model="edu.professional" ></mu-text-field>
	            </mu-col>
	          </mu-row>
	          <mu-row gutter>
	            <mu-col>
	              <mu-text-field help-text="输入教育背景详情"  multiLine :rows="1" :rowsMax="5" fullWidth v-model="edu.content" ></mu-text-field>
	            </mu-col>
	          </mu-row>
    </Modal>
	    

	      <div class="education-content lists">
	          <div v-if="empty" class="empty">请添加教育背景</div>
	          <div v-for="(item,index) in edus" class="list" v-else>
	              <mu-row gutter>
	                  <mu-col span="4">
	                    <span class="title-font">{{item.startTime}}</span>
	                    <span class="title-font">至</span>
	                    <span class="title-font">{{item.endTime}}</span>
	                  </mu-col>
	                  <mu-col span="3">
	                    <span class="title-font">{{item.school}}</span>
	                  </mu-col>
	                  <mu-col span="3">
	                    <span class="title-font">{{item.professional}}</span>
	                  </mu-col>
	                  <mu-col span="2">
	                    <a href="javascript:;" class="deleteBtn" @click="deleteData(index)">
	                        <mu-icon value="delete" color="#fff" ></mu-icon>
	                    </a>
	                  </mu-col>
	                </mu-row>
	                <mu-row gutter>
	                <mu-col width="100" table="100" desktop="100">
	                  <span class="content-font">{{item.content}}</span>
	                </mu-col>
	            </mu-row>
	          </div>
	      </div>
	  </div>
</template>
<style></style>
<script>
  import MuIcon from "muse-ui/src/Icon/Icon";
	export default{
    components: {MuIcon},
    data(){
			return {
		      	educationDialog:false,
		      	empty:true,
		     	edu:{
			        startTime:"",
			        endTime:"",
			        school:"",
			        professional:"",
			        content:"",
		      	},
		      	edus:[],
		    }
		},
		methods: {
		    openEducation () {
		      this.educationDialog = true
		    },
		    closeEducation () {
		      this.educationDialog = false
		    },
		    eduData(){
		      this.edus.push(this.edu);
		      this.edu = {
		        startTime:"",
		        endTime:"",
		        school:"",
		        professional:"",
		        content:"",
		      };
		      this.educationDialog = false;
		      this.empty = false;
		    },
		    deleteData(index){
		      this.edus.splice(index,1);
		      if(this.edus.length===0){
		          this.empty = true;
		      }
		    },
		},
	}
</script>








