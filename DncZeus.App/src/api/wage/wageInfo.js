import axios from '@/libs/api.request'
export const getWageInfoList = (data) => {
        return axios.request({
            url: 'wage/wageInfo/list',
            method: 'post',
            data
        })
    }
    // create wageInfo
export const createWageInfo = data => {
    return axios.request({
        url: "wage/wageInfo/create",
        method: "post",
        data
    });
};

// import wageInfo
export const importWageInfo = data => {
    return axios.request({
        url: "wage/wageInfo/import",
        method: "post",
        data
    });
};

// export wageInfo
export const exportWageInfo = data => {
    return axios.request({
        url: "wage/wageInfo/export",
        method: "post",
        data
    });
};


//loadWageInfo
export const loadWageInfo = (data) => {
    return axios.request({
        url: 'wage/wageInfo/edit/' + data.code,
        method: 'get'
    })
}

// editWageInfo
export const editWageInfo = (data) => {
    return axios.request({
        url: 'wage/wageInfo/edit',
        method: 'put',
        data
    })
}

// delete wageInfo
export const deleteWageInfo = (ids) => {
    return axios.request({
        url: 'wage/wageInfo/delete/' + ids,
        method: 'delete'
    })
}

// batch command
export const batchCommand = (data) => {
    return axios.request({
        url: 'wage/wageInfo/batch',
        method: 'post',
        params: data
    })
}