let TeamEvaluation = {
    data: function () {
        return {
            loading: false,//loading状态开关
            Department: {
                value: '',
                list: [],
            },
            Group: {
                value: '',
                list: [],
            },
            Ordernum: {
                value: '',
                list: [],
            },
            ExecuteTime: {
                value: '',
                list: [],
            },
            Section: {
                value: '',
                list: [],
            },
            Process: {
                value: '',
                list: [],
            },
        }
    },
    created: function () {
        this.GetDeprment();
        this.GetGroup();
        this.GetOrdernum();
        this.GetExTime();
        this.GetSectionList();
    },
    methods: {
        GetDeprment() {
            axios.post('/KPI/GetDeprment').then(res => {
                this.Department.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
        GetGroup() {
            axios.post('/KPI/GetGroup').then(res => {
                this.Group.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
        GetOrdernum() {
            axios.post('/Packagings/GetOrderList').then(res => {
                this.Ordernum.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
        GetExTime() {
            axios.post('/KPI/GetExTime').then(res => {
                this.ExecuteTime.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
        GetSectionList() {
            axios.post('/KPI/GetSectionList').then(res => {
                this.Section.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
        GetProcessList(v) {
            axios.post('/KPI/GetProcessList', { section: v }).then(res => {
                this.Process.list = res.data;
            }).catch(err => {
                console.warn(err);
            });
        },
    },
};