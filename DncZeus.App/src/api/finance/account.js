import axios from '@/libs/api.request'
export const getAccountList = (data) => {
        return axios.request({
            url: 'finance/account/list',
            method: 'post',
            data
        })
    }
    // create account
export const createAccount = data => {
    return axios.request({
        url: "finance/account/create",
        method: "post",
        data
    });
};
//loadAccount
export const loadAccount = (data) => {
    return axios.request({
        url: 'finance/account/edit/' + data.code,
        method: 'get'
    })
}

// editAccount
export const editAccount = (data) => {
    return axios.request({
        url: 'finance/account/edit',
        method: 'put',
        data
    })
}

// delete account
export const deleteAccount = (ids) => {
    return axios.request({
        url: 'finance/account/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'finance/account/batch',
        method: 'post',
        params: data
    })
}

//load account simple list
export const loadAccountSimpleList = () => {
    return axios.request({
        url: 'finance/account/find_simple_list',
        method: 'get'
    })
}