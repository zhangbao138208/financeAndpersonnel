import axios from '@/libs/api.request'
export const getDictionaryList = (data) => {
        return axios.request({
            url: 'system/dictionary/list',
            method: 'post',
            data
        })
    }
    // create dictionary
export const createDictionary = data => {
    return axios.request({
        url: "system/dictionary/create",
        method: "post",
        data
    });
};
//loadDictionary
export const loadDictionary = (data) => {
    return axios.request({
        url: 'system/dictionary/edit/' + data.code,
        method: 'get'
    })
}

// editDictionary
export const editDictionary = (data) => {
    return axios.request({
        url: 'system/dictionary/edit',
        method: 'put',
        data
    })
}

// delete dictionary
export const deleteDictionary = (ids) => {
    return axios.request({
        url: 'system/dictionary/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'system/dictionary/batch',
        method: 'post',
        params: data
    })
}

//load dictionary simple list
export const loadDictionarySimpleList = (data) => {
    return axios.request({
        url: 'system/dictionary/find_simple_list/' + data,
        method: 'get',

    })
}