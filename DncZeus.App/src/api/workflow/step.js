import axios from '@/libs/api.request'
export const getStepList = (data) => {
        return axios.request({
            url: 'workflow/step/list',
            method: 'post',
            data
        })
    }
    // create step
export const createStep = data => {
    return axios.request({
        url: "workflow/step/create",
        method: "post",
        data
    });
};
//loadStep
export const loadStep = (data) => {
    return axios.request({
        url: 'workflow/step/edit/' + data.code,
        method: 'get'
    })
}

// editStep
export const editStep = (data) => {
    return axios.request({
        url: 'workflow/step/edit',
        method: 'put',
        data
    })
}

// delete step
export const deleteStep = (ids) => {
    return axios.request({
        url: 'workflow/step/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'workflow/step/batch',
        method: 'post',
        params: data
    })
}

//load step simple list
export const loadStepSimpleList = (data) => {
    return axios.request({
        url: 'workflow/step/find_simple_list?' + toQueryString(data),
        method: 'get'
    })
}

function cleanArray(actual) {
    const newArray = [];
    for (let i = 0; i < actual.length; i++) {
        if (actual[i]) {
            newArray.push(actual[i]);
        }
    }
    return newArray;
}
// 将一个对象转成QueryString
function toQueryString(obj) {
    if (!obj) return "";
    return cleanArray(
        Object.keys(obj).map(key => {
            if (obj[key] === undefined) return "";
            return encodeURIComponent(key) + "=" + encodeURIComponent(obj[key]);
        })
    ).join("&");
}