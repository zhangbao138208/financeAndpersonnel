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
        v-model="stores.dictionary.data"
        :totalCount="stores.dictionary.query.totalCount"
        :columns="stores.dictionary.columns"
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
                      v-model="stores.dictionary.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchDictionary()"
                    >
                      <!-- <Select
                        slot="prepend"
                        v-model="stores.dictionary.query.isDeleted"
                        @on-change="handleSearchDictionary"
                        placeholder="删除状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.dictionary.sources
                            .isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                          >{{ item.text }}</Option
                        >
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.dictionary.query.status"
                        @on-change="handleSearchDictionary"
                        placeholder="字典表类型状态"
                        style="width: 60px"
                      >
                        <Option
                          v-for="item in stores.dictionary.sources.statusSources"
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
                  title="新增字典表类型"
                  >新增字典表类型</Button
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
        ref="formDictionary"
        :rules="formModel.rules"
        label-dictionary="left"
      >
        <FormItem label="字典类型" prop="typeCode">
          <mu-select v-model="formModel.fields.typeCode">
            <mu-option
              :key="dicType.code"
              :label="dicType.name"
              :value="dicType.code"
              avatar
              v-for="(dicType, index) in dicTypes"
            >
              <mu-list-item-action avatar>
                <mu-avatar :size="36" color="primary">{{
                  dicType.name.substring(0, 1)
                }}</mu-avatar>
              </mu-list-item-action>
              <mu-list-item-content>
                <mu-list-item-title>{{ dicType.name }}</mu-list-item-title>
              </mu-list-item-content>
            </mu-option>
          </mu-select>
        </FormItem>
        <FormItem label="字典名称" prop="name" label-dictionary="left">
          <Input v-model="formModel.fields.name" placeholder="请输入字典名称" />
        </FormItem>
        <FormItem label="字典值" prop="value" label-dictionary="left">
          <Input v-model="formModel.fields.value" placeholder="请输入字典值" />
        </FormItem>
        <FormItem label="固定" label-position="left">
          <i-switch
            size="large"
            v-model="formModel.fields.fixed"
            :true-value="1"
            :false-value="0"
          >
            <span slot="open">固定</span>
            <span slot="close">不固定</span>
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
import { loadDicTypeSimpleList } from '@/api/system/dicType'
import {
  getDictionaryList,
  createDictionary,
  loadDictionary,
  editDictionary,
  deleteDictionary,
  batchCommand,
} from '@/api/system/dictionary'
export default {
  name: 'system_dictionary_page',
  components: {
    Tables,
  },
  data() {
    return {
      dicTypes: [],
      commands: {
        delete: { name: 'delete', title: '删除' },
        recover: { name: 'recover', title: '恢复' },
        forbidden: { name: 'forbidden', title: '禁用' },
        normal: { name: 'normal', title: '启用' },
      },
      formModel: {
        opened: false,
        title: '创建字典表类型',
        mode: 'create',
        selection: [],
        fields: {
          code: '',
          name: '',
          typeCode: '',
          value: '',
          fixed: false,
        },
        rules: {
          name: [
            {
              type: 'string',
              required: true,
              message: '请输入字典名称',
              min: 2,
            },
          ],
          typeCode: [
            {
              type: 'string',
              required: true,
              message: '请选择字典类型',
              min: 2,
            },
          ],
          value: [
            {
              type: 'string',
              required: true,
              message: '请输入字典值',
              min: 1,
            },
          ],
        },
      },
      stores: {
        dictionary: {
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
            { title: '名称', key: 'name', width: 300 },

            { title: '值', key: 'value', width: 300 },
            { title: '字典类型', key: 'typeName', width: 300 },

            {
              title: '操作',
              align: 'center',
              key: 'handle',
              //   width: 150,
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
        dictionary: 'static',
      },
    }
  },
  computed: {
    formTitle() {
      if (this.formModel.mode === 'create') {
        return '创建字典表类型'
      }
      if (this.formModel.mode === 'edit') {
        return '编辑字典表类型'
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
      loadDicTypeSimpleList().then((res) => {
        this.dicTypes = res.data.data
      })
    },
    loadDictionaryList() {
      getDictionaryList(this.stores.dictionary.query).then((res) => {
        this.stores.dictionary.data = res.data.data
        this.stores.dictionary.query.totalCount = res.data.totalCount
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
      this.handleResetFormDictionary()
      this.doLoadDictionary(params.row.code)
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection
    },
    handleRefresh() {
      this.loadDictionaryList()
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate()
      this.handleOpenFormWindow()
      this.handleResetFormDictionary()
    },
    handleSubmitDePartment() {
      let valid = this.validateDictionaryForm()
      if (valid) {
        if (this.formModel.mode === 'create') {
          this.doCreateDictionary()
        }
        if (this.formModel.mode === 'edit') {
          this.doEditDictionary()
        }
      }
    },
    handleResetFormDictionary() {
      this.$refs['formDictionary'].resetFields()
    },
    doCreateDictionary() {
      createDictionary(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadDictionaryList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    doEditDictionary() {
      editDictionary(this.formModel.fields).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadDictionaryList()
        } else {
          this.$Message.warning(res.data.message)
        }
        this.handleCloseFormWindow()
      })
    },
    validateDictionaryForm() {
      let _valid = false
      this.$refs['formDictionary'].validate((valid) => {
        if (!valid) {
          this.$Message.error('请完善表单信息')
          _valid = false
        } else {
          _valid = true
        }
      })
      return _valid
    },
    doLoadDictionary(code) {
      loadDictionary({ code: code }).then((res) => {
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
      deleteDictionary(ids).then((res) => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message)
          this.loadDictionaryList()
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
          this.loadDictionaryList()
          this.formModel.selection = []
        } else {
          this.$Message.warning(res.data.message)
        }
        this.$Modal.remove()
      })
    },
    handleSearchDictionary() {
      this.loadDictionaryList()
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return 'table-row-disabled'
      }
      return ''
    },
    handlePageChanged(page) {
      this.stores.dictionary.query.currentPage = page
      this.loadDictionaryList()
    },
    handlePageSizeChanged(pageSize) {
      this.stores.dictionary.query.pageSize = pageSize
      this.loadDictionaryList()
    },
  },
  mounted() {
    this.initData()
    this.loadDictionaryList()
  },
}
</script>
