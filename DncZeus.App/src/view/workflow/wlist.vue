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
        v-model="stores.wlist.data"
        :totalCount="stores.wlist.query.totalCount"
        :columns="stores.wlist.columns"
        @on-delete="handleDelete"
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
                      v-model="stores.wlist.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchWlist()"
                    >
                      <!-- <Select
                        slot="prepend"
                        v-model="stores.wlist.query.isDeleted"
                        @on-change="handleSearchWlist"
                        placeholder="删除状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.wlist.sources
                            .isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.wlist.query.status"
                        @on-change="handleSearchWlist"
                        placeholder="工作表类型状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.wlist.sources.statusSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select> -->
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
                  <!-- <Button
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
                  ></Button> -->
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
                  title="新增工作"
                  >新增工作</Button
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
      width="800"
      :mask-closable="false"
      :mask="true"
      :styles="styles"
    >
      <Form
        :model="formModel.fields"
        ref="formWlist"
        :rules="formModel.rules"
        label-wlist="left"
      >
        <Row :gutter="8">
          <Col span="12">
            <FormItem label="模板" label-position="left" prop="templateCode">
              <mu-select
                v-model="formModel.fields.templateCode"
                @change="handTemplate"
                :disabled="isView"
              >
                <mu-option
                  :key="template.code"
                  :label="template.name"
                  :value="template.code"
                  avatar
                  v-for="(template, index) in templates"
                >
                  <mu-list-item-action avatar>
                    <mu-avatar :size="36" color="primary">{{
                      template.name.substring(0, 1)
                    }}</mu-avatar>
                  </mu-list-item-action>
                  <mu-list-item-content>
                    <mu-list-item-title>{{ template.name }}</mu-list-item-title>
                  </mu-list-item-content>
                </mu-option>
              </mu-select>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem prop="title" label-wlist="left">
              <Input
                :disabled="isView"
                v-model="formModel.fields.title"
                placeholder="请输入工作标题"
              />
            </FormItem>
          </Col>
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
          <div :key="item.code" v-for="(item ,index) in steps" style="display:inline-block">
            <Button type="primary" v-if="index+1<stepLength">{{ item.title }}</Button>
             <Button type="default" v-if="index+1>=stepLength">{{ item.title }}</Button>
             <Icon type="md-fastforward" color='#ccc'/><Icon type="md-fastforward" color='#ccc'/><Icon
              type="md-fastforward"
              color='#ccc'
            />
          </div>

          <Button type="error" shape="circle">止</Button>
        </FormItem>

        <FormItem label="意见" label-position="top" v-if="!!formModel.fields.notes">
          <Table
            width="650"
            border
            :columns="stores.notes.columns"
            :data="formModel.fields.notes"
          >
          </Table>
        </FormItem>

        <Row :gutter="8">
          <Col span="12">
            <FormItem label="程度" prop="type">
              <mu-select v-model="formModel.fields.type" :disabled="isView">
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
            <FormItem label="到期" prop="endDate" label-wlist="left">
              <DatePicker
                :disabled="isView"
                format="yyyy-MM-dd"
                type="date"
                v-model="formModel.fields.endDate"
              ></DatePicker>
            </FormItem>
          </Col>
        </Row>

        <Row :gutter="8">
          <Col span="12">
            <FormItem
              label="下一步"
              prop="endDate"
              label-wlist="left"
              v-if="!isView && !!steps.length"
            >
              <mu-select v-model="formModel.fields.nextStepCode">
                <mu-option
                  :key="s.code"
                  :label="s.title"
                  :value="s.code"
                  avatar
                  v-for="s in [steps[0]]"
                >
                  <mu-list-item-action avatar>
                    <mu-avatar :size="36" color="primary">{{
                      s.title.substring(0, 1)
                    }}</mu-avatar>
                  </mu-list-item-action>
                  <mu-list-item-content>
                    <mu-list-item-title>{{ s.title }}</mu-list-item-title>
                  </mu-list-item-content>
                </mu-option>
              </mu-select>
            </FormItem>
          </Col>
          <Col span="12">
            <FormItem
              label="审批人"
              prop="type"
              v-if="
                !isView && !!steps.length && !!formModel.fields.nextStepCode
              "
            >
              <mu-select
                v-model="formModel.fields.approver"
                multiple
                v-if="steps[0].isCounterSign"
              >
                <mu-option
                  :key="user"
                  :label="steps[0].usersName[index]"
                  :value="user"
                  avatar
                  v-for="(user, index) in steps[0].users"
                ></mu-option>
              </mu-select>

              <mu-select
                v-model="formModel.fields.approver"
                v-if="!steps[0].isCounterSign"
              >
                <mu-option
                  :key="user"
                  :label="steps[0].usersName[index]"
                  :value="user"
                  avatar
                  v-for="(user, index) in steps[0].users"
                ></mu-option>
              </mu-select>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div class="demo-drawer-footer">
        <Button
          :disabled="isView||isloading"
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
import { loadDictionarySimpleList } from '@/api/system/dictionary'
import { loadTemplateSimpleList } from '@/api/workflow/template'
import config from '@/config'
import Moment from 'moment'

import {
  getWlistList,
  createWlist,
  loadWlist,
  editWlist,
  deleteWlist,
  batchCommand,
} from '@/api/workflow/wlist'
import { loadStepSimpleList } from '@/api/workflow/step'
export default {
  name: 'workflow_wlist_page',
  components: {
    Tables,
  },
  data() {
    return {
      isloading:false,
      steps: [],
      isView: false,
      uploadJson:
        process.env.NODE_ENV === 'development'
          ? config.baseUrl.dev + 'editorupload'
          : config.baseUrl.pro + 'editorupload',
      wtypes: [],
      wstatus: [],
      templates: [],
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建工作',
        mode: 'create',
        selection: [],
        fields: {
          nextStepCode: '',
          code: '',
          title: '',
          templateCode: '',
          //审批人
          approver: '',
          startDate: '',
          endDate: new Date(
            new Date().getTime() + 7 * 24 * 3600 * 1000
          ).toLocaleDateString(),
          description: '',
          type: '',
        },
        rules: {
          title: [
            {
              type: 'string',
              required: true,
              message: '请输入工作标题',
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
        wlist: {
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
            { title: '标题', key: 'title', width: 300 },

            {
              title: '程度',
              key: 'typeName',
              width: 150,
              render: (h, params) => {
                let color = 'green'
                switch (params.row.type) {
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
                  params.row.typeName
                )
              },
            },
            {
              title: '当前节点',
              key: 'departmentName',
              width: 150,
              render: (h, params) => {
                let show = params.row.departmentName
                if (!show) {
                  show = params.row.statusName
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
                  case '1':
                    statusColor = '#00C0EF'
                    break
                  case '2':
                    statusColor = 'green'
                    break
                  case '3':
                    statusColor = 'red'
                    break
                  case '4':
                    statusColor = '#ccc'
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
              title: '截止时间',
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: 'endDate',
              render: (h, params) => {
                return h(
                  'span',
                  Moment(params.row.endDate).format('YYYY-MM-DD HH:MM:SS')
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
                  } else {
                    return ''
                  }
                },
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
                          '编辑'
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
        wlist: 'static',
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
        return '创建工作'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑工作'
      }
      if (this.formModel.mode === 'view') {
        this.isView = true
        return '查看工作'
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
    handTemplate() {
      const f = this.templates.filter(
        (_) => _.code == this.formModel.fields.templateCode
      )
      if (!f.length) {
        return
      }
      this.formModel.fields.description = f[0].description
      loadStepSimpleList({
        templateCode: this.formModel.fields.templateCode,
      }).then((res) => {
        this.steps = res.data.data
      })
    },
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
      loadDictionarySimpleList('workflow_list_status').then((res) => {
        if (res.data.code === 200) {
          this.wstatus = res.data.data
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
    loadWlistList() {
      getWlistList(this.stores.wlist.query).then((res) => {
        this.stores.wlist.data = res.data.data
        this.stores.wlist.query.totalCount = res.data.totalCount
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
      this.handleResetFormWlist()
      this.doLoadWlist(params.row.code)
      loadStepSimpleList({ templateCode: params.row.templateCode }).then(
        (res) => {
          this.steps = res.data.data
        }
      )
    },
    handleView(params) {
      this.handleSwitchFormModeToView()
      this.handleResetFormWlist()
      this.doLoadWlist(params.row.code)
      loadStepSimpleList({ templateCode: params.row.templateCode }).then(
        (res) => {
          this.steps = res.data.data
        }
      )
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadWlistList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormWlist()
    },
    handleSubmitDePartment() {
      this.isloading=true
      let valid = this.validateWlistForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateWlist()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditWlist()
        }
      }
    },
    handleResetFormWlist() {
      this.$refs['formWlist'].resetFields()
    },
    doCreateWlist() {
      createWlist(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWlistList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
        this.isloading=false
      })
    },
    doEditWlist() {
      editWlist(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWlistList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
        this.isloading=false
      })
    },
    validateWlistForm() {
      let _valid = false
      this.$refs['formWlist'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadWlist(code) {
      loadWlist({ code: code }).then((res) => {
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
      deleteWlist(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWlistList()
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
          this.loadWlistList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
    },
    handleSearchWlist() {
      this.loadWlistList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.wlist.query.currentPage = page
      this.loadWlistList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.wlist.query.pageSize = pageSize
      this.loadWlistList()
    },
  },
  mounted() {
    this.initData()
    this.loadWlistList()
  },
}
</script>
