import axios from '@/libs/api.request'
export const getLogList = (data) => {
    return axios.request({
        url: 'loger/list',
        method: 'post',
        data
    })
}