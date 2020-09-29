import axios from '@/libs/api.request'
export const getFinanceInfoList = (data) => {
        return axios.request({
            url: 'finance/financeinfo/list',
            method: 'post',
            data
        })
    }
    // create financeinfo
export const createFinanceInfo = data => {
    return axios.request({
        url: "finance/financeinfo/create",
        method: "post",
        data
    });
};
//loadFinanceInfo
export const loadFinanceInfo = (data) => {
    return axios.request({
        url: 'finance/financeinfo/edit/' + data.code,
        method: 'get'
    })
}

// editFinanceInfo
export const editFinanceInfo = (data) => {
    return axios.request({
        url: 'finance/financeinfo/edit',
        method: 'put',
        data
    })
}

// delete financeinfo
export const deleteFinanceInfo = (ids) => {
    return axios.request({
        url: 'finance/financeinfo/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'finance/financeinfo/batch',
        method: 'post',
        params: data
    })
}

//load financeinfo simple list
export const loadFinanceInfoSimpleList = () => {
    return axios.request({
        url: 'finance/financeinfo/find_simple_list',
        method: 'get'
    })
}