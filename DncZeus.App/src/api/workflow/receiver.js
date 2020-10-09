import axios from '@/libs/api.request'
export const getReceiverList = (data) => {
        return axios.request({
            url: 'workflow/receiver/list',
            method: 'post',
            data
        })
    }
    // create receiver

//loadReceiver
export const loadReceiver = (data) => {
    return axios.request({
        url: 'workflow/receiver/edit/' + data.code,
        method: 'get'
    })
}

// editReceiver
export const editReceiver = (data) => {
    return axios.request({
        url: 'workflow/receiver/edit',
        method: 'put',
        data
    })
}

//loadReceiver
export const viewReceiver = (data) => {
    return axios.request({
        url: 'workflow/receiver/view/' + data.code,
        method: 'get'
    })
}