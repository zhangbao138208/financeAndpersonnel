<template>
  <div class="deduction">
    <mu-button @click="openDeduction" class="demo-float-button deductionBtn" icon>
      <mu-icon color="#1d3653" value="add"></mu-icon>
    </mu-button>
    <mu-dialog :open="deductionDialog" @close="closeDeduction" title="额外扣除" width="500">
      <mu-row gutter>
        <mu-col span="6">
          <InputNumber
            :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :min="0"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
            v-model="deduction.val"
          ></InputNumber>
        </mu-col>
        <mu-col span="6">
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="备注"
            multiLine
            v-model="deduction.remark"
          ></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-button @click="closeDeduction" flat label="关闭" primary>关闭</mu-button>
      <mu-button @click="deductionData" flat label="确定" primary>确定</mu-button>
    </mu-dialog>

    <div class="deduction-content">
      <div class="empty" v-if="deductionEmpty">请添加额外扣除</div>
      <div class="list" v-else v-for="(item,index) in deductions">
        <mu-row gutter>
          <mu-col>
            <span class="content-font">金额 ￥{{item.val}}</span>
          </mu-col>
          <mu-col>
            <span class="content-font">备注 {{item.remark}}</span>
          </mu-col>
          <mu-col>
            <a @click="deleteDeduction(index)" class="deleteBtn" href="javascript:;">
              <mu-icon color="#fff" value="delete"></mu-icon>
            </a>
          </mu-col>
        </mu-row>
      </div>
    </div>
  </div>
</template>
<style scoped>
.deleteBtn {
    display: block;
    width: 25px;
    height: 25px;
    background-color: #fb3942;
    border-radius: 50%;
    text-align: center;
    line-height: 25px;
    display: none;
    position: absolute;
}

.deleteBtn .material-icons {
    line-height: 25px;
    font-size: 18px;
}

.list:hover .deleteBtn {
    display: block;
}
</style>
<script>
import MuIcon from 'muse-ui/src/Icon/Icon'
export default {
  components: { MuIcon },
  props: ['fields'],
  watch: {
    fields: {
      handler: function (val, oldVal) {
        if (val.deductions && val.deductions.length) {
          this.deductions = JSON.parse(val.deductions)
          this.deductionEmpty = false
        }
        this.$emit('getChildFields', val)
      },
      deep: true,
    },
    deductions: {
      handler: function (val, oldVal) {
        this.fields.deductions = JSON.stringify(val)
      },
      deep: true,
    },
  },
  data() {
    return {
      deductionDialog: false,
      deduction: {
        val: 0,
        remark: '',
      },
      deductions: [],
      deductionEmpty: true,
    }
  },
  methods: {
    openDeduction() {
      this.deductionDialog = true
    },
    closeDeduction() {
      this.deductionDialog = false
    },
    deductionData() {
      this.deductions.push(this.deduction)
      this.deduction = {
        val: 0,
        remark: '',
      }
      this.deductionDialog = false
      this.deductionEmpty = false
    },
    deleteDeduction(index) {
      this.deductions.splice(index, 1)
      if (this.deductions.length == 0) {
        this.deductionEmpty = true
      }
    },
  },
}
</script>
