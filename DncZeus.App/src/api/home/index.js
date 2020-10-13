import axios from '@/libs/api.request'
export const initHome = (data) => {
    return axios.request({
        url: 'home/init',
        method: 'get',

    })
}