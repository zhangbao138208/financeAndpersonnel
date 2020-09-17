<template>
 <Home></Home>
</template>

<script>
import Tables from "_c/tables";
import Home from './drawer/Home';
import {
  getResumeInfoList,
  createResumeInfo,
  loadResumeInfo,
  editResumeInfo,
  deleteResumeInfo,
  batchCommand
 
} from "@/api/resume/resumeInfo";
export default {
  name: "resume_list_page",
  components: {
    Tables,
    Home
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
        title: "创建简历",
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
              message: "请输入简历名称",
              min: 2
            }
          ]
        }
      },
      stores: {
        resumeInfo: {
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
            { title: "简历名称", key: "name", width: 200, sortable: true},
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
        resumeInfo: "static"
      }
    };
  },
  computed: {
    formTitle() {
      if (this.formModel.mode === "create") {
        return "创建简历";
      }
      if (this.formModel.mode === "edit") {
        return "编辑简历";
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
    loadResumeInfoList() {
      getResumeInfoList(this.stores.resumeInfo.query).then(res => {
        this.stores.resumeInfo.data = res.data.data;
        this.stores.resumeInfo.query.totalCount = res.data.totalCount;
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
      this.handleResetFormResumeInfo();
      this.doLoadResumeInfo(params.row.code);
    },
    handleSelect(selection, row) {},
    handleSelectionChange(selection) {
      this.formModel.selection = selection;
    },
    handleRefresh() {
      this.loadResumeInfoList();
    },
    handleShowCreateWindow() {
      this.handleSwitchFormModeToCreate();
      this.handleOpenFormWindow();
      this.handleResetFormResumeInfo();
    },
    handleSubmitDePartment() {
      let valid = this.validateResumeInfoForm();
      if (valid) {
        if (this.formModel.mode === "create") {
          this.doCreateResumeInfo();
        }
        if (this.formModel.mode === "edit") {
          this.doEditResumeInfo();
        }
      }
    },
    handleResetFormResumeInfo() {
      this.$refs["formResumeInfo"].resetFields();
    },
    doCreateResumeInfo() {
      createResumeInfo(this.formModel.fields).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadResumeInfoList();
        } else {
          this.$Message.warning(res.data.message);
        }
        this.handleCloseFormWindow();
      });
    },
    doEditResumeInfo() {
      editResumeInfo(this.formModel.fields).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadResumeInfoList();
        } else {
          this.$Message.warning(res.data.message);
        }
        this.handleCloseFormWindow();
      });
    },
    validateResumeInfoForm() {
      let _valid = false;
      this.$refs["formResumeInfo"].validate(valid => {
        if (!valid) {
          this.$Message.error("请完善表单信息");
          _valid = false;
        } else {
          _valid = true;
        }
      });
      return _valid;
    },
    doLoadResumeInfo(code) {
      loadResumeInfo({ code: code }).then(res => {
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
      deleteResumeInfo(ids).then(res => {
        if (res.data.code === 200) {
          this.$Message.success(res.data.message);
          this.loadResumeInfoList();
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
          this.loadResumeInfoList();
          this.formModel.selection=[];
        } else {
          this.$Message.warning(res.data.message);
        }
        this.$Modal.remove();
      });
    },
    handleSearchResumeInfo() {
      this.loadResumeInfoList();
    },
    rowClsRender(row, index) {
      if (row.isDeleted) {
        return "table-row-disabled";
      }
      return "";
    },
    handlePageChanged(page) {
      this.stores.resumeInfo.query.currentPage = page;
      this.loadResumeInfoList();
    },
    handlePageSizeChanged(pageSize) {
      this.stores.resumeInfo.query.pageSize = pageSize;
      this.loadResumeInfoList();
    }
  },
  mounted() {
    this.loadResumeInfoList();
  }
};
</script>
