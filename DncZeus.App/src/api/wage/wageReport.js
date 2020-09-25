import axios from '@/libs/api.request'
// export wageInfo
export const loadWageReport = data => {
    return axios.request({
        url: "wage/wageInfo/report",
        method: "post",
        data
    });
};