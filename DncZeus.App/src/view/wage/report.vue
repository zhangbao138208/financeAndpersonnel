<template>
  <div>
    <Card>
      <div>
        <div slot="search">
          <section class="dnc-toolbar-wrap">
            <Row :gutter="16">
              <Col span="16">
                <Form @submit.native.prevent inline>
                  <FormItem>
                    <label>分类 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                    <Select
                      @on-change="loadReport"
                      placeholder="分类"
                      style="width: 100px"
                      v-model="query.category"
                    >
                      <Option
                        :key="item.code"
                        :value="item.code"
                        v-for="item in categorys"
                        >{{ item.name }}</Option
                      >
                    </Select>
                    <label>
                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 年份
                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </label>
                    <Select
                      @on-change="loadReport"
                      placeholder="分类"
                      style="width: 100px"
                      v-model="query.year"
                    >
                      <Option
                        :key="item.code"
                        :value="item.code"
                        v-for="item in years"
                        >{{ item.name }}</Option
                      >
                    </Select>
                  </FormItem>
                </Form>
              </Col>
            </Row>
          </section>
        </div>
      </div>
      <div ref="echats">
        <div v-for="(item, index) in reportData" class="chart-item-area">
          <div
            class="echarts"
            style="height: 410px"
            :id="item.title"
            v-on-echart-resize
          ></div>
        </div>
      </div>
    </Card>
  </div>
</template>

<script>
import { loadWageReport } from '@/api/wage/wageReport'
import echarts from 'echarts'
import { on, off } from '@/libs/tools'
import $ from 'jquery'
export default {
  name: 'serviceRequests',
  data() {
    return {
      // reportStyle: {
      //   height: 300,
      // },
      // heights: [100, 200, 300, 400, 500, 600],
      option: {
        title: {
          left: '50%',
          textAlign: 'center',
          text: '报表统计', // 标题栏
          textStyle: {
            color: '#0DB9F2', //颜色
            fontStyle: 'normal', //风格
            fontWeight: 'normal', //粗细
            fontFamily: 'Microsoft yahei', //字体
            fontSize: 16, //大小
            align: 'center', //水平对齐
          },

          itemGap: 7,
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'cross',
            label: {
              backgroundColor: '#6a7985',
            },
          },
        },
        legend: {
          top: '10%',
          // left: '20%',
          // right: '20%',
        },
        grid: {
          top: '30%',
          left: '1.2%',
          right: '5%',
          bottom: '3%',
          containLabel: true,
        },
        xAxis: [
          {
            type: 'category',
            boundaryGap: false,
            data: [
              '一月',
              '二月',
              '三月',
              '四月',
              '五月',
              '六月',
              '七月',
              '八月',
              '九月',
              '十月',
              '十一月',
              '十二月',
            ],
          },
        ],
        yAxis: [
          {
            type: 'value',
          },
        ],
        series: [],
      },
      options: [],
      reportData: [],
      categorys: [
        { code: '0', name: '总体' },
        { code: '1', name: '个人' },
        { code: '2', name: '部门' },
        { code: '3', name: '职位' },
      ],
      years: [
        { code: '1', name: '今年' },
        { code: '2', name: '两年' },
        { code: '3', name: '三年' },
        { code: '4', name: '四年' },
        { code: '5', name: '五年' },
        { code: '6', name: '六年' },
        { code: '7', name: '七年' },
        { code: '8', name: '八年' },
        { code: '9', name: '九年' },
        { code: '10', name: '十年' },
      ],
      query: {
        category: '0',
        year: '1',
      },
      dom: null,
      doms: [],
    }
  },
  // methods: {
  //   resize() {
  //     debugger
  //     this.doms.forEach((_) => _.resize())
  //     //this.dom.resize()
  //   },
  // },
  watch: {
    reportData: function () {
      this.$nextTick(function () {
        //方法
        this.init()
      })
    },
  },
  methods: {
    init() {
      for (let i = 0; i < this.reportData.length; i++) {
        this.dom = echarts.init(
          document.getElementById(this.reportData[i].title)
        )
        let option = this.option
        option.legend.data = this.reportData[i].legend.data
        option.title.text = this.reportData[i].title
        option.series = this.reportData[i].wageSeries
        //echart重新渲染没有清空数据的问题
        this.dom.setOption(option, true)
      }
    },
    loadReport() {
      loadWageReport(this.query).then((res) => {
        if (res.data.code == 200) {
          this.reportData = res.data.data
          //this.init()
          // $(this.$refs.domChart).empty()
          // this.reportData.forEach((r, i) => {
          //   $(this.$refs.domChart).append(
          //     `<div id='dom${i}' style="height:310px" v-on-echart-resize></div>`
          //   )
          //   this.dom = echarts.init(document.getElementById(`dom${i}`))
          //   // this.dom = echarts.init(this.$refs.domtest)
          //   let option = this.option
          //   option.title.text = r.title
          //   option.series = r.wageSeries
          //   this.dom.setOption(option)
          // })

          //this.$Message.success(res.data.message)
        } else {
          this.$Message.error(res.data.message)
        }
      })
    },
  },
  mounted() {
    //window.addEventListener("resize",this.resize)
    // on(window, 'resize', this.resize)
    this.loadReport()
  },
  beforeDestroy() {
    off(window, 'resize', this.resize)
  },
}
</script>
