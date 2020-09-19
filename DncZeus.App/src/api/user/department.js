import axios from '@/libs/api.request'
export const getDepartmentList = (data) => {
        return axios.request({
            url: 'user/department/list',
            method: 'post',
            data
        })
    }
    // create department
export const createDepartment = data => {
    return axios.request({
        url: "user/department/create",
        method: "post",
        data
    });
};
//loadDepartment
export const loadDepartment = (data) => {
    return axios.request({
        url: 'user/department/edit/' + data.code,
        method: 'get'
    })
}

// editDepartment
export const editDepartment = (data) => {
    return axios.request({
        url: 'user/department/edit',
        method: 'put',
        data
    })
}

// delete department
export const deleteDepartment = (ids) => {
    return axios.request({
        url: 'user/department/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'user/department/batch',
        method: 'post',
        params: data
    })
}

//load department simple list
export const loadDepartmentSimpleList = () => {
    return axios.request({
        url: 'user/department/find_simple_list',
        method: 'get'
    })
}