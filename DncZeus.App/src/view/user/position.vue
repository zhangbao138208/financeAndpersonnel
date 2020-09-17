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
        v-model="stores.position.data"
        :totalCount="stores.position.query.totalCount"
        :columns="stores.position.columns"
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
                      v-model="stores.position.query.kw"
                      placeholder="输入关键字搜索..."
                      @on-search="handleSearchPosition()"
                    >
                      <Select
                        slot="prepend"
                        v-model="stores.position.query.isDeleted"
                        @on-change="handleSearchPosition"
                        placeholder="删除状态"
                        style="width:60px;"
                      >
                        <Option
                          v-for="item in stores.position.sources.isDeletedSources"
                          :value="item.value"
                          :key="item.value"
                        >{{item.text}}</Option>
                      </Select>
                      <Select
                        slot="prepend"
                        v-model="stores.position.query.status"
                        @on-change="handleSearchPosition"
                        placeholder="职位状态"
                        style="width:60px;"
                      >
                        <Option
                          v-for="item in stores.position.sources.statusSources"
                          :value="item.value"
                          :key="item.value"
                        >{{item.text}}</Option>
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
                  <Button icon="md-refresh" title="刷新" @click="handleRefresh"></Button>
                </ButtonGroup>
                <Button
                  icon="md-create"
                  type="primary"
                  @click="handleShowCreateWindow"
                  title="新增职位"
                >新增职位</Button>
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
      :mask="false"
      :styles="styles"
    >
      <Form :model="formModel.fields" ref="formPosition" :rules="formModel.rules" label-position="left">
        <FormItem label="职位名称" prop="name" label-position="left">
          <Input v-model="formModel.fields.name" placeholder="请输入职位名称"/>
        </FormItem>
         <FormItem label="层级" label-position="left">
          <InputNumber  v-model="formModel.fields.levelID" placeholder="请输入排序"/>
        </FormItem>
        <FormItem label="职位状态" label-position="left">
          <i-switch size="large" v-model="formModel.fields.status" :true-value="1" :false-value="0">
            <span slot="open">正常</span>
            <span slot="close">禁用</span>
          </i-switch>
        </FormItem>
       
        
      </Form>
      <div class="demo-drawer-footer">
        <Button icon="md-checkmark-circle" type="primary" @click="handleSubmitDePartment">保 存</Button>
        <Button style="margin-left: 8px" icon="md-close" @click="formModel.opened = false">取 消</Button>
      </div>
    </Drawer>
  </div>
</template>

<script>
import Tables from "_c/tables";

import {
  getPositionList,
  createPosition,
  loadPosition,
  editPosition,
  deletePosition,
  batchCommand
 
} from "@/api/user/position";
export default {
  name: "user_position_page",
  components: {
    Tables
  },
  data() {
    return {
      commands: {
        delete: { name: "delete", title: "删除" },
        recover: { name: "recover", title: "恢复" },
        forbidden: { name: "forbidden", title: "禁用" },
        normal: { name: "normal", title: "启用" }
      },
      formModel: {
        opened: false,
        title: "创建职位",
        mode: "create",
        selection: [],
        fields: {
          code: "",
          name: "",
          levelID: 0,
          isLocked: 0,
          status: 1,
          isDeleted: 0,
          description: ""
        },
        rules: {
          name: [
            {
              type: "string",
              required: true,
              message: "请输入职位名称",
              min: 2
            }
          ]
        }
      },
      stores: {
        position: {
          query: {
            totalCount: 0,
            pageSize: 20,
            currentPage: 1,
            kw: "",
            isDeleted: 0,
            status: -1,
            sort: [
              {
                direct: "DESC",
                field: "CreatedOn"
              }
            ]
          },
          sources: {
            isDeletedSources: [
              { value: -1, text: "全部" },
              { value: 0, text: "正常" },
              { value: 1, text: "已删" }
            ],
            statusSources: [
              { value: -1, text: "全部" },
              { value: 0, text: "禁用" },
              { value: 1, text: "正常" }
            ],
            statusFormSources: [
              { value: 0, text: "禁用" },
              { value: 1, text: "正常" }
            ]
          },
          columns: [
            { type: "selection", width: 50, key: "handle" },
            { title: "职位名称", key: "name", width: 200, sortable: true},
             {
              title: "状态",
              key: "status",
              align: "center",
              width: 120,
              render: (h, params) => {
                let status = params.row.status;
                let statusColor = "success";
                let statusText = "正常";
                switch (status) {
                  case 0:
                    statusText = "禁用";
                    statusColor = "default";
                    break;
                }
                return h(
                  "Tooltip",
                  {
                    props: {
                      placement: "top",
                      transfer: true,
                      delay: 500
                    }
                  },
                  [
                    //这个中括号表示是Tooltip标签的子标签
                    h(
                      "Tag",
                      {
                        props: {
                          //type: "dot",
                          color: statusColor
                        }
                      },
                      statusText
                    ), //表格列显示文字
                    h(
                      "p",
                      {
                        slot: "content",
                        style: {
                          whiteSpace: "normal"
                        }
                      },
                      statusText //整个的信息即气泡内文字
                    )
                  ]
                );
              }
            },
           { title: "层级", key: "levelID", width: 100},
            {
              title: "创建时间",
              width: 150,
              ellipsis: true,
              tooltip: true,
              key: "createdOn"
            },
            { title: "创建者", key: "createdByUserName" },
            {
              title: "操作",
              align: "center",
              key: "handle",
              width: 150,
              className: "table-command-column",
              options: ["edit"],
              button: [
                (h, params, vm) => {
                  return h(
                    "Poptip",
                    {
                      props: {
                        confirm: true,
                        title: "你确定要删除吗?"
                      },
                      on: {
                        "on-ok": () => {
                          vm.$emit("on-delete", params);
                        }
                      }
                    },
                    [
                      h(
                        "Tooltip",
                        {
                          props: {
                            placement: "left",
                            transfer: true,
                            delay: 1000
                          }
                        },
                        [
                          h("Button", {
                            props: {
                              shape: "circle",
                              size: "small",
                              icon: "md-trash",
                              type: "error"
                            }
                          }),
                          h(
                            "p",
                            {
                              slot: "content",
                              style: {
                                whiteSpace: "normal"
                              }
                            },
                            "删除"
                          )
                        ]
                      )
                    ]
                  );
                },
                (h, params, vm) => {
                  return h(
                    "Tooltip",
                    {
                      props: {
                        placement: "left",
                        transfer: true,
                        delay: 1000
                      }
                    },
                    [
                      h("Button", {
                        props: {
                          shape: "circle",
                          size: "small",
                          icon: "md-create",
                          type: "primary"
                        },
                        on: {
                          click: () => {
                            vm.$emit("on-edit", params);
                            vm.$emit("input", params.tableData);
                          }
                        }
                      }),
                      h(
                        "p",
                        {
                          slot: "content",
                          style: {
                            whiteSpace: "normal"
                          }
                        },
                        "编辑"
                      )
                    ]
                  );
                }
              ]
            }
          ],
          data: []
        }
      },
      styles: {
        height: "calc(100% - 55px)",
        overflow: "auto",
        paddingBottom: "53px",
        position: "static"
      }
    };
  },
  computed: {
    formTitle() {
      if (this.formModel.mode === "create") {
        return "创建职位";
      }
      if (this.formModel.mode === "edit") {
        return "编辑职位";
      }
      return "";
    },
    selectedRows() {
      return this.formModel.selection;
    },
    selectedRowsId() {
      return this.formModel.selection.map(x => x.code);
    }
  },
  methods: {
    loadPositionList() {
      getPositionList(this.stores.position.query).then(res => {
        this.stores.position.data = res.data.data;
        this.stores.position.query.totalCount = res.data.totalCount;
      });
    },
    handleOpenFormWindow() {
      this.formModel.opened = true;
    },
    handleCloseFormWindow() {
      this.formModel.opened = false;
    },
    handleSwitchFormModeToCreate() {
      this.formModel.mode = "create";
    },
    handleSwitchFormModeToEdit() {
      this.formModel.mode = "edit";
      this.handleOpenFormWindow();
    },
    handleEdit(params) {
      this.handleSwitchFormModeToEdit();
      this.handleResetFormPosition();
      this.doLoadPosition(params.row.code);
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection;
    },
    handleRefresh() {
      this.loadPositionList();
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate();
      this.handleOpenFormWindow();
      this.handleResetFormPosition();
    },
    handleSubmitDePartment() {
      let valid = this.validatePositionForm();
      if (valid) {
        if (this.formModel.mode === "create") {
          this.doCreatePosition();
        }
        if (this.formModel.mode === "edit") {
          this.doEditPosition();
        }
      }
    },
    handleResetFormPosition() {
      this.$refs["formPosition"].resetFields();
    },
    doCreatePosition() {
      createPosition(this.formModel.fields).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadPositionList();
        } else {
          this.$Message.warning(res.data.message);
        }
        this.handleCloseFormWindow();
      });
    },
    doEditPosition() {
      editPosition(this.formModel.fields).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadPositionList();
        } else {
          this.$Message.warning(res.data.message);
        }
        this.handleCloseFormWindow();
      });
    },
    validatePositionForm() {
      let _valid = false;
      this.$refs["formPosition"].validate(valid => {
        if (!valid) {
          this.$Message.error("请完善表单信息");
          _valid = false;
        } else {
          _valid = true;
        }
      });
      return _valid;
    },
    doLoadPosition(code) {
      loadPosition({ code: code }).then(res => {
        this.formModel.fields = res.data.data;
        console.log(this.formModel.fields)
      });
    },
    handleDelete(params) {
      this.doDelete(params.row.code);
    },
    doDelete(ids) {
      if (!ids) {
        this.$Message.warning("请选择至少一条数据");
        return;
      }
      deletePosition(ids).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadPositionList();
        } else {
          this.$Message.warning(res.data.message);
        }
      });
    },
    handleBatchCommand(command) {
      if (!this.selectedRowsId || this.selectedRowsId.length <= 0) {
        this.$Message.warning("请选择至少一条数据");
        return;
      }
      this.$Modal.confirm({
        title: "操作提示",
        content:
          "<p>确定要执行当前 [" +
          this.commands[command].title +
          "] 操作吗?</p>",
        loading: true,
        onOk: () => {
          this.doBatchCommand(command);
        }
      });
    },
    doBatchCommand(command) {
      batchCommand({
        command: command,
        ids: this.selectedRowsId.join(",")
      }).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadPositionList();
          this.formModel.selection=[];
        } else {
          this.$Message.warning(res.data.message);
        }
        this.$Modal.remove();
      });
    },
    handleSearchPosition() {
      this.loadPositionList();
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return "table-row-disabled";
      }
      return "";
    },
    handlePageChanged(page) {
      this.stores.position.query.currentPage = page;
      this.loadPositionList();
    },
    handlePageSizeChanged(pageSize) {
      this.stores.position.query.pageSize = pageSize;
      this.loadPositionList();
    }
  },
  mounted() {
    this.loadPositionList();
  }
};
</script>
