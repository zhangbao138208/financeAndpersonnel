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
        v-model="stores.financeinfo.data"
        :totalCount="stores.financeinfo.query.totalCount"
        :columns="stores.financeinfo.columns"
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
                      v-model="stores.financeinfo.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchFinanceInfo()"
                    >
                      <Select
                        slot="prepend"
                        v-model="stores.financeinfo.query.isDeleted"
                        @on-change="handleSearchFinanceInfo"
                        placeholder="删除状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.financeinfo.sources
                            .isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.financeinfo.query.status"
                        @on-change="handleSearchFinanceInfo"
                        placeholder="状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.financeinfo.sources
                            .statusSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.name }}</Option
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
                  title="新增财务管理"
                  >新增财务管理</Button
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
        ref="formFinanceInfo"
        :rules="formModel.rules"
        label-financeinfo="left"
      >
        <FormItem label="标题" prop="title" label-financeinfo="left">
          <Input v-model="formModel.fields.title" placeholder="标题" />
        </FormItem>
        <FormItem label="金额" prop="amount" label-financeinfo="left">
          <InputNumber :min="0" v-model="formModel.fields.amount"></InputNumber>
        </FormItem>

        <FormItem label="类型" prop="type" label-financeinfo="left">
          <mu-select v-model="formModel.fields.type">
            <mu-option
              :key="payType.value"
              :label="payType.name"
              :value="payType.value"
              avatar
              v-for="(payType, index) in types"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{
                  payType.name.substring(0, 1)
                }}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{ payType.name }}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </FormItem>
        <FormItem label="账号" prop="financeAccount" label-financeinfo="left">
          <mu-select v-model="formModel.fields.financeAccount">
            <mu-option
              :key="financeAccount.code"
              :label="financeAccount.name"
              :value="financeAccount.code"
              avatar
              v-for="(financeAccount, index) in financeAccounts"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{
                  financeAccount.name.substring(0, 1)
                }}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{
                  financeAccount.name
                }}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </FormItem>
        <FormItem label="部门" prop="financeAccount" label-financeinfo="left">
          <mu-select v-model="formModel.fields.departmentCode">
            <mu-option
              :key="department.code"
              :label="department.name"
              :value="department.code"
              avatar
              v-for="(department, index) in departments"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{
                  department.name.substring(0, 1)
                }}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{ department.name }}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </FormItem>
        <FormItem label="状态" label-financeinfo="left">
          <mu-select v-model="formModel.fields.infoStatus">
            <mu-option
              :key="status.value"
              :label="status.name"
              :value="status.value"
              avatar
              v-for="(status, index) in stores.financeinfo.sources
                .statusFormSources"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{
                  status.name.substring(0, 1)
                }}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{ status.name }}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </FormItem>
        <FormItem label="经手人" prop="handleName" label-financeinfo="left">
          <Input v-model="formModel.fields.handleName" placeholder="经手人" />
        </FormItem>
        <FormItem label="经手时间" prop="handleDate" label-financeinfo="left">
          <DatePicker
            format="yyyy-MM-dd"
            type="date"
            v-model="formModel.fields.handleDate"
          ></DatePicker>
        </FormItem>
        <FormItem label="描述" prop="description" label-financeinfo="left">
          <Input
            type="textarea"
            v-model="formModel.fields.description"
            placeholder="请输入描述"
          />
        </FormItem>
        <FormItem label="附件" prop="filePath" label-financeinfo="left">
          <Upload multiple type="drag" action="" :before-upload="beforeUpload">
            <div style="padding: 20px 0">
              <Icon
                type="ios-cloud-upload"
                size="52"
                style="color: #3399ff"
              ></Icon>
              <p>点击或拖动文件以上传</p>
            </div>
            <div>
              {{ formModel.fileName }}
            </div>
          </Upload>
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
  getFinanceInfoList,
  createFinanceInfo,
  loadFinanceInfo,
  editFinanceInfo,
  deleteFinanceInfo,
  batchCommand,
} from '@/api/finance/financeInfo'
import { loadAccountSimpleList } from '@/api/finance/account'
import { loadDepartmentSimpleList } from '@/api/user/department'
import { loadDictionarySimpleList } from '@/api/system/dictionary'
import { upload } from '@/api/common'

export default {
  name: 'finance_financeinfo_page',
  components: {
    Tables,
  },
  data() {
    return {
      types: [],
      financeAccounts: [],
      departments: [],
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        fileName: '',
        opened: false,
        title: '创建财务管理',
        mode: 'create',
        selection: [],
        fields: {
          code: '',
          name: '',
          financeAccount: '',
          departmentCode: '',
          type: '',
          handleName: '',
          handleDate: '',
          infoStatus: '',
          description: '',
          filePath: '',
          amount: 0,
          isLocked: 0,
          status: 1,
          isDeleted: 0,
          description: '',
        },
        rules: {
          name: [
            {
              type: 'string',
              required: true,
              message: '请输入财务管理名称',
              min: 2,
            },
          ],
          financeinfo: [
            {
              type: 'string',
              required: true,
              message: '请输入账号',
              min: 2,
            },
          ],
          type: [
            {
              type: 'string',
              required: true,
              message: '请输入类型',
              min: 2,
            },
          ],
          holder: [
            {
              type: 'string',
              required: true,
              message: '请输入持有人',
              min: 2,
            },
          ],
        },
      },
      stores: {
        financeinfo: {
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
            statusSources: [{ value: '', name: '全部' }],
            statusFormSources: [],
          },
          columns: [
            { type: 'selection', width: 50, key: 'handle' },
            { title: '类型', key: 'typeName', width: 100 },

            { title: '标题', key: 'title', width: 150 },
            { title: '金额', key: 'amount', width: 100 },
            {
              title: '状态',
              key: 'infoStatus',
              align: 'center',
              width: 120,
              render: (h, params) => {
                let status = params.row.infoStatus
                let statusColor = 'success'
                let statusText = params.row.infoStatusName
                switch (status) {
                  case '0':
                  case 0:
                    statusColor = 'default'
                    break
                  case '-1':
                  case -1:
                    statusColor = 'error'
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

            { title: '部门', key: 'departmentName', width: 100 },
            { title: '账号', key: 'financeAccountName', width: 100 },
            { title: '开户人', key: 'handleName', width: 100 },
            {
              title: '开户时间',
              key: 'handleDate',
              width: 100,
              ellipsis: true,
              tooltip: true,
            },
            {
              title: '附件',
              key: 'filePath',
              ellipsis: true,
              tooltip: true,
              width: 150,
              render: (h, params) => {
                let downloadUrl = params.row.filePath

                let index = downloadUrl.indexOf('uploads')
                let a = downloadUrl.substr(index).split('/')
                var title = '下载'
                if (!!a[a.length - 1]) {
                  title = a[a.length - 1]
                }
                console.log(downloadUrl)
                return h(
                  'a',
                  {
                    attrs: {
                      href: downloadUrl,
                    },
                  },
                  title //整个的信息即气泡内文字
                )
              },
            },
            { title: '描述', key: 'description', width: 100 },
            {
              title: '创建时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'createdOn',
            },
            { title: '创建者', width: 100, key: 'createdByUserName' },
            {
              title: '操作',
              align: 'center',
              fixed: 'right',
              key: 'handle',
              width: 150,
              className: 'table-command-column',
              options: ['edit'],
              button: [
                (h, params, vm) => {
                  return h(
                    'Poptip',
                    {
                      style:{
                        zIndex:500,
                      },
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
        financeinfo: 'static',
      },
    }
  },
  watch: {
    formModel: {
      handler: function (newVal, old) {
        let s = newVal.fields.filePath
        let index = s.indexOf('uploads')
        let a = s.substr(index).split('/')
        this.formModel.fileName = a[a.length - 1]
      },
      deep: true,
    },
  },
  computed: {
    formTitle() {
      if (this.formModel.mode === 'create') {
        return '创建财务管理'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑财务管理'
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
      loadDictionarySimpleList('finance_manager_type').then((res) => {
        this.types = res.data.data
      })
      loadDepartmentSimpleList().then((res) => {
        this.departments = res.data.data
      })
      loadAccountSimpleList().then((res) => {
        this.financeAccounts = res.data.data
      })
      loadDictionarySimpleList('finance_manager_status').then((res) => {
        this.stores.financeinfo.sources.statusSources = [
          ...this.stores.financeinfo.sources.statusSources,
          ...res.data.data,
        ]
        this.stores.financeinfo.sources.statusFormSources = res.data.data
      })
    },
    loadFinanceInfoList() {
      getFinanceInfoList(this.stores.financeinfo.query).then((res) => {
        this.stores.financeinfo.data = res.data.data
        this.stores.financeinfo.query.totalCount = res.data.totalCount
      })
    },
    beforeUpload(file) {
      let formData = new FormData()
      formData.append('file', file)
      upload(formData).then((res) => {
        if (res.data.code == 200) {
          this.formModel.fields.filePath = res.data.data.hostUrl
          //   let index =res.data.data.hostUrl.indexOf('uploads')
          //   let s=res.data.data.hostUrl.substr(index).split('/')
          //   console.log(s)
          //   this.formModel.fileName = res.data.data.name
          //   console.log(res.data)
          // return true
        } else {
          this.$Message.error(res.data.message)
        }
        return false
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
      this.handleResetFormFinanceInfo()
      this.doLoadFinanceInfo(params.row.code)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadFinanceInfoList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormFinanceInfo()
    },
    handleSubmitDePartment() {
      let valid = this.validateFinanceInfoForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateFinanceInfo()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditFinanceInfo()
        }
      }
    },
    handleResetFormFinanceInfo() {
      this.$refs['formFinanceInfo'].resetFields()
    },
    doCreateFinanceInfo() {
      createFinanceInfo(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadFinanceInfoList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    doEditFinanceInfo() {
      editFinanceInfo(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadFinanceInfoList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    validateFinanceInfoForm() {
      let _valid = false
      this.$refs['formFinanceInfo'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadFinanceInfo(code) {
      loadFinanceInfo({ code: code }).then((res) => {
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
      deleteFinanceInfo(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadFinanceInfoList()
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
          this.loadFinanceInfoList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
    },
    handleSearchFinanceInfo() {
      this.loadFinanceInfoList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.financeinfo.query.currentPage = page
      this.loadFinanceInfoList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.financeinfo.query.pageSize = pageSize
      this.loadFinanceInfoList()
    },
  },
  mounted() {
    this.initData()
    this.loadFinanceInfoList()
  },
}
</script>
