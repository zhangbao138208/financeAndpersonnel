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
      <Col
        span="12"
        class="demo-tabs-style1"
        style="background: #e3e8ee; width: 100%; padding: 16px"
      >
        <Tabs v-model="tab_name" type="card">
          <TabPane label="基本信息" name="receiver_tabs1">
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
                <Button v-if="isView" type="primary" v-print="'#receiverPrint'"
                  >打印</Button
                >
                <div
                  id="receiverPrint"
                  v-if="isView"
                  v-html="formModel.fields.description"
                ></div>
              </FormItem>

              <FormItem label="进度" label-position="top" v-if="isView">
                <Button type="success" shape="circle">起</Button>
                <img
                  src="../../assets/images/arrow.png"
                  style="width: 40px; vertical-align: sub"
                />
                <div
                  :key="item"
                  v-for="(item, index) in formModel.fields.steps"
                  style="display: inline-block"
                >
                  <Button
                    type="primary"
                    v-if="index + 1 < formModel.fields.currentStep"
                    >{{ item }}</Button
                  >
                  <Button
                    type="default"
                    v-if="index + 1 >= formModel.fields.currentStep"
                    >{{ item }}</Button
                  >
                  <img
                    src="../../assets/images/arrow.png"
                    style="width: 40px; vertical-align: sub"
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
                    <mu-select
                      v-model="formModel.fields.listType"
                      :disabled="true"
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
                      !!formModel.fields.nextUsers.length &&
                      formModel.fields.canChoose
                    "
                  >
                    <mu-select
                      v-model="formModel.fields.approver"
                      v-if="formModel.fields.isCounterSign"
                    >
                      <mu-option
                        :key="index"
                        :label="
                          formModel.fields.nextUsers.reduce(
                            (total, cur, currentIndex) => {
                              if (
                                currentIndex ==
                                formModel.fields.nextUsers.length - 1
                              ) {
                                return total + cur.userName
                              } else {
                                return total + cur.userName + ','
                              }
                            },
                            ''
                          )
                        "
                        :value="
                          formModel.fields.nextUsers.reduce(
                            (total, cur, currentIndex) => {
                              if (
                                currentIndex ==
                                formModel.fields.nextUsers.length - 1
                              ) {
                                return total + cur.user
                              } else {
                                return total + cur.user + ','
                              }
                            },
                            ''
                          )
                        "
                        avatar
                        v-for="(next, index) in [1]"
                      ></mu-option>
                    </mu-select>

                    <mu-select v-model="formModel.fields.approver" v-else>
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
      <div class="demo-drawer-footer">
        <Button
          :disabled="isView || isloading"
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
import { upload, deleteFile } from '@/api/common'

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
      tab_name: 'receiver_tabs1',
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
          additions: '',
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
                let color = '#19be6b'
                switch (params.row.listType) {
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
                          lineHeight: '20px',
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
                              // vm.$emit('input', params.tableData)
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
        receiver: 'static',
      },
    }
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
      this.tab_name = 'receiver_tabs1'
      this.handleSwitchFormModeToEdit()
      this.handleResetFormReceiver()
      this.doLoadReceiver(params.row.id)
    },
    handleView(params) {
      this.tab_name = 'receiver_tabs1'
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
    async handleSubmitAgree() {
      if (!!this.deleteUrls.length) {
        await deleteFile(this.deleteUrls)
      }
      let valid = this.validateReceiverForm()
      if (valid) {
        this.isloading = true
        if (this.formModel.mode === 'edit') {
          //同意
          if (
            !this.formModel.fields.approver &&
            this.formModel.fields.canChoose
          ) {
            this.$Message.error('请选择审批人')
            this.isloading = false
            return
          }
          this.formModel.fields.status = '1'
          this.doEditReceiver()
        }
      }
    },
    async handleSubmitReject() {
      if (!!this.deleteUrls.length) {
        await deleteFile(this.deleteUrls)
      }
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
      //this.$refs['formReceiver'].resetFields()
      Object.keys(this.formModel.fields).forEach(
        (key) => (this.formModel.fields[key] = '')
      )
      this.deleteUrls = []
      this.additions = []
      Object.keys(this.addition).forEach((key) => (this.addition[key] = ''))
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
        this.isloading = false
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
      })
    },

    doViewReceiver(code) {
      viewReceiver({ code: code }).then((res) => {
        this.formModel.fields = res.data.data
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
