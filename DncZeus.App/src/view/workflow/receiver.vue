<template>
  <div>
    <Card>
      <tables
        ref="tables"
        editable
        searchable
        :border="false"
        size="small"
        search-place="top"
        v-model="stores.receiver.data"
        :totalCount="stores.receiver.query.totalCount"
        :columns="stores.receiver.columns"
        @on-edit="handleEdit"
        @on-view="handleView"
        @on-select="handleSelect"
        @on-selection-change="handleSelectionChange"
        @on-refresh="handleRefresh"
        :row-class-name="rowClsRender"
        @on-page-change="handlePageChanged"
        @on-page-size-change="handlePageSizeChanged"
      >
        <div slot="search">
          <section class="dnc-toolbar-wrap">
            <Row :gutter="16">
              <Col span="16">
                <Form inline @submit.native.prevent>
                  <FormItem>
                    <Input
                      type="text"
                      search
                      :clearable="true"
                      v-model="stores.receiver.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchReceiver()"
                    >
                    </Input>
                  </FormItem>
                </Form>
              </Col>
              <Col span="8" class="dnc-toolbar-btns">
                <RadioGroup
                  @on-change="handleSearchReceiver()"
                  v-model="stores.receiver.query.status"
                  type="button"
                >
                  <Radio label="all">全部</Radio>
                  <Radio
                    v-for="(item, i) in rstatus"
                    :key="i"
                    :label="item.value"
                    >{{ item.name }}
                  </Radio>
                </RadioGroup>
              </Col>
            </Row>
          </section>
        </div>
      </tables>
    </Card>
    <Drawer
      :title="formTitle"
      v-model="formModel.opened"
      width="800"
      :mask-closable="false"
      :mask="true"
      :styles="styles"
    >
      <Form
        :model="formModel.fields"
        ref="formReceiver"
        :rules="formModel.rules"
        label-receiver="left"
      >
        <Row :gutter="8">
          <Col span="24">
            <FormItem label="标题" label-position="left">
              <Input
                :disabled="true"
                v-model="formModel.fields.workflowName"
                placeholder="请输入审批标题"
              />
            </FormItem>
          </Col>
          <!-- <Col span="12">
            <FormItem label="到期"  label-position="left">
              <Input
                :disabled="isView"
                v-model="formModel.fields.endDate"
                />
            </FormItem>
          </Col> -->
        </Row>

        <FormItem
          label="审批单"
          label-position="top"
          v-if="formModel.opened && !!this.formModel.fields.description"
        >
          <editor
            v-if="!isView"
            id="editor_id"
            height="500px"
            width="700px"
            :content="formModel.fields.description"
            pluginsPath="/kindeditor/plugins/"
            :loadStyleMode="false"
            :uploadJson="uploadJson"
            @on-content-change="onContentChange"
          ></editor>
          <div v-if="isView" v-html="formModel.fields.description"></div>
        </FormItem>

        <FormItem label="进度" label-position="top" v-if="isView">
          <Button type="success" shape="circle">起</Button>
           <Icon type="md-fastforward" color='#ccc'/><Icon type="md-fastforward" color='#ccc'/><Icon
              type="md-fastforward"
              color='#ccc'
            />
          <div :key="item" v-for="(item ,index) in formModel.fields.steps" style="display:inline-block">
            <Button type="primary" v-if="index+1<formModel.fields.currentStep">{{ item }}</Button>
             <Button type="default" v-if="index+1>=formModel.fields.currentStep">{{ item }}</Button>
            <Icon type="md-fastforward" color='#ccc'/><Icon type="md-fastforward" color='#ccc'/><Icon
              type="md-fastforward"
              color='#ccc'
            />
          </div>

          <Button type="error" shape="circle">止</Button>
        </FormItem>
        <FormItem label="时间" label-position="top" v-if="isView">
          {{ formModel.fields.dateSpan }}
        </FormItem>
        <FormItem label="发起人" label-position="top" v-if="isView">
          {{ formModel.fields.createUserName }}
        </FormItem>
        <FormItem label="状态" label-position="top" v-if="isView">
          <Button v-if="formModel.fields.status == '1'" type="primary">{{
            formModel.fields.statusName
          }}</Button>
          <Button v-if="formModel.fields.status == '2'" type="error">{{
            formModel.fields.statusName
          }}</Button>
          <Button v-if="formModel.fields.status == '0'" type="warning">{{
            formModel.fields.statusName
          }}</Button>
        </FormItem>
        <FormItem label="意见" label-position="top" v-if="isView">
          <Table
            width="650"
            border
            :columns="stores.notes.columns"
            :data="formModel.fields.notes"
          >
          </Table>
        </FormItem>
        <FormItem
          v-if="!isView"
          label="审批意见"
          label-position="top"
          prop="note"
        >
          <Input
            type="textarea"
            v-model="formModel.fields.note"
            :rows="4"
            placeholder="审批意见"
          />
        </FormItem>
        <Row :gutter="8" v-if="!isView">
          <Col span="12">
            <FormItem label="程度">
              <mu-select v-model="formModel.fields.listType" :disabled="true">
                <mu-option
                  :key="wtype.value"
                  :label="wtype.name"
                  :value="wtype.value"
                  avatar
                  v-for="wtype in wtypes"
                >
                  <mu-list-item-action avatar>
                    <mu-avatar :size="36" color="primary">{{
                      wtype.name.substring(0, 1)
                    }}</mu-avatar>
                  </mu-list-item-action>
                  <mu-list-item-content>
                    <mu-list-item-title>{{ wtype.name }}</mu-list-item-title>
                  </mu-list-item-content>
                </mu-option>
              </mu-select>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem label="到期" label-receiver="left">
              <DatePicker
                :disabled="true"
                format="yyyy-MM-dd"
                type="date"
                v-model="formModel.fields.endDate"
              ></DatePicker>
            </FormItem>
          </Col>
        </Row>

        <Row :gutter="8" v-if="!isView">
          <Col span="12">
            <FormItem label="下一步">
              <Button type="warning">{{
                formModel.fields.nextStepName
              }}</Button>
              <!-- <Input :disabled="true" v-model="" /> -->
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem
              label="审批人"
              v-if="
                formModel.fields.nextUsers &&
                !!formModel.fields.nextUsers.length
              "
            >
              <mu-select
                v-model="formModel.fields.approver"
                multiple
                v-if="steps[0].isCounterSign"
              >
                <mu-option
                  :key="next.user"
                  :label="next.userName"
                  :value="next.user"
                  avatar
                  v-for="next in formModel.fields.nextUsers"
                ></mu-option>
              </mu-select>

              <mu-select
                v-model="formModel.fields.approver"
                v-if="!steps[0].isCounterSign"
              >
                <mu-option
                  :key="next.user"
                  :label="next.userName"
                  :value="next.user"
                  avatar
                  v-for="next in formModel.fields.nextUsers"
                ></mu-option>
              </mu-select>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div class="demo-drawer-footer">
        <Button
          :disabled="isView"
          icon="md-checkmark-circle"
          type="primary"
          @click="handleSubmitAgree"
          >同 意</Button
        >
        <Button
          style="margin-left: 8px"
          :disabled="isView"
          icon="md-close-circle"
          type="error"
          @click="handleSubmitReject"
          >拒 绝</Button
        >
        <Button
          style="margin-left: 8px"
          icon="md-close"
          @click="formModel.opened = false"
          >取 消</Button
        >
      </div>
    </Drawer>
  </div>
</template>

<script>
import Tables from '_c/tables'
import { loadDictionarySimpleList } from '@/api/system/dictionary'
import { loadTemplateSimpleList } from '@/api/workflow/template'
import config from '@/config'
import Moment from 'moment'

import {
  getReceiverList,
  loadReceiver,
  editReceiver,
  viewReceiver,
} from '@/api/workflow/receiver'
import { loadStepSimpleList } from '@/api/workflow/step'
export default {
  name: 'workflow_receiver_page',
  components: {
    Tables,
  },
  data() {
    return {
      steps: [],
      isView: false,
      uploadJson:
        process.env.NODE_ENV === 'development'
          ? config.baseUrl.dev + 'editorupload'
          : config.baseUrl.pro + 'editorupload',
      wtypes: [],
      rstatus: [],
      searchStatus: '',
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建审批',
        mode: 'create',
        selection: [],
        fields: {
          nextUsers: [],
          nextStepName: '',
          id: '',
          workflowName: '',
          templateCode: '',
          //审批人
          approver: [],
          endDate: '',
          note: '',
          description: '',
          listType: '',
        },
        rules: {
          note: [
            {
              type: 'string',
              required: true,
              message: '请输入审批意见',
              min: 2,
            },
          ],
          type: [
            {
              type: 'string',
              required: true,
              message: '请选择紧急程度',
            },
          ],
          templateCode: [
            {
              type: 'string',
              required: true,
              message: '请选择模板',
              min: 2,
            },
          ],
        },
      },
      stores: {
        notes: {
          columns: [
            { title: '节点', key: 'nodeName' },
             {
              title: '状态',
              key: 'statusName',
              render: (h, params) => {
                let status = params.row.status
                let statusColor = 'orange'
                let statusText = params.row.statusName
                switch (status) {
                  case '-1':
                    statusColor = '#ccc'
                    break
                  case '1':
                    statusColor = 'green'
                    break
                  case '2':
                    statusColor = 'red'
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
                      'Button',
                      {
                        props: {
                          //type: "dot",
                          size: 'small',
                        },
                        style: {
                          backgroundColor: statusColor,
                          color: '#fff',
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
            {
              title: '发起时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'startDate',
              render: (h, params) => {
                return h(
                  'span',
                  Moment(params.row.startDate).format('YYYY-MM-DD HH:MM:SS')
                )
              },
            },
            { title: '职位', key: 'position' },
            { title: '部门', key: 'department' },
            { title: '名称', key: 'userName' },
            { title: '意见', key: 'opinion' },
            { title: '时间', key: 'nodeDate' },
          ],
        },
        receiver: {
          query: {
            totalCount: 0,
            pageSize: 20,
            currentPage: 1,
            kw: '',
            status: 'all',
            sort: [
              {
                direct: 'DESC',
                field: 'CreatedOn',
              },
            ],
          },
          sources: {},
          columns: [
            { type: 'selection', width: 50, key: 'handle' },
            { title: '标题', key: 'workflowName', width: 300 },

            {
              title: '程度',
              key: 'listTypeName',
              width: 150,
              render: (h, params) => {
                let color = 'green'
                switch (params.row.listType) {
                  case '1':
                  case 1:
                    color = 'orange'
                    break
                  case '2':
                  case 2:
                    color = 'red'
                    break
                  default:
                    break
                }
                return h(
                  'span',
                  {
                    style: {
                      color: color,
                    },
                  },
                  params.row.listTypeName
                )
              },
            },
            {
              title: '当前节点',
              key: 'stepName',
              width: 150,
              render: (h, params) => {
                let show = params.row.stepName
                if (!show) {
                  show = params.row.stepName
                }
                return h(
                  'span',

                  show
                )
              },
            },
            {
              title: '状态',
              key: 'statusName',
              width: 150,
              render: (h, params) => {
                let status = params.row.status
                let statusColor = 'orange'
                let statusText = params.row.statusName
                switch (status) {
                  case '-1':
                    statusColor = '#ccc'
                    break
                  case '1':
                    statusColor = 'green'
                    break
                  case '2':
                    statusColor = 'red'
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
                      'Button',
                      {
                        props: {
                          //type: "dot",
                          size: 'small',
                        },
                        style: {
                          backgroundColor: statusColor,
                          color: '#fff',
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
            {
              title: '发起时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'startDate',
              render: (h, params) => {
                return h(
                  'span',
                  Moment(params.row.startDate).format('YYYY-MM-DD HH:MM:SS')
                )
              },
            },
            {
              title: '审核时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'checkDate',
              render: (h, params) => {
                return h(
                  'span',
                  Moment(params.row.checkDate).format('YYYY-MM-DD HH:MM:SS') ==
                    '0001-01-01 00:01:00'
                    ? ''
                    : Moment(params.row.checkDate).format('YYYY-MM-DD HH:MM:SS')
                )
              },
            },
            {
              title: '操作',
              align: 'center',
              key: 'handle',
              //   width: 150,
              className: 'table-command-column',
              options: ['edit'],
              button: [
                (h, params, vm) => {
                  if (params.row.status == '0') {
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
                          '审核'
                        ),
                      ]
                    )
                  } else {
                    return ''
                  }
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
                          icon: 'md-eye',
                          type: 'warning',
                        },
                        on: {
                          click: () => {
                            vm.$emit('on-view', params)
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
                        '查看'
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
        receiver: 'static',
      },
    }
  },
  computed: {
    stepLength() {
      let ret = 1
      this.steps.forEach((_, i) => {
        if (_.code == this.formModel.fields.currentStepCode) {
          ret += i + 1
        }
      })
      console.log(ret)
      return ret
    },
    formTitle() {
      this.isView = false
      if (this.formModel.mode === 'create') {
        return '创建审批'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑审批'
      }
      if (this.formModel.mode === 'view') {
        this.isView = true
        return '查看审批'
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
    onContentChange(val) {
      this.formModel.fields.description = val
    },
    initData() {
      loadTemplateSimpleList().then((res) => {
        if (res.data.code === 200) {
          this.templates = res.data.data
        } else {
          this.$Message.warning(res.data.message)
        }
      })
      loadDictionarySimpleList('workflow_receiver_status').then((res) => {
        if (res.data.code === 200) {
          this.rstatus = res.data.data
        } else {
          this.$Message.warning(res.data.message)
        }
      })
      loadDictionarySimpleList('workflow_list_type').then((res) => {
        if (res.data.code === 200) {
          this.wtypes = res.data.data
        } else {
          this.$Message.warning(res.data.message)
        }
      })
    },
    loadReceiverList() {
      getReceiverList(this.stores.receiver.query).then((res) => {
        this.stores.receiver.data = res.data.data
        this.stores.receiver.query.totalCount = res.data.totalCount
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
    handleSwitchFormModeToView() {
      this.formModel.mode = 'view'
      this.handleOpenFormWindow()
    },
    handleEdit(params) {
      this.handleSwitchFormModeToEdit()
      this.handleResetFormReceiver()
      this.doLoadReceiver(params.row.id)
    },
    handleView(params) {
      this.handleSwitchFormModeToView()
      this.handleResetFormReceiver()
      this.doViewReceiver(params.row.id)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadReceiverList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormReceiver()
    },
    handleSubmitAgree() {
      let valid = this.validateReceiverForm()
      if (valid) {
        if (this.formModel.mode === 'edit') {
          //同意
          console.log(this.formModel.fields)
          if (!this.formModel.fields.approver) {
            this.$Message.error('请选择审批人')
            return
          }
          this.formModel.fields.status = '1'
          this.doEditReceiver()
        }
      }
    },
    handleSubmitReject() {
      let valid = this.validateReceiverForm()
      if (valid) {
        if (this.formModel.mode === 'edit') {
          //拒绝
          this.formModel.fields.status = '2'
          this.doEditReceiver()
        }
      }
    },
    handleResetFormReceiver() {
      this.$refs['formReceiver'].resetFields()
    },

    doEditReceiver() {
      editReceiver(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadReceiverList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    validateReceiverForm() {
      let _valid = false
      this.$refs['formReceiver'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadReceiver(code) {
      loadReceiver({ code: code }).then((res) => {
        this.formModel.fields = res.data.data
        console.log(this.formModel.fields)
      })
    },

    doViewReceiver(code) {
      viewReceiver({ code: code }).then((res) => {
        this.formModel.fields = res.data.data
        console.log(this.formModel.fields)
      })
    },

    handleSearchReceiver() {
      this.loadReceiverList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.receiver.query.currentPage = page
      this.loadReceiverList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.receiver.query.pageSize = pageSize
      this.loadReceiverList()
    },
  },
  mounted() {
    this.initData()
    this.loadReceiverList()
  },
}
</script>
