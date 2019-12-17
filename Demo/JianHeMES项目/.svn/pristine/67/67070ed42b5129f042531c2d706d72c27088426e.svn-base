
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


window.onload = function () {

    var roles = JSON.parse(localStorage.getItem("rigths"));

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