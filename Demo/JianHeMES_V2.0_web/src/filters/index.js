//<!--------格式化日期--------->
//2020-05-21T10:54:56.955+08:00 ——> 2020-05-21 18:54:56
const formatTime = function (v) {
  if (v == "" || v == null) {
    return ""
  } else {
    let date = new Date(v);
    let month = (date.getMonth() + 1).toString().padStart(2, 0);
    return date.getFullYear() + "-" + month + "-" + (date.getDate().toString().padStart(2, 0)) + " " + date.getHours().toString().padStart(2, 0) + ":" + date.getMinutes().toString().padStart(2, 0) + ":" + date.getSeconds().toString().padStart(2, 0);
  }
}

//2020-05-21T10:54:56.955+08:00 ——> 2020-05-21 18:54
const formatMinutes = function (v) {
  if (v == "" || v == null) {
    return ""
  } else {
    let date = new Date(v);
    let month = (date.getMonth() + 1).toString().padStart(2, 0);
    return date.getFullYear() + "-" + month + "-" + (date.getDate().toString().padStart(2, 0)) + " " + date.getHours().toString().padStart(2, 0) + ":" + date.getMinutes().toString().padStart(2, 0);
  }
}

//2020-05-21T10:54:56.955+08:00 ——> 2020-05-21
const formatDate = function (v) {
  if (v == "" || v == null) {
    return ""
  } else {
    let date = new Date(v);
    let month = (date.getMonth() + 1).toString().padStart(2, 0);
    return date.getFullYear() + "-" + month + "-" + date.getDate().toString().padStart(2, 0);
  }
}
//2020-05-21T10:54:56.955+08:00 ——> 2020-05
const formatMonth = function (v) {
  if (v == "" || v == null) {
    return ""
  } else {
    let date = new Date(v);
    let month = (date.getMonth() + 1).toString().padStart(2, 0);
    return date.getFullYear() + "-" + month;
  }
}


//<!--------格式化金额--------->
//15862354695852——>15,862,354,695,852
const formatMoney = (number, decimals = 0, decPoint = '.', thousandsSep = ',') => {
  number = (number + '').replace(/[^0-9+-Ee.]/g, '')
  let n = !isFinite(+number) ? 0 : +number
  let prec = !isFinite(+decimals) ? 0 : Math.abs(decimals)
  let sep = (typeof thousandsSep === 'undefined') ? ',' : thousandsSep
  let dec = (typeof decPoint === 'undefined') ? '.' : decPoint
  let s = ''
  let toFixedFix = function (n, prec) {
    let k = Math.pow(10, prec)
    return '' + Math.ceil(n * k) / k
  }
  s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.')
  let re = /(-?\d+)(\d{3})/
  while (re.test(s[0])) {
    s[0] = s[0].replace(re, '$1' + sep + '$2')
  }
  if ((s[1] || '').length < prec) {
    s[1] = s[1] || ''
    s[1] += new Array(prec - s[1].length + 1).join('0')
  }
  return s.join(dec)
}


//<!--------格式化百分比--------->
//98——>98%
const formatPercentage = (v) => {
    return v + "%";
}

export {
  //格式化日期
  formatTime, //2020-05-21T10:54:56.955+08:00 ——> 2020-05-21 18:54:56
  formatMinutes, //2020-05-21T10:54:56.955+08:00 ——> 2020-05-21 18:54
  formatDate, //2020-05-21T10:54:56.955+08:00 ——> 2020-05-21
  formatMonth, //2020-05-21T10:54:56.955+08:00 ——> 2020-05
  //格式化金额
  formatMoney,//15862354695852——>15,862,354,695,852
  //格式化金额
  formatPercentage,//98——>98%
}
