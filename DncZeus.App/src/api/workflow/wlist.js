import axios from '@/libs/api.request'
export const getWlistList = (data) => {
        return axios.request({
            url: 'workflow/wlist/list',
            method: 'post',
            data
        })
    }
    // create wlist
export const createWlist = data => {
    return axios.request({
        url: "workflow/wlist/create",
        method: "post",
        data
    });
};
//loadWlist
export const loadWlist = (data) => {
    return axios.request({
        url: 'workflow/wlist/edit/' + data.code,
        method: 'get'
    })
}

// editWlist
export const editWlist = (data) => {
    return axios.request({
        url: 'workflow/wlist/edit',
        method: 'put',
        data
    })
}

// delete wlist
export const deleteWlist = (ids) => {
    return axios.request({
        url: 'workflow/wlist/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'workflow/wlist/batch',
        method: 'post',
        params: data
    })
}