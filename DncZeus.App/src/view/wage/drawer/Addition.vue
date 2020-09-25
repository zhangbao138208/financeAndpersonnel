<template>
  <div class="addition">
    <mu-button @click="openAddition" class="demo-float-button additionBtn" icon>
      <mu-icon color="#1d3653" value="add"></mu-icon>
    </mu-button>
    <mu-dialog :open="additionDialog" @close="closeAddition" title="额外工资" width="500">
      <mu-row gutter>
        <mu-col span="6">
          <InputNumber
            :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :min="0"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
            v-model="addition.val"
          ></InputNumber>
        </mu-col>
        <mu-col span="6">
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="备注"
            multiLine
            v-model="addition.remark"
          ></mu-text-field>
        </mu-col>
      </mu-row>
      <mu-button @click="closeAddition" flat label="关闭" primary>关闭</mu-button>
      <mu-button @click="additionData" flat label="确定" primary>确定</mu-button>
    </mu-dialog>

    <div class="addition-content">
      <div class="empty" v-if="additionEmpty">请添加额外工资</div>
      <div class="list" v-else v-for="(item,index) in additions">
        <mu-row gutter>
          <mu-col>
            <span class="content-font">金额 ￥{{item.val}}</span>
          </mu-col>
          <mu-col>
            <span class="content-font">备注 {{item.remark}}</span>
          </mu-col>
          <mu-col>
            <a @click="deleteAddition(index)" class="deleteBtn" href="javascript:;">
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
        if (val.additions && val.additions.length) {
          this.additions = JSON.parse(val.additions)
          this.additionEmpty = false
        }
        this.$emit('getSubChildFields', val)
      },
      deep: true,
    },
    additions: {
      handler: function (val, oldVal) {
        this.fields.additions = JSON.stringify(val)
      },
      deep: true,
    },
  },
  data() {
    return {
      additionDialog: false,
      addition: {
        val: 0,
        remark: '',
      },
      additions: [],
      additionEmpty: true,
    }
  },
  methods: {
    openAddition() {
      this.additionDialog = true
    },
    closeAddition() {
      this.additionDialog = false
    },
    additionData() {
      this.additions.push(this.addition)
      this.addition = {
        val: 0,
        remark: '',
      }
      this.additionDialog = false
      this.additionEmpty = false
    },
    deleteAddition(index) {
      this.additions.splice(index, 1)
      if (this.additions.length == 0) {
        this.additionEmpty = true
      }
    },
  },
}
</script>
