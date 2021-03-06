﻿var roles = JSON.parse(localStorage.getItem("rigths"));
function checkRoles(list, roleName) {   //检测权限
    if (list && roleName) {
        for (let item in list) {
            if (list[item] == roleName) {
                return true;
            };
        };
    };
    return false;
}
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    console.log(r)
    if (r != null) return decodeURI(r[2]); return null;
}

window.onload = function () {
    if (checkRoles(roles, '录入物料信息')) {
        $('.luru').show()
    } else {
        $('.luru').hide()
    }
    if (checkRoles(roles, 'MC锡膏仓库操作')) {
        $('.cangkucaozuo').show()
    } else {
        $('.cangkucaozuo').hide()
    }
    if (checkRoles(roles, 'SMT锡膏入SMT冰柜')) {
        $('.rubinggui').show()
    } else {
        $('.rubinggui').hide()
    }
    if (checkRoles(roles, 'SMT锡膏回温')) {
        $('.huiwen').show()
    } else {
        $('.huiwen').hide()
    }
    if (checkRoles(roles, 'SMT锡膏搅拌')) {
        $('.jiaoban').show()
    } else {
        $('.jiaoban').hide()
    }
    if (checkRoles(roles, 'SMT锡膏使用')) {
        $('.shiyong').show()
    } else {
        $('.shiyong').hide()
    }
    if (checkRoles(roles, 'SMT锡膏回收')) {
        $('.huishou').show()
    } else {
        $('.huishou').hide()
    }

    //设备点检权限限制
    if (checkRoles(roles, '查询点检保养表')) {
        $('.checkTally').show()
    } else {
        //$('.checkTally').hide()
    }

    if (checkRoles(roles, '设备产线添加')) {
        $('.addLineBtn').show()
    } else {
        $('.addLineBtn').hide()
    }
    if (checkRoles(roles, '设备添加')) {
        $('.inputtext2').show()
    } else {
        $('.inputtext2').hide()
    }
    if (checkRoles(roles, '上传安全库存清单')) {
        $('.eqpment_sfate_upload_all').show()
    } else {
        $('.eqpment_sfate_upload_all').hide()
    }
    // 包装权限
    if (checkRoles(roles, '外箱条码标签打印')) {
        $('.PoutsideBinningPrintOne').show()
    } else {
        $('.PoutsideBinningPrintOne').hide()
    }
    if (checkRoles(roles, '外箱条码删除')) {
        $('.PoutsideBinningPrintThree').show()
    } else {
        $('.PoutsideBinningPrintThree').hide()
    }
    if (checkRoles(roles, '修改外箱标签LOGO')) {
        $('.PoutsideBinningPrintFour').show()
    } else {
        $('.PoutsideBinningPrintFour').hide()
    }

    // 人事日报
    if (checkRoles(roles, '部门版本')) {
        $('.bumenbanben').show()
    } else {
        $('.bumenbanben').hide()
    }


    // 人事质量指标
    if (checkRoles(roles, '批量添加目标值模板')) {
        $('.Persionel_contrast_addRecords').show()
    } else {
        $('.Persionel_contrast_addRecords').hide()
    }

    // SOP
    if (checkRoles(roles, '操作SOP文档')) {
        $('.sop_PDF_operation').show()
    } else {
        $('.sop_PDF_operation').hide()
    }


    // 客诉订单
    //Customer_ComlaintsCreate
    if (checkRoles(roles, '创建订单客诉表')) {
        $('.Customer_ComlaintsCreate').show()
    } else {
        $('.Customer_ComlaintsCreate').hide()
    }
    if (checkRoles(roles, '修改订单客诉表')) {
        $('.Customer_ComlaintsEdit').show()
    } else {
        $('.Customer_ComlaintsEdit').hide()
    }

}


// 格式化时间-年月日
Vue.filter('YMD', function (val) {
    if (val == null) return '';
    let dd = new Date(val);
    let year = dd.getFullYear();
    let month = (dd.getMonth() + 1).toString().padStart(2, 0);
    let days = dd.getDate().toString().padStart(2, 0);
    let hours = dd.getHours().toString().padStart(2, 0);
    let minius = dd.getMinutes().toString().padStart(2, 0);
    let seconds = dd.getSeconds().toString().padStart(2, 0);
    return `${year}-${month}-${days}`
});

// 格式化时间-年月
Vue.filter('YM', function (val) {
    if (val == null) return '';
    let dd = new Date(val);
    let year = dd.getFullYear();
    let month = (dd.getMonth() + 1).toString().padStart(2, 0);
    let days = dd.getDate().toString().padStart(2, 0);
    let hours = dd.getHours().toString().padStart(2, 0);
    let minius = dd.getMinutes().toString().padStart(2, 0);
    let seconds = dd.getSeconds().toString().padStart(2, 0);
    return `${year}-${month}`
});

Vue.filter("YYYY", function (val) {
    if (val == null) return '';
    let dd = new Date(val);
    let year = dd.getFullYear();
    return `${year}`
})

// 表格赋值粘贴
function copy(val, containArr, keyArr) {
    var valOfPaste = val.split("\n");
    valOfPaste.pop();
    var initDatas = []
    valOfPaste.forEach((item, i) => {
        var items = item.split("\t");
        initDatas.push(items)
    });
    initDatas.forEach(item => {
        let obj = { color: false }
        for (let i = 0; i < keyArr.length; i++) {
            obj[`${keyArr[i]}`] = item[i]
        }
        containArr.push(obj);
    });
}
