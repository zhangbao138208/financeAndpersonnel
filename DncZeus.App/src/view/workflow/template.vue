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
        v-model="stores.template.data"
        :totalCount="stores.template.query.totalCount"
        :columns="stores.template.columns"
        @on-delete="handleDelete"
        @on-edit="handleEdit"
        @on-step="handleStep"
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
                      v-model="stores.template.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchTemplate()"
                    >
                      <Select
                        slot="prepend"
                        v-model="stores.template.query.isDeleted"
                        @on-change="handleSearchTemplate"
                        placeholder="删除状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.template.sources
                            .isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.template.query.status"
                        @on-change="handleSearchTemplate"
                        placeholder="模板状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.template.sources.statusSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Dropdown
                        slot="prepend"
                        trigger="click"
                        :transfer="true"
                        placement="bottom-start"
                        style="min-width: 80px"
                        @on-visible-change="
                          handleSearchTemplateTreeVisibleChange
                        "
                      >
                        <Button type="primary">
                          <span
                            v-text="stores.template.query.parentName"
                          ></span>
                          <Icon type="ios-arrow-down"></Icon>
                        </Button>
                        <div
                          class="text-left"
                          slot="list"
                          style="min-width: 390px"
                        >
                          <div>
                            <Button
                              type="primary"
                              icon="ios-search"
                              @click="handleRefreshSearchTemplateTreeData"
                              >刷新模板</Button
                            >
                            <Button
                              class="ml3"
                              type="primary"
                              icon="md-close"
                              @click="handleClearSearchTemplateTreeSelection"
                              >清空</Button
                            >
                          </div>
                          <Tree
                            class="text-left dropdown-tree"
                            :data="stores.template.sources.templateTree.data"
                            @on-select-change="
                              handleSearchTemplateTreeSelectChange
                            "
                          ></Tree>
                        </div>
                      </Dropdown>
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
                  title="新增模板"
                  >新增模板</Button
                >
              </Col>
            </Row>
          </section>
        </div>
      </tables>
    </Card>
    <Modal v-model="stepShow" :footer-hide='footerHideStatus'>
      <!-- <Steps :current="steps.length+2">
        <Step title="起" icon="md-radio-button-off"></Step>
        <Step :title="item.title" :key="item.code" v-for="item in steps" 
        icon='md-fastforward'></Step>
        <Step title="止" icon="md-radio-button-off"></Step>
      </Steps> -->
        <Button type="success" shape="circle">起</Button>
          <Icon type="md-fastforward" color='#ccc'/><Icon type="md-fastforward" color='#ccc'/><Icon
              type="md-fastforward"
              color='#ccc'
            />
          <div :key="item.code" v-for="(item) in steps" style="display:inline-block">
            <Button type="primary">{{ item.title }}</Button>
            
            <Icon type="md-fastforward" color='#ccc'/><Icon type="md-fastforward" color='#ccc'/><Icon
              type="md-fastforward"
              color='#ccc'
            />
          </div>

          <Button type="error" shape="circle">止</Button>
    </Modal>
    <Drawer
      :title="formTitle"
      v-model="formModel.opened"
      width="800"
      :mask-closable="true"
      :mask="true"
      :styles="styles"
    >
      <Form
        :model="formModel.fields"
        ref="formTemplate"
        :rules="formModel.rules"
        label-position="left"
      >
        <FormItem label="模板名称" prop="name" label-position="left">
          <Input v-model="formModel.fields.name" placeholder="请输入模板名称" />
        </FormItem>

        <Row>
          <Col span="24">
            <FormItem label-position="left">
              <Input
                v-model="formModel.fields.parentName"
                placeholder="请选择上级模板"
                :readonly="true"
              >
                <Dropdown
                  slot="append"
                  trigger="click"
                  :transfer="true"
                  placement="bottom-end"
                >
                  <Button type="primary"
                    >选择...
                    <Icon type="ios-arrow-down"></Icon>
                  </Button>
                  <div
                    class="text-left pad10"
                    slot="list"
                    style="min-width: 360px"
                  >
                    <div>
                      <Button
                        type="primary"
                        icon="ios-search"
                        @click="handleRefreshTemplateTreeData"
                        >刷新模板</Button
                      >
                    </div>
                    <Tree
                      class="text-left dropdown-tree"
                      :data="stores.templateTree.data"
                      @on-select-change="handleTemplateTreeSelectChange"
                    ></Tree>
                  </div>
                </Dropdown>
              </Input>
            </FormItem>
          </Col>
        </Row>
        <Row>
          <Col span="12">
            <FormItem label="模板状态" label-position="left">
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
          </Col>
          <Col span="12">
            <FormItem label="自由选择步骤" label-position="left">
              <i-switch size="large" v-model="formModel.fields.isStepFree">
                <span slot="open">是</span>
                <span slot="close">否</span>
              </i-switch>
            </FormItem>
          </Col>
        </Row>
        <Row>
          <Col span="12">
            <FormItem label="模板显示" label-position="left">
              <i-switch size="large" v-model="formModel.fields.visible">
                <span slot="open">是</span>
                <span slot="close">否</span>
              </i-switch>
            </FormItem>
          </Col>
          <Col span="12"> </Col>
        </Row>

        <FormItem
          label="模板"
          label-position="top"
          v-if="
            formModel.opened &&
            formModel.fields.parentName != '顶级模板' &&
            !!formModel.fields.parentName
          "
        >
          <editor
            id="editor_id"
            height="500px"
            width="700px"
            :content="formModel.fields.description"
            pluginsPath="/kindeditor/plugins/"
            :loadStyleMode="false"
            :uploadJson="uploadJson"
            @on-content-change="onContentChange"
          ></editor>
        </FormItem>
      </Form>
      <div class="demo-drawer-footer">
        <Button
          icon="md-checkmark-circle"
          type="primary"
          @click="handleSubmitTemplate"
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
import config from '@/config'

import VueFroala from 'vue-froala-wysiwyg'
import Tables from '_c/tables'
import {
  getTemplateList,
  createTemplate,
  loadTemplate,
  editTemplate,
  deleteTemplate,
  batchCommand,
  loadTemplateTree,
} from '@/api/workflow/template'

import { loadStepSimpleList } from '@/api/workflow/step'
export default {
  name: 'workflow_template_page',
  components: {
    Tables,
  },
  data() {
    return {
      footerHideStatus:true,
      stepShow: false,
      steps: [],
      uploadJson:
        process.env.NODE_ENV === 'development'
          ? config.baseUrl.dev + 'editorupload'
          : config.baseUrl.pro + 'editorupload',
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建模板',
        mode: 'create',
        selection: [],
        selectOption: {
          icon: {},
        },
        fields: {
          code: '',
          name: '',

          parentCode: '',
          parentName: '',

          status: 1,
          visible: 1,
          isDeleted: 0,
          isStepFree: false,
          description: ``,
        },
        rules: {
          name: [
            {
              type: 'string',
              required: true,
              message: '请输入模板名称',
              min: 2,
            },
          ],
          alias: [
            {
              type: 'string',
              required: true,
              message: '请输入模板名称',
              min: 2,
            },
          ],
          icon: [
            {
              type: 'string',
              required: true,
              message: '请选择模板图标',
            },
          ],
        },
      },
      stores: {
        template: {
          query: {
            totalCount: 0,
            pageSize: 20,
            currentPage: 1,
            kw: '',
            isDeleted: 0,
            status: -1,
            parentCode: '',
            parentName: '请选择...',
            sort: [
              {
                direct: 'DESC',
                field: 'id',
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
            templateTree: {
              inited: false,
              data: [],
            },
            iconSources: {
              loading: false,
              data: [],
            },
          },
          columns: [
            { type: 'selection', width: 30, key: 'handle' },
            { title: '模板名称', key: 'name', width: 200 },
            { title: '上级模板', key: 'parentName', width: 150 },
            {
              title: '状态',
              key: 'status',
              align: 'center',
              width: 60,
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

            {
              title: '创建时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'createdOn',
            },
            {
              title: '创建者',
              key: 'createdByUserName',
              ellipsis: true,
              tooltip: true,
              width: 80,
            },
            {
              title: '操作',
              align: 'center',
              key: 'handle',
              // width: 100,
              className: 'table-command-column',
              options: ['edit'],
              fixed: 'right',
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
                (h, params, vm) => {
                if (!params.row.parentCode) {
                   return ""
                }
               
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
                          icon: 'md-fastforward',
                          type: 'warning',
                        },
                        on: {
                          click: () => {
                            vm.$emit('on-step', params)
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
                        '流程图'
                      ),
                    ]
                  )
                },
              ],
            },
          ],
          data: [],
        },
        templateTree: {
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
        return '创建模板'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑模板'
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
    handleStep(params) {
      loadStepSimpleList({ templateCode: params.row.code }).then((res) => {
        this.steps = res.data.data
        this.stepShow=true
      })
    },
    onContentChange(val) {
      this.formModel.fields.description = val
    },
    loadTemplateList() {
      getTemplateList(this.stores.template.query).then((res) => {
        this.stores.template.data = res.data.data
        this.stores.template.query.totalCount = res.data.totalCount
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
      this.handleResetFormTemplate()
      this.doLoadTemplate(params.row.code)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadTemplateList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormTemplate()
    },
    handleSubmitTemplate() {
      let valid = this.validateTemplateForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateTemplate()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditTemplate()
        }
      }
    },
    handleResetFormTemplate() {
      this.$refs['formTemplate'].resetFields()
    },
    doCreateTemplate() {
      createTemplate(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.handleCloseFormWindow()
          this.loadTemplateList()
          this.handleRefreshTemplateTreeData()
        } else {
          this.$Message.warning(res.data.message)
        }
      })
    },
    doEditTemplate() {
      editTemplate(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.handleCloseFormWindow()
          this.loadTemplateList()
          this.handleRefreshTemplateTreeData()
        } else {
          this.$Message.warning(res.data.message)
        }
      })
    },
    validateTemplateForm() {
      let _valid = false
      this.$refs['formTemplate'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadTemplate(code) {
      loadTemplate({ code: code }).then((res) => {
        this.formModel.fields = res.data.data.model
        this.stores.templateTree.data = res.data.data.tree
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
      deleteTemplate(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadTemplateList()
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
          this.loadTemplateList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
    },
    handleSearchTemplate() {
      this.loadTemplateList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    doLoadTemplateTree() {
      loadTemplateTree(null).then((res) => {
        this.stores.templateTree.data = res.data.data
      })
    },
    handleTemplateTreeSelectChange(nodes) {
      var node = nodes[0]
      if (node) {
        this.formModel.fields.parentCode = node.code
        this.formModel.fields.parentName = node.title
      }
    },
    handleRefreshTemplateTreeData() {
      this.doLoadTemplateTree()
    },
    doLoadSearchTemplateTree() {
      loadTemplateTree(null).then((res) => {
        this.stores.template.sources.templateTree.data = res.data.data
      })
    },
    handleSearchTemplateTreeSelectChange(nodes) {
      var node = nodes[0]
      if (node) {
        this.stores.template.query.parentCode = node.code
        this.stores.template.query.parentName = node.Title
      }
      this.loadTemplateList()
    },
    handleRefreshSearchTemplateTreeData() {
      this.doLoadSearchTemplateTree()
    },
    handleSearchTemplateTreeVisibleChange(visible) {
      if (visible && !this.stores.template.sources.templateTree.inited) {
        this.stores.template.sources.templateTree.inited = true
        this.handleRefreshSearchTemplateTreeData()
      }
    },
    handleClearSearchTemplateTreeSelection() {
      this.stores.template.query.parentCode = ''
      this.stores.template.query.parentName = '请选择...'
      this.loadTemplateList()
    },
    handlePageChanged(page) {
      this.stores.template.query.currentPage = page
      this.loadTemplateList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.template.query.pageSize = pageSize
      this.loadTemplateList()
    },
    handleLoadIconDataSource(keyword) {
      this.stores.template.sources.iconSources.loading = true
      let query = { keyword: keyword }
      findIconDataSourceByKeyword(query).then((res) => {
        this.stores.template.sources.iconSources.data = res.data.data
        this.stores.template.sources.iconSources.loading = false
      })
    },
  },
  mounted() {
    this.loadTemplateList()
    this.doLoadTemplateTree()
    this.doLoadSearchTemplateTree()
  },
}
</script>
