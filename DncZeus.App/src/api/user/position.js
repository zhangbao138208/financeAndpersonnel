import axios from '@/libs/api.request'
export const getPositionList = (data) => {
        return axios.request({
            url: 'user/position/list',
            method: 'post',
            data
        })
    }
    // create position
export const createPosition = data => {
    return axios.request({
        url: "user/position/create",
        method: "post",
        data
    });
};
//loadPosition
export const loadPosition = (data) => {
    return axios.request({
        url: 'user/position/edit/' + data.code,
        method: 'get'
    })
}

// editPosition
export const editPosition = (data) => {
    return axios.request({
        url: 'user/position/edit',
        method: 'put',
        data
    })
}

// delete position
export const deletePosition = (ids) => {
    return axios.request({
        url: 'user/position/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'user/position/batch',
        method: 'post',
        params: data
    })
}

//load position simple list
export const loadPositionSimpleList = () => {
    return axios.request({
        url: 'user/position/find_simple_list',
        method: 'get'
    })
}