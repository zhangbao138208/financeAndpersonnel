<template>
  <div>
    <Card>
      <tables
        :border="false"
        :columns="stores.wageInfo.columns"
        :row-class-name="rowClsRender"
        :totalCount="stores.wageInfo.query.totalCount"
        @on-page-change="handlePageChanged"
        @on-page-size-change="handlePageSizeChanged"
        @on-refresh="handleRefresh"
        @on-select="handleSelect"
        @on-selection-change="handleSelectionChange"
        editable
        ref="tables"
        search-place="top"
        searchable
        size="small"
        v-model="stores.wageInfo.data"
      >
        <div slot="search">
          <section class="dnc-toolbar-wrap">
            <Row :gutter="16">
              <Col span="16">
                <Form @submit.native.prevent inline>
                  <FormItem>
                    <DatePicker
                      placeholder="开始时间"
                      style="width: 160px"
                      type="date"
                      v-model="stores.wageInfo.query.start"
                    ></DatePicker>
                    <DatePicker
                      placeholder="结束时间"
                      style="width: 160px"
                      type="date"
                      v-model="stores.wageInfo.query.end"
                    ></DatePicker>
                    <!-- </FormItem>
                    <FormItem>-->
                  </FormItem>
                  <FormItem>
                    <Input
                      :clearable="true"
                      @on-search="handleSearchWageInfo()"
                      placeholder="输入关键字搜索..."
                      search
                      type="text"
                      v-model="stores.wageInfo.query.kw"
                    >
                    </Input>
                  </FormItem>
                </Form>
              </Col>
              <Col class="dnc-toolbar-btns" span="8">
                <ButtonGroup class="mr3">
                  <!-- <Button
                    @click="ExportWithParms"
                    class="txt-success"
                    icon="md-cloud-done"
                    title="导出"
                  ></Button>
                  -->
                </ButtonGroup>
              </Col>
            </Row>
          </section>
        </div>
      </tables>
    </Card>
  </div>
</template>
<style scoped>
.demo-split {
  height: 300px;

  border: 1px solid #dcdee2;
}
.demo-split-pane {
  height: 150px;
  overflow-y: auto;
  padding: 10px;
}
</style>
<script>
import Tables from '_c/tables'
import Moment from 'moment'

import { getLogList } from '@/api/loger'
export default {
  name: 'error_logger_page',
  components: {
    Tables,
  },
  data() {
    return {
      exportModel: {
        realName: '',
        start: '',
        end: '',
        position: '',
        department: '',
      },

      stores: {
        wageInfo: {
          query: {
            start: '',
            end: '',
            
            totalCount: 0,
            pageSize: 20,
            currentPage: 1,
            kw: '',
            isDeleted: 0,
            status: -1,
            sort: [
              {
                direct: 'DESC',
                field: 'CreatedOn',
              },
            ],
          },
          sources: {
            isDeletedSources: [
              { value: -1, text: '全部' },
              { value: 0, text: '正常' },
              { value: 1, text: '已删' },
            ],
            statusSources: [
              { value: -1, text: '全部' },
              { value: 0, text: '禁用' },
              { value: 1, text: '正常' },
            ],
            statusFormSources: [
              { value: 0, text: '禁用' },
              { value: 1, text: '正常' },
            ],
          },

          columns: [
            { title: '级别', key: 'levels', width: 150 },
           
            { title: '操作时间', key: 'operatingtime', width: 100, sortable: true },
            { title: '操作地址', key: 'operatingaddress', width: 100 },
            { title: '请求地址', key: 'requesturl', width: 200 },
            { title: '方法', key: 'action', width: 100},
            { title: '信息', key: 'message' },
            
          ],
          data: [],
        },
      },
      styles: {
        height: 'calc(100% - 55px)',
        overflow: 'auto',
        paddingBottom: '53px',
        wageInfo: 'static',
      },
    }
  },

  methods: {
    downloadDemoExcel() {
      window.location.href = 'wagedemo.xlsx'
    },

    //导出的方法
    exportExcel() {
      require.ensure([], () => {
        const { export_json_to_excel } = require('../../excel/Export2Excel')
        const tHeader = [
          '真实姓名',
          '部门',
          '职位',
          '开始日期',
          '结束日期',
          '应发工资',
          '基本工资',
          '工作天数',
          '加班工资',
          '加班天数',
          '绩效工资',
          '补贴',
          '补发工资',
          '提成',
          '奖金',
          '额外工资',
          '社保',
          '公积金',
          '个税',
          '额外扣除',
        ]
        // 上面设置Excel的表格第一行的标题
        const filterVal = [
          'realName',
          'departmentName',
          'positionName',
          'startDate',
          'endDate',
          'totalWage',
          'baseWage',
          'workDays',
          'otWage',
          'otDays',
          'performanceWage',
          'subsidy',
          'reissueWage',
          'commission',
          'bonus',
          'additions',
          'socialSecurity',
          'accumulationFund',
          'incomeTax',
          'deductions',
        ]
        // 上面的index、nickName、name是tableData里对象的属性
        const list = this.tableData //把data里的tableData存到list
        const data = this.formatJson(filterVal, list)
        export_json_to_excel(tHeader, data, '薪资列表')
      })
    },

    loadWageInfoList() {
      getLogList(this.stores.wageInfo.query).then((res) => {
        this.stores.wageInfo.data = res.data.data
        this.stores.wageInfo.query.totalCount = res.data.totalCount
      })
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadWageInfoList()
    },

    handleSearchWageInfo() {
      this.loadWageInfoList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.wageInfo.query.currentPage = page
      this.loadWageInfoList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.wageInfo.query.pageSize = pageSize
      this.loadWageInfoList()
    },
  },
  mounted() {
    this.loadWageInfoList()
  },
}
</script>
