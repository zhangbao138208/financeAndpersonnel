import axios from '@/libs/api.request'
export const getResumeInfoList = (data) => {
    return axios.request({
      url: 'resume/resumeInfo/list',
      method: 'post',
      data
    })
  }
  // create resumeInfo
export const createResumeInfo = data => {
    return axios.request({
      url: "resume/resumeInfo/create",
      method: "post",
      data
    });
  };
  //loadResumeInfo
export const loadResumeInfo = (data) => {
    return axios.request({
      url: 'resume/resumeInfo/edit/' + data.code,
      method: 'get'
    })
  }
  
  // editResumeInfo
  export const editResumeInfo = (data) => {
    return axios.request({
      url: 'resume/resumeInfo/edit',
      method: 'post',
      data
    })
  }
  
  // delete resumeInfo
  export const deleteResumeInfo = (ids) => {
    return axios.request({
      url: 'resume/resumeInfo/delete/' + ids,
      method: 'get'
    })
  }
  
  // batch command
  export const batchCommand = (data) => {
    return axios.request({
      url: 'resume/resumeInfo/batch',
      method: 'get',
      params: data
    })
  }
  