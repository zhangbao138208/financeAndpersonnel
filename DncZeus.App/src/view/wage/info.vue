<template>
  <div>
    <Card>
      <tables
        :border="false"
        :columns="stores.wageInfo.columns"
        :row-class-name="rowClsRender"
        :totalCount="stores.wageInfo.query.totalCount"
        @on-delete="handleDelete"
        @on-edit="handleEdit"
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
                      style="width:160px;"
                      type="date"
                      v-model="stores.wageInfo.query.start"
                    ></DatePicker>
                    <DatePicker
                      placeholder="结束时间"
                      style="width:160px;"
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
                      <Select
                        @on-change="handleSearchWageInfo"
                        placeholder="职位"
                        slot="prepend"
                        style="width:100px;"
                        v-model="stores.wageInfo.query.position"
                      >
                        <Option
                          :key="item.code"
                          :value="item.code"
                          v-for="item in ePositions"
                        >{{item.name}}</Option>
                      </Select>
                      <Select
                        @on-change="handleSearchWageInfo"
                        placeholder="部门"
                        slot="prepend"
                        style="width:80px;"
                        v-model="stores.wageInfo.query.department"
                      >
                        <Option
                          :key="item.code"
                          :value="item.code"
                          v-for="item in eDepartments"
                        >{{item.name}}</Option>
                      </Select>
                      <Select
                        @on-change="handleSearchWageInfo"
                        placeholder="删除状态"
                        slot="prepend"
                        style="width:60px;"
                        v-model="stores.wageInfo.query.isDeleted"
                      >
                        <Option
                          :key="item.value"
                          :value="item.value"
                          v-for="item in stores.wageInfo.sources.isDeletedSources"
                        >{{item.text}}</Option>
                      </Select>
                      <Select
                        @on-change="handleSearchWageInfo"
                        placeholder="工资状态"
                        slot="prepend"
                        style="width:60px;"
                        v-model="stores.wageInfo.query.status"
                      >
                        <Option
                          :key="item.value"
                          :value="item.value"
                          v-for="item in stores.wageInfo.sources.statusSources"
                        >{{item.text}}</Option>
                      </Select>
                    </Input>
                  </FormItem>
                </Form>
              </Col>
              <Col class="dnc-toolbar-btns" span="8">
                <ButtonGroup class="mr3">
                  <Button
                    @click="handleBatchCommand('delete')"
                    class="txt-danger"
                    icon="md-trash"
                    title="删除"
                  ></Button>
                  <Button
                    @click="handleBatchCommand('recover')"
                    class="txt-success"
                    icon="md-redo"
                    title="恢复"
                  ></Button>
                  <Button
                    @click="handleBatchCommand('forbidden')"
                    class="txt-danger"
                    icon="md-hand"
                    title="禁用"
                  ></Button>
                  <Button
                    @click="handleBatchCommand('normal')"
                    class="txt-success"
                    icon="md-checkmark"
                    title="启用"
                  ></Button>
                  <Button @click="handleRefresh" icon="md-refresh" title="刷新"></Button>
                  <Button
                    @click="downloadDemoExcel"
                    class="txt-success"
                    icon="md-arrow-down"
                    title="【导入模板】下载"
                  ></Button>
                  <Button
                    @click="ExportWithParms"
                    class="txt-success"
                    icon="md-cloud-done"
                    title="导出"
                  ></Button>
                  <Button
                    @click="handleImport"
                    class="txt-success"
                    icon="md-cloud-upload"
                    title="导入"
                  ></Button>
                  <input @change="getFile" ref="fileInput" style="display: none" type="file" />
                </ButtonGroup>
                <Button
                  @click="handleShowCreateWindow"
                  icon="md-create"
                  title="新增工资"
                  type="primary"
                >新增工资</Button>
              </Col>
            </Row>
          </section>
        </div>
      </tables>
    </Card>
    <Modal
      :styles="{top: '200px'}"
      @on-cancel="closeInfo"
      @on-ok="workInfo"
      title="薪资导出"
      v-model="infoDialog"
      
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
              v-model="exportModel.start"
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
            v-model="exportModel.end"
          ></DatePicker>
        </mu-col>
        <mu-col span="6">
          <mu-select label="部门" v-model="exportModel.department">
            <mu-option
              :key="department.code"
              :label="department.name"
              :value="department.code"
              avatar
              v-for="department,index in eDepartments"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{department.name.substring(0, 1)}}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{department.name}}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </mu-col>
        <mu-col span="6">
          <mu-select label="职位" v-model="exportModel.position">
            <mu-option
              :key="position.code"
              :label="position.name"
              :value="position.code"
              avatar
              v-for="position,index in ePositions"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{position.name.substring(0, 1)}}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{position.name}}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </mu-col>
      </mu-row>
      <mu-row gutter>
        <mu-col>
          <mu-text-field
            :rows="1"
            :rowsMax="5"
            fullWidth
            help-text="姓名"
            multiLine
            v-model="exportModel.realName"
          ></mu-text-field>
        </mu-col>
      </mu-row>
    </Modal>
    <Drawer
      :mask-closable="false"
      :mask="true"
      :styles="styles"
      :title="formTitle"
      v-model="formModel.opened"
      width="480"
    >
      <Form
        :label-width="80"
        :model="formModel.fields"
        :rules="formModel.rules"
        label-position="right"
        ref="formWageInfo"
      >
        <div class="ambition">
          <FormItem label="部门" prop="departmentCode">
            <mu-select v-model="formModel.fields.departmentCode">
              <mu-option
                :key="department.code"
                :label="department.name"
                :value="department.code"
                avatar
                v-for="department,index in departments"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{department.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{department.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
          </FormItem>
          <FormItem label="职位" prop="departmentCode">
            <mu-select v-model="formModel.fields.positionCode">
              <mu-option
                :key="position.code"
                :label="position.name"
                :value="position.code"
                avatar
                v-for="position,index in positions"
              >
                <mu-list-item-action avatar>
                  <mu-avatar :size="36" color="primary">{{position.name.substring(0, 1)}}</mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>{{position.name}}</mu-list-item-title>
                </mu-list-item-content>
              </mu-option>
            </mu-select>
          </FormItem>
        </div>
        <FormItem label="真实姓名" label-wageInfo="left" prop="realName">
          <Input placeholder="请输入真实姓名" v-model="formModel.fields.realName" />
        </FormItem>
        <FormItem label="日期">
          <Row>
            <Col span="11">
              <FormItem prop="startDate">
                <DatePicker placeholder="选择开始日期" type="date" v-model="formModel.fields.startDate"></DatePicker>
              </FormItem>
            </Col>
            <Col span="2" style="text-align: center">-</Col>
            <Col span="11">
              <FormItem prop="endDate">
                <DatePicker placeholder="选择结束日期" type="date" v-model="formModel.fields.endDate"></DatePicker>
              </FormItem>
            </Col>
          </Row>
        </FormItem>

        <FormItem label="工资状态" label-wageInfo="left">
          <i-switch :false-value="0" :true-value="1" size="large" v-model="formModel.fields.status">
            <span slot="open">正常</span>
            <span slot="close">禁用</span>
          </i-switch>
        </FormItem>

        <div class="demo-split">
          <Split mode="vertical">
            <div class="demo-split-pane" slot="top">
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="基本工资">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.baseWage"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12">
                    <FormItem label="工作天数">
                      <InputNumber :min="0" v-model="formModel.fields.workDays"></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
              </FormItem>
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="加班工资">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.otWage"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12">
                    <FormItem label="加班天数">
                      <InputNumber :min="0" v-model="formModel.fields.otDays"></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
              </FormItem>
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="绩效工资">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.performanceWage"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12">
                    <FormItem label="补发工资">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.reissueWage"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
              </FormItem>
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="提成">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.commission"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12">
                    <FormItem label="奖金">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.bonus"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
              </FormItem>
              <FormItem label="额外工资">
                <Addition
                  :fields="formModel.fields"
                  @getChildFields="_getChildFields"
                  v-if="formModel.opened"
                ></Addition>
              </FormItem>
            </div>
            <div class="demo-split-pane" slot="bottom">
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="社保">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.socialSecurity"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12">
                    <FormItem label="公积金">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.accumulationFund"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                </Row>
              </FormItem>
              <FormItem>
                <Row>
                  <Col span="12">
                    <FormItem label="个税">
                      <InputNumber
                        :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                        :min="0"
                        :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                        v-model="formModel.fields.incomeTax"
                      ></InputNumber>
                    </FormItem>
                  </Col>
                  <Col span="12"></Col>
                </Row>
              </FormItem>
              <FormItem label="额外扣除">
                <Deduction
                  :fields="formModel.fields"
                  @getChildFields="_getChildFields"
                  v-if="formModel.opened"
                ></Deduction>
              </FormItem>
            </div>
          </Split>
        </div>

        <FormItem label="应发工资" label-wageInfo="left">
          <InputNumber
            :formatter="value => `￥ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :min="0"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
            readonly
            v-model="formModel.fields.totalWage"
          ></InputNumber>
        </FormItem>
      </Form>
      <div class="demo-drawer-footer">
        <Button @click="handleSubmitDePartment" icon="md-checkmark-circle" type="primary">保 存</Button>
        <Button @click="formModel.opened = false" icon="md-close" style="margin-left: 8px">取 消</Button>
      </div>
    </Drawer>
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
import Addition from './drawer/Addition'
import Deduction from './drawer/Deduction'

import { loadPositionSimpleList } from '@/api/user/position'
import { loadDepartmentSimpleList } from '@/api/user/department'
import moment from 'moment'

import {
  getWageInfoList,
  createWageInfo,
  loadWageInfo,
  editWageInfo,
  deleteWageInfo,
  batchCommand,
  importWageInfo,
  exportWageInfo,
} from '@/api/wage/wageInfo'
export default {
  name: 'wage_info_page',
  components: {
    Tables,
    Addition,
    Deduction,
  },
  data() {
    return {
      infoDialog: false,
      tableData: [],
      positions: [],
      departments: [],
      ePositions: [],
      eDepartments: [],
      exportModel: {
        realName: '',
        start: '',
        end: '',
        position: '',
        department: '',
      },
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建工资',
        mode: 'create',
        selection: [],
        fields: {
          code: '',
          realName: '',
          departmentCode: '',
          positionCode: '',
          baseWage: 0,
          workDays: 0,
          otWage: 0,
          otDays: 0,
          performanceWage: 0,
          reissueWage: 0,
          commission: 0,
          bonus: 0,
          subsidy: 0,
          socialSecurity: 0,
          accumulationFund: 0,
          incomeTax: 0,
          deductions: '',
          additions: '',
          totalWage: 0,
          isLocked: 0,
          status: 1,
          isDeleted: 0,
          description: '',
        },
        rules: {
          realName: [
            {
              type: 'string',
              required: true,
              message: '请输入真实姓名',
              min: 2,
            },
          ],
          departmentCode: [
            {
              type: 'string',
              required: true,
              message: '请选择部门',
              min: 2,
            },
          ],
          positionCode: [
            {
              type: 'string',
              required: true,
              message: '请选择部门',
              min: 2,
            },
          ],
          startDate: [
            {
              type: 'date',
              required: true,
              message: '请选择开始日期',
              min: 2,
            },
          ],
          endDate: [
            {
              type: 'date',
              required: true,
              message: '请选择结束日期',
              min: 2,
            },
          ],
        },
      },
      stores: {
        wageInfo: {
          query: {
            start: '',
            end: '',
            position: '-1',
            department: '-1',
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
            { type: 'selection', width: 50, key: 'handle' },
            { title: '真实姓名', key: 'realName', width: 150 },
            {
              title: '状态',
              key: 'status',
              align: 'center',
              width: 120,
              render: (h, params) => {
                let status = params.row.status
                let statusColor = 'success'
                let statusText = '正常'
                switch (status) {
                  case 0:
                    statusText = '禁用'
                    statusColor = 'default'
                    break
                }
                return h(
                  'Tooltip',
                  {
                    props: {
                      placement: 'top',
                      transfer: true,
                      delay: 500,
                    },
                  },
                  [
                    //这个中括号表示是Tooltip标签的子标签
                    h(
                      'Tag',
                      {
                        props: {
                          //type: "dot",
                          color: statusColor,
                        },
                      },
                      statusText
                    ), //表格列显示文字
                    h(
                      'p',
                      {
                        slot: 'content',
                        style: {
                          whiteSpace: 'normal',
                        },
                      },
                      statusText //整个的信息即气泡内文字
                    ),
                  ]
                )
              },
            },
            { title: '应发工资', key: 'totalWage', width: 100, sortable: true },
            { title: '基本工资', key: 'baseWage', width: 100, sortable: true },
            { title: '工作天数', key: 'workDays', width: 100, sortable: true },
            { title: '加班工资', key: 'otWage', width: 100, sortable: true },
            { title: '加班天数', key: 'otDays', width: 100, sortable: true },
            {
              title: '绩效工资',
              key: 'performanceWage',
              width: 100,
              sortable: true,
            },
            {
              title: '补发工资',
              key: 'commission',
              width: 100,
              sortable: true,
            },
            { title: '提成', key: 'bonus', width: 100, sortable: true },
            { title: '奖金', key: 'subsidy', width: 100, sortable: true },
            {
              title: '社保',
              key: 'socialSecurity',
              width: 100,
              sortable: true,
            },
            {
              title: '公积金',
              key: 'accumulationFund',
              width: 100,
              sortable: true,
            },
            { title: '个税', key: 'incomeTax', width: 100, sortable: true },

            {
              title: '创建时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'createdOn',
              render: (h, params) => {
                return h(
                  'span',
                  Moment(params.row.createdOn).format('YYYY-MM-DD')
                )
              },
            },
            { title: '创建者', key: 'createdByUserName', width: 150 },
            {
              title: '操作',
              align: 'center',
              key: 'handle',
              width: 150,
              className: 'table-command-column',
              fixed: 'right',
              options: ['edit'],
              button: [
                (h, params, vm) => {
                  return h(
                    'Poptip',
                    {
                      props: {
                        confirm: true,
                        title: '你确定要删除吗?',
                      },
                      on: {
                        'on-ok': () => {
                          vm.$emit('on-delete', params)
                        },
                      },
                    },
                    [
                      h(
                        'Tooltip',
                        {
                          props: {
                            placement: 'left',
                            transfer: true,
                            delay: 1000,
                          },
                        },
                        [
                          h('Button', {
                            props: {
                              shape: 'circle',
                              size: 'small',
                              icon: 'md-trash',
                              type: 'error',
                            },
                          }),
                          h(
                            'p',
                            {
                              slot: 'content',
                              style: {
                                whiteSpace: 'normal',
                              },
                            },
                            '删除'
                          ),
                        ]
                      ),
                    ]
                  )
                },
                (h, params, vm) => {
                  return h(
                    'Tooltip',
                    {
                      props: {
                        placement: 'left',
                        transfer: true,
                        delay: 1000,
                      },
                    },
                    [
                      h('Button', {
                        props: {
                          shape: 'circle',
                          size: 'small',
                          icon: 'md-create',
                          type: 'primary',
                        },
                        on: {
                          click: () => {
                            vm.$emit('on-edit', params)
                            vm.$emit('input', params.tableData)
                          },
                        },
                      }),
                      h(
                        'p',
                        {
                          slot: 'content',
                          style: {
                            whiteSpace: 'normal',
                          },
                        },
                        '编辑'
                      ),
                    ]
                  )
                },
              ],
            },
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
  watch: {
    totalWage: function (newVal, oldVal) {
      this.formModel.fields.totalWage = this.totalWage
    },
  },
  computed: {
    totalWage() {
      let ret =
        parseFloat(this.formModel.fields.baseWage) +
        parseFloat(this.formModel.fields.otWage) +
        parseFloat(this.formModel.fields.performanceWage) +
        parseFloat(this.formModel.fields.reissueWage) +
        parseFloat(this.formModel.fields.commission) +
        parseFloat(this.formModel.fields.bonus) +
        parseFloat(this.formModel.fields.subsidy)

      if (!!this.formModel.fields.additions) {
        JSON.parse(this.formModel.fields.additions).forEach((_) => {
          ret += parseFloat(_.val)
        })
      }

      //扣除
      ret =
        ret -
        parseFloat(this.formModel.fields.socialSecurity) -
        parseFloat(this.formModel.fields.accumulationFund) -
        parseFloat(this.formModel.fields.incomeTax)

      if (!!this.formModel.fields.deductions) {
        console.log(this.formModel.fields.deductions)
        JSON.parse(this.formModel.fields.deductions).forEach((_) => {
          ret -= parseFloat(_.val)
        })
      }

      return ret
    },
    formTitle() {
      if (this.formModel.mode === 'create') {
        return '创建工资'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑工资'
      }
      return ''
    },
    selectedRows() {
      return this.formModel.selection
    },
    selectedRowsId() {
      return this.formModel.selection.map((x) => x.code)
    },
  },
  methods: {
    closeInfo() {
      this.infoDialog = false
    },
    workInfo() {
      this.handleExport()
      this.exportModel = {
        realName: '',
        start: '',
        end: '',
        position: '',
        department: '',
      }
      this.infoDialog = false
    },
    ExportWithParms() {
      this.infoDialog = true
    },
    downloadDemoExcel() {
      window.location.href = 'wagedemo.xlsx'
    },
    handleExport() {
      let data = this.exportModel
      exportWageInfo(data).then((res) => {
        if (res.data.code == 200) {
          this.tableData = res.data.data
          this.exportExcel()
          this.$Message.success(res.data.message)
        } else {
          this.$Message.error(res.data.message)
        }
      })
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

    formatJson(filterVal, jsonData) {
      return jsonData.map((v) => filterVal.map((j) => v[j]))
    },
    handleImport() {
      this.$refs.fileInput.click()
    },
    getFile(event) {
      const files = event.target.files
      let file = files[0]
      this.handleUpload(file)
    },
    handleUpload(file) {
      let formData = new FormData()
      formData.append('file', file) //formData.append('data', data);   // 上传文件的同时， 也可以上传其他数据
      importWageInfo(formData).then((res) => {
        if (res.data.code == 200) {
          this.$Message.success(res.data.message)
          this.loadWageInfoList()
        } else {
          this.$Message.error(res.data.message)
        }
      })
      return false
    },
    _getChildFields(fields) {
      this.formModel.fields = fields
    },
    InitData() {
      loadPositionSimpleList().then((res) => {
        this.positions = res.data.data
        this.ePositions = [{ code: '-1', name: '全部' }, ...this.positions]
      })
      loadDepartmentSimpleList().then((res) => {
        this.departments = res.data.data
        this.eDepartments = [{ code: '-1', name: '全部' }, ...this.departments]
      })
    },
    loadWageInfoList() {
      getWageInfoList(this.stores.wageInfo.query).then((res) => {
        this.stores.wageInfo.data = res.data.data
        this.stores.wageInfo.query.totalCount = res.data.totalCount
      })
    },
    handleOpenFormWindow() {
      this.formModel.opened = true
    },
    handleCloseFormWindow() {
      this.formModel.opened = false
    },
    handleSwitchFormModeToCreate() {
      this.formModel.mode = 'create'
    },
    handleSwitchFormModeToEdit() {
      this.formModel.mode = 'edit'
      this.handleOpenFormWindow()
    },
    handleEdit(params) {
      this.handleSwitchFormModeToEdit()
      this.handleResetFormWageInfo()
      this.doLoadWageInfo(params.row.code)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadWageInfoList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormWageInfo()
    },
    handleSubmitDePartment() {
      let valid = this.validateWageInfoForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateWageInfo()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditWageInfo()
        }
      }
    },
    handleResetFormWageInfo() {
      this.$refs['formWageInfo'].resetFields()
    },
    doCreateWageInfo() {
      createWageInfo(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWageInfoList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    doEditWageInfo() {
      editWageInfo(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWageInfoList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    validateWageInfoForm() {
      let _valid = false
      this.$refs['formWageInfo'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadWageInfo(code) {
      loadWageInfo({ code: code }).then((res) => {
        this.formModel.fields = res.data.data
      })
    },
    handleDelete(params) {
      this.doDelete(params.row.code)
    },
    doDelete(ids) {
      if (!ids) {
        this.$Message.warning('请选择至少一条数据')
        return
      }
      deleteWageInfo(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWageInfoList()
        } else {
          this.$Message.warning(res.data.message)
        }
      })
    },
    handleBatchCommand(command) {
      if (!this.selectedRowsId || this.selectedRowsId.length <= 0) {
        this.$Message.warning('请选择至少一条数据')
        return
      }
      this.$Modal.confirm({
        title: '操作提示',
        content:
          '<p>确定要执行当前 [' +
          this.commands[command].title +
          '] 操作吗?</p>',
        loading: true,
        onOk: () => {
          this.doBatchCommand(command)
        },
      })
    },
    doBatchCommand(command) {
      batchCommand({
        command: command,
        ids: this.selectedRowsId.join(','),
      }).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWageInfoList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
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
    this.InitData()
    this.loadWageInfoList()
  },
}
</script>
