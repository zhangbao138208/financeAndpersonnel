import axios from '@/libs/api.request'

export const upload = (data) => {
    return axios.request({
        withPrefix: false,
        url: 'upload',
        method: 'post',
        data
    })
}

export const deleteFile = (data) => {
    return axios.request({
        withPrefix: false,
        url: 'filedelete',
        method: 'delete',
        data
    })
}