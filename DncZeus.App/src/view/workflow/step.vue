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
        v-model="stores.step.data"
        :totalCount="stores.step.query.totalCount"
        :columns="stores.step.columns"
        @on-delete="handleDelete"
        @on-edit="handleEdit"
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
                      v-model="stores.step.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchStep()"
                    >
                      <Select
                        slot="prepend"
                        v-model="stores.step.query.isDeleted"
                        @on-change="handleSearchStep"
                        placeholder="删除状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.step.sources.isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.step.query.status"
                        @on-change="handleSearchStep"
                        placeholder="步骤状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.step.sources.statusSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                    </Input>
                  </FormItem>
                </Form>
              </Col>
              <Col span="8" class="dnc-toolbar-btns">
                <ButtonGroup class="mr3">
                  <Button
                    class="txt-danger"
                    icon="md-trash"
                    title="删除"
                    @click="handleBatchCommand('delete')"
                  ></Button>
                  <Button
                    class="txt-success"
                    icon="md-redo"
                    title="恢复"
                    @click="handleBatchCommand('recover')"
                  ></Button>
                  <Button
                    class="txt-danger"
                    icon="md-hand"
                    title="禁用"
                    @click="handleBatchCommand('forbidden')"
                  ></Button>
                  <Button
                    class="txt-success"
                    icon="md-checkmark"
                    title="启用"
                    @click="handleBatchCommand('normal')"
                  ></Button>
                  <Button
                    icon="md-refresh"
                    title="刷新"
                    @click="handleRefresh"
                  ></Button>
                </ButtonGroup>
                <Button
                  icon="md-create"
                  type="primary"
                  @click="handleShowCreateWindow"
                  title="新增步骤"
                  >新增步骤</Button
                >
              </Col>
            </Row>
          </section>
        </div>
      </tables>
    </Card>

    <Drawer
      :title="formTitle"
      v-model="formModel.opened"
      width="400"
      :mask-closable="false"
      :mask="true"
      :styles="styles"
    >
      <Form
        :model="formModel.fields"
        ref="formStep"
        :rules="formModel.rules"
        label-position="left"
      >
        <FormItem label="步骤名称" prop="title" label-position="left">
          <Input
            v-model="formModel.fields.title"
            placeholder="请输入步骤名称"
          />
        </FormItem>
        <FormItem label="模板" label-position="left" prop="templateCode">
          <Select v-model="formModel.fields.templateCode" style="width: 260px">
            <Option
              v-for="item in templates"
              :value="item.code"
              :key="item.code"
              >{{ item.name }}</Option
            >
          </Select>
        </FormItem>
        <FormItem label="审批人" label-position="left" prop="userList">
          <Select
            v-model="formModel.fields.userList"
            multiple
            style="width: 260px"
          >
            <Option
              v-for="item in userList"
              :value="item.code"
              :key="item.code"
              >{{ item.name }}</Option
            >
          </Select>
        </FormItem>
        <FormItem label="排序" label-position="left">
          <InputNumber
            v-model="formModel.fields.sortID"
            placeholder="请输入排序"
          />
        </FormItem>
        <FormItem label="状态" label-position="left">
          <i-switch
            size="large"
            v-model="formModel.fields.status"
            :true-value="1"
            :false-value="0"
          >
            <span slot="open">正常</span>
            <span slot="close">禁用</span>
          </i-switch>
        </FormItem>
        <FormItem label="签状" label-position="left">
          <i-switch
            size="large"
            v-model="formModel.fields.isCounterSign"
            :true-value="1"
            :false-value="0"
          >
            <span slot="open">会签</span>
            <span slot="close">或签</span>
          </i-switch>
        </FormItem>
      </Form>
      <div class="demo-drawer-footer">
        <Button
          icon="md-checkmark-circle"
          type="primary"
          @click="handleSubmitDePartment"
          >保 存</Button
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

import {
  getStepList,
  createStep,
  loadStep,
  editStep,
  deleteStep,
  batchCommand,
} from '@/api/workflow/step'
import { loadUserSimpleList } from '@/api/rbac/user'
import { loadTemplateSimpleList } from '@/api/workflow/template'
export default {
  name: 'workflow_step_page',
  components: {
    Tables,
  },
  data() {
    return {
     
      userList: [],
      templates: [],
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建步骤',
        mode: 'create',
        selection: [],
        fields: {
          title: '',
          code: '',
          userList: [],
          templateCode: '',
          sortID: 0,
          workTime: '',
          restDays: [],
          isLocked: 0,
          status: 1,
          isCounterSign: 0,
          isDeleted: 0,
          description: '',
        },
        rules: {
          title: [
            {
              type: 'string',
              required: true,
              message: '请输入真实姓名',
              min: 2,
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
          userList: [
            {
              required: true,
              message: '请选择审批人',
            },
          ],
        },
      },
      stores: {
        step: {
          query: {
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
            { title: '步骤名称', key: 'title', width: 200 },
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
            { title: '审批人', key: 'userListName', width: 200 },
            { title: '排序', key: 'sortID', width: 100 },
            {
              title: '签状',
              key: 'isCounterSign',
              align: 'center',
              width: 120,
              render: (h, params) => {
                let isCounterSign = params.row.isCounterSign
                let isCounterSignColor = 'success'
                let statusText = '会签'
                switch (isCounterSign) {
                  case 0:
                  case '0':
                  case false:
                    statusText = '或签'
                    isCounterSignColor = 'default'
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
                          color: isCounterSignColor,
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
              title: '创建时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'createdOn',
            },
            { title: '创建者', key: 'createdByUserName' },
            {
              title: '操作',
              align: 'center',
              key: 'handle',
              width: 150,
              className: 'table-command-column',
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
        position: 'static',
      },
    }
  },
  computed: {
    formTitle() {
      if (this.formModel.mode === 'create') {
        return '创建步骤'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑步骤'
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
    initData() {
      loadUserSimpleList().then((res) => {
        if (res.data.code === 200) {
          this.userList = res.data.data
        } else {
          this.$Message.warning(res.data.message)
        }
      })
      loadTemplateSimpleList().then((res) => {
        if (res.data.code === 200) {
          this.templates = res.data.data
        } else {
          this.$Message.warning(res.data.message)
        }
      })
    },
    loadStepList() {
      getStepList(this.stores.step.query).then((res) => {
        this.stores.step.data = res.data.data
        this.stores.step.query.totalCount = res.data.totalCount
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
      this.handleResetFormStep()
      this.doLoadStep(params.row.code)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadStepList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormStep()
    },
    handleSubmitDePartment() {
      let valid = this.validateStepForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateStep()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditStep()
        }
      }
    },
    handleResetFormStep() {
      this.$refs['formStep'].resetFields()
    },
    doCreateStep() {
      createStep(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadStepList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    doEditStep() {
      editStep(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadStepList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    validateStepForm() {
      let _valid = false
      this.$refs['formStep'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadStep(code) {
      loadStep({ code: code }).then((res) => {
        this.formModel.fields = res.data.data
        console.log(this.formModel.fields)
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
      deleteStep(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadStepList()
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
          this.loadStepList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
    },
    handleSearchStep() {
      this.loadStepList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.step.query.currentPage = page
      this.loadStepList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.step.query.pageSize = pageSize
      this.loadStepList()
    },
  },
  mounted() {
    this.loadStepList()
    this.initData()
  },
}
</script>
