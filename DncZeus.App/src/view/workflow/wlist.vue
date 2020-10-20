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
      <Col
        span="12"
        class="demo-tabs-style1"
        style="background: #e3e8ee; width: 100%; padding: 16px"
      >
        <Tabs v-model="tab_name" type="card">
          <TabPane label="基本信息" name="wlist_tabs1">
            <Form
              :model="formModel.fields"
              ref="formWlist"
              :rules="formModel.rules"
              label-wlist="left"
            >
              <Row :gutter="8">
                <Col span="12">
                  <FormItem
                    label="模板"
                    label-position="left"
                    prop="templateCode"
                  >
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
                        v-for="template in templates"
                      >
                        <mu-list-item-action avatar>
                          <mu-avatar :size="36" color="primary">{{
                            template.name.substring(0, 1)
                          }}</mu-avatar>
                        </mu-list-item-action>
                        <mu-list-item-content>
                          <mu-list-item-title>{{
                            template.name
                          }}</mu-list-item-title>
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
                <Button v-if="isView" type="primary" v-print="'#wlistPrint'"
                  >打印</Button
                >
                <div
                  id="wlistPrint"
                  v-if="isView"
                  v-html="formModel.fields.description"
                ></div>
              </FormItem>

              <FormItem label="进度" label-position="top" v-if="isView">
                <Button type="success" shape="circle">起</Button>

                <!-- <Icon type="md-fastforward" color="#ccc" /> -->
                <img
                  src="../../assets/images/arrow.png"
                  style="width: 40px; vertical-align: sub"
                />
                <div
                  :key="item.code"
                  v-for="(item, index) in steps"
                  style="display: inline-block"
                >
                  <Button type="primary" v-if="index + 1 < stepLength">{{
                    item.title
                  }}</Button>
                  <Button type="default" v-if="index + 1 >= stepLength">{{
                    item.title
                  }}</Button>
                  <img
                    src="../../assets/images/arrow.png"
                    style="width: 40px; vertical-align: sub"
                  />
                </div>

                <Button type="error" shape="circle">止</Button>
              </FormItem>

              <FormItem
                label="意见"
                label-position="top"
                v-if="!!formModel.fields.notes && isView"
              >
                <Table
                  width="650"
                  border
                  :columns="stores.notes.columns"
                  :data="formModel.fields.notes"
                >
                </Table>
              </FormItem>

              <FormItem label="附件" v-if="!!additions.length && isView">
                <ul>
                  <li
                    v-for="(list, index) in additions"
                    :key="index"
                    style="list-style: none"
                  >
                    <a :href="list.url">{{ list.title }}</a>
                  </li>
                </ul>
              </FormItem>

              <Row :gutter="8">
                <Col span="12">
                  <FormItem label="程度" prop="type">
                    <mu-select
                      v-model="formModel.fields.type"
                      :disabled="isView"
                    >
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
                          <mu-list-item-title>{{
                            wtype.name
                          }}</mu-list-item-title>
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
                      !isView &&
                      !!steps.length &&
                      !!formModel.fields.nextStepCode
                    "
                  >
                    <mu-select
                      v-model="formModel.fields.approver"
                      v-if="steps[0].isCounterSign"
                    >
                      <mu-option
                        :key="index"
                        :label="steps[0].usersName.join(',')"
                        :value="steps[0].users.join(',')"
                        avatar
                        v-for="(item, index) in [1]"
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
          </TabPane>
          <TabPane label="附件管理" name="wlist_tabs2" :disabled="isView"
            ><Form>
              <Row :gutter="8">
                <Col span="12">
                  <FormItem label="标题">
                    <Input v-model="addition.title" />
                  </FormItem>
                </Col>
                <Col span="12">
                  <FormItem label="说明">
                    <Input v-model="addition.description" />
                  </FormItem>
                </Col>
              </Row>
              <Row :gutter="8">
                <Col span="12">
                  <FormItem>
                    　　　　　　　　
                    <Upload
                      ref="upload"
                      :before-upload="handleUpload"
                      :show-upload-list="false"
                      :action="http"
                    >
                      　　　　　　　　　　<Button
                        style="width: 200px"
                        icon="ios-cloud-upload-outline"
                        >{{ uploadShow }}</Button
                      >
                      　　　　　　</Upload
                    >
                    　　　　　　　
                  </FormItem> </Col
                ><Col span="12"
                  ><Button type="primary" @click="upload">上传</Button></Col
                > </Row
              >　
              <div>
                <ul class="file-list" v-if="!!additions.length">
                  <li>
                    <span>标题</span>
                    <span>说明</span>
                    <span>操作</span>
                  </li>
                  　　　　　　　　　
                  <li v-for="(list, index) in additions" :key="index">
                    <span>{{ list.title }}</span>
                    <span>{{ list.description }}</span>
                    <!-- 　　　　　　　　　　　　<Icon
                      type="ios-close"
                      size="20"
                      style="float: right; cursor: pointer"
                      @click="delFileList(index)"
                    ></Icon> -->
                    <span>
                      <Button
                        size="small"
                        style="margin-right: 5px"
                        type="primary"
                        @click="downloadFileList(list.url)"
                        >下载</Button
                      >
                      <Button
                        size="small"
                        type="error"
                        @click="delFileList(index)"
                        >删除</Button
                      >
                    </span>
                    　　　　　　　　　　
                  </li>
                  　　　　　　　　　
                </ul>
                　　　　　　
              </div>
              　 　
            </Form></TabPane
          >
        </Tabs>
      </Col>
      <div class="demo-drawer-footer" style="margin-top: 20px">
        <Button
          :disabled="isView || isloading"
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
import { upload, deleteFile } from '@/api/common'

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
      tab_name:'wlist_tabs1',
      deleteUrls: [],
      file: [],
      additions: [],
      addition: {
        title: '',
        description: '',
        url: '',
      },

      http: '',
      uploadShow: '文件大小规定不能超过20兆',
      isloading: false,
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
          additions: '',
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
                let statusColor = '#f29100'
                let statusText = params.row.statusName
                switch (status) {
                  case '-1':
                    statusColor = '#ccc'
                    break
                  case '1':
                    statusColor = '#19be6b'
                    break
                  case '2':
                    statusColor = '#e13d13'
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
                let color = '#19be6b'
                switch (params.row.type) {
                  case '1':
                  case 1:
                    color = '#f29100'
                    break
                  case '2':
                  case 2:
                    color = '#e13d13'
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
                let statusColor = '#f29100'
                let statusText = params.row.statusName
                switch (status) {
                  case '1':
                    statusColor = '#00C0EF'
                    break
                  case '2':
                    statusColor = '#19be6b'
                    break
                  case '3':
                    statusColor = '#e13d13'
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
                        '预览'
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
        return '预览工作'
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
  watch: {
    formModel: {
      handler: function (val, oldVal) {
        if (val.fields && val.fields.additions) {
          this.additions = JSON.parse(val.fields.additions)
        }
      },
      deep: true,
    },
    assessments: function (val, oldVal) {
      this.fields.selfEvaluations = JSON.stringify(val)
    },
  },
  methods: {
    downloadFileList(url) {
      window.location.href = url
    },
    handleUpload(file) {
      var fileType = file.name.split('.')
      fileType = fileType[fileType.length - 1].toLowerCase()
      // if (fileType != 'doc' && fileType != 'docx') {
      //   this.$Message.error('文件格式不正确，只能上传.doc .docx 类型的文件')
      //   return
      // } //20971520
      if (file.size > 20971520) {
        this.$Message.error('最大上传20兆的文件')
        return
      }
      if (this.file.length > 5) {
        this.$Message.info('最多只能上传5个文件')
      } else {
        this.uploadShow = file.name
        this.addition.title = file.name
        this.file.push(file)
      }
      return false
    },

    delFileList(index) {
      var url = this.additions[index].url
      this.additions.splice(index, 1)
      this.formModel.fields.additions = JSON.stringify(this.additions)
      this.deleteUrls.push(url)
    },

    //上传
    upload() {
      // //其他条件判断
      // if (this.formItem.shengbh == '') {
      //   this.$Message.error('请刷新页面')
      //   return
      // }
      // if (this.formItem.shibh == '') {
      //   this.$Message.error('中心名称不能为空')
      //   return
      // }
      // if (this.formItem.vsj == '') {
      //   this.$Message.error('评价日期不能为空')
      //   return
      // }
      var jsonStr = JSON.stringify(this.data) //创建 formData 对象：很重要
      let formData = new FormData() //多个文件上传
      //向 formData 对象中添加文件
      formData.append('uploadFileBg', this.file[0]) // 文件对象1：报告

      // formData.append('id', this.formItem.id) //其他参数

      // formData.append('jsonStr', jsonStr) //使用post方式上传
      upload(formData).then((res) => {
        if (res.data.code == '200') {
          this.addition.url = res.data.data.hostUrl
          this.additions.push(this.addition)
          this.formModel.fields.additions = JSON.stringify(this.additions)
          this.$Message.success('添加成功')

          this.file = []
        } else {
          this.$Message.error('添加失败')
        }
        this.uploadShow = '文件大小规定不能超过20兆'
      })
    },
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
       this.tab_name='wlist_tabs1'
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
       this.tab_name='wlist_tabs1'
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
    async handleSubmitDePartment() {
      if (!!this.deleteUrls.length) {
        await deleteFile(this.deleteUrls)
      }
     

      this.isloading = true
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
      // this.$refs['formWlist'].resetFields()
      Object.keys(this.formModel.fields).forEach(
        (key) => (this.formModel.fields[key] = '')
      )
      this.deleteUrls = []
      this.additions = []
       Object.keys(this.addition).forEach(
        (key) => (this.addition[key] = '')
      )
     
    },
    doCreateWlist() {
      createWlist(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadWlistList()
        } else {
          this.isloading = false
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
        this.isloading = false
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
        this.isloading = false
      })
    },
    validateWlistForm() {
      let _valid = false
      this.$refs['formWlist'].validate((valid) => {
        if (!valid) {
          this.isloading=false
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
<style media="wlistPrint">
@page {
  size: auto; /* auto is the initial value */
  margin: 5mm; /* this affects the margin in the printer settings */
}

html {
  background-color: #ffffff;
  margin: 0px; /* this affects the margin on the html before sending to printer */
}
</style>
<style scoped>
.file-list li {
  list-style: none;
  width: 100%;
  margin: 5px 0 5px 0;
}
.file-list > li:first-child > span {
  font-weight: 800;
}
.file-list > li > span {
  width: 220px;
  display: inline-block;
}
.file-list li:hover {
  background-color: rgb(245, 245, 245);
}
.demo-tabs-style1 > .ivu-tabs-card > .ivu-tabs-content {
  height: 120px;
  margin-top: -16px;
}

.demo-tabs-style1 > .ivu-tabs-card > .ivu-tabs-content > .ivu-tabs-tabpane {
  background: #fff;
  padding: 16px;
}

.demo-tabs-style1 > .ivu-tabs.ivu-tabs-card > .ivu-tabs-bar .ivu-tabs-tab {
  border-color: transparent;
}

.demo-tabs-style1 > .ivu-tabs-card > .ivu-tabs-bar .ivu-tabs-tab-active {
  border-color: #fff;
}
</style>
