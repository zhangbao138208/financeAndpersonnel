import axios from "@/libs/api.request";

export const getTemplateList = data => {
    return axios.request({
        url: "workflow/template/list",
        method: "post",
        data
    });
};

// create template
export const createTemplate = data => {
    return axios.request({
        url: "workflow/template/create",
        method: "post",
        data
    });
};

//load template
export const loadTemplate = data => {
    return axios.request({
        url: "workflow/template/edit/" + data.code,
        method: "get"
    });
};

// edit template
export const editTemplate = data => {
    return axios.request({
        url: "workflow/template/edit",
        method: "put",
        data
    });
};

// delete template
export const deleteTemplate = ids => {
    return axios.request({
        url: "workflow/template/delete/" + ids,
        method: "delete"
    });
};

// batch command
export const batchCommand = data => {
    return axios.request({
        url: "workflow/template/batch",
        method: "post",
        params: data
    });
};

//load template truee
export const loadTemplateTree = (code) => {
    let url = "workflow/template/tree";
    if (code != null) {
        url += "/" + code;
    }
    return axios.request({
        url: url,
        method: "get"
    });
};

//load Template simple list
export const loadTemplateSimpleList = () => {
    return axios.request({
        url: 'workflow/template/find_simple_list',
        method: 'get'
    })
}