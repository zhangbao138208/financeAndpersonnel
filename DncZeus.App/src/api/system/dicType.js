import axios from '@/libs/api.request'
export const getDicTypeList = (data) => {
        return axios.request({
            url: 'system/dictionarytype/list',
            method: 'post',
            data
        })
    }
    // create dictionarytype
export const createDicType = data => {
    return axios.request({
        url: "system/dictionarytype/create",
        method: "post",
        data
    });
};
//loadDicType
export const loadDicType = (data) => {
    return axios.request({
        url: 'system/dictionarytype/edit/' + data.code,
        method: 'get'
    })
}

// editDicType
export const editDicType = (data) => {
    return axios.request({
        url: 'system/dictionarytype/edit',
        method: 'put',
        data
    })
}

// delete dictionarytype
export const deleteDicType = (ids) => {
    return axios.request({
        url: 'system/dictionarytype/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'system/dictionarytype/batch',
        method: 'post',
        params: data
    })
}

//load dictionarytype simple list
export const loadDicTypeSimpleList = () => {
    return axios.request({
        url: 'system/dictionarytype/find_simple_list',
        method: 'get'
    })
}